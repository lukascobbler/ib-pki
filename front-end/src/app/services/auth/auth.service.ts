import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthState } from '../../models/AuthState';
import { BasicUser } from '../../models/BasicUser';
import { LoginRequestDTO } from '../../models/LoginRequestDTO';
import { LoginResponseDTO } from '../../models/LoginResponseDTO';
import { RegisterRequestDTO } from '../../models/RegisterRequestDTO';
import { RefreshRequestDTO } from '../../models/RefreshRequestDTO';
import { RefreshResponseDTO } from '../../models/RefreshResponseDTO';
import { BehaviorSubject, Observable, map, tap, throwError, of } from 'rxjs';
import { finalize, shareReplay, catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  urlCore = 'https://localhost:8081/api/users';

  private static STORAGE_KEY = 'auth.state.v1';
  private refreshTimer: any = null;

  private readonly _state$ = new BehaviorSubject<AuthState>({
    accessToken: null,
    accessExpiresAt: null,
    refreshToken: null,
    refreshExpiresAt: null,
    user: null,
  });

  state$ = this._state$.asObservable();

  isLoggedIn$ = this.state$.pipe(map((s) => !!s.accessToken && !!s.user));
  userId$ = this.state$.pipe(map((s) => s.user?.id ?? null));
  role$ = this.state$.pipe(map((s) => s.user?.role ?? null));
  user$ = this.state$;

  constructor(private httpClient: HttpClient) {
    this.hydrateFromStorage();
  }

  get isLoggedIn(): boolean {
    return !!this._state$.value.accessToken && !!this._state$.value.user;
  }
  get userId(): string | null {
    return this._state$.value.user?.id ?? null;
  }
  get role(): string | null {
    return this._state$.value.user?.role ?? null;
  }
  get accessToken(): string | null {
    return this._state$.value.accessToken;
  }

  register(body: RegisterRequestDTO): Observable<any> {
    return this.httpClient.post<any>(`${this.urlCore}/register`, body);
  }

  login(body: LoginRequestDTO): Observable<BasicUser> {
    return this.httpClient
      .post<LoginResponseDTO>(`${this.urlCore}/login`, body)
      .pipe(
        tap((res) => this.applyLoginResponse(res)),
        map((res) => this._state$.value.user!)
      );
  }

  refresh(): Observable<void> {
    const rt = this._state$.value.refreshToken;
    if (!rt) return throwError(() => new Error('Missing refresh token'));

    const body: RefreshRequestDTO = { refreshToken: rt };
    return this.httpClient
      .post<RefreshResponseDTO>(`${this.urlCore}/refresh`, body)
      .pipe(
        tap((res) => this.applyRefreshResponse(res)),
        map(() => void 0)
      );
  }

  logout(): Observable<void> {
    return this.httpClient
      .post<void>(`${this.urlCore}/logout`, {}, {})
      .pipe(finalize(() => this.clearState()));
  }

  logoutAll(): Observable<void> {
    return this.httpClient
      .post<void>(`${this.urlCore}/logout-all`, {}, {})
      .pipe(finalize(() => this.clearState()));
  }

  private applyLoginResponse(res: LoginResponseDTO) {
    const state: AuthState = {
      accessToken: res.accessToken,
      accessExpiresAt: res.accessExpiresAt,
      refreshToken: res.refreshToken,
      refreshExpiresAt: res.refreshExpiresAt,
      user: {
        id: res.userId,
        role: res.role,
        name: res.name ?? null,
        surname: res.surname ?? null,
      },
    };
    this.setState(state);
    this.scheduleRefresh();
  }

  private applyRefreshResponse(res: RefreshResponseDTO) {
    // Role can be read from JWT
    const claims = this.decodeJwt(res.accessToken);
    const roleFromJwt = (claims['role'] ||
      claims[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ]) as string | undefined;

    const prevUser = this._state$.value.user;
    const state: AuthState = {
      accessToken: res.accessToken,
      accessExpiresAt: res.accessExpiresAt,
      refreshToken: res.refreshToken,
      refreshExpiresAt: res.refreshExpiresAt,
      user: {
        id: res.userId,
        role: roleFromJwt ?? prevUser?.role ?? '',
        name: prevUser?.name ?? null,
        surname: prevUser?.surname ?? null,
      },
    };
    this.setState(state);
    this.scheduleRefresh();
  }

  private setState(state: AuthState) {
    this._state$.next(state);
    localStorage.setItem(AuthService.STORAGE_KEY, JSON.stringify(state));
  }

  private clearState() {
    this._state$.next({
      accessToken: null,
      accessExpiresAt: null,
      refreshToken: null,
      refreshExpiresAt: null,
      user: null,
    });
    localStorage.removeItem(AuthService.STORAGE_KEY);
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
      this.refreshTimer = null;
    }
  }

  private hydrateFromStorage() {
    try {
      const raw = localStorage.getItem(AuthService.STORAGE_KEY);
      if (!raw) return;
      const saved: AuthState = JSON.parse(raw);
      if (saved?.accessToken && saved?.accessExpiresAt) {
        // If already expired, don’t restore
        const msLeft = new Date(saved.accessExpiresAt).getTime() - Date.now();
        if (msLeft > 0) {
          this._state$.next(saved);
          this.scheduleRefresh();
        } else {
          // Try refresh immediately if rheres still a refresh token
          this._state$.next(saved);
          if (saved.refreshToken)
            this.refresh().subscribe({ error: () => this.clearState() });
          else this.clearState();
        }
      }
    } catch {
      this.clearState();
    }
  }

  private scheduleRefresh() {
    if (this.refreshTimer) clearTimeout(this.refreshTimer);

    const expIso = this._state$.value.accessExpiresAt;
    if (!expIso) return;

    // refresh a little before expiry
    const SKEW_MS = 30_000; // 30s
    const dueIn = new Date(expIso).getTime() - Date.now() - SKEW_MS;

    if (dueIn <= 0) {
      // already due – kick off refresh now
      this.refresh().subscribe({ error: () => this.clearState() });
      return;
    }

    this.refreshTimer = setTimeout(() => {
      this.refresh().subscribe({ error: () => this.clearState() });
    }, Math.min(dueIn, 2_147_000_000)); // clamp to 24 days
  }

  private decodeJwt(token: string): Record<string, unknown> {
    try {
      const [, payload] = token.split('.');
      const json = atob(this.base64UrlToBase64(payload));
      return JSON.parse(json);
    } catch {
      return {};
    }
  }

  private base64UrlToBase64(input: string): string {
    let b64 = input.replace(/-/g, '+').replace(/_/g, '/');
    const pad = b64.length % 4;
    if (pad === 2) b64 += '==';
    else if (pad === 3) b64 += '=';
    else if (pad !== 0) b64 += '===';
    return b64;
  }

  private refreshInFlight$?: Observable<void>;

  private msUntilAccessExpiry(): number {
    const expIso = this._state$.value.accessExpiresAt;
    return expIso ? new Date(expIso).getTime() - Date.now() : -Infinity;
  }

  isAccessTokenFresh(skewMs = 30_000): boolean {
    return this.msUntilAccessExpiry() > skewMs;
  }

  // ensure theres a fresh access token, refreshing if needed
  ensureValidAccessToken(skewMs = 30_000): Observable<void> {
    if (this.isAccessTokenFresh(skewMs)) return of(void 0);
    if (this.refreshInFlight$) return this.refreshInFlight$;

    this.refreshInFlight$ = this.refresh().pipe(
      catchError((err) => {
        this.clearState();
        return throwError(() => err);
      }),
      finalize(() => {
        this.refreshInFlight$ = undefined;
      }),
      shareReplay({ bufferSize: 1, refCount: true })
    );
    return this.refreshInFlight$;
  }
}

import {HttpClient} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {AuthState} from '../../models/AuthState';
import {BasicUser} from '../../models/BasicUser';
import {LoginRequest} from '../../models/LoginRequest';
import {LoginResponse} from '../../models/LoginResponse';
import {RegisterRequest} from '../../models/RegisterRequest';
import {RefreshRequest} from '../../models/RefreshRequest';
import {RefreshResponse} from '../../models/RefreshResponse';
import {BehaviorSubject, Observable, map, tap, throwError, of} from 'rxjs';
import {finalize, shareReplay, catchError} from 'rxjs/operators';
import {Role} from '../../models/Role';
import {ConfirmEmailResponse} from '../../models/ConfirmEmailResponse';
import {CreateCaUser} from '../../models/CreateCaUser';
import {RegisterResponse} from '../../models/RegisterResponse';

@Injectable({providedIn: 'root'})
export class AuthService {
  httpClient = inject(HttpClient)
  urlCore = 'https://localhost:8081/api/v1/users';

  private static STORAGE_KEY = 'auth.state.v1';

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

  constructor() {
    this.hydrateFromStorage();
  }

  get isLoggedIn(): boolean {
    return !!this._state$.value.accessToken && !!this._state$.value.user;
  }

  get userId(): string | null {
    return this._state$.value.user?.id ?? null;
  }

  get role(): Role | null {
    return this._state$.value.user?.role ?? null;
  }

  get accessToken(): string | null {
    return this._state$.value.accessToken;
  }

  register(body: RegisterRequest): Observable<RegisterResponse> {
    return this.httpClient.post<RegisterResponse>(`${this.urlCore}/register`, body);
  }

  registerCaUser(body: CreateCaUser): Observable<RegisterResponse> {
    return this.httpClient.post<RegisterResponse>(`${this.urlCore}/register-ca`, body);
  }

  login(body: LoginRequest): Observable<BasicUser> {
    return this.httpClient
      .post<LoginResponse>(`${this.urlCore}/login`, body)
      .pipe(
        tap((res) => this.applyLoginResponse(res)),
        map(() => this._state$.value.user!)
      );
  }

  refresh(): Observable<void> {
    const rt = this._state$.value.refreshToken;
    if (!rt) return throwError(() => new Error('Missing refresh token'));

    const body: RefreshRequest = {refreshToken: rt};
    return this.httpClient
      .post<RefreshResponse>(`${this.urlCore}/refresh`, body)
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

  private applyLoginResponse(res: LoginResponse) {
    const state: AuthState = {
      accessToken: res.accessToken,
      accessExpiresAt: res.accessExpiresAt,
      refreshToken: res.refreshToken,
      refreshExpiresAt: res.refreshExpiresAt,
      user: {
        id: res.userId,
        role: res.role as Role,
        name: res.name ?? null,
        surname: res.surname ?? null,
      },
    };
    this.setState(state);
  }

  private applyRefreshResponse(res: RefreshResponse) {
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
        role: (roleFromJwt ?? prevUser?.role ?? '') as Role,
        name: prevUser?.name ?? null,
        surname: prevUser?.surname ?? null,
      },
    };
    this.setState(state);
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
        } else {
          // Try refresh immediately if rheres still a refresh token
          this._state$.next(saved);
          if (saved.refreshToken)
            this.refresh().subscribe({error: () => this.clearState()});
          else this.clearState();
        }
      }
    } catch {
      this.clearState();
    }
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

  // ensure there's a fresh access token, refreshing if needed
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
      shareReplay({bufferSize: 1, refCount: true})
    );
    return this.refreshInFlight$;
  }

  confirmEmail(token: string): Observable<ConfirmEmailResponse> {
    return this.httpClient.get<ConfirmEmailResponse>(`${this.urlCore}/confirm`, {params: {token}});
  }
}

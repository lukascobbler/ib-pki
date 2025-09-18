import { Injectable } from '@angular/core';
import {
  HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { catchError, switchMap } from 'rxjs/operators';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private auth: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const isApi = req.url.startsWith('https://localhost:8081/');
    const isAuthEndpoint =
      req.url.includes('/api/users/login') ||
      req.url.includes('/api/users/register') ||
      req.url.includes('/api/users/confirm') ||
      req.url.includes('/api/users/refresh');

    // Skip non-API and auth endpoints
    if (!isApi || isAuthEndpoint) {
      return next.handle(req);
    }

    // Ensure token is fresh before the request
    return this.auth.ensureValidAccessToken().pipe(
      switchMap(() => {
        const token = this.auth.accessToken;
        const authReq = token
          ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
          : req;

        // Send, and if its 401 still, try one refresh + retry once
        let retried = false;
        return next.handle(authReq).pipe(
          catchError((err: HttpErrorResponse) => {
            const shouldRetry = !retried && err.status === 401;
            retried = true;

            if (shouldRetry) {
              return this.auth.ensureValidAccessToken().pipe(
                switchMap(() => {
                  const token2 = this.auth.accessToken;
                  const retryReq = token2
                    ? authReq.clone({ setHeaders: { Authorization: `Bearer ${token2}` } })
                    : authReq;
                  return next.handle(retryReq);
                })
              );
            }
            return throwError(() => err);
          })
        );
      })
    );
  }
}

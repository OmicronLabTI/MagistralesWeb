import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError, TimeoutError } from 'rxjs';
import { catchError, flatMap, timeout } from 'rxjs/operators';
import { DataService } from '../services/data.service';
import { TokenExcludedEndpoints } from 'src/environments/endpoints';
import { AppConfig } from '../constants/app-config';
import { CONST_STRING, HttpStatus } from '../constants/const';
import { ErrorHttpInterface } from '../model/http/commons';
import { SecurityService } from '../services/security.service';
import { ILoginRes } from '../model/http/security.model';
import { LocalStorageService } from '../services/local-storage.service';

const DEFAULT_TIMEOUT = 250000;

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  retries = 0;

  constructor(
    private dataService: DataService,
    private securityService: SecurityService,
    private localStorageService: LocalStorageService) { }

  private applyCredentials = (req: HttpRequest<any>) => {
    const token = this.localStorageService.getToken();
    if ((token && token !== CONST_STRING.empty) && !this.endpointExcluded(req.url)) {
      req = req.clone({
        setHeaders: {
          Accept: 'application/json',
          'Content-Type': `application/json`,
          Authorization: `Bearer ${token}`
        }
      });
    }
    return req;
  }

  private endpointExcluded(url: string): boolean {
    const excluded = TokenExcludedEndpoints.find(endpoint => {
      return url.includes(endpoint);
    }) || [];
    return excluded.length > 0;
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authReq = this.applyCredentials(req);

    return next.handle(authReq).pipe(
      timeout(AppConfig.httpTimeout || DEFAULT_TIMEOUT),
      catchError((error: ErrorHttpInterface) => {
        if (error instanceof TimeoutError) {
          return throwError({ status: HttpStatus.timeOut } as ErrorHttpInterface);
        } else {
          if (this.retries > 0) {
            this.retries = 0;
            return throwError(error);
          }
          if (error.status === HttpStatus.notFound && this.retries === 0) {
            this.retries++;
            return next.handle(this.applyCredentials(req));
          }
          if ((error.status === HttpStatus.unauthorized)
            && this.localStorageService.getRefreshToken() !== CONST_STRING.empty
            && this.localStorageService.getRememberSession() !== null) {
            return this.securityService.refreshToken().pipe(
              catchError(err => {
                return throwError(err);
              }),
              flatMap((resRefresh: ILoginRes) => {
                this.localStorageService.setToken(resRefresh.access_token);
                this.localStorageService.setRefreshToken(resRefresh.refresh_token);
                return next.handle(this.applyCredentials(req));
              })
            );
          }
          return throwError(error);
        }
      })
    );
  }
}

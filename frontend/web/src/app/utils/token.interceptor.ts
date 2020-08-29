import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError, TimeoutError } from 'rxjs';
import {catchError, flatMap, timeout} from 'rxjs/operators';

import { Messages } from '../constants/messages';
import { DataService } from '../services/data.service';
import { TokenExcludedEndpoints } from 'src/environments/endpoints';
import { AppConfig } from '../constants/app-config';
import {CONST_STRING, HttpStatus} from '../constants/const';
import {ErrorHttpInterface} from '../model/http/commons';
import {SecurityService} from '../services/security.service';
import {ILoginRes} from '../model/http/security.model';

const DEFAULT_TIMEOUT = 40000;

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(private dataService: DataService, private securityService: SecurityService) { }

  private applyCredentials = (req: HttpRequest<any>) => {
    const token = this.dataService.getToken();
    if ((token && token !== CONST_STRING.empty) && !this.endpointExcluded(req.url)) {
      req = req.clone({
        setHeaders: {
          Accept: 'application/json',
          'Content-Type': `application/json`,
          Authorization: `Bearer ${token}`
        }
      });
    }
    console.log('req1: ', req);
    return req;
  }

  private endpointExcluded(url: string): boolean {
    const excluded = TokenExcludedEndpoints.find(endpoint => {
      return url.includes(endpoint);
    }) || [];
    return excluded.length > 0;
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const authReq = this.applyCredentials(req);

    return next.handle(authReq).pipe(
      timeout(AppConfig.httpTimeout || DEFAULT_TIMEOUT),
      catchError((error: ErrorHttpInterface) => {
        if (error instanceof TimeoutError) {
          return throwError({status: HttpStatus.timeOut} as ErrorHttpInterface);
        } else {
         if (error.status === HttpStatus.unauthorized && this.dataService.getRefreshToken() !== CONST_STRING.empty
              && this.dataService.getRememberSession() !== null) {
            return this.securityService.refreshToken().pipe(
                catchError(err => {
                  return throwError(err);
                }),
                flatMap((resRefresh: ILoginRes) => {
                  this.dataService.setToken(resRefresh.access_token);
                  this.dataService.setRefreshToken(resRefresh.refresh_token);
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

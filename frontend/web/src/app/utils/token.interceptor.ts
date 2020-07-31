import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError, TimeoutError } from 'rxjs';
import { catchError, timeout } from 'rxjs/operators';

import { Messages } from '../constants/messages';
import { DataService } from '../services/data.service';
import { TokenExcludedEndpoints } from 'src/environments/endpoints';
import { AppConfig } from '../constants/app-config';

let DEFAULT_TIMEOUT = 30000;

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  constructor(private _dataService: DataService) { }

  private applyCredentials = (req: HttpRequest<any>) => {
    const token = this._dataService.getToken();

    if (token && !this.endpointExcluded(req.url)) {
      req = req.clone({ headers: req.headers.append('token', token) });
    }
    console.log('req: ', req)
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
      // Log de peticiones HTTP
      // map((event: HttpEvent<any>) => {
      //   if (event instanceof HttpResponse) {
      //     console.log('event--->>>', event);
      //   }
      //   return event;
      // }),
      catchError((error) => {
        if (error instanceof TimeoutError) {
          return throwError(Messages.timeout);
        }
        if (error instanceof HttpErrorResponse) {
          return throwError(error.error.message || error.message || Messages.generic);
        } else {
          return throwError(error);
        }
      })
    );
  }
}

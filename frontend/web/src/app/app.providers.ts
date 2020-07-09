import { MAT_DATE_LOCALE } from '@angular/material';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from './utils/token.interceptor';

export const APP_PROVIDERS = [
  { provide: MAT_DATE_LOCALE, useValue: 'es-MX' },
  {
    provide: HTTP_INTERCEPTORS,
    useClass: TokenInterceptor,
    multi: true
  }
];

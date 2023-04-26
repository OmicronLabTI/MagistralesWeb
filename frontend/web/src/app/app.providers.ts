import { MAT_DATE_LOCALE } from '@angular/material';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from './utils/token.interceptor';
import {CustomPaginator} from './services/CustomPaginatorConfiguration';
import {MatPaginatorIntl} from '@angular/material/paginator';
import {DatePipe} from '@angular/common';
import { MAT_DATE_FORMATS, DateAdapter } from 'saturn-datepicker';
import { AppDateAdapter } from './utils/date.adapter';

export const MY_FORMATS = {
  parse: {
    dateInput: 'YYYY-MM-DD'
  },
  display: {
    dateInput: 'YYYY-MM-DD',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY'
  }
};

export const APP_PROVIDERS = [
  { provide: MAT_DATE_LOCALE, useValue: 'es-MX' },
  {
    provide: HTTP_INTERCEPTORS,
    useClass: TokenInterceptor,
    multi: true
  },
  { provide: MatPaginatorIntl, useValue: CustomPaginator() },
    DatePipe,
    { provide: DateAdapter, useClass: AppDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS }
];

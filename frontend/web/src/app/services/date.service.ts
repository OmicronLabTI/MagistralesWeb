import { DatePipe } from '@angular/common';
import { Injectable } from '@angular/core';
import { CONST_NUMBER, MODAL_FIND_ORDERS } from '../constants/const';

@Injectable({
  providedIn: 'root'
})
export class DateService {

  constructor(private datePipe: DatePipe) { }

  getDateFormatted(
    initDate: Date,
    finishDate: Date,
    isBeginDate: boolean,
    isProductivity: boolean = false,
    numberCustomRange: number = CONST_NUMBER.lessOne
  ) {
    if (isBeginDate) {
      if (isProductivity) {
        initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.ninetyDays);
      } else {
        initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.thirtyDays);
      }
    }
    if (
      numberCustomRange !== CONST_NUMBER.lessOne &&
      numberCustomRange > CONST_NUMBER.zero
    ) {
      initDate = new Date(
        initDate.getTime() - MODAL_FIND_ORDERS.operationDay * numberCustomRange
      );
    }
    return `${this.transformDate(initDate)}-${this.transformDate(finishDate)}`;
  }
  transformDate(date: Date, isSecondFormat: boolean = false) {
    if (!isSecondFormat) {
      return this.datePipe.transform(date, 'dd/MM/yyyy');
    } else {
      return this.datePipe.transform(date, 'yyyy-MM-dd');
    }
  }

  getDateArray(startDate: Date) {
    return this.transformDate(startDate).split('/');
  }

  getMaxMinDate(date: Date, moths: number, isAdd: boolean) {
    return new Date(
      date.getFullYear(),
      isAdd ? date.getMonth() + moths : date.getMonth() - moths,
      date.getDate()
    );
  }
}

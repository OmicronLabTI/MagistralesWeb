import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import Swal, {SweetAlertIcon} from 'sweetalert2';
import {CONST_NUMBER, CONST_STRING, HttpServiceTOCall} from '../constants/const';
import {DatePipe} from '@angular/common';
import {QfbWithNumber} from '../model/http/users';
import {GeneralMessage} from '../model/device/general';


@Injectable({
  providedIn: 'root'
})
export class DataService {
  private isLoading = new Subject<boolean>();
  private generalNotificationMessage = new Subject<string>();
  private isLogin = new Subject<boolean>();
  private qfbTOPlace = new Subject<QfbWithNumber>();
  private callHttpService = new Subject<HttpServiceTOCall>();
  private messageGenericCallHttp = new Subject<GeneralMessage>();
  constructor(private datePipe: DatePipe) { }

  setMessageGeneralCallHttp(messageGeneral: GeneralMessage) {
    this.messageGenericCallHttp.next(messageGeneral);
  }
  getMessageGeneralCalHttp() {
    return this.messageGenericCallHttp.asObservable();
  }
  setCallHttpService(numberServiceToCall: HttpServiceTOCall) {
    this.callHttpService.next(numberServiceToCall);
  }
  getCallHttpService() {
    return this.callHttpService.asObservable();
  }
  setIsLogin(isLogin: boolean) {
    this.isLogin.next(isLogin);
  }

  getIsLogin() {
    return this.isLogin.asObservable();
  }
  setIsLoading(loading: boolean) {
    this.isLoading.next(loading);
  }

  getIsLoading() {
    return this.isLoading.asObservable();
  }

  getGeneralNotificationMessage() {
    return this.generalNotificationMessage.asObservable();
  }
  setQbfToPlace(qfb: QfbWithNumber) {
    this.qfbTOPlace.next(qfb);
  }
  getQfbToPlace() {
    return this.qfbTOPlace.asObservable();
  }
  setGeneralNotificationMessage(msg: string) {
    this.generalNotificationMessage.next(msg);
  }

  getToken(): string {
    return sessionStorage.getItem('token');
  }

  setToken(token: string) {
    sessionStorage.setItem('token', token);
  }
  clearToken() {
    sessionStorage.removeItem('token');
  }

  setUserId(userId: string) {
    sessionStorage.setItem('userId', userId);
  }

  getUserId() {
    return sessionStorage.getItem('userId');
  }

  setUserName(userName: string) {
    sessionStorage.setItem('userName', userName);
  }

  getUserName() {
    return sessionStorage.getItem('userName');
  }


  userIsAuthenticated(): boolean {
    return !!sessionStorage.getItem('token');
  }
  presentToastCustom(title: string, icon: SweetAlertIcon, text: string = CONST_STRING.empty,
                     showConfirmButton: boolean = false, showCancelButton: boolean = false) {
    return new Promise (resolve => {
      Swal.fire({
        title,
        text,
        icon,
        timer: showConfirmButton ? CONST_NUMBER.zero : CONST_NUMBER.timeToast,
        showConfirmButton,
        showCancelButton,
        heightAuto: false,
        confirmButtonText: 'Aceptar',
        cancelButtonText: 'Cancelar',
        buttonsStyling: false,
        customClass: {
          container: 'swal2-actions',
          confirmButton: 'confirm-button-class',
          cancelButton: 'cancel-button-class',
        }
      }).then((result) => resolve(result));
    });
  }
  transformDate(date: Date) {
    return this.datePipe.transform(date, 'dd/MM/yyyy');
  }
}

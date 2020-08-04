import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
// const Swal = require('sweetalert2');
import Swal, {SweetAlertIcon} from 'sweetalert2';
import {CONST_NUMBER, CONST_STRING} from '../constants/const';


@Injectable({
  providedIn: 'root'
})
export class DataService {
  private isLoading = new Subject<boolean>();
  private generalNotificationMessage = new Subject<string>();
  private isLogin = new Subject<boolean>();
  // private isCallToUsersList = new Subject()
  constructor() { }

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
}

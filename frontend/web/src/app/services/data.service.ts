import {Injectable} from '@angular/core';
import {Subject} from 'rxjs';
import Swal, {SweetAlertIcon} from 'sweetalert2';
import {CONST_NUMBER, CONST_STRING, HttpServiceTOCall, MessageType} from '../constants/const';
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
  private isToSaVeAnything = false;
  private urlActive = new Subject<HttpServiceTOCall>();
  constructor(private datePipe: DatePipe) { }

  setIsToSaveAnything(isToSave: boolean) {
    this.isToSaVeAnything = isToSave;
  }
  getIsToSaveAnything() {
    return this.isToSaVeAnything;
  }
   setUrlActive(url: HttpServiceTOCall) {
    this.urlActive.next(url);
   }
   getUrlActive() {
    return this.urlActive;
   }
  setDetailOrderDescription(description: string) {
    localStorage.setItem('descriptionDetail', description || CONST_STRING.empty);
  }
  getDetailOrderDescription(): string {
    return localStorage.getItem('descriptionDetail');
  }
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
    return localStorage.getItem('token-omi');
  }

  setToken(token: string) {
    localStorage.setItem('token-omi', token);
  }
  clearToken() {
    localStorage.removeItem('token-omi');
  }

  setUserId(userId: string) {
    localStorage.setItem('userId', userId);
  }

  getUserId() {
    return localStorage.getItem('userId');
  }

  setUserName(userName: string) {
    localStorage.setItem('userName', userName);
  }

  getUserName() {
    return localStorage.getItem('userName');
  }


  userIsAuthenticated(): boolean {
    return !!localStorage.getItem('token-omi');
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
  getMessageTitle(itemsWithError: string[], messageType: MessageType): string {
    let errorOrders = '';
    let firstMessage = '';
    let finishMessaje = '';
    switch (messageType) {
      case MessageType.processOrder:
        firstMessage = 'El producto ';
        finishMessaje = 'no pudo ser Planificado \n';
        break;
      case MessageType.placeOrder:
        firstMessage = 'La orden de fabricación ';
        finishMessaje = 'no pudo ser Asignada \n';
        break;
      case MessageType.cancelOrder:
        firstMessage = 'El producto ';
        finishMessaje = 'no pudo ser Cancelado \n';
        break;
    }

    itemsWithError.forEach(order => {
      errorOrders += `${firstMessage} ${order} ${finishMessaje}`;
    });
    return errorOrders;
  }
}

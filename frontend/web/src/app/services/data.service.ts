import {Injectable} from '@angular/core';
import {Subject} from 'rxjs';
import Swal, {SweetAlertIcon} from 'sweetalert2';
import {CONST_NUMBER, CONST_STRING, ConstToken, HttpServiceTOCall, MessageType} from '../constants/const';
import {DatePipe} from '@angular/common';
import {QfbWithNumber} from '../model/http/users';
import {GeneralMessage} from '../model/device/general';
import {CancelOrders, SearchComponentModal} from '../model/device/orders';
import {CancelOrderReq} from '../model/http/pedidos';

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
  private cancelOrders = new Subject<CancelOrders>();
  private finalizeOrders = new Subject<CancelOrders>();
  private pathUrl = new Subject<any[]>();
  private isLogout = new Subject<boolean>();
  private searchComponentModal = new Subject<SearchComponentModal>();
  private newFormulaComponent = new Subject<any>();
  constructor(private datePipe: DatePipe) { }

  setNewFormulaComponent(newFormulaComponent: any) {
    this.newFormulaComponent.next(newFormulaComponent);
  }
  getNewFormulaComponent() {
    return this.newFormulaComponent.asObservable();
  }
  setSearchComponentModal(searchComponentModal: SearchComponentModal) {
    this.searchComponentModal.next(searchComponentModal);
  }
  getSearchComponentModal() {
    return this.searchComponentModal.asObservable();
  }
  setIsLogout(isLogout: boolean) {
    this.isLogout.next(isLogout);
  }
  getIsLogout() {
    return this.isLogout.asObservable();
  }
  setRememberSession(rememberSession: string) {
    localStorage.setItem(ConstToken.rememberSession, rememberSession);
  }
  getRememberSession() {
    return localStorage.getItem(ConstToken.rememberSession);
  }
  setRefreshToken(refreshToken: string) {
    localStorage.setItem(ConstToken.refreshToken, refreshToken);
  }
  getRefreshToken() {
    return localStorage.getItem(ConstToken.refreshToken);
  }

  setPathUrl(pathUrl: any[]) {
    this.pathUrl.next(pathUrl);
  }
  getPathUrl() {
    return this.pathUrl.asObservable();
  }
  setFinalizeOrders(finalizeOrders: CancelOrders) {
    this.finalizeOrders.next(finalizeOrders);
  }
  getFinalizeOrders() {
    return this.finalizeOrders.asObservable();
  }
  setCancelOrders(cancelOrder: CancelOrders) {
    this.cancelOrders.next(cancelOrder);
  }
  getCancelOrder() {
    return this.cancelOrders.asObservable();
  }
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
    return localStorage.getItem(ConstToken.accessToken);
  }
  setToken(token: string) {
    localStorage.setItem(ConstToken.accessToken, token);
  }
  clearSession() {
    localStorage.removeItem(ConstToken.accessToken);
    localStorage.removeItem(ConstToken.rememberSession);
    localStorage.removeItem(ConstToken.refreshToken);
    localStorage.removeItem(ConstToken.userId);
    localStorage.removeItem(ConstToken.userName);
  }

  setUserId(userId: string) {
    localStorage.setItem(ConstToken.userId, userId);
  }

  getUserId() {
    return localStorage.getItem(ConstToken.userId);
  }

  setUserName(userName: string) {
    localStorage.setItem(ConstToken.userName, userName);
  }

  getUserName() {
    return localStorage.getItem(ConstToken.userName);
  }


  userIsAuthenticated(): boolean {
    return !!localStorage.getItem(ConstToken.accessToken);
  }
  presentToastCustom(title: string, icon: SweetAlertIcon, text: string = CONST_STRING.empty,
                     showConfirmButton: boolean = false, showCancelButton: boolean = false, popupCustom = CONST_STRING.empty) {
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
          popup: popupCustom,
          confirmButton: 'confirm-button-class',
          cancelButton: 'cancel-button-class',
        }
      }).then((result) => resolve(result));
    });
  }
  transformDate(date: Date) {
    return this.datePipe.transform(date, 'dd/MM/yyyy');
  }
  getMessageTitle(itemsWithError: any[], messageType: MessageType, isFromCancel = false): string {
    let errorOrders = '';
    let firstMessage = '';
    let finishMessaje = '';
    switch (messageType) {
      case MessageType.processOrder:
        firstMessage = 'El producto ';
        finishMessaje = 'no pudo ser Planificado \n';
        break;
      case MessageType.processDetailOrder:
        firstMessage = 'La orden de fabricación ';
        finishMessaje = 'no pudo ser Planificado \n';
        break;
      case MessageType.placeOrder:
        firstMessage = 'La orden de fabricación ';
        finishMessaje = 'no pudo ser Asignada \n';
        break;
      case MessageType.saveBatches:
        firstMessage = 'Error al asignar lotes a ';
        finishMessaje = ', por favor verificar \n';
        break;
    }
    if (!isFromCancel) {
      itemsWithError.forEach((order: string) => {
        errorOrders += `${firstMessage} ${order} ${finishMessaje}`;
      });
    } else {
      itemsWithError.forEach((order: CancelOrderReq) => {
        errorOrders += `${order.reason} \n`;
      });
    }
    return errorOrders;
  }
}

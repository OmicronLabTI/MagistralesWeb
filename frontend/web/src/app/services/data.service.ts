import {Injectable} from '@angular/core';
import {Subject} from 'rxjs';
import Swal, {SweetAlertIcon} from 'sweetalert2';
import {
  CONST_NUMBER,
  CONST_STRING,
  ConstOrders,
  ConstStatus,
  ConstToken,
  FromToFilter,
  HttpServiceTOCall,
  MessageType,
  MODAL_FIND_ORDERS
} from '../constants/const';
import {DatePipe} from '@angular/common';
import {QfbWithNumber} from '../model/http/users';
import {GeneralMessage} from '../model/device/general';
import {CancelOrders, SearchComponentModal} from '../model/device/orders';
import {CancelOrderReq, ParamsPedidos} from '../model/http/pedidos';

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
  private searchOrdersModal = new Subject<SearchComponentModal>();
  private newSearchOrdersParams = new Subject<ParamsPedidos>();
  constructor(private datePipe: DatePipe) { }

  setNewSearchOrderModal(searchOrdersParams: ParamsPedidos) {
    this.newSearchOrdersParams.next(searchOrdersParams);
  }
  getNewSearchOrdersModal() {
    return this.newSearchOrdersParams.asObservable();
  }
  setSearchOrdersModal(searchOrder: SearchComponentModal) {
    this.searchOrdersModal.next(searchOrder);
  }
  getSearchOrdersModal() {
    return this.searchOrdersModal.asObservable();
  }
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
    localStorage.removeItem(ConstToken.isolatedOrder);
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
  presentToastCustom(title: string, icon: SweetAlertIcon, html: string = CONST_STRING.empty,
                     showConfirmButton: boolean = false, showCancelButton: boolean = false, popupCustom = CONST_STRING.empty) {
    return new Promise (resolve => {
      Swal.fire({
        title,
        html,
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
  getDateFormatted(initDate: Date, finishDate: Date, isBeginDate: boolean, isProductivity: boolean = false) {
    if (isBeginDate) {
      if (isProductivity) {
        initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.ninetyDays);
      } else {
        initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.thirtyDays);
      }
    }
    return `${this.transformDate(initDate)}-${this.transformDate(finishDate)}`;
  }
  transformDate(date: Date, isTest: boolean = false) {
    if (!isTest) {
      return this.datePipe.transform(date, 'dd/MM/yyyy');
    } else {
      return this.datePipe.transform(date, 'yyyy-MM-dd');
    }
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
  getIsThereOnData(dataToSearch: any[], status: string, fromToFilter: FromToFilter) {
    switch (fromToFilter) {
      case FromToFilter.fromOrders:
        return dataToSearch.filter(t => (t.isChecked && t.pedidoStatus === status)).length > 0;
      case FromToFilter.fromOrdersReassign:
        return dataToSearch.filter(t => (t.isChecked && (t.pedidoStatus === status
            || t.pedidoStatus === ConstStatus.terminado))).length > 0;
      case FromToFilter.fromOrdersCancel:
        return dataToSearch.filter(t => (t.isChecked &&
            (t.pedidoStatus !== status && t.pedidoStatus !== ConstStatus.cancelado))).length > 0;
      case FromToFilter.fromDetailOrder:
        return dataToSearch.filter(t => t.isChecked && (t.status !== status && t.status !== ConstStatus.cancelado
            && t.status !== ConstStatus.abierto)).length > 0;
      case FromToFilter.fromOrderIsolatedReassign:
        return dataToSearch.filter(t => t.isChecked && (t.status === status || t.status === ConstStatus.asignado
            || t.status.toLowerCase() === ConstStatus.enProceso.toLowerCase() || t.status === ConstStatus.pendiente
            || t.status === ConstStatus.terminado)).length > 0;
      case FromToFilter.fromOrdersIsolatedCancel:
        return dataToSearch.filter(t => (t.isChecked &&
            (t.status !== status && t.status !== ConstStatus.cancelado))).length > 0;
      default:
        return dataToSearch.filter(t => (t.isChecked && t.status === status)).length > 0;
    }
  }
  getItemOnDateWithFilter(dataToSearch: any[], fromToFilter: FromToFilter, status?: string) {
    switch (fromToFilter) {
      case FromToFilter.fromOrderIsolatedReassignItems:
        return dataToSearch.filter(t => (t.isChecked && (t.status === ConstStatus.reasingado || t.status === ConstStatus.asignado
            || t.status.toLowerCase() === ConstStatus.enProceso.toLowerCase() || t.status === ConstStatus.pendiente
            || t.status === ConstStatus.terminado)));
      case FromToFilter.fromOrdersReassign:
        return dataToSearch.filter(t => (t.isChecked && (t.pedidoStatus === status || t.pedidoStatus === ConstStatus.terminado)));
      default:
        return dataToSearch.filter(t => (t.isChecked && t.status === status));
    }
  }
  getIsWithFilter(resultSearchOrderModal: ParamsPedidos) {
    let isSearchWithFilter = false;
    if ((resultSearchOrderModal && resultSearchOrderModal.dateType === ConstOrders.defaultDateInit) &&
        (resultSearchOrderModal && resultSearchOrderModal.status === '' || resultSearchOrderModal.qfb === ''
            || resultSearchOrderModal.productCode === '' || resultSearchOrderModal.clientName === '')) {
      isSearchWithFilter = false;
    }
    if ((resultSearchOrderModal && resultSearchOrderModal.dateType === ConstOrders.defaultDateInit) &&
        (resultSearchOrderModal && resultSearchOrderModal.status !== '' || resultSearchOrderModal.qfb !== ''
            || resultSearchOrderModal.productCode !== '' || resultSearchOrderModal.clientName !== '')) {
      isSearchWithFilter = true;
    }
    if ((resultSearchOrderModal && resultSearchOrderModal.dateType === ConstOrders.dateFinishType) &&
        (resultSearchOrderModal && resultSearchOrderModal.status !== '' || resultSearchOrderModal.qfb !== ''
            || resultSearchOrderModal.productCode !== '' || resultSearchOrderModal.clientName !== '')) {
      isSearchWithFilter = true;
    }
    if ((resultSearchOrderModal && resultSearchOrderModal.dateType === ConstOrders.dateFinishType) &&
        (resultSearchOrderModal && resultSearchOrderModal.status === '' || resultSearchOrderModal.qfb === ''
            || resultSearchOrderModal.productCode === '' || resultSearchOrderModal.clientName === '')) {
      isSearchWithFilter = true;
    }
    if (resultSearchOrderModal && resultSearchOrderModal.docNum !== '') {
      isSearchWithFilter = true;
    }

    return isSearchWithFilter;
  }

  getNewDataToFilter(resultSearchOrderModal: ParamsPedidos): [ParamsPedidos, string] {
    let queryString = CONST_STRING.empty;
    let rangeDate = CONST_STRING.empty;
    const filterDataOrders = new  ParamsPedidos();
    filterDataOrders.isFromOrders = resultSearchOrderModal.isFromOrders;

    if (resultSearchOrderModal.docNum) {
      filterDataOrders.docNum = resultSearchOrderModal.docNum;
      filterDataOrders.dateFull = this.getDateFormatted(new Date(), new Date(), true);
      queryString = `?docNum=${resultSearchOrderModal.docNum}`;
    } else {
      if (resultSearchOrderModal.dateType) {
        filterDataOrders.dateType = resultSearchOrderModal.dateType;
        rangeDate = this.getDateFormatted(resultSearchOrderModal.fini, resultSearchOrderModal.ffin, false);
        if ( resultSearchOrderModal.dateType === ConstOrders.defaultDateInit) {
          queryString = `?fini=${rangeDate}`;
        } else {
          queryString = `?ffin=${rangeDate}`;
        }
        filterDataOrders.dateFull = rangeDate;
      }
      if (resultSearchOrderModal.status !== '' && resultSearchOrderModal.status) {
        queryString = `${queryString}&status=${resultSearchOrderModal.status}`;
        filterDataOrders.status = resultSearchOrderModal.status;
      }
      if (resultSearchOrderModal.qfb !== '' && resultSearchOrderModal.qfb) {
        queryString = `${queryString}&qfb=${resultSearchOrderModal.qfb}`;
        filterDataOrders.qfb = resultSearchOrderModal.qfb;
      }
      if (resultSearchOrderModal.productCode !== '' && resultSearchOrderModal.productCode) {
        queryString = `${queryString}&code=${resultSearchOrderModal.productCode}`;
        filterDataOrders.productCode = resultSearchOrderModal.productCode;
      }
      if (resultSearchOrderModal.clientName !== '' && resultSearchOrderModal.clientName) {
        queryString = `${queryString}&cliente=${resultSearchOrderModal.clientName}`;
        filterDataOrders.clientName = resultSearchOrderModal.clientName;
      }
    }

    return [filterDataOrders, queryString];
  }
  getFormattedNumber(numberToFormatted: any) {
    return new Intl.NumberFormat().format(Number(numberToFormatted));
  }

  getMaxMinDate(date: Date, moths: number , isAdd: boolean) {
    return new Date(
        date.getFullYear(),
         isAdd ? date.getMonth() + moths : date.getMonth() - moths ,
        date.getDate());
  }

  setUserRole(role: number) {
    localStorage.setItem(ConstToken.userRole, String(role));
  }

  getUserRole() {
    return localStorage.getItem(ConstToken.userRole);
  }
  setOrderIsolated(isolatedOrder: string) {
    localStorage.setItem(ConstToken.isolatedOrder, isolatedOrder);
  }

  getOrderIsolated() {
    return localStorage.getItem(ConstToken.isolatedOrder);
  }
   removeOrderIsolated() {
     localStorage.removeItem(ConstToken.isolatedOrder);
   }
}

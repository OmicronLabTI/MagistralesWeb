import {Injectable} from '@angular/core';
import {Subject} from 'rxjs';
import Swal, {SweetAlertIcon} from 'sweetalert2';
import {
  Colors, ColorsBarGraph,
  CONST_NUMBER,
  CONST_STRING,
  ConstOrders,
  ConstStatus,
  ConstToken,
  FromToFilter,
  HttpServiceTOCall,
  MessageType,
  MODAL_FIND_ORDERS, RouterPaths,
  TypeToSeeTap
} from '../constants/const';
import {DatePipe} from '@angular/common';
import {QfbWithNumber} from '../model/http/users';
import {GeneralMessage} from '../model/device/general';
import {CancelOrders, SearchComponentModal} from '../model/device/orders';
import {CancelOrderReq, ParamsPedidos} from '../model/http/pedidos';
import {IncidentsGraphicsMatrix} from '../model/http/incidents.model';
import {CommentsConfig} from '../model/device/incidents.model';
import {Router} from '@angular/router';

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
  private newMaterialComponent = new Subject<any>();
  private searchOrdersModal = new Subject<SearchComponentModal>();
  private newSearchOrdersParams = new Subject<ParamsPedidos>();
  private openSignatureDialog = new Subject<any>();
  private newDataSignature = new Subject<any>();
  private openCommentsDialog = new Subject<CommentsConfig>();
  private newCommentsResult = new Subject<CommentsConfig>();
  constructor(private datePipe: DatePipe, private router: Router
  ) { }

  setNewCommentsResult(newCommentsConfig: CommentsConfig) {
    this.newCommentsResult.next(newCommentsConfig);
  }
  getNewCommentsResult() {
    return this.newCommentsResult.asObservable();
  }
  setOpenCommentsDialog(commentsConfig: CommentsConfig) {
    this.openCommentsDialog.next(commentsConfig);
  }
  getOpenCommentsDialog() {
    return this.openCommentsDialog.asObservable();
  }

  setNewDataSignature(newSignature: any) {
    this.newDataSignature.next(newSignature);
  }
  getNewDataSignature() {
    return this.newDataSignature.asObservable();
  }
  setOpenSignatureDialog(datSignature: any) {
    this.openSignatureDialog.next(datSignature);
  }
  getOpenSignatureDialog() {
    return this.openSignatureDialog.asObservable();
  }
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
  setNewMaterialComponent(newFormulaComponent: any) {
    this.newMaterialComponent.next(newFormulaComponent);
  }
  getNewMaterialComponent() {
    return this.newMaterialComponent.asObservable();
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
          title: popupCustom !== CONST_STRING.empty ? 'swal2-title2' : CONST_STRING.empty,
          content: popupCustom !== CONST_STRING.empty ? 'swal2-title2' : CONST_STRING.empty,
        }
      }).then((result) => resolve(result));
    });
  }
  getDateFormatted(initDate: Date, finishDate: Date, isBeginDate: boolean,
                   isProductivity: boolean = false, numberCustomRange: number = CONST_NUMBER.lessOne) {
    if (isBeginDate) {
      if (isProductivity) {
        initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.ninetyDays);
      } else {
        initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.thirtyDays);
      }
    }
    if (numberCustomRange !== CONST_NUMBER.lessOne && numberCustomRange > CONST_NUMBER.zero) {
      initDate = new Date(initDate.getTime() - (MODAL_FIND_ORDERS.operationDay * numberCustomRange));
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
      case MessageType.materialRequest:
        firstMessage = 'Ya se ha generado una solicitud para la orden ';
        finishMessaje = '\n';
        break;
      case MessageType.ordersWithoutQr:
        firstMessage = 'La orden de fabricación ';
        finishMessaje = 'no cuenta con código qr \n';
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
            (t.pedidoStatus !== status && t.pedidoStatus !== ConstStatus.cancelado
                && t.pedidoStatus !== ConstStatus.almacenado && t.pedidoStatus !== ConstStatus.rechazado))).length > 0;
      case FromToFilter.fromDetailOrder:
        return dataToSearch.filter(t => t.isChecked && (t.status !== status && t.status !== ConstStatus.cancelado
            && t.status !== ConstStatus.abierto && t.status !== ConstStatus.almacenado)).length > 0;
      case FromToFilter.fromOrderIsolatedReassign:
        return dataToSearch.filter(t => t.isChecked && (t.status === status || t.status === ConstStatus.asignado
            || t.status.toLowerCase() === ConstStatus.enProceso.toLowerCase() || t.status === ConstStatus.pendiente
            || t.status === ConstStatus.terminado)).length > 0;
      case FromToFilter.fromOrdersIsolatedCancel:
        return dataToSearch.filter(t => (t.isChecked &&
            (t.status !== status && t.status !== ConstStatus.cancelado && t.status !== ConstStatus.almacenado))).length > 0;
      case FromToFilter.fromOrderDetailLabel:
        return dataToSearch.filter(t => t.isChecked && (t.status !== status && t.status !== ConstStatus.cancelado)).length > 0;
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
            || resultSearchOrderModal.productCode === '' || resultSearchOrderModal.clientName === ''
            || resultSearchOrderModal.label === '' || resultSearchOrderModal.finlabel === ''
            || !resultSearchOrderModal.orderIncidents)) {
      isSearchWithFilter = false;
    }
    if ((resultSearchOrderModal && resultSearchOrderModal.dateType === ConstOrders.defaultDateInit) &&
        (resultSearchOrderModal && resultSearchOrderModal.status !== '' || resultSearchOrderModal.qfb !== ''
            || resultSearchOrderModal.productCode !== '' || resultSearchOrderModal.clientName !== ''
            || resultSearchOrderModal.label !== '' || resultSearchOrderModal.finlabel !== ''
            || resultSearchOrderModal.orderIncidents)) {
      isSearchWithFilter = true;
    }
    if ((resultSearchOrderModal && resultSearchOrderModal.dateType === ConstOrders.dateFinishType) &&
        (resultSearchOrderModal && resultSearchOrderModal.status !== '' || resultSearchOrderModal.qfb !== ''
            || resultSearchOrderModal.productCode !== '' || resultSearchOrderModal.clientName !== ''
            || resultSearchOrderModal.label !== '' || resultSearchOrderModal.finlabel !== ''
            || resultSearchOrderModal.orderIncidents)) {
      isSearchWithFilter = true;
    }
    if ((resultSearchOrderModal && resultSearchOrderModal.dateType === ConstOrders.dateFinishType) &&
        (resultSearchOrderModal && resultSearchOrderModal.status === '' || resultSearchOrderModal.qfb === ''
            || resultSearchOrderModal.productCode === '' || resultSearchOrderModal.clientName === ''
            || resultSearchOrderModal.label === '' || resultSearchOrderModal.finlabel === ''
            || !resultSearchOrderModal.orderIncidents)) {
      isSearchWithFilter = true;
    }
    if (resultSearchOrderModal && resultSearchOrderModal.docNum !== '') {
      isSearchWithFilter = true;
    }

    return isSearchWithFilter;
  }

  getfiniOrffin(resultSearchOrderModal: ParamsPedidos, date: string,  ) {
    if ( resultSearchOrderModal.dateType === ConstOrders.defaultDateInit) {
      return `?fini=${date}`;
    } else {
      return `?ffin=${date}`;
    }
  }
  getNewDataToFilter(resultSearchOrderModal: ParamsPedidos): [ParamsPedidos, string] {
    let queryString = CONST_STRING.empty;
    let rangeDate = CONST_STRING.empty;

    const filterDataOrders = new  ParamsPedidos();
    filterDataOrders.isFromOrders = resultSearchOrderModal.isFromOrders;
    filterDataOrders.isFromIncidents = resultSearchOrderModal.isFromIncidents;

    if (resultSearchOrderModal.docNum) {
      filterDataOrders.docNum = resultSearchOrderModal.docNum;
      filterDataOrders.dateFull = this.getDateFormatted(new Date(), new Date(), true);
      filterDataOrders.docNumUntil = resultSearchOrderModal.docNumUntil;
      queryString =  this.getRangeOrders(resultSearchOrderModal.docNum, resultSearchOrderModal.docNumUntil);
    } else {
      if (resultSearchOrderModal.dateType) {
        filterDataOrders.dateType = resultSearchOrderModal.dateType;
        if (resultSearchOrderModal.fini || resultSearchOrderModal.ffin) {
          rangeDate = this.getDateFormatted(resultSearchOrderModal.fini, resultSearchOrderModal.ffin, false);
          queryString = this.getfiniOrffin(resultSearchOrderModal, rangeDate);
          filterDataOrders.dateFull = rangeDate;
        } else {
          queryString = this.getfiniOrffin(resultSearchOrderModal, resultSearchOrderModal.dateFull);
          filterDataOrders.dateFull = resultSearchOrderModal.dateFull;
        }
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
        queryString = `${queryString}&cliente=${resultSearchOrderModal.clientName.replace(/\s+/g, ',')}`;
        filterDataOrders.clientName = resultSearchOrderModal.clientName;
      }
      if (resultSearchOrderModal.label !== '' && resultSearchOrderModal.label) {
        queryString = `${queryString}&label=${resultSearchOrderModal.label}`;
        filterDataOrders.label = resultSearchOrderModal.label;
      }
      if (resultSearchOrderModal.finlabel !== '' && resultSearchOrderModal.finlabel) {
        queryString = `${queryString}&finlabel=${resultSearchOrderModal.finlabel}`;
        filterDataOrders.finlabel = resultSearchOrderModal.finlabel;
      }
      if (resultSearchOrderModal.orderIncidents !== CONST_NUMBER.zero && resultSearchOrderModal.orderIncidents) {
        queryString = `${queryString}&docnum=${resultSearchOrderModal.orderIncidents}`;
        filterDataOrders.orderIncidents = resultSearchOrderModal.orderIncidents;
      }
      if (resultSearchOrderModal.clasification !== '' && resultSearchOrderModal.clasification) {
        queryString = `${queryString}&ordtype=${resultSearchOrderModal.clasification}`;
        filterDataOrders.clasification = resultSearchOrderModal.clasification;
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

  getOrderIsolated() {
    return localStorage.getItem(ConstToken.isolatedOrder);
  }

  removeOrderIsolated() {
     localStorage.removeItem(ConstToken.isolatedOrder);
   }

  openNewTapByUrl(url: string, typeToSeeTap: TypeToSeeTap, orderId?: number) {
    let tapTitle = CONST_STRING.empty;
    switch (typeToSeeTap) {
      case TypeToSeeTap.order:
        tapTitle = `Pedido ${orderId}`;
        break;
      case TypeToSeeTap.receipt:
        tapTitle = `Receta pedido ${orderId}`;
        break;
    }
    window.open(url);
  }

  getItemOnDataOnlyIds(dataToSearch: any[], type: FromToFilter) {
    switch (type) {
      case FromToFilter.fromOrders:
        return dataToSearch.filter(t => (t.isChecked && t.pedidoStatus === ConstStatus.planificado)).map(t => t.docNum);
      case FromToFilter.fromDetailOrder:
        return dataToSearch.filter(t => t.isChecked && t.status === ConstStatus.planificado).map(order => order.ordenFabricacionId);
      case FromToFilter.fromDetailOrderQr:
        return dataToSearch.filter(t => t.isChecked && t.status !== ConstStatus.abierto && t.status !== ConstStatus.cancelado)
            .map(order => order.ordenFabricacionId);
      case FromToFilter.fromOrdersIsolated:
        return dataToSearch.filter(t => t.isChecked && t.status === ConstStatus.planificado).map(order => Number(order.fabOrderId));
    }
  }
  getNormalizeString(valueToNormalize: string) {
    return valueToNormalize.normalize('NFD').replace(/[\u0300-\u036f]/g, '');
  }
  getOptionsGraphToShow = (isPie: boolean, titleForGraph: string ) => (
      {
        tooltips: {
          callbacks: {
            label: (tooltipItem, data) => {
              if (Boolean(isPie)) {
                return `${data.labels[tooltipItem.index]}: ${
                    this.getPercentageByItem(data.datasets[0].data[tooltipItem.index], data.datasets[0].data)}`;
              } else {
                return `${data.datasets[0].data[tooltipItem.index]}`;
              }
            }
          }
        },
        legend: { display: false },
        title: {
          display: true,
          text: titleForGraph
        },
        plugins: {
          labels: isPie ? [
            {
              render: 'label',
              precision: 2,
              position: 'outside'
            }
          ] : [
          ]
        },
        scales: {
          yAxes: !isPie ? [{
            ticks: {
              beginAtZero: true
            }
          }] : []
        }
      }
  )
  getPercentageByItem(valueItem: number, valuesArray: number[], isOnlyNumberPercent: boolean = false) {
    if (!isOnlyNumberPercent) {
      return `${Math.round((valueItem / valuesArray.reduce((a, b) => a + b, 0)) * 100)} %`;
    } else {
      return Math.round((valueItem / valuesArray.reduce((a, b) => a + b, 0)) * 100);
    }
}
  getDataForGraphic = (itemsArray: IncidentsGraphicsMatrix[], isBarGraph: boolean) => (
    {
      labels: itemsArray.map(item => item.fieldKey),
      datasets: [{
        backgroundColor: this.getRandomColorsArray(itemsArray.length, isBarGraph),
        data: itemsArray.map(item => item.totalCount),
        borderColor: '#fff',
        borderWidth: 3,
        hoverBorderWidth: 10,
        hoverBorderColor: '#c0c8ce'
      }]
    })
  getRandomColorsArray(lengthArrayForGraph: number, isBarGraph: boolean) {
    let countIndex = CONST_NUMBER.zero;
    const range = Colors.length;
    const colorsArray = isBarGraph ? ColorsBarGraph : Colors;

    let colorsString: string[] = [];
    for (let i = 0; i < lengthArrayForGraph; i++) {
      if (range === countIndex) {
        countIndex = CONST_NUMBER.zero;
      }
      colorsString = [...colorsString, colorsArray[countIndex]];
      countIndex++;
    }
    return colorsString;
  }


  changeRouterForFormula(ordenFabricacionId: string, ordersIds: string, isFromOrders: number) {
    this.router.navigate([RouterPaths.detailFormula,
      ordenFabricacionId, ordersIds, isFromOrders]);
  }
  getFullStringForCarousel(baseQueryString: string, currentOrder: string, optionsCarousel: string) {
    return `${baseQueryString}&current=${currentOrder}&advance=${optionsCarousel}`;
  }

  getRangeOrders(docNum: any, docNumUntil: any) {
    if (docNum === docNumUntil || docNumUntil === CONST_STRING.empty || !docNumUntil) {
      return `?docNum=${docNum}-${docNum}`;
    } else {
      return `?docNum=${docNum}-${docNumUntil}`;
    }
  }
  setFiltersActives(filters: string) {
    localStorage.setItem(ConstToken.filtersActive, filters);
  }
  getFiltersActives() {
    return  localStorage.getItem(ConstToken.filtersActive);
  }
  removeFiltersActive() {
    localStorage.removeItem(ConstToken.filtersActive);
  }
  getFiltersActivesAsModel(): ParamsPedidos {
    return  JSON.parse(this.getFiltersActives());
  }
  setFiltersActivesOrders(filters: string) {
    localStorage.setItem(ConstToken.filtersActiveOrders, filters);
  }
  getFiltersActivesOrders() {
    return  localStorage.getItem(ConstToken.filtersActiveOrders);
  }
  removeFiltersActiveOrders() {
    localStorage.removeItem(ConstToken.filtersActiveOrders);
  }
  getFiltersActivesAsModelOrders(): ParamsPedidos {
    return  JSON.parse(this.getFiltersActivesOrders());
  }
  setCurrentDetailOrder(detailOrder: string) {
    localStorage.setItem(ConstToken.detailOrderCurrent, detailOrder);
  }
  getCurrentDetailOrder() {
    return localStorage.getItem(ConstToken.detailOrderCurrent);
  }
  removeCurrentDetailOrder() {
    localStorage.removeItem(ConstToken.detailOrderCurrent);
  }
}

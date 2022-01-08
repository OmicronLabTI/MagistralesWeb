import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { HttpServiceTOCall } from '../constants/const';
import { GeneralMessage } from '../model/device/general';
import { CommentsConfig } from '../model/device/incidents.model';
import { CancelOrders, SearchComponentModal } from '../model/device/orders';
import { ParamsPedidos } from '../model/http/pedidos';
import { QfbWithNumber } from '../model/http/users';

@Injectable({
  providedIn: 'root'
})
export class ObservableService {
  private isLoading = new Subject<boolean>();
  private generalNotificationMessage = new Subject<string>();
  private isLogin = new Subject<boolean>();
  private qfbTOPlace = new Subject<QfbWithNumber>();
  private callHttpService = new Subject<HttpServiceTOCall>();
  private messageGenericCallHttp = new Subject<GeneralMessage>();
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

  constructor() { }

  setNewCommentsResult(newCommentsConfig: CommentsConfig): void {
    this.newCommentsResult.next(newCommentsConfig);
  }
  getNewCommentsResult(): Observable<CommentsConfig> {
    return this.newCommentsResult.asObservable();
  }
  setOpenCommentsDialog(commentsConfig: CommentsConfig): void {
    this.openCommentsDialog.next(commentsConfig);
  }
  getOpenCommentsDialog(): Observable<CommentsConfig> {
    return this.openCommentsDialog.asObservable();
  }
  setNewDataSignature(newSignature: any): void {
    this.newDataSignature.next(newSignature);
  }
  getNewDataSignature(): Observable<any> {
    return this.newDataSignature.asObservable();
  }
  setOpenSignatureDialog(datSignature: any): void {
    this.openSignatureDialog.next(datSignature);
  }
  getOpenSignatureDialog(): Observable<any> {
    return this.openSignatureDialog.asObservable();
  }
  setNewSearchOrderModal(searchOrdersParams: ParamsPedidos): void {
    this.newSearchOrdersParams.next(searchOrdersParams);
  }
  getNewSearchOrdersModal(): Observable<ParamsPedidos> {
    return this.newSearchOrdersParams.asObservable();
  }
  setSearchOrdersModal(searchOrder: SearchComponentModal): void {
    this.searchOrdersModal.next(searchOrder);
  }
  getSearchOrdersModal(): Observable<SearchComponentModal> {
    return this.searchOrdersModal.asObservable();
  }
  setNewFormulaComponent(newFormulaComponent: any): void {
    this.newFormulaComponent.next(newFormulaComponent);
  }
  getNewFormulaComponent(): Observable<any> {
    return this.newFormulaComponent.asObservable();
  }
  setNewMaterialComponent(newFormulaComponent: any): void {
    this.newMaterialComponent.next(newFormulaComponent);
  }
  getNewMaterialComponent(): Observable<any> {
    return this.newMaterialComponent.asObservable();
  }
  setSearchComponentModal(searchComponentModal: SearchComponentModal): void {
    this.searchComponentModal.next(searchComponentModal);
  }
  getSearchComponentModal(): Observable<SearchComponentModal> {
    return this.searchComponentModal.asObservable();
  }
  setIsLogout(isLogout: boolean): void {
    this.isLogout.next(isLogout);
  }
  getIsLogout(): Observable<boolean> {
    return this.isLogout.asObservable();
  }
  setPathUrl(pathUrl: any[]): void {
    this.pathUrl.next(pathUrl);
  }
  getPathUrl(): Observable<any[]> {
    return this.pathUrl.asObservable();
  }
  setFinalizeOrders(finalizeOrders: CancelOrders): void {
    this.finalizeOrders.next(finalizeOrders);
  }
  getFinalizeOrders(): Observable<CancelOrders> {
    return this.finalizeOrders.asObservable();
  }
  setCancelOrders(cancelOrder: CancelOrders): void {
    this.cancelOrders.next(cancelOrder);
  }
  getCancelOrder(): Observable<CancelOrders> {
    return this.cancelOrders.asObservable();
  }
  setUrlActive(url: HttpServiceTOCall): void {
    this.urlActive.next(url);
  }
  getUrlActive(): Observable<HttpServiceTOCall> {
    return this.urlActive;
  }
  setMessageGeneralCallHttp(messageGeneral: GeneralMessage): void {
    this.messageGenericCallHttp.next(messageGeneral);
  }
  getMessageGeneralCalHttp(): Observable<GeneralMessage> {
    return this.messageGenericCallHttp.asObservable();
  }
  setCallHttpService(numberServiceToCall: HttpServiceTOCall): void {
    this.callHttpService.next(numberServiceToCall);
  }
  getCallHttpService(): Observable<HttpServiceTOCall> {
    return this.callHttpService.asObservable();
  }
  setIsLogin(isLogin: boolean): void {
    this.isLogin.next(isLogin);
  }
  getIsLogin(): Observable<boolean> {
    return this.isLogin.asObservable();
  }
  setIsLoading(loading: boolean): void {
    this.isLoading.next(loading);
  }
  getIsLoading(): Observable<boolean> {
    return this.isLoading.asObservable();
  }
  setQbfToPlace(qfb: QfbWithNumber): void {
    this.qfbTOPlace.next(qfb);
  }
  getQfbToPlace(): Observable<QfbWithNumber> {
    return this.qfbTOPlace.asObservable();
  }
  setGeneralNotificationMessage(msg: string): void {
    this.generalNotificationMessage.next(msg);
  }
  getGeneralNotificationMessage(): Observable<string> {
    return this.generalNotificationMessage.asObservable();
  }
}

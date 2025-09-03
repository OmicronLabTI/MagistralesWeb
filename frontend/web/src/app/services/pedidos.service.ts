import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {IPlaceOrdersReq, IQfbWithNumberRes, IUserListRes} from '../model/http/users';
import {IComponentsRes, IComponentsSaveReq, IFormulaRes, IProductWarehouses} from '../model/http/detalleformula';
import {
  CancelOrderReq,
  CreateIsolatedOrderReq,
  CreatePdfOrder,
  ICancelOrdersRes, ICatalogRes,
  ICreateIsolatedOrderRes,
  ICreatePdfOrdersRes,
  IExistsBachCodeRes,
  IGetNewBachCodeRes,
  IPedidosListRes,
  IPlaceOrdersAutomaticReq, IPlaceOrdersAutomaticRes,
  IProcessOrdersRes, IRecipesRes, IWorkLoadRes, OrderToDelivered,
  ProcessOrdersDetailReq
} from '../model/http/pedidos';
import {
  IOrdersRefuseReq,
  IPedidoDetalleLabelReq,
  IPedidoDetalleListRes,
  IPedidoRefuseRes, IQrByOrdersRes
} from '../model/http/detallepedidos.model';
import { IComponentsLotesRes, IResponseSaveChanges } from '../model/http/addComponent';
import { BaseResponseHttp } from '../model/http/commons';


@Injectable({
  providedIn: 'root'
})
export class PedidosService {

  constructor(private consumeService: ConsumeService) { }

  getPedidos(queryString: string) {
    return this.consumeService.httpGet<IPedidosListRes>(`${Endpoints.pedidos.getPedidos}${queryString}`);
  }

  getDetallePedido(docNum: string) {
    return this.consumeService.httpGet<IPedidoDetalleListRes>(Endpoints.pedidos.getDetallePedido + docNum);
  }
  getDetailCarousel(queryStringFull: string) {
    return this.consumeService.httpGet<IPedidoDetalleListRes>(Endpoints.pedidos.detailCarousel + queryStringFull);
  }
  getFormulaDetail(orderNum: string) {
    return this.consumeService.httpGet<IFormulaRes>(`${Endpoints.pedidos.formulaDetail}/${orderNum}`);
  }
  getFormulaCarousel(queryString: string) {
    return this.consumeService.httpGet<IFormulaRes>(Endpoints.pedidos.formulaCarousel + queryString);
  }
  processOrders(ordersToProcess) {
    return this.consumeService.httpPost<IProcessOrdersRes>(Endpoints.pedidos.processOrders, ordersToProcess);
  }
  getQfbs(defaultQfbId: number ) {
    return this.consumeService.httpGet<IUserListRes>(`${Endpoints.users.qfbs}/${defaultQfbId}`);
  }
  getQfbsWithOrders() {
    return this.consumeService.httpGet<IQfbWithNumberRes>(`${Endpoints.users.qfbsWithOrders}`);
  }
  postPlaceOrders(placeOrder: IPlaceOrdersReq, isFromReassign: boolean) {
    return this.consumeService.httpPost<IPlaceOrdersAutomaticRes>( !isFromReassign ? Endpoints.pedidos.placeOrders :
        Endpoints.pedidos.reAssignManual, placeOrder);
  }
  getComponents(queryStringComponents: string, isFromSearchComponents) {
    return this.consumeService.httpGet<IComponentsRes>(`${isFromSearchComponents ? Endpoints.pedidos.components :
            Endpoints.pedidos.getProducts}${queryStringComponents}`);
  }
  getComponentsLotes(queryString: string) {
    return this.consumeService.httpGet<IComponentsLotesRes>(`${Endpoints.pedidos.componentsLotes}${queryString}`);
  }

  updateFormula(formulaTOSave: IComponentsSaveReq) {
    return this.consumeService.httpPut<IResponseSaveChanges>(Endpoints.pedidos.updateFormula, formulaTOSave);
  }
  postPlaceOrdersDetail(placeOrderDetail: ProcessOrdersDetailReq) {
    return this.consumeService.httpPost<IProcessOrdersRes>(Endpoints.pedidos.processOrdersDetail, placeOrderDetail);
  }
  postPlaceOrderAutomatic(placeOrderAutomatic: IPlaceOrdersAutomaticReq) {
    return this.consumeService.httpPost<IPlaceOrdersAutomaticRes>(Endpoints.pedidos.placeOrdersAutomatic, placeOrderAutomatic);
  }
  putCancelOrders(cancelOrders: CancelOrderReq[] , isCancelOrder: boolean) {
    return this.consumeService.httpPut<ICancelOrdersRes>(isCancelOrder ? Endpoints.pedidos.cancelOrders :
        Endpoints.pedidos.cancelOrdersDetail, cancelOrders);
  }
  putFinalizeOrders(cancelOrders: CancelOrderReq[] , isFinalizeOrder: boolean) {
    return this.consumeService.httpPut<ICancelOrdersRes>(isFinalizeOrder ? Endpoints.pedidos.finalizeOrders :
        Endpoints.pedidos.finalizeOrdersDetail, cancelOrders);
  }
  createIsolatedOrder(createOrder: CreateIsolatedOrderReq) {
    return this.consumeService.httpPost<ICreateIsolatedOrderRes>(Endpoints.pedidos.createIsolatedOrder, createOrder);
  }

  getWorLoad(queryString: string) {
    return this.consumeService.httpGet<IWorkLoadRes>(`${Endpoints.productivity.getWorkLoad}?fini=${queryString}`);
  }
  getNextBatchCode(productCode: string) {
    return this.consumeService.httpGet<IGetNewBachCodeRes>(`${Endpoints.pedidos.getNextBatchCode}?productCode=${productCode}`);
  }
  getIfExistsBatchCode(productCode: string, batchCode: string) {
    return this.consumeService.httpGet<IExistsBachCodeRes>(
        `${Endpoints.pedidos.checkIfExistsBatchCode}?productCode=${productCode}&batchCode=${batchCode}`);
  }
  getOrdersPdfViews(orderIds: CreatePdfOrder[]) {
    return this.consumeService.httpPost<ICreatePdfOrdersRes>(`${Endpoints.orders.viewPdf}`, orderIds);
  }
  putOrdersToDelivered(ordersToDelivered: OrderToDelivered[]) {
    return this.consumeService.httpPut<ICreatePdfOrdersRes>(`${Endpoints.orders.ordersToDelivered}`, ordersToDelivered);
  }
  getRecipesByOrder(orderId: number) {
    return this.consumeService.httpGet<IRecipesRes>(`${Endpoints.pedidos.getRecipes}/${orderId}`);
  }
  createPdfOrders(orderIds: number[]) {
    return this.consumeService.httpPost<ICreatePdfOrdersRes>(`${Endpoints.orders.createPdf}`, orderIds);
  }
  savedComments(orderId: number, comments: string) {
    return this.consumeService.httpPut<IPedidoDetalleListRes>(`${Endpoints.orders.savedComments}`, {
      orderId,
      comments
    });
  }
  finishLabels(labelsToFinish: IPedidoDetalleLabelReq) {
    return this.consumeService.httpPut<IPedidoDetalleListRes>(`${Endpoints.orders.finishLabels}`, labelsToFinish);
  }
  getInitRangeDate() {
    return this.consumeService.httpGet<ICatalogRes>(`${Endpoints.pedidos.rangeDateInit}`);
  }
  qrByEachOrder(idsByEachOrders: number[]) {
    return this.consumeService.httpPost<IQrByOrdersRes>(`${Endpoints.orders.qrByOrder}`, idsByEachOrders);
  }
  putRefuseOrders(refuseOrdersReq: IOrdersRefuseReq) {
    return this.consumeService.httpPut<IPedidoRefuseRes>(Endpoints.pedidos.refuseOrdersService, refuseOrdersReq);
  }

  getProductWarehouses(productId: string) {
    return this.consumeService.httpGet<IProductWarehouses>(`${Endpoints.pedidos.productWarehouses}?itemCode=${productId}`);
  }
}

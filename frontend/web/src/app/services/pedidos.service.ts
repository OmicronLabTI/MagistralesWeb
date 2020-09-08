import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {IPlaceOrdersReq, IQfbWithNumberRes, IUserListRes} from '../model/http/users';
import {IComponentsRes, IComponentsSaveReq, IFormulaRes} from '../model/http/detalleformula';
import {
  CancelOrderReq, CreateIsolatedOrderReq, ICancelOrdersRes, ICreateIsolatedOrderRes,
  IPedidosListRes,
  IPlaceOrdersAutomaticReq, IPlaceOrdersAutomaticRes,
  IProcessOrdersRes,
  ProcessOrdersDetailReq
} from '../model/http/pedidos';
import {IPedidoDetalleListRes} from '../model/http/detallepedidos.model';

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
  getFormulaDetail(orderNum: string) {
    return this.consumeService.httpGet<IFormulaRes>(`${Endpoints.pedidos.getFormulaDetail}/${orderNum}`);
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
    return this.consumeService.httpGet<IComponentsRes>(`${isFromSearchComponents ? Endpoints.pedidos.getComponents :
            Endpoints.pedidos.getProducts}${queryStringComponents}`);
  }
  updateFormula(formulaTOSave: IComponentsSaveReq) {
    return this.consumeService.httpPut(Endpoints.pedidos.updateFormula, formulaTOSave);
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
}

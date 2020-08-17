import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {IPlaceOrdersReq, IQfbWithNumberRes, IUserListRes} from '../model/http/users';
import {IComponentsRes, IComponentsSaveReq, IFormulaRes} from '../model/http/detalleformula';
import {IPedidosListRes, IProcessOrdersRes, ProcessOrdersDetailReq} from '../model/http/pedidos';
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
  getQfbs() {
    return this.consumeService.httpGet<IUserListRes>(`${Endpoints.users.qfbs}/2`);
  }
  getQfbsWithOrders() {
    return this.consumeService.httpGet<IQfbWithNumberRes>(`${Endpoints.users.qfbsWithOrders}`);
  }
  postPlaceOrders(placeOrder: IPlaceOrdersReq) {
    return this.consumeService.httpPost(Endpoints.pedidos.placeOrders, placeOrder);
  }
  getComponents(queryStringComponents: string) {
    return this.consumeService.httpGet<IComponentsRes>(`${Endpoints.pedidos.getComponents}${queryStringComponents}`);
  }
  updateFormula(formulaTOSave: IComponentsSaveReq) {
    return this.consumeService.httpPut(Endpoints.pedidos.updateFormula, formulaTOSave);
  }
  postPlaceOrdersDetail(placeOrderDetail: ProcessOrdersDetailReq) {
    return this.consumeService.httpPost(Endpoints.pedidos.processOrdersDetail, placeOrderDetail);
  }
}

import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {IPlaceOrdersReq, IQfbWithNumberRes, IUserListRes} from '../model/http/users';
import {IComponentsRes} from '../model/http/detalleformula';

@Injectable({
  providedIn: 'root'
})
export class PedidosService {

  constructor(private consumeService: ConsumeService) { }

  getPedidos(queryString: string) {
    return this.consumeService.httpGet(`${Endpoints.pedidos.getPedidos}${queryString}`);
  }

  getDetallePedido(docNum: string) {
    return this.consumeService.httpGet(Endpoints.pedidos.getDetallePedido + docNum);
  }
  getFormulaDetail(orderNum: string) {
    return this.consumeService.httpGet(`${Endpoints.pedidos.getFormulaDetail}/${orderNum}`);
  }

  processOrders(ordersToProcess) {
    return this.consumeService.httpPost(Endpoints.pedidos.processOrders, ordersToProcess);
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
}

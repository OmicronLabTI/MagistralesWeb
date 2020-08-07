import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {IUserListRes} from '../model/http/users';

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

  processOrders(ordersToProcess){
    return this.consumeService.httpPost(Endpoints.pedidos.processOrders,ordersToProcess);
  }
  getQfbs() {
    return this.consumeService.httpGet<IUserListRes>(`${Endpoints.users.qfbs}/2`);
  }
}

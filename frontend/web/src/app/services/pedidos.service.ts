import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {ParamsPedidos} from '../model/http/pedidos';

@Injectable({
  providedIn: 'root'
})
export class PedidosService {

  constructor(private consumeService: ConsumeService) { }

  getPedidos(params: ParamsPedidos) {
    const queryString = `?fini=${params.fini}&ffin=${params.ffin}&offset=${params.offset}&limit=${params.limit}`;
    return this.consumeService.httpGet(`${Endpoints.pedidos.getPedidos}${queryString}`);
  }

  getDetallePedido(docNum: string) {
    return this.consumeService.httpGet(Endpoints.pedidos.getDetallePedido + docNum);
  }

  processOrders(docNum: number[]){
    return this.consumeService.httpPost(Endpoints.pedidos.processOrders,docNum);
  }
}

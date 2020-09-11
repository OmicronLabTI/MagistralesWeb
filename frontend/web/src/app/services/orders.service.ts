import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {IOrdersRes} from '../model/http/ordenfabricacion';
import { IMyListRes } from '../model/http/listacomponentes';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {

  constructor(private consumeService: ConsumeService) { }

  getOrders(queryString: string) {
    return this.consumeService.httpGet<IOrdersRes>(`${Endpoints.orders.getOrders}${queryString}`);
  }

  saveMyListComponent(myList) {
    return this.consumeService.httpPost<IMyListRes>(Endpoints.orders.saveMyList, myList);
  }
}

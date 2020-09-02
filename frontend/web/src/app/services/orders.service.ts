import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {IOrdersRes} from '../model/http/ordenfabricacion';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {

  constructor(private consumeService: ConsumeService) { }

  getOrders(queryString: string) {
    return this.consumeService.httpGet<IOrdersRes>(`${Endpoints.orders.getOrders}${queryString}`);
  }
}

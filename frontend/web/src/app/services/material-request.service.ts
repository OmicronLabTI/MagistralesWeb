import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {IMaterialRequestRes} from '../model/http/materialReques';

@Injectable({
  providedIn: 'root'
})
export class MaterialRequestService {

  constructor(private consumeService: ConsumeService) { }
  getPreMaterialRequest(ordersId: any, isOrder: boolean) {
    return this.consumeService.httpGet<IMaterialRequestRes>(`${Endpoints.materialRequest.getPreMaterialRequest}?${
      isOrder ? 'salesOrders' : 'productionOrders'}=${ordersId}`);
  }
}

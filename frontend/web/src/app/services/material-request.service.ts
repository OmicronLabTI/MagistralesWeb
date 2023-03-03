import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {DestinationStoreResponse, IMaterialPostRes, IMaterialRequestRes, RawRequestPost} from '../model/http/materialReques';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MaterialRequestService {

  constructor(private consumeService: ConsumeService) { }
  getPreMaterialRequest(ordersId: any, isOrder: boolean) {
    return this.consumeService.httpGet<IMaterialRequestRes>(`${Endpoints.materialRequest.getPreMaterialRequest}?${
      isOrder ? 'salesOrders' : 'productionOrders'}=${ordersId}`);
  }
  postMaterialRequest(materialRequest: RawRequestPost) {
    return this.consumeService.httpPost<IMaterialPostRes>(Endpoints.materialRequest.postMaterialRequest, materialRequest);
  }
  getDestinationStore(): Observable<DestinationStoreResponse> {
    return this.consumeService.httpGet<DestinationStoreResponse>(Endpoints.destination);
  }
}

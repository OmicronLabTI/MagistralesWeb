import { Injectable } from '@angular/core';
import { ConsumeService } from './consume.service';
import { Endpoints } from '../../environments/endpoints';
import {
  DestinationStoreResponse,
  IMaterialHistoryRes,
  IMaterialPostRes,
  IMaterialRequestRes,
  RawRequestPost
} from '../model/http/materialReques';
import { Observable, of } from 'rxjs';
import { MaterialRequestHistoryMock } from 'src/mocks/materialRequest';

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

  gethistoryMaterial = (query: string): Observable<IMaterialHistoryRes> => {
    return this.consumeService.httpGet(`${Endpoints.materialRequest.historyMaterial}${query}`);
  }
}

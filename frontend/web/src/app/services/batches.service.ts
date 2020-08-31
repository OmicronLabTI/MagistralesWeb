import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import {ILotesFormulaRes, ILotesToSaveReq, ILotesSaveRes} from '../model/http/lotesformula';

@Injectable({
  providedIn: 'root'
})
export class BatchesService {

  constructor(private consumeService: ConsumeService) { }

  getInventoryBatches(ordenFabId: string){
    return this.consumeService.httpGet<ILotesFormulaRes>(Endpoints.inventoryBatches.getInventoryBatches + ordenFabId);
  }

  updateBatches(objectToSave: ILotesToSaveReq[]) {
    return this.consumeService.httpPut<ILotesSaveRes>(Endpoints.inventoryBatches.assignBatches, objectToSave);
  }
}

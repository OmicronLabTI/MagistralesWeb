import { Injectable } from '@angular/core';
import { ConsumeService } from './consume.service';
import { IAutomaticBillingRes, IManualRetrieResponse, IUpdateManualChangeRes, ManualRetryequest } from '../model/http/invoices';
import { Endpoints } from 'src/environments/endpoints';
import { automaticBillingResponseMock, changesAppliedConfirmMock } from 'src/mocks/invoicesMock';
import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InvoicesService {

  constructor(
    private consumeService: ConsumeService
  ) { }

  getAutomaticBillingTableData(queryString: string) {
    return this.consumeService.httpGet<IAutomaticBillingRes>(`${Endpoints.invoices.automaticBillingTableData}/${queryString}`);
  }

  adjustmentMade(id: string) {
    return this.consumeService.httpPut<IUpdateManualChangeRes>(`${Endpoints.invoices.confirmAdjustment}`, {id});
  }

  sendManualRetry(request: ManualRetryequest) {
    return this.consumeService.httpPost<IManualRetrieResponse>(`${Endpoints.invoices.manualRetry}`, request);
  }
}

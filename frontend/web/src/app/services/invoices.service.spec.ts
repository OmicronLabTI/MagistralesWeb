import { TestBed } from '@angular/core/testing';

import { InvoicesService } from './invoices.service';
import { ConsumeService } from './consume.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Endpoints } from 'src/environments/endpoints';

describe('InvoicesService', () => {
  let consumeServiceSpy: jasmine.SpyObj<ConsumeService>;
  beforeEach(() => {
    consumeServiceSpy = jasmine.createSpyObj<ConsumeService>('ConsumeService', [
      'httpGet', 'httpPut', 'httpPost'
    ]);
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [{ provide: ConsumeService, useValue: consumeServiceSpy }]
    });
  });

  it('should be created', () => {
    const service: InvoicesService = TestBed.get(InvoicesService);
    expect(service).toBeTruthy();
  });
  it('getAutomaticBillingTableData', () => {
    const service: InvoicesService = TestBed.get(InvoicesService);
    service.getAutomaticBillingTableData('?status=Errror al crear&offset=10&limit=10');
    expect(consumeServiceSpy.httpGet).toHaveBeenCalled();
  });
  it('getAutomaticBillingTableData', () => {
    const service: InvoicesService = TestBed.get(InvoicesService);
    const id = '774bf9cb-39e2-4978-93e5-f1bfc38402f3';
    service.adjustmentMade(id);
    expect(consumeServiceSpy.httpPut).toHaveBeenCalledWith(Endpoints.invoices.confirmAdjustment, { id });
  });
  it('sentManualRetry', () => {
    const service: InvoicesService = TestBed.get(InvoicesService);
    const request = {
      invoiceIds: ['774bf9cb-39e2-4978-93e5-f1bfc38402f3'],
      requestingUser: '774bf9cb-39e2-4978-93e5-f1bfc38402f3',
      offset: 0,
      limit: 0
    };
    service.sendManualRetry(request);
    expect(consumeServiceSpy.httpPost).toHaveBeenCalledWith(Endpoints.invoices.manualRetry, request);
  });
  it('getMissingSAPOrders', () => {
    const service: InvoicesService = TestBed.get(InvoicesService);
    const request = `pedidodxp=e1a4c24a-b511-4d6a-9b52-d0b3f9a91b33`;
    service.getMissingSAPOrders(request);
    expect(consumeServiceSpy.httpGet).toHaveBeenCalled();
  });
});

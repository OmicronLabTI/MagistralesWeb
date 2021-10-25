import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { BatchesService } from './batches.service';
import { DatePipe } from '@angular/common';
import { ConsumeService } from './consume.service';
import { ILotesToSaveReq } from '../model/http/lotesformula';
import { Observable } from 'rxjs';
import {RouterTestingModule} from '@angular/router/testing';

describe('BatchesService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule, RouterTestingModule],
    providers: [DatePipe, ConsumeService]
  }));

  it('should be created', () => {
    const service: BatchesService = TestBed.get(BatchesService);
    expect(service).toBeTruthy();
  });

  it('should return an observable', () => {
    const batchesService: BatchesService = TestBed.get(BatchesService);
    const obs = batchesService.getInventoryBatches('1234');
    expect(obs instanceof Observable).toBeTruthy();
  });

  it('should getBatches', () => {
    const service: BatchesService = TestBed.get(BatchesService);
    expect(service.getInventoryBatches('anystring') instanceof Observable).toBeTruthy();
  });

  it('should updateBatches', () => {
    const objeto: ILotesToSaveReq[] = [
      {
        batchNumber: '1234',
        orderId: 1234,
        assignedQty: 222,
        itemCode: 'MP   009',
        action: 'insert',
        areBatchesComplete: 0
      }
    ];
    const service: BatchesService = TestBed.get(BatchesService);
    expect(service.updateBatches(objeto)).toBeTruthy();
  });
});

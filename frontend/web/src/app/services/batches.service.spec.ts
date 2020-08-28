import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { BatchesService } from './batches.service';
import { DatePipe } from '@angular/common';
import { ConsumeService } from './consume.service';
import { ILotesFormulaReq, ILotesToSaveReq } from '../model/http/lotesformula';
import { Endpoints } from 'src/environments/endpoints';
import { Observable } from 'rxjs';
import { Component } from '@angular/core';

describe('BatchesService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [DatePipe, ConsumeService]
  }));

  it('should be created', () => {
    const service: BatchesService = TestBed.get(BatchesService);
    expect(service).toBeTruthy();
  });

  it('should return an observable', () =>{
    const batchesService: BatchesService = TestBed.get(BatchesService);
    const obs = batchesService.getInventoryBatches("1234");
    expect(obs instanceof Observable).toBeTruthy();
  });

  it('should getBatches', () => {
    const service: BatchesService = TestBed.get(BatchesService);
    expect(service.getInventoryBatches("anysring") instanceof Observable).toBeTruthy();
  });

  it('should updateBatches', () => {
    let objeto: ILotesToSaveReq[] = [
      {
        batchNumber: "1234",
        orderId: 1234,
        assignedQty: 222,
        itemCode: "MP   009",
        action: "insert"
      }
    ]
    const service: BatchesService = TestBed.get(BatchesService);
    expect(service.updateBatches(objeto)).toBeTruthy();
  });
});

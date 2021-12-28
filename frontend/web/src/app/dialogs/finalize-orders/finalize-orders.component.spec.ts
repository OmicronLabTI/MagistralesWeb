import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FinalizeOrdersComponent } from './finalize-orders.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MATERIAL_COMPONENTS } from '../../app.material';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DatePipe } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';
import { OrdersService } from 'src/app/services/orders.service';
import { PedidosService } from '../../services/pedidos.service';
import { of } from 'rxjs';
import { IExistsBachCodeRes, IGetNewBachCodeRes } from 'src/app/model/http/pedidos';
import { ICancelOrdersRes } from '../../model/http/pedidos';
import { ErrorService } from 'src/app/services/error.service';
import { DataService } from 'src/app/services/data.service';

describe('FinalizeOrdersComponent', () => {
  let component: FinalizeOrdersComponent;
  let fixture: ComponentFixture<FinalizeOrdersComponent>;
  let orderServiceSpy: jasmine.SpyObj<PedidosService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;

  const getNextBatchCodeRes = new IGetNewBachCodeRes();
  const getIfExistsBatchCodeRes = new IExistsBachCodeRes();
  const putFinalizeOrders = new ICancelOrdersRes();
  beforeEach(async(() => {
    orderServiceSpy = jasmine.createSpyObj<PedidosService>
      ('PedidosService',
        [
          'getNextBatchCode',
          'getIfExistsBatchCode',
          'putFinalizeOrders'
        ]);
    orderServiceSpy.getNextBatchCode.and.returnValue(of(getNextBatchCodeRes));
    orderServiceSpy.getIfExistsBatchCode.and.returnValue(of(getIfExistsBatchCodeRes));
    orderServiceSpy.putFinalizeOrders.and.returnValue(of(putFinalizeOrders));


    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', ['httpError']);


    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService',
      ['getUserId',
        'transformDate',
        'getMessageTitle',
        'setCallHttpService',
        'presentToastCustom',
        'setMessageGeneralCallHttp',
      ]);

    dataServiceSpy.getUserId.and.returnValue('');
    dataServiceSpy.transformDate.and.returnValue('');
    dataServiceSpy.getMessageTitle.and.returnValue('');

    TestBed.configureTestingModule({
      declarations: [FinalizeOrdersComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      imports: [MATERIAL_COMPONENTS, FormsModule, HttpClientTestingModule, RouterTestingModule],
      providers: [
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: { finalizeData: { finalizeOrdersData: [] } } },
        DatePipe,
        { provide: PedidosService, useValue: orderServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinalizeOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

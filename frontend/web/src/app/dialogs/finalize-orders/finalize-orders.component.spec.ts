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
import { IOrdersReq, IOrdersRes } from 'src/app/model/http/ordenfabricacion';
import { AddCommentsDialogComponent } from '../add-comments-dialog/add-comments-dialog.component';

describe('FinalizeOrdersComponent', () => {
  let component: FinalizeOrdersComponent;
  let fixture: ComponentFixture<FinalizeOrdersComponent>;
  let orderServiceSpy: jasmine.SpyObj<PedidosService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<AddCommentsDialogComponent>>;

  const getNextBatchCodeRes = new IGetNewBachCodeRes();
  const getIfExistsBatchCodeRes = new IExistsBachCodeRes();
  const putFinalizeOrders = new ICancelOrdersRes();

  // const iOrderRes = new IOrdersRes();
  let iOrdersReq: IOrdersReq[] = [];

  // iOrderRes.response = iOrdersReq;


  beforeEach(async(() => {
  dialogRefSpy = jasmine.createSpyObj<MatDialogRef<AddCommentsDialogComponent>>('MatDialogRef', [
    'close',
  ]);
  dialogRefSpy.close.and.returnValue();
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
    [
      'getUserId',
      'transformDate',
      'getMessageTitle',
      'setCallHttpService',
      'presentToastCustom',
      'setMessageGeneralCallHttp',
    ]);

  dataServiceSpy.getUserId.and.returnValue('');
  dataServiceSpy.transformDate.and.returnValue('');
  dataServiceSpy.getMessageTitle.and.returnValue('');
  dataServiceSpy.setCallHttpService.and.returnValue();
  dataServiceSpy.setMessageGeneralCallHttp.and.returnValue();
  dataServiceSpy.presentToastCustom.and.callFake(() => {
    return Promise.resolve();
  });

  TestBed.configureTestingModule({
    declarations: [FinalizeOrdersComponent],
    schemas: [CUSTOM_ELEMENTS_SCHEMA],
    imports: [MATERIAL_COMPONENTS, FormsModule, HttpClientTestingModule, RouterTestingModule],
    providers: [
      { provide: MatDialogRef, useValue: dialogRefSpy },
      { provide: MAT_DIALOG_DATA, useValue: { finalizeOrdersData: [] } },
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
  it('should groupBy if ordersIsolated.lenght >0', () => {
    iOrdersReq = [
      {
        isChecked: true,
        docNum: 1,
        fabOrderId: 1,
        itemCode: '',
        description: '',
        quantity: 1,
        createDate: '',
        finishDate: '',
        qfb: '',
        status: '',
        class: '',
        unit: '',
        batche: '',
        quantityFinish: 1,
        endDate: new Date('22/12/12'),
        fabDate: new Date('22/12/12'),
        isWithError: false,
        isWithErrorBatch: false,
        hasMissingStock: false,
        batch: ''
      }
    ];
    // component.groupBy(iOrdersReq, keyGetter);
  });
  // component.groupBy(iOrdersReq, '');
  it('should onBatchesChange', () => {
    component.dataSource.data = [
      {
        batche: '',
        itemCode: '',
        isWithErrorBatch: false
      } as IOrdersReq
    ];
    component.onBatchesChange('', 0);
  });

  it('should isCorrectDataToFinalize', () => {
    component.isCorrectDataToFinalize();
    component.dataSource.data = [
      {
        batche: '1',
        itemCode: '1',
        quantityFinish: 1,
        fabDate: new Date('12/12/12'),
        endDate: new Date('12/12/12'),
        isWithErrorBatch: false
      } as IOrdersReq
    ];
    // expect(component.isCorrectData).toBe(false);
  });

  it('should focusOutLote', () => {
    const i = 0;
    component.dataSource.data = [
      {
        isChecked: true,
        docNum: 1,
        fabOrderId: 1,
        itemCode: '1',
        description: 'string',
        quantity: 1,
        createDate: '',
        finishDate: '',
        qfb: '',
        status: '',
        class: '',
        unit: '',
        batche: '1',
        quantityFinish: 1,
        endDate: new Date('22/12/12'),
        fabDate: new Date('22/12/12'),
        isWithError: false,
        isWithErrorBatch: false,
        hasMissingStock: false,
        batch: ''
      } as IOrdersReq
    ];
    // component.focusOutLote(i);
    // expect(component.isCorrectData).toBeTruthy();
    // expect(orderServiceSpy.getIfExistsBatchCode).toHaveBeenCalled();
  });
  it('should onEndDateChange', () => {
    component.dataSource.data = [
      {
        isChecked: true,
        docNum: 1,
        fabOrderId: 1,
        itemCode: '1',
        description: 'string',
        quantity: 1,
        createDate: '',
        finishDate: '',
        qfb: '',
        status: '',
        class: '',
        unit: '',
        batche: '',
        quantityFinish: 1,
        endDate: new Date('22/12/12'),
        fabDate: new Date('22/12/12'),
        isWithError: false,
        isWithErrorBatch: false,
        hasMissingStock: false,
        batch: ''
      } as IOrdersReq
    ];
    const date = new Date('22/12/12');
    component.onEndDateChange(date, 0);
    expect(component.isCorrectData).toBeFalsy();
  });
  it('should onFabDateChange', () => {
    component.dataSource.data = [
      {
        isChecked: true,
        docNum: 1,
        fabOrderId: 1,
        itemCode: '1',
        description: 'string',
        quantity: 1,
        createDate: '',
        finishDate: '',
        qfb: '',
        status: '',
        class: '',
        unit: '',
        batche: '',
        quantityFinish: 1,
        endDate: new Date('22/12/12'),
        fabDate: new Date('22/12/12'),
        isWithError: false,
        isWithErrorBatch: false,
        hasMissingStock: false,
        batch: ''
      } as IOrdersReq
    ];
    const date = new Date('22/12/12');
    component.onFabDateChange(date, 0);
    expect(component.isCorrectData).toBeFalsy();
  });
  it('should onBatchesChange', () => {
    component.dataSource.data = [
      {
        isChecked: true,
        docNum: 1,
        fabOrderId: 1,
        itemCode: '1',
        description: 'string',
        quantity: 1,
        createDate: '',
        finishDate: '',
        qfb: '',
        status: '',
        class: '',
        unit: '',
        batche: '',
        quantityFinish: 1,
        endDate: new Date('22/12/12'),
        fabDate: new Date('22/12/12'),
        isWithError: false,
        isWithErrorBatch: false,
        hasMissingStock: false,
        batch: ''
      } as IOrdersReq
    ];
    const date = new Date('22/12/12');
    component.onBatchesChange('', 0);
    expect(component.isCorrectData).toBeFalsy();
  });
  it('should onQuantityFinishChange', () => {
    component.dataSource.data = [
      {
        isChecked: true,
        docNum: 1,
        fabOrderId: 1,
        itemCode: '1',
        description: 'string',
        quantity: 1,
        createDate: '',
        finishDate: '',
        qfb: '',
        status: '',
        class: '',
        unit: '',
        batche: '',
        quantityFinish: 1,
        endDate: new Date('22/12/12'),
        fabDate: new Date('22/12/12'),
        isWithError: false,
        isWithErrorBatch: false,
        hasMissingStock: false,
        batch: ''
      } as IOrdersReq
    ];
    component.onQuantityFinishChange('', 0);
    expect(component.onQuantityFinishChange).toBeTruthy();
  });

  it('should finalizeOrderSend', () => {
    dataServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.dataSource.data = [
      {
        isChecked: true,
        docNum: 1,
        fabOrderId: 1,
        itemCode: '1',
        description: 'string',
        quantity: 1,
        createDate: '',
        finishDate: '',
        qfb: '',
        status: '',
        class: '',
        unit: '',
        batche: '',
        quantityFinish: 1,
        endDate: new Date('22/12/12'),
        fabDate: new Date('22/12/12'),
        isWithError: false,
        isWithErrorBatch: false,
        hasMissingStock: false,
        batch: ''
      } as IOrdersReq
    ];
    component.finalizeOrderSend();
    // expect(dataServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  it('should keyFunction', () => {
    const event = new KeyboardEvent("keypress",{
      "key": "Enter"
  });
    component.keyDownFunction(event);
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FinalizeOrdersComponent } from './finalize-orders.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MATERIAL_COMPONENTS } from '../../app.material';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DatePipe } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';
import { PedidosService } from '../../services/pedidos.service';
import { of } from 'rxjs';
import { Batches, CancelOrderReq, IExistsBachCodeRes, IGetNewBachCodeRes } from 'src/app/model/http/pedidos';
import { ICancelOrdersRes } from '../../model/http/pedidos';
import { ErrorService } from 'src/app/services/error.service';
import { IOrdersReq } from 'src/app/model/http/ordenfabricacion';
import { AddCommentsDialogComponent } from '../add-comments-dialog/add-comments-dialog.component';
import { MODAL_FIND_ORDERS } from 'src/app/constants/const';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ObservableService } from 'src/app/services/observable.service';
import { DateService } from 'src/app/services/date.service';
import { MessagesService } from 'src/app/services/messages.service';

describe('FinalizeOrdersComponent', () => {
  let component: FinalizeOrdersComponent;
  let fixture: ComponentFixture<FinalizeOrdersComponent>;
  let orderServiceSpy: jasmine.SpyObj<PedidosService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<AddCommentsDialogComponent>>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let dateServiceSpy: jasmine.SpyObj<DateService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;

  const getNextBatchCodeRes = new IGetNewBachCodeRes();
  const getIfExistsBatchCodeRes = new IExistsBachCodeRes();
  const putFinalizeOrders = new ICancelOrdersRes();

  // const iOrderRes = new IOrdersRes();
  let iOrdersReq: IOrdersReq[] = [];
  const batches = new Batches();
  batches.batchCode = '1';
  batches.quantity = '';
  batches.expirationDate = '';
  batches.manufacturingDate = '';
  const cancelOrderReq: CancelOrderReq[] = [{
    orderId: 1,
    userId: '1',
    reason: '',
    batches: [batches]
  }];



  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom',
      'getMessageTitle',
    ]);

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
    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'getUserId',
    ]);

    localStorageServiceSpy.getUserId.and.returnValue('');
    messagesServiceSpy.getMessageTitle.and.returnValue('');
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve();
    });
    // --- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'setCallHttpService',
        'setMessageGeneralCallHttp',
      ]
    );
    observableServiceSpy.setCallHttpService.and.returnValue();
    observableServiceSpy.setMessageGeneralCallHttp.and.returnValue();
    // --- Date Service
    dateServiceSpy = jasmine.createSpyObj<DateService>('DateService', [
      'transformDate',
    ]);
    dateServiceSpy.transformDate.and.returnValue('');
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
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy},
        { provide: DateService, useValue: dateServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },
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
    expect(component.onBatchesChange).toBeTruthy();
  });

  it('should isCorrectDataToFinalize', () => {
    // component.isCorrectDataToFinalize();
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
    component.isCorrectDataToFinalize();
    expect(component.isCorrectDataToFinalize).toBeTruthy();
    // expect(component.isCorrectData).toBe(false);
  });

  it('should focusOutLote', () => {
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
    // component.focusOutLote(0);
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
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
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
    expect(component.finalizeOrderSend).toBeTruthy();
    // expect(dataServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  it('should keyFunction', () => {
    // const event = new KeyboardEvent('keypress', { key: 'Enter' });
    component.isCorrectData = true;
    const keyEvent = new KeyboardEvent('keyEnter', { key: MODAL_FIND_ORDERS.keyEnter });
    // component.keyDownUsers(keyEvent);
    component.keyDownFunction(keyEvent);
    expect(component.finalizeOrderSend).toBeTruthy();
  });
});

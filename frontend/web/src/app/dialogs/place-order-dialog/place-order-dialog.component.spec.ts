import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { PlaceOrderDialogComponent } from './place-order-dialog.component';
import { MATERIAL_COMPONENTS } from '../../app.material';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DataService } from '../../services/data.service';
import { MODAL_NAMES } from '../../constants/const';
import { PedidosService } from 'src/app/services/pedidos.service';
import { IQfbWithNumberRes, QfbWithNumber } from 'src/app/model/http/users';
import { of } from 'rxjs';
import { ObservableService } from 'src/app/services/observable.service';
import { MessagesService } from 'src/app/services/messages.service';

describe('PlaceOrderDialogComponent', () => {
  let component: PlaceOrderDialogComponent;
  let fixture: ComponentFixture<PlaceOrderDialogComponent>;
  let dataServiceSpy;
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;

  const iQfbWithNumberRes = new IQfbWithNumberRes();
  const qfbWithNumber: QfbWithNumber[] = [{
    countTotalOrders: 1,
    countTotalFabOrders: 1,
    countTotalPieces: 1,
    clasification: ''
  }];
  iQfbWithNumberRes.response = qfbWithNumber;

  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom'
    ]);

    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService',
      [
        'getFormattedNumber',
      ]);
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService',
      [
        'getQfbsWithOrders'
      ]);
    pedidosServiceSpy.getQfbsWithOrders.and.callFake(() => {
      return of(iQfbWithNumberRes);
    });
    // --- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'getCallHttpService',
        'setMessageGeneralCallHttp',
        'setUrlActive',
        'setQbfToPlace',
        'setIsLoading'
      ]
    );
    observableServiceSpy.getCallHttpService.and.returnValue(of());
    TestBed.configureTestingModule({
      declarations: [PlaceOrderDialogComponent],
      imports: [
        HttpClientTestingModule,
        MatDialogModule,
        MATERIAL_COMPONENTS,
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        { provide: MatDialogRef, useValue: { close: () => { } } },
        {
          provide: MAT_DIALOG_DATA, useValue: {
            placeOrdersData: {
              list: [],
              modalType: 'placeOrder',
              userId: '',
            }
          }
        },
        { provide: DataService, useValue: dataServiceSpy },
        DatePipe,
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlaceOrderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should call changePlaceManual()', () => {
    component.isPlaceManual = false;
    component.changePlaceManual();
    expect(component.isPlaceManual).toBeTruthy();
  });
  it('should call placeOrder()', () => {
    component.placeOrder('anyUserId', 'anyUserName');
    expect(observableServiceSpy.setQbfToPlace).toHaveBeenCalledWith({
      userId: 'anyUserId', userName: 'anyUserName',
      modalType: 'placeOrder', list: [], assignType: MODAL_NAMES.assignManual,
      isFromOrderIsolated: undefined, isFromReassign: undefined, clasification: 'mg'
    });

  });
  it('should call placeOrderAutomatic()', () => {
    component.placeOrderAutomatic();
    expect(observableServiceSpy.setQbfToPlace).toHaveBeenCalledWith({
      modalType: 'placeOrder',
      list: [],
      assignType: MODAL_NAMES.assignAutomatic
    });
  });

  it('should changeCurrentQfbs', () => {
    component.qfbs = qfbWithNumber;
    component.changeCurrentQfbs();
  });
});

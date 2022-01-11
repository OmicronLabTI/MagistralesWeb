import { async, ComponentFixture, fakeAsync, TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';

import { PlaceOrderDialogComponent } from './place-order-dialog.component';
import { MATERIAL_COMPONENTS } from '../../app.material';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DataService } from '../../services/data.service';
import { MODAL_NAMES } from '../../constants/const';
import { PedidosService } from 'src/app/services/pedidos.service';
import { QfbClassification, QfbWithNumber } from 'src/app/model/http/users';
import { of, throwError } from 'rxjs';
import { ObservableService } from 'src/app/services/observable.service';
import { MessagesService } from 'src/app/services/messages.service';
import { CountOrdersMock } from 'src/mocks/countOrdersMock';
import { ErrorService } from 'src/app/services/error.service';

describe('PlaceOrderDialogComponent', () => {
  let component: PlaceOrderDialogComponent;
  let fixture: ComponentFixture<PlaceOrderDialogComponent>;
  let dataServiceSpy;
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;

  const getQfbsWithOrdersMock = CountOrdersMock;

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
      return of(getQfbsWithOrdersMock);
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
    // --- Error Service
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', ['httpError']);

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
        { provide: PedidosService, useValue: pedidosServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
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

  it('getQfbsWithOrders should be success', fakeAsync(() => {
    pedidosServiceSpy.getQfbsWithOrders.and.returnValue(of(getQfbsWithOrdersMock));
    component.ngOnInit();
    expect(component.qfbs.length).toBeGreaterThanOrEqual(0);
  }));
  it('getQfbsWithOrders should failed', fakeAsync(() => {
    pedidosServiceSpy.getQfbsWithOrders.and.returnValue(throwError({ status: 500 }));
    component.ngOnInit();
    expect(pedidosServiceSpy.getQfbsWithOrders).toHaveBeenCalled();
  }));
  it('should change Type Qfb -> be', () => {
    component.changeTypeQfb(QfbClassification.be, true);
    expect(component.currentQfbType).toBe(QfbClassification.be);
  });
  it('should change Type Qfb -> mg', () => {
    component.changeTypeQfb(QfbClassification.mg, true);
    expect(component.currentQfbType).toBe(QfbClassification.mg);
  });
  it('should change Type Qfb -> mn', () => {
    component.changeTypeQfb(QfbClassification.mn, false);
    expect(component.currentQfbType).toBe(QfbClassification.mn);
  });
});

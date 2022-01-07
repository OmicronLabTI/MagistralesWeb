import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { FabordersListComponent } from './faborders-list.component';
import { DataService } from '../../services/data.service';
import { Observable, of, throwError } from 'rxjs';
import { FabOrderListMock } from '../../../mocks/fabOrderListMock';
import { OrdersService } from '../../services/orders.service';
import { ErrorService } from '../../services/error.service';
import { ErrorHttpInterface } from '../../model/http/commons';
import { PageEvent } from '@angular/material/paginator';
import { PipesModule } from '../../pipes/pipes.module';
import { ParamsPedidos } from 'src/app/model/http/pedidos';
import { IOrdersReq } from 'src/app/model/http/ordenfabricacion';
import { ObservableService } from 'src/app/services/observable.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { DateService } from 'src/app/services/date.service';
import { MessagesService } from 'src/app/services/messages.service';

describe('FabordersListComponent', () => {
  let component: FabordersListComponent;
  let fixture: ComponentFixture<FabordersListComponent>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let dataServiceSpy;
  let ordersServiceSpy;
  let errorServiceSpy;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let dateServiceSpy: jasmine.SpyObj<DateService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;

  const paramsPedidos = new ParamsPedidos();

  const iOrdersReq: IOrdersReq[] = [];
  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom'
    ]);

    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'getOrderIsolated',
      'removeOrderIsolated',
      'setFiltersActivesOrders',
      'getFiltersActivesOrders',
      'removeFiltersActiveOrders',
      'getFiltersActivesAsModelOrders',
    ]);

    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'getNewDataToFilter',
      'getItemOnDataOnlyIds',
      'getIsThereOnData',
      'getItemOnDateWithFilter',
      'getIsWithFilter',
      'changeRouterForFormula'
    ]);
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    ordersServiceSpy = jasmine.createSpyObj<OrdersService>('OrdersService', [
      'getOrders'
    ]);
    ordersServiceSpy.getOrders.and.callFake(() => {
      return of(FabOrderListMock);
    });
    localStorageServiceSpy.getOrderIsolated.and.callFake(() => {
      return '12345Id';
    });
    dataServiceSpy.getNewDataToFilter.and.callFake(() => {
      return [paramsPedidos, ''];
    });

    dataServiceSpy.getItemOnDataOnlyIds.and.callFake(() => {
      return [];
    });
    // --- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'getCallHttpService',
        'setMessageGeneralCallHttp',
        'setUrlActive',
        'setIsLoading',
        'setCallHttpService',
        'getNewSearchOrdersModal',
        'setSearchComponentModal',
        'setCancelOrders',
        'setQbfToPlace',
      ]);
    observableServiceSpy.getCallHttpService.and.callFake(() => {
      return new Observable();
    });
    observableServiceSpy.getNewSearchOrdersModal.and.callFake(() => {
      return new Observable();
    });
    observableServiceSpy.setCancelOrders.and.callFake(() => {
      return;
    });

    //  Date Service
    dateServiceSpy = jasmine.createSpyObj<DateService>('DateService', [
      'transformDate',
      'getDateFormatted',
    ]);
    dateServiceSpy.transformDate.and.returnValue('');
    dateServiceSpy.getDateFormatted.and.returnValue('');

    TestBed.configureTestingModule({
      declarations: [FabordersListComponent],
      imports: [RouterTestingModule, MATERIAL_COMPONENTS, HttpClientTestingModule,
        BrowserAnimationsModule, PipesModule],
      providers: [
        DatePipe,
        { provide: DataService, useValue: dataServiceSpy },
        { provide: OrdersService, useValue: ordersServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy},
        { provide: DateService, useValue: dateServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FabordersListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should updateAllComplete', () => {
    component.dataSource.data = FabOrderListMock.response;
    component.dataSource.data.forEach(user => user.isChecked = false);
    component.updateAllComplete();
    expect(component.allComplete).toBeFalsy();
    component.dataSource.data.forEach(user => user.isChecked = true);
    component.updateAllComplete();
    expect(component.allComplete).toBeTruthy();

  });
  it('should someComplete', () => {
    component.dataSource.data = [];
    component.dataSource.data = FabOrderListMock.response;
    component.dataSource.data.forEach(user => user.isChecked = false);
    component.allComplete = false;
    expect(component.someComplete()).toBeFalsy();
    component.dataSource.data.forEach(user => user.isChecked = true);
    expect(component.someComplete()).toBeTruthy();
  });
  it('should setAll', () => {
    component.dataSource.data = null;
    component.setAll(true);
    expect(component.allComplete).toBeTruthy();
    component.dataSource.data = FabOrderListMock.response;
    component.setAll(true);
    expect(component.allComplete).toBeTruthy();
    expect(component.dataSource.data.every(user => user.isChecked)).toBeTruthy();
    component.setAll(false);
    expect(component.allComplete).toBeFalsy();
    expect(component.dataSource.data.every(user => user.isChecked)).toBeFalsy();
  });
  it('should getOrdersAction()', () => {
    component.getOrdersAction();
    expect(component.dataSource.data).toEqual(FabOrderListMock.response);
    expect(component.isThereOrdersIsolatedToCancel).toBeFalsy();
    expect(component.isAssignOrderIsolated).toBeFalsy();
    expect(component.isReAssignOrderIsolated).toBeFalsy();
    expect(component.isFinalizeOrderIsolated).toBeFalsy();
  });
  it('should getOrdersAction() failed', () => {
    ordersServiceSpy.getOrders.and.callFake(() => {
      return throwError({ status: 500 } as ErrorHttpInterface);
    });
    component.getOrdersAction();
    // expect(errorServiceSpy.httpError).toHaveBeenCalled();
    expect(component.dataSource.data.length).toEqual(0);

    ordersServiceSpy.getOrders.and.callFake(() => {
      return throwError({ status: 404 } as ErrorHttpInterface);
    });
    component.getOrdersAction();
    expect(errorServiceSpy.httpError).not.toHaveBeenCalled();

  });
  it('should getFullQueryString', () => {
    component.queryString = '?status=Abierto';
    component.offset = 10;
    component.limit = 20;
    component.getFullQueryString();
    expect(component.fullQueryString).toEqual('?status=Abierto&offset=10&limit=20');
  });
  it('should getDateFormatted()', () => {
    component.getDateFormatted(new Date(), new Date(), true);
    /*expect(dataServiceSpy.transformDate).toHaveBeenCalledTimes(4);*/
  });
  it('should changeDataEvent()', () => {
    expect(component.changeDataEvent({ pageIndex: 0, pageSize: 5 } as PageEvent)).toEqual({ pageIndex: 0, pageSize: 5 } as PageEvent);
    expect(component.pageIndex).toEqual(0);
    expect(component.offset).toEqual(0);
    expect(component.limit).toEqual(5);
  });
  it('should createOrderIsolated()', () => {
    component.createOrderIsolated();
    expect(observableServiceSpy.setSearchComponentModal).toHaveBeenCalled();
  });
  it('should onSuccessSearchOrdersModal', () => {
    paramsPedidos[0] = [{
      finlabel: '',
      pageIndex: 1,
      offset: 1,
      limit: 10
    }];
    component.onSuccessSearchOrdersModal(paramsPedidos);
    // ordersServiceSpy.getOrders();
  });
  it('should materialRequestIsolatedOrder', () => {
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
    // component.materialRequestIsolatedOrder();
    // ordersServiceSpy.getOrders();
  });

  it('should cancelOrder', () => {
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
        status: 'abierto',
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
    component.cancelOrder();
  });

  it('should goToFormulaDetail', () => {
    component.goToFormulaDetail('98656');
  });

});

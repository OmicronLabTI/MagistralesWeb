import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {DatePipe} from '@angular/common';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import { FabordersListComponent } from './faborders-list.component';
import {DataService} from '../../services/data.service';
import {Observable, of, throwError} from 'rxjs';
import {FabOrderListMock} from '../../../mocks/fabOrderListMock';
import {OrdersService} from '../../services/orders.service';
import {ErrorService} from '../../services/error.service';
import {ErrorHttpInterface} from '../../model/http/commons';
import {PageEvent} from '@angular/material/paginator';
import {PipesModule} from '../../pipes/pipes.module';

describe('FabordersListComponent', () => {
  let component: FabordersListComponent;
  let fixture: ComponentFixture<FabordersListComponent>;
  let dataServiceSpy;
  let ordersServiceSpy;
  let errorServiceSpy;
  beforeEach(async(() => {
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'presentToastCustom', 'getCallHttpService', 'setMessageGeneralCallHttp', 'setUrlActive', 'setIsLoading',
      'setCallHttpService', 'setMessageGeneralCallHttp', 'getOrderIsolated', 'removeOrderIsolated', 'getNewSearchOrdersModal',
        'getCallHttpService', 'transformDate', 'setSearchComponentModal', 'getNewDataToFilter', 'setCancelOrders', 'setQbfToPlace',
        'getItemOnDataOnlyIds', 'getIsThereOnData', 'getItemOnDateWithFilter', 'setFiltersActivesOrders', 'getFiltersActivesOrders',
      'removeFiltersActiveOrders', 'getFiltersActivesAsModelOrders'
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
    dataServiceSpy.getNewSearchOrdersModal.and.callFake(() => {
      return new Observable();
    });
    dataServiceSpy.getCallHttpService.and.callFake(() => {
      return new Observable();
    });
    dataServiceSpy.getOrderIsolated.and.callFake(() => {
      return '12345Id';
    });
    TestBed.configureTestingModule({
      declarations: [ FabordersListComponent ],
      imports: [ RouterTestingModule, MATERIAL_COMPONENTS, HttpClientTestingModule,
        BrowserAnimationsModule, PipesModule ],
      providers: [
        DatePipe,
        { provide: DataService, useValue: dataServiceSpy },
        { provide: OrdersService, useValue: ordersServiceSpy }
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
    component.dataSource.data.forEach( user => user.isChecked = false);
    component.updateAllComplete();
    expect(component.allComplete).toBeFalsy();
    component.dataSource.data.forEach( user => user.isChecked = true);
    component.updateAllComplete();
    expect(component.allComplete).toBeTruthy();

  });
  it('should someComplete', () => {
    component.dataSource.data = [];
    component.dataSource.data = FabOrderListMock.response;
    component.dataSource.data.forEach( user => user.isChecked = false);
    component.allComplete = false;
    expect(component.someComplete()).toBeFalsy();
    component.dataSource.data.forEach( user => user.isChecked = true);
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
    expect(component.changeDataEvent({pageIndex: 0, pageSize: 5} as PageEvent)).toEqual({pageIndex: 0, pageSize: 5} as PageEvent);
    expect(component.pageIndex ).toEqual(0);
    expect(component.offset).toEqual(0);
    expect(component.limit).toEqual(5);
  });
  it('should createOrderIsolated()', () => {
    component.createOrderIsolated();
    expect(dataServiceSpy.setSearchComponentModal).toHaveBeenCalled();
  });
});

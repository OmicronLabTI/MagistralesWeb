import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { PedidosComponent } from './pedidos.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { PedidosService } from '../../services/pedidos.service';
import { of, throwError } from 'rxjs';
import { PedidosListMock } from '../../../mocks/pedidosListMock';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ConstStatus } from '../../constants/const';
import { PageEvent } from '@angular/material/paginator';
import { DataService } from '../../services/data.service';
import Swal from 'sweetalert2';
import { IProcessOrdersRes, ParamsPedidos } from '../../model/http/pedidos';
import { PipesModule } from '../../pipes/pipes.module';
import { RangeDateMOck } from '../../../mocks/rangeDateMock';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ObservableService } from 'src/app/services/observable.service';
import { DateService } from 'src/app/services/date.service';

describe('PedidosComponent', () => {
  let component: PedidosComponent;
  let fixture: ComponentFixture<PedidosComponent>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let pedidosServiceSpy;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let dateServiceSpy: jasmine.SpyObj<DateService>
  const paramsPedidos = new ParamsPedidos();
  beforeEach(async(() => {
    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'getUserId', 'setRefreshToken',
    ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService',
      [
        'presentToastCustom',
        'setFiltersActives',
        'getFiltersActives',
        'removeFiltersActive',
        'getFiltersActivesAsModel',
        'getItemOnDataOnlyIds',
        'getUserRole',
        'getIsThereOnData',
      ]);
    dataServiceSpy.getIsThereOnData.and.returnValue(true);
    dataServiceSpy.getFiltersActives.and.returnValue('');
    dataServiceSpy.getFiltersActivesAsModel.and.returnValue(paramsPedidos);
    dataServiceSpy.getItemOnDataOnlyIds.and.returnValue([]);
    dataServiceSpy.presentToastCustom.and.returnValue(Promise.resolve(true));
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getPedidos', 'processOrders', 'getInitRangeDate'
    ]);
    pedidosServiceSpy.processOrders.and.callFake(() => {
      return of({ success: true, response: ['id'] } as IProcessOrdersRes);
    });
    pedidosServiceSpy.getInitRangeDate.and.callFake(() => {
      return of(RangeDateMOck);
    });
    pedidosServiceSpy.getPedidos.and.callFake(() => {
      return of(PedidosListMock);
    });

    // -- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>
      ('ObservableService',
        [
          'getCallHttpService',
          'setMessageGeneralCallHttp',
          'setUrlActive',
          'setQbfToPlace',
          'setIsLoading',
          'setFinalizeOrders',
          'setCancelOrders',
          'getNewCommentsResult',
          'getNewSearchOrdersModal'
        ]
      );
    observableServiceSpy.getCallHttpService.and.returnValue(of());
    observableServiceSpy.getNewSearchOrdersModal.and.returnValue(of());
    observableServiceSpy.getNewCommentsResult.and.returnValue(of());
    // --- Date Service
    dateServiceSpy = jasmine.createSpyObj<DateService>('DateService', 
    [
      'transformDate',
      'getDateFormatted',
    ]);
    dateServiceSpy.transformDate.and.returnValue('');
    dateServiceSpy.getDateFormatted.and.returnValue('');
    TestBed.configureTestingModule({
      declarations: [PedidosComponent],
      imports: [RouterTestingModule, MATERIAL_COMPONENTS,
        HttpClientTestingModule, BrowserAnimationsModule, PipesModule],
      providers: [
        DatePipe,
        { provide: PedidosService, useValue: pedidosServiceSpy },
        // { provide: DataService, useValue: dataServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: DateService, useValue: dateServiceSpy },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PedidosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.displayedColumns)
      .toEqual(['seleccion', 'codigo', 'codigoDxp', 'cliente', 'medico', 'asesor',
        'orderType', 'f_inicio', 'f_fin', 'qfb_asignado', 'status', 'actions']);
    expect(component.limit).toEqual(10);
    expect(component.offset).toEqual(0);
  });
  it('should call getPedidos ok', () => {
    component.offset = 0;
    component.limit = 10;
    component.queryString = 'rango de fechas';
    component.getFullQueryString();
    expect(component.fullQueryString).toEqual(`${component.queryString}&offset=0&limit=10`);
    component.getPedidos();
    expect(pedidosServiceSpy.getPedidos).toHaveBeenCalledWith(`${component.queryString}&offset=0&limit=10`);
    expect(component.lengthPaginator).toEqual(PedidosListMock.comments);
    expect(component.dataSource.data).toEqual(PedidosListMock.response);
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.abierto)
      .forEach(pedido => expect(pedido.class).toEqual('abierto'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.planificado)
      .forEach(pedido => expect(pedido.class).toEqual('planificado'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.liberado)
      .forEach(pedido => expect(pedido.class).toEqual('liberado'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.cancelado)
      .forEach(pedido => expect(pedido.class).toEqual('cancelado'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.enProceso)
      .forEach(pedido => expect(pedido.class).toEqual('proceso'));
    expect(component.isThereOrdersToPlan).toBeFalsy();
    expect(component.isThereOrdersToPlace).toBeFalsy();
    expect(component.isThereOrdersToCancel).toBeFalsy();
    expect(component.isThereOrdersToFinalize).toBeFalsy();
  });
  it('should call getPedidos error', () => {
    pedidosServiceSpy.getPedidos.and.callFake(() => {
      return throwError({ status: 500 });
    });
    component.getPedidos();
    expect(component.dataSource.data.length).toEqual(0);
  });
  // only Locally
  it('should updateAllComplete', () => {
    component.dataSource.data = PedidosListMock.response;
    component.dataSource.data.forEach(user => user.isChecked = false);
    component.updateAllComplete();
    expect(component.allComplete).toBeFalsy();
    component.dataSource.data.forEach(user => user.isChecked = true);
    component.updateAllComplete();
    expect(component.allComplete).toBeTruthy();

  });
  it('should someComplete', () => {
    component.dataSource.data = [];
    component.dataSource.data = PedidosListMock.response;
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
    component.dataSource.data = PedidosListMock.response;
    component.setAll(true);
    expect(component.allComplete).toBeTruthy();
    expect(component.dataSource.data.every(user => user.isChecked)).toBeTruthy();
    component.setAll(false);
    expect(component.allComplete).toBeFalsy();
    expect(component.dataSource.data.every(user => user.isChecked)).toBeFalsy();
  });
  it('should changeDataEvent', () => {
    expect(component.changeDataEvent({ pageIndex: 0, pageSize: 5 } as PageEvent)).toEqual({ pageIndex: 0, pageSize: 5 } as PageEvent);
    expect(component.pageIndex).toEqual(0);
    expect(component.offset).toEqual(0);
    expect(component.limit).toEqual(5);
  });
  it('should openPlaceOrdersDialog', () => {
    component.dataSource.data = [];
    component.openPlaceOrdersDialog();
    component.dataSource.data = PedidosListMock.response;
    component.dataSource.data.filter(order => order.pedidoStatus === ConstStatus.planificado)
      .forEach(order => order.isChecked = true);
  });
  it('should cancelOrders', () => {
    component.dataSource.data = [];
    component.cancelOrders();
    component.dataSource.data = PedidosListMock.response;
    component.dataSource.data.filter(order => order.pedidoStatus !== ConstStatus.finalizado)
      .forEach(order => order.isChecked = true);
  });
  it('should finalizeOrders', () => {
    component.dataSource.data = [];
    component.finalizeOrders();
    component.dataSource.data = PedidosListMock.response;
    component.dataSource.data.filter(order => order.pedidoStatus === ConstStatus.terminado)
      .forEach(order => order.isChecked = true);
  });
  it('should processOrders', (done) => {
    component.dataSource.data = [];
    component.processOrdersService();
    setTimeout(() => {
      expect(Swal.isVisible()).toBeTruthy();
      Swal.clickConfirm();
      // expect(pedidosServiceSpy.processOrders).toHaveBeenCalled();
      done();
    });
  });
});

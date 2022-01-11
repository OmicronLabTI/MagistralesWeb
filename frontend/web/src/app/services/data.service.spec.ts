import { TestBed } from '@angular/core/testing';
import { DatePipe } from '@angular/common';
import { DataService } from './data.service';
import { Catalogs, IPedidoReq, ParamsPedidos } from '../model/http/pedidos';
import { RouterTestingModule } from '@angular/router/testing';
import { ConstOrders, FromToFilter, ConstStatus, CarouselOptionString } from '../constants/const';
import { FabOrderListMock } from 'src/mocks/fabOrderListMock';
import { IOrdersReq } from '../model/http/ordenfabricacion';
import { PedidosListMock } from 'src/mocks/pedidosListMock';
import { DetailOrderMock } from 'src/mocks/detailOrder.Mock';
import { IPedidoDetalleReq } from '../model/http/detallepedidos.model';
import { INCIDENTS_GRAPHIC_MATRIX_MOCK } from 'src/mocks/incidentsGraphicsMatrixMock';
import { Router } from '@angular/router';

fdescribe('DataService', () => {
  let routerSpy: jasmine.SpyObj<Router>;
  const catalogs = new Catalogs();
  catalogs.id = 74;
  catalogs.value = 'DZ';
  catalogs.type = 'string';
  catalogs.field = 'ProductNoLabel';

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj<Router>('Router', ['navigate']);
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        DatePipe,
        { provide: Router, useValue: routerSpy },
      ]
    });
  });

  it('should be created', () => {
    const service: DataService = TestBed.get(DataService);
    expect(service).toBeTruthy();
  });

  it('should getIsToSaveAnything', () => {
    const service: DataService = TestBed.get(DataService);
    service.setIsToSaveAnything(false);
    expect(service.getIsToSaveAnything()).toBeFalsy();
    service.setIsToSaveAnything(true);
    expect(service.getIsToSaveAnything()).toBeTruthy();
  });
  it('should getfiniOrffin', () => {
    const service: DataService = TestBed.get(DataService);
    const paramPedidos = new ParamsPedidos();
    const result = service.getfiniOrffin(paramPedidos, '12/12/21');
    expect(result).toEqual('?ffin=12/12/21');
  });
  it('should getfiniOrffin with defaultDateInit', () => {
    const service: DataService = TestBed.get(DataService);
    const paramPedidos = new ParamsPedidos();
    paramPedidos.dateType = ConstOrders.defaultDateInit;
    const result = service.getfiniOrffin(paramPedidos, '12/12/21');
    expect(result).toEqual('?fini=12/12/21');
  });
  it('should getFormattedNumber', () => {
    const service: DataService = TestBed.get(DataService);
    const result = service.getFormattedNumber(5);
    expect(result).toEqual('5');
  });
  it('should getItemOnDataOnlyIds when type is fromOrders', () => {
    const service: DataService = TestBed.get(DataService);
    const ordersMock = PedidosListMock.response as IPedidoReq[];
    ordersMock.forEach(x => {
      x.isChecked = true;
      x.pedidoStatus = ConstStatus.planificado;
    });
    const result = service.getItemOnDataOnlyIds(ordersMock, FromToFilter.fromOrders);
    expect(result.length).toBeGreaterThanOrEqual(5);
    expect(result[0]).toEqual(60021);
    expect(result[1]).toEqual(60022);
    expect(result[2]).toEqual(60023);
    expect(result[3]).toEqual(60024);
    expect(result[4]).toEqual(60025);
  });
  it('should getItemOnDataOnlyIds when type fromDetailOrder', () => {
    const service: DataService = TestBed.get(DataService);
    const ordersMock = DetailOrderMock.response as IPedidoDetalleReq[];
    ordersMock.forEach(x => {
      x.isChecked = true;
      x.status = ConstStatus.planificado;
    });
    const result = service.getItemOnDataOnlyIds(ordersMock, FromToFilter.fromDetailOrder);
    expect(result.length).toBeGreaterThanOrEqual(6);
    expect(result[0]).toEqual(0);
    expect(result[1]).toEqual(89101);
    expect(result[2]).toEqual(89102);
    expect(result[3]).toEqual(89103);
    expect(result[4]).toEqual(89104);
    expect(result[5]).toEqual(89105);
  });
  it('should getItemOnDataOnlyIds when type fromDetailOrderQr', () => {
    const service: DataService = TestBed.get(DataService);
    const ordersMock = DetailOrderMock.response as IPedidoDetalleReq[];
    ordersMock.forEach(x => {
      x.isChecked = true;
      x.status = ConstStatus.planificado;
    });
    const result = service.getItemOnDataOnlyIds(ordersMock, FromToFilter.fromDetailOrderQr);
    expect(result.length).toBeGreaterThanOrEqual(6);
    expect(result[0]).toEqual(0);
    expect(result[1]).toEqual(89101);
    expect(result[2]).toEqual(89102);
    expect(result[3]).toEqual(89103);
    expect(result[4]).toEqual(89104);
    expect(result[5]).toEqual(89105);
  });
  it('should getItemOnDataOnlyIds when type fromOrdersIsolated', () => {
    const service: DataService = TestBed.get(DataService);
    const ordersMock = FabOrderListMock.response as IOrdersReq[];
    ordersMock.forEach(x => {
      x.isChecked = true;
      x.status = ConstStatus.planificado;
    });
    const result = service.getItemOnDataOnlyIds(ordersMock, FromToFilter.fromOrdersIsolated);
    expect(result.length).toBeGreaterThanOrEqual(10);
    expect(result[0]).toEqual(89374);
    expect(result[1]).toEqual(89375);
    expect(result[2]).toEqual(89376);
    expect(result[3]).toEqual(89377);
    expect(result[4]).toEqual(89378);
    expect(result[5]).toEqual(89379);
    expect(result[6]).toEqual(89380);
    expect(result[7]).toEqual(89381);
    expect(result[8]).toEqual(89382);
    expect(result[9]).toEqual(89383);
  });
  it('should getNormalizeString', () => {
    const service: DataService = TestBed.get(DataService);
    const result = service.getNormalizeString('\u1E9B\u0323');
    expect(result).toEqual('ſ');
  });
  it('should getPercentageByItem type number format', () => {
    const service: DataService = TestBed.get(DataService);
    const result = service.getPercentageByItem(2, [1, 23, 5], true);
    expect(result).toBe(7);
  });
  it('should getPercentageByItem type string number format', () => {
    const service: DataService = TestBed.get(DataService);
    const result = service.getPercentageByItem(2, [1, 23, 5], false);
    expect(result).toBe('7 %');
  });
  it('should getDataForGraphic', () => {
    const service: DataService = TestBed.get(DataService);
    const mock = INCIDENTS_GRAPHIC_MATRIX_MOCK;
    const result = service.getDataForGraphic(mock[0], true);
    expect(result.labels.length).toBe(1);
    expect(result.labels[0]).toBe('IncidentReason');
  });
  it('should getOptionsGraphToShow', () => {
    const service: DataService = TestBed.get(DataService);
    const result = service.getOptionsGraphToShow(true, 'titulo');
    expect(result.title.text).toBe('titulo');
  });
  it('should getRandomColorsArray', () => {
    const service: DataService = TestBed.get(DataService);
    const result = service.getRandomColorsArray(2, true);
    expect(result.length).toBe(2);
    expect(result[0]).toBe('rgb(224,168,125)');
    expect(result[1]).toBe('rgb(224,25,64)');
  });
  it('should changeRouterForFormula', () => {
    const service: DataService = TestBed.get(DataService);
    service.changeRouterForFormula('122356', '1234', 0);
    expect(routerSpy.navigate).toHaveBeenCalled();
  });
  it('should getFullStringForCarousel', () => {
    const service: DataService = TestBed.get(DataService);
    const result = service.getFullStringForCarousel('baseQueryString', 'currentOrdenFabricacionId', CarouselOptionString.backDetail);
    expect(result).toBe('baseQueryString&current=currentOrdenFabricacionId&advance=b');
  });
  it('should getRangeOrders with range', () => {
    const service: DataService = TestBed.get(DataService);
    const result = service.getRangeOrders(123, 126);
    expect(result).toBe('?docNum=123-126');
    console.log('result', result);
  });
  it('should getRangeOrders without range', () => {
    const service: DataService = TestBed.get(DataService);
    const result = service.getRangeOrders(123, '');
    expect(result).toBe('?docNum=123-123');
  });
  it('should not inputNumbersOnly', () => {
    const service: DataService = TestBed.get(DataService);
    const event = new KeyboardEvent('keypress', {
      key: 'escape'
    });
    const result = service.inputNumbersOnly(event);
    expect(result).toBeFalsy();
  });
  it('should inputNumbersOnly', () => {
    const service: DataService = TestBed.get(DataService);
    const event = new KeyboardEvent('keypress', {
      key: '1'
    });
    const result = service.inputNumbersOnly(event);
    expect(result).toBeTruthy();
  });
});

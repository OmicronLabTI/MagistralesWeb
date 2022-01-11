import { TestBed } from '@angular/core/testing';
import { DatePipe } from '@angular/common';
import { DataService } from './data.service';
import { Catalogs, ParamsPedidos } from '../model/http/pedidos';
import { RouterTestingModule } from '@angular/router/testing';
import { ConstOrders, TypeToSeeTap } from '../constants/const';

describe('DataService', () => {
  const catalogs = new Catalogs();
  catalogs.id = 74;
  catalogs.value = 'DZ';
  catalogs.type = 'string';
  catalogs.field = 'ProductNoLabel';

  beforeEach(() => TestBed.configureTestingModule({
    imports: [RouterTestingModule],
    providers: [DatePipe]
  }));

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
});

import { TestBed } from '@angular/core/testing';
import { ConstOrders, ConstStatus, FromToFilter } from '../constants/const';

import { FiltersService } from './filters.service';
import { DatePipe } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';
import { ParamsPedidos } from '../model/http/pedidos';

describe('FiltersService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      RouterTestingModule
    ],
    providers: [
      DatePipe,
    ]
  }));

  it('should be created', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    expect(service).toBeTruthy();
  });
  it('should getIsThereOnData', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    expect(service.getIsThereOnData([
      {
        isChecked: true,
        pedidoStatus: ConstStatus.cancelado
      }
    ], ConstStatus.cancelado, FromToFilter.fromOrders)).toBeTruthy();
    expect(service.getIsThereOnData([
      {
        isChecked: true,
        pedidoStatus: ConstStatus.terminado
      }
    ], ConstStatus.terminado, FromToFilter.fromOrdersReassign)).toBeTruthy();
    expect(service.getIsThereOnData([
      {
        isChecked: true,
        pedidoStatus: ConstStatus.terminado
      }
    ], ConstStatus.cancelado, FromToFilter.fromOrdersCancel)).toBeTruthy();
    expect(service.getIsThereOnData([{
      isChecked: true,
      pedidoStatus: ConstStatus.terminado
    }], ConstStatus.cancelado, FromToFilter.fromDetailOrder)).toBeTruthy();
    expect(service.getIsThereOnData([{
      isChecked: true,
      status: ConstStatus.cancelado
    }], ConstStatus.cancelado, FromToFilter.fromOrderIsolatedReassign)).toBeTruthy();
    expect(service.getIsThereOnData([{
      isChecked: true,
      status: ConstStatus.terminado
    }], ConstStatus.cancelado, FromToFilter.fromOrdersIsolatedCancel)).toBeTruthy();
    expect(service.getIsThereOnData([{
      isChecked: true,
      status: ConstStatus.terminado,
      finishedLabel: 0
    }], ConstStatus.cancelado, FromToFilter.fromOrderDetailLabel)).toBeTruthy();
    expect(service.getIsThereOnData([{
      isChecked: true,
      status: ConstStatus.terminado,
    }], ConstStatus.terminado, FromToFilter.fromDefault)).toBeTruthy();
  });
  it('should getItemOnDateWithFilter', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    expect(service.getItemOnDateWithFilter([{
      isChecked: true,
      status: ConstStatus.reasingado
    },
    {
      isChecked: true,
      status: ConstStatus.asignado
    },
    {
      isChecked: true,
      status: ConstStatus.enProceso
    },    {
      isChecked: true,
      status: ConstStatus.pendiente
    },
    {
      isChecked: true,
      status: ConstStatus.terminado
    }], FromToFilter.fromOrderIsolatedReassignItems, ConstStatus.cancelado).length).toEqual(5);
    expect(service.getItemOnDateWithFilter([], FromToFilter.fromOrdersReassign, ConstStatus.cancelado).length).toEqual(0);
    expect(service.getItemOnDateWithFilter([
      {
        isChecked: true,
        status: ConstStatus.terminado,
      }
    ], FromToFilter.fromDefault, ConstStatus.terminado).length).toEqual(1);

  });
  it('should continueGetDataOrders', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    const params = new ParamsPedidos();
    params.status = 'ACTIVO';
    params.clientName = 'DANIEL';
    params.orderIncidents = 123456;
    const params2 = new ParamsPedidos();
    const url = service.continueGetDataOrders(params, '', params2);
    expect(url.includes('status=ACTIVO')).toBeTruthy();
    expect(url.includes('cliente=DANIEL')).toBeTruthy();
    expect(url.includes('docnum=123456')).toBeTruthy();
  });
  it('should getIsWithFilter without filters', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    const params = new ParamsPedidos();
    params.dateType = ConstOrders.defaultDateInit;
    params.orderIncidents = null;
    params.docNum = '';
    expect(service.getIsWithFilter(params)).toBeFalsy();

    const params2 = new ParamsPedidos();
    params2.dateType = ConstOrders.defaultDateInit;
    params2.orderIncidents = 1;
    params2.docNum = '';
    expect(service.getIsWithFilter(params2)).toBeTruthy();


    const params3 = new ParamsPedidos();
    params3.dateType = ConstOrders.dateFinishType;
    params3.orderIncidents = 1;
    params3.docNum = '';
    expect(service.getIsWithFilter(params3)).toBeTruthy();

    const params4 = new ParamsPedidos();
    params4.dateType = ConstOrders.dateFinishType;
    params4.orderIncidents = null;
    params4.docNum = '';
    expect(service.getIsWithFilter(params4)).toBeTruthy();
  });
  it('should getNewDataToFilter with docnum', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    const params = new ParamsPedidos();
    params.docNum = '12345';
    const res = service.getNewDataToFilter(params);
    expect(res[0] instanceof ParamsPedidos).toBeTruthy();
  });

  it('should getNewDataToFilter with dxp', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    const params = new ParamsPedidos();
    params.docNumDxp = '123456';
    const res = service.getNewDataToFilter(params);
    expect(res[0] instanceof ParamsPedidos).toBeTruthy();
    expect(res[1].includes('docNumDxp=123456')).toBeTruthy();
  });

  it('should getNewDataToFilter with dateType', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    const params = new ParamsPedidos();
    params.dateType = '0';
    params.fini = new Date('12/12/2023');
    const res = service.getNewDataToFilter(params);
    expect(res[0] instanceof ParamsPedidos).toBeTruthy();
    expect(res[1].includes('fini=12/12/2023')).toBeTruthy();
  });

  it('should getNewDataToFilter default', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    const params = new ParamsPedidos();
    params.dateType = '0';
    params.dateFull = '12/12/2023';
    const res = service.getNewDataToFilter(params);
    expect(res[0] instanceof ParamsPedidos).toBeTruthy();
    expect(res[1].includes('fini=12/12/2023')).toBeTruthy();
  });
});

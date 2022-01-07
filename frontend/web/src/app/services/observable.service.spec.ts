import { TestBed } from '@angular/core/testing';
import { ConstOrders, ConstStatus, HttpServiceTOCall } from '../constants/const';
import { CommentsConfig } from '../model/device/incidents.model';
import { ParamsPedidos } from '../model/http/pedidos';

import { ObservableService } from './observable.service';

describe('ObservableService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    expect(service).toBeTruthy();
  });
  it('should is loading true', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getIsLoading().subscribe(res => {
      expect(res).toBeTruthy();
      done();
    });
    service.setIsLoading(true);
  });
  it('should newCommentsResult', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    const commentsConfig = { isReadOnly: true, isForClose: true, comments: 'anyComments' } as CommentsConfig;
    service.getNewCommentsResult().subscribe(res => {
      expect(res).toEqual(commentsConfig);
      done();
    });
    service.setNewCommentsResult(commentsConfig);
  });
  it('should openDialog', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    const commentsConfig = { isReadOnly: true, isForClose: true, comments: 'anyComments' } as CommentsConfig;
    service.getOpenCommentsDialog().subscribe(res => {
      expect(res).toEqual(commentsConfig);
      done();
    });
    service.setOpenCommentsDialog(commentsConfig);
  });
  it('should newDataSignature', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getNewDataSignature().subscribe(res => {
      expect(res).toEqual('signature on base 64');
      done();
    });
    service.setNewDataSignature('signature on base 64');
  });
  it('should openSignatureDialog', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getOpenSignatureDialog().subscribe(res => {
      expect(res).toEqual('signature on base 64');
      done();
    });
    service.setOpenSignatureDialog('signature on base 64');
  });

  it('should newFormulaComponent', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getNewFormulaComponent().subscribe(res => {
      expect(res).toEqual({ modalType: 'formulaComponent' });
      done();
    });
    service.setNewFormulaComponent({ modalType: 'formulaComponent' });
  });
  it('should materialComponent', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getNewMaterialComponent().subscribe(res => {
      expect(res).toEqual('materialComponentData');
      done();
    });
    service.setNewMaterialComponent('materialComponentData');
  });
  it('should searchOrdersModal', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    const filterOrdersDataConfig = {
      dateType: ConstOrders.defaultDateInit,
      dateFull: '20/12/2020-01/12/2021', isFromIncidents: true, clientName: 'Client name',
      status: ConstStatus.finalizado, docNum: 12345
    } as ParamsPedidos;
    service.getSearchOrdersModal().subscribe(res => {
      expect(res.filterOrdersData).toEqual(filterOrdersDataConfig);
      done();
    });
    service.setSearchOrdersModal({
      modalType: 'search',
      filterOrdersData: filterOrdersDataConfig, data: [123, 1234, 1345]
    });
  });
  it('should searchComponentModal', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    const filterOrdersDataConfig = {
      dateType: ConstOrders.defaultDateInit,
      dateFull: '20/12/2020-01/12/2021', isFromIncidents: true, clientName: 'Client name',
      status: ConstStatus.finalizado, docNum: 12345
    } as ParamsPedidos;
    service.getSearchComponentModal().subscribe(res => {
      expect(res.filterOrdersData).toEqual(filterOrdersDataConfig);
      done();
    });
    service.setSearchComponentModal({
      modalType: 'search',
      filterOrdersData: filterOrdersDataConfig, data: [123, 1234, 1345]
    });
  });
  it('should is loading false', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getIsLoading().subscribe(res => {
      expect(res).toBeFalsy();
      done();
    });
    service.setIsLoading(false);
  });

  it('should set message', (done) => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getGeneralNotificationMessage().subscribe(res => {
      expect(res).toBe('test');
      done();
    });
    service.setGeneralNotificationMessage('test');
  });

  it('should getQfbToPlace', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getQfbToPlace().subscribe(resultQfbToPlace => {
      expect(resultQfbToPlace.userId).toEqual('userId');
      expect(resultQfbToPlace.userName).toEqual('name');
      expect(resultQfbToPlace.list).toEqual([1, 20]);
      expect(resultQfbToPlace.assignType).toEqual('manual');
      expect(resultQfbToPlace.countTotalPieces).toEqual(2);
      expect(resultQfbToPlace.modalType).toEqual('Pedido');
    });
    service.setQbfToPlace({userId: 'userId', list: [1, 20], userName: 'name',
      assignType: 'manual', countTotalPieces: 2, modalType: 'Pedido'});
  });
  it('should getIsLogin true', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getIsLogin().subscribe(isLogin => {
      expect(isLogin).toBeTruthy();
    });
    service.setIsLogin(true);
  });
  it('should getIsLogin false', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getIsLogin().subscribe(isLogin => {
      expect(isLogin).toBeFalsy();
    });
    service.setIsLogin(false);
  });
  it('should getCallHttpService', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getCallHttpService().subscribe(callHttpService => {
      expect(callHttpService).toEqual(HttpServiceTOCall.ORDERS);
    });
    service.setCallHttpService(HttpServiceTOCall.ORDERS);
  });
  it('should getMessageGeneralCalHttp', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getMessageGeneralCalHttp().subscribe(messageGeneral => {
      expect(messageGeneral).toEqual({title: 'title', icon: 'success', isButtonAccept: true});
    });
    service.setMessageGeneralCallHttp({title: 'title', icon: 'success', isButtonAccept: true});
  });
  it('should getUrlActive', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getUrlActive().subscribe(urlActive => {
      expect(urlActive).toEqual(HttpServiceTOCall.ORDERS);
    });
    service.setUrlActive(HttpServiceTOCall.ORDERS);
  });
  it('should getCancelOrder', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getCancelOrder().subscribe(resultCancel => {
      expect(resultCancel).toEqual({list: [{orderId: 55, reason: 'Hubo un error', userId: 'user-id-22'}], cancelType: 'Pedido'});
    });
    service.setCancelOrders({list: [{orderId: 55, reason: 'Hubo un error', userId: 'user-id-22'}], cancelType: 'Pedido'});
  });
  it('should getIsLogout true', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getIsLogout().subscribe(isLogout => {
      expect(isLogout).toBeTruthy();
    });
    service.setIsLogout(true);
  });
  it('should getIsLogout false', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getIsLogout().subscribe(isLogout => {
      expect(isLogout).toBeFalsy();
    });
    service.setIsLogout(false);
  });
  it('should getPathUrl', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getPathUrl().subscribe(pathUrl => {
      expect(pathUrl).toEqual(['orders']);
    });
    service.setPathUrl(['orders']);
  });
  it('should getFinalizeOrders', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getFinalizeOrders().subscribe(finalizeOrders => {
      expect(finalizeOrders).toEqual({list: [{orderId: 123}], cancelType: 'pedidos'});
    });
    service.setFinalizeOrders({list: [{orderId: 123}], cancelType: 'pedidos'});
  });
  it('should getNewSearchOrdersModal', () => {
    const service: ObservableService = TestBed.get(ObservableService);
    service.getNewSearchOrdersModal().subscribe(newSearchOrdersModal => {
      expect(newSearchOrdersModal).toEqual({dateType: 'Pedido', docNum: 1234, fini: new Date('01/12/2020'),
        ffin: new Date('01/12/2020'), status: 'Finalizado', finlabel: '1'});
    });
    service.setNewSearchOrderModal({dateType: 'Pedido', docNum: 1234, fini: new Date('01/12/2020'), ffin: new Date('01/12/2020'),
      status: 'Finalizado', finlabel: '1'});
  });

});

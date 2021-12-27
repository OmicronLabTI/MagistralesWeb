import {TestBed} from '@angular/core/testing';
import {DatePipe} from '@angular/common';
import {DataService} from './data.service';
import {
  ConstOrders,
  ConstStatus,
  FromToFilter,
  HttpServiceTOCall,
  MessageType,
} from '../constants/const';
import {CommentsConfig} from '../model/device/incidents.model';
import {Catalogs, ParamsPedidos} from '../model/http/pedidos';
import {RouterTestingModule} from '@angular/router/testing';

describe('DataService', () => {
  let catalogs = new Catalogs();
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

  it('should is loading true', (done) => {
    const service: DataService = TestBed.get(DataService);
    service.getIsLoading().subscribe(res => {
      expect(res).toBeTruthy();
      done();
    });
    service.setIsLoading(true);
  });
  it('should newCommentsResult', (done) => {
    const service: DataService = TestBed.get(DataService);
    const commentsConfig = {isReadOnly: true, isForClose: true, comments: 'anyComments'} as CommentsConfig;
    service.getNewCommentsResult().subscribe(res => {
      expect(res).toEqual(commentsConfig);
      done();
    });
    service.setNewCommentsResult(commentsConfig);
  });
  it('should openDialog', (done) => {
    const service: DataService = TestBed.get(DataService);
    const commentsConfig = {isReadOnly: true, isForClose: true, comments: 'anyComments'} as CommentsConfig;
    service.getOpenCommentsDialog().subscribe(res => {
      expect(res).toEqual(commentsConfig);
      done();
    });
    service.setOpenCommentsDialog(commentsConfig);
  });
  it('should newDataSignature', (done) => {
    const service: DataService = TestBed.get(DataService);
    service.getNewDataSignature().subscribe(res => {
      expect(res).toEqual('signature on base 64');
      done();
    });
    service.setNewDataSignature('signature on base 64');
  });
  it('should openSignatureDialog', (done) => {
    const service: DataService = TestBed.get(DataService);
    service.getOpenSignatureDialog().subscribe(res => {
      expect(res).toEqual('signature on base 64');
      done();
    });
    service.setOpenSignatureDialog('signature on base 64');
  });

  it('should newFormulaComponent', (done) => {
    const service: DataService = TestBed.get(DataService);
    service.getNewFormulaComponent().subscribe(res => {
      expect(res).toEqual({modalType: 'formulaComponent'});
      done();
    });
    service.setNewFormulaComponent({modalType: 'formulaComponent'});
  });
  it('should materialComponent', (done) => {
    const service: DataService = TestBed.get(DataService);
    service.getNewMaterialComponent().subscribe(res => {
      expect(res).toEqual('materialComponentData');
      done();
    });
    service.setNewMaterialComponent('materialComponentData');
  });
  it('should searchOrdersModal', (done) => {
    const service: DataService = TestBed.get(DataService);
    const filterOrdersDataConfig = { dateType: ConstOrders.defaultDateInit,
      dateFull: '20/12/2020-01/12/2021', isFromIncidents: true, clientName: 'Client name',
      status: ConstStatus.finalizado, docNum: 12345} as ParamsPedidos;
    service.getSearchOrdersModal().subscribe(res => {
      expect(res.filterOrdersData).toEqual(filterOrdersDataConfig);
      done();
    });
    service.setSearchOrdersModal({modalType: 'search',
                                 filterOrdersData: filterOrdersDataConfig, data: [123, 1234, 1345] });
  });
  it('should searchComponentModal', (done) => {
    const service: DataService = TestBed.get(DataService);
    const filterOrdersDataConfig = { dateType: ConstOrders.defaultDateInit,
      dateFull: '20/12/2020-01/12/2021', isFromIncidents: true, clientName: 'Client name',
      status: ConstStatus.finalizado, docNum: 12345} as ParamsPedidos;
    service.getSearchComponentModal().subscribe(res => {
      expect(res.filterOrdersData).toEqual(filterOrdersDataConfig);
      done();
    });
    service.setSearchComponentModal({modalType: 'search',
      filterOrdersData: filterOrdersDataConfig, data: [123, 1234, 1345] });
  });
  it('should is loading false', (done) => {
    const service: DataService = TestBed.get(DataService);
    service.getIsLoading().subscribe(res => {
      expect(res).toBeFalsy();
      done();
    });
    service.setIsLoading(false);
  });

  it('should set message', (done) => {
    const service: DataService = TestBed.get(DataService);
    service.getGeneralNotificationMessage().subscribe(res => {
      expect(res).toBe('test');
      done();
    });
    service.setGeneralNotificationMessage('test');
  });

  it('should set token', () => {
    const service: DataService = TestBed.get(DataService);
    service.setToken('token');
    expect(service.getToken()).toBe('token');
  });
  it('should get userIsAuthenticated', () => {
    const service: DataService = TestBed.get(DataService);
    service.clearSession();
    expect(service.userIsAuthenticated()).toBeFalsy();
    service.setToken('token');
    expect(service.userIsAuthenticated()).toBeTruthy();
  });
  it('should get userName', () => {
    const service: DataService = TestBed.get(DataService);
    service.setUserName('xxxx');
    expect(service.getUserName()).toEqual('xxxx');
  });


  it('should get userIds', () => {
    const service: DataService = TestBed.get(DataService);
    service.setUserId('asdkjf-lakds');
    expect(service.getUserId()).toEqual('asdkjf-lakds');
  });
  it('should clear token', () => {
    const service: DataService = TestBed.get(DataService);
    service.setToken('token');
    service.clearSession();
    expect(service.getToken()).toBeFalsy();
  });
  it('should getQfbToPlace', () => {
    const service: DataService = TestBed.get(DataService);
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
    const service: DataService = TestBed.get(DataService);
    service.getIsLogin().subscribe(isLogin => {
      expect(isLogin).toBeTruthy();
    });
    service.setIsLogin(true);
  });
  it('should getIsLogin false', () => {
    const service: DataService = TestBed.get(DataService);
    service.getIsLogin().subscribe(isLogin => {
      expect(isLogin).toBeFalsy();
    });
    service.setIsLogin(false);
  });
  it('should getCallHttpService', () => {
    const service: DataService = TestBed.get(DataService);
    service.getCallHttpService().subscribe(callHttpService => {
      expect(callHttpService).toEqual(HttpServiceTOCall.ORDERS);
    });
    service.setCallHttpService(HttpServiceTOCall.ORDERS);
  });
  it('should getMessageGeneralCalHttp', () => {
    const service: DataService = TestBed.get(DataService);
    service.getMessageGeneralCalHttp().subscribe(messageGeneral => {
      expect(messageGeneral).toEqual({title: 'title', icon: 'success', isButtonAccept: true});
    });
    service.setMessageGeneralCallHttp({title: 'title', icon: 'success', isButtonAccept: true});
  });
  it('should getUrlActive', () => {
    const service: DataService = TestBed.get(DataService);
    service.getUrlActive().subscribe(urlActive => {
      expect(urlActive).toEqual(HttpServiceTOCall.ORDERS);
    });
    service.setUrlActive(HttpServiceTOCall.ORDERS);
  });
  it('should getIsToSaveAnything', () => {
    const service: DataService = TestBed.get(DataService);
    service.setIsToSaveAnything(false);
    expect(service.getIsToSaveAnything()).toBeFalsy();
    service.setIsToSaveAnything(true);
    expect(service.getIsToSaveAnything()).toBeTruthy();
  });
  it('should getCancelOrder', () => {
    const service: DataService = TestBed.get(DataService);
    service.getCancelOrder().subscribe(resultCancel => {
      expect(resultCancel).toEqual({list: [{orderId: 55, reason: 'Hubo un error', userId: 'user-id-22'}], cancelType: 'Pedido'});
    });
    service.setCancelOrders({list: [{orderId: 55, reason: 'Hubo un error', userId: 'user-id-22'}], cancelType: 'Pedido'});
  });
  it('should getMessageTitle', () => {
    const service: DataService = TestBed.get(DataService);
    expect(service.getMessageTitle(['1234'], MessageType.processOrder)).toEqual(' 1234 \n');
    expect(service.getMessageTitle(['1234'], MessageType.processDetailOrder))
        .toEqual(' 1234 \n');
    expect(service.getMessageTitle(['1234'], MessageType.placeOrder))
        .toEqual('La orden de fabricación  1234 no pudo ser Asignada \n');
    expect(service.getMessageTitle([{reason: 'Hubo un error'}], MessageType.cancelOrder, true))
        .toEqual('Hubo un error \n');
    expect(service.getMessageTitle(['1234'], MessageType.saveBatches))
        .toEqual('Error al asignar lotes a  1234 , por favor verificar \n');
  });
  it('should getRefreshToken', () => {
    const service: DataService = TestBed.get(DataService);
    service.setRefreshToken('anyRefreshToken');
    expect(service.getRefreshToken()).toEqual('anyRefreshToken');
  });
  it('should getRememberSession', () => {
    const service: DataService = TestBed.get(DataService);
    service.setRememberSession('anyRememberSession');
    expect(service.getRememberSession()).toEqual('anyRememberSession');
  });
  // it('should getProductNoLabel', () => {
  //   const service: DataService = TestBed.get(DataService);
  //   service.setProductNoLabel(catalogs);
  //   expect(service.getProductNoLabel());
  // });
  it('should getIsLogout true', () => {
    const service: DataService = TestBed.get(DataService);
    service.getIsLogout().subscribe(isLogout => {
      expect(isLogout).toBeTruthy();
    });
    service.setIsLogout(true);
  });
  it('should getIsLogout false', () => {
    const service: DataService = TestBed.get(DataService);
    service.getIsLogout().subscribe(isLogout => {
      expect(isLogout).toBeFalsy();
    });
    service.setIsLogout(false);
  });
  it('should getPathUrl', () => {
    const service: DataService = TestBed.get(DataService);
    service.getPathUrl().subscribe(pathUrl => {
      expect(pathUrl).toEqual(['orders']);
    });
    service.setPathUrl(['orders']);
  });
  it('should getFinalizeOrders', () => {
    const service: DataService = TestBed.get(DataService);
    service.getFinalizeOrders().subscribe(finalizeOrders => {
      expect(finalizeOrders).toEqual({list: [{orderId: 123}], cancelType: 'pedidos'});
    });
    service.setFinalizeOrders({list: [{orderId: 123}], cancelType: 'pedidos'});
  });
  it('should getDateFormatted', () => {
    const service: DataService = TestBed.get(DataService);
    expect(service.getDateFormatted(new Date(), new Date(), true).includes('/')).toBeTruthy();
    expect(service.getDateFormatted(new Date(), new Date(), false).includes('/')).toBeTruthy();
  });
  it('should getNewSearchOrdersModal', () => {
    const service: DataService = TestBed.get(DataService);
    service.getNewSearchOrdersModal().subscribe(newSearchOrdersModal => {
      expect(newSearchOrdersModal).toEqual({dateType: 'Pedido', docNum: 1234, fini: new Date('01/12/2020'),
        ffin: new Date('01/12/2020'), status: 'Finalizado', finlabel: '1'});
    });
    service.setNewSearchOrderModal({dateType: 'Pedido', docNum: 1234, fini: new Date('01/12/2020'), ffin: new Date('01/12/2020'),
      status: 'Finalizado', finlabel: '1'});
  });
  it('should getIsThereOnData', () => {
    const service: DataService = TestBed.get(DataService);
    expect(service.getIsThereOnData([], ConstStatus.cancelado, FromToFilter.fromOrdersIsolatedCancel)).toBeFalsy();
  });
  it('should getItemOnDateWithFilter', () => {
    const service: DataService = TestBed.get(DataService);
    expect(service.getItemOnDateWithFilter([], FromToFilter.fromOrderIsolatedReassignItems, ConstStatus.cancelado).length).toEqual(0);
    expect(service.getItemOnDateWithFilter([], FromToFilter.fromOrdersReassign, ConstStatus.cancelado).length).toEqual(0);
    expect(service.getItemOnDateWithFilter([], FromToFilter.fromDefault, ConstStatus.cancelado).length).toEqual(0);

  });
  it('should transformDate', () => {
    const service: DataService = TestBed.get(DataService);
    expect(service.transformDate(new Date(), true).includes('-')).toBeTruthy();
  });
  it('should getDateArray', () => {
    const service: DataService = TestBed.get(DataService);
    expect(service.getDateArray(new Date()).length > 0).toBeTruthy();
  });
  it('should getMaxMinDate', () => {
    const service: DataService = TestBed.get(DataService);
    expect(service.getMaxMinDate(new Date(), 3, false).getMonth()
            !== new Date().getMonth()).toBeTruthy();
    expect(service.getMaxMinDate(new Date(), 3, true).getMonth()
        !== new Date().getMonth()).toBeTruthy();

  });
  it('should userRole', () => {
    const service: DataService = TestBed.get(DataService);
    service.setUserRole(3);
    expect(service.getUserRole).toBeTruthy('3');
  });

});

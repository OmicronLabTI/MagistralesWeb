import {TestBed} from '@angular/core/testing';
import {DatePipe} from '@angular/common';
import {DataService} from './data.service';
import {HttpServiceTOCall, MessageType} from '../constants/const';

describe('DataService', () => {
  beforeEach(() => TestBed.configureTestingModule({
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
    service.clearToken();
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
    service.clearToken();
    expect(service.getToken()).toBeFalsy();
  });
  it('should getQfbToPlace', () => {
    const service: DataService = TestBed.get(DataService);
    service.getQfbToPlace().subscribe(resultQfbToPlace => {
      expect(resultQfbToPlace.userId).toEqual('userId');
      expect(resultQfbToPlace.userName).toEqual('name');
      expect(resultQfbToPlace.list).toEqual([1, 20]);
      expect(resultQfbToPlace.assignType).toEqual('manual');
      expect(resultQfbToPlace.countTotal).toEqual(2);
      expect(resultQfbToPlace.modalType).toEqual('Pedido');
    });
    service.setQbfToPlace({userId: 'userId', list: [1, 20], userName: 'name',
      assignType: 'manual', countTotal: 2, modalType: 'Pedido'});
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
  it('should getDetailOrderDescription', () => {
    const service: DataService = TestBed.get(DataService);
    service.setDetailOrderDescription('anyDescription');
    expect(service.getDetailOrderDescription()).toEqual('anyDescription');
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
    expect(service.getMessageTitle(['1234'], MessageType.processOrder)).toEqual('El producto  1234 no pudo ser Planificado \n');
    expect(service.getMessageTitle(['1234'], MessageType.processDetailOrder))
        .toEqual('La orden de fabricación  1234 no pudo ser Planificado \n');
    expect(service.getMessageTitle(['1234'], MessageType.placeOrder))
        .toEqual('La orden de fabricación  1234 no pudo ser Asignada \n');
    expect(service.getMessageTitle([{reason: 'Hubo un error'}], MessageType.cancelOrder, true))
        .toEqual('Hubo un error \n');
  });
});

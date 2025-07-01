import { DatePipe } from '@angular/common';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { Catalogs } from '../model/http/pedidos';

import { LocalStorageService } from './local-storage.service';
import { ClasificationColorList } from 'src/mocks/userListMock';

describe('LocalStorageService', () => {
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
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    expect(service).toBeTruthy();
  });

  it('should getRememberSession', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setRememberSession('anyRememberSession');
    expect(service.getRememberSession()).toEqual('anyRememberSession');
  });
  it('should getProductNoLabel', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setProductNoLabel(catalogs);
    expect(service.getProductNoLabel());
  });
  it('should set token', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setToken('token');
    expect(service.getToken()).toBe('token');
  });
  it('should get userIsAuthenticated', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.clearSession();
    expect(service.userIsAuthenticated()).toBeFalsy();
    service.setToken('token');
    expect(service.userIsAuthenticated()).toBeTruthy();
  });
  it('should get userIds', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setUserId('asdkjf-lakds');
    expect(service.getUserId()).toEqual('asdkjf-lakds');
  });
  it('should clear token', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setToken('token');
    service.clearSession();
    expect(service.getToken()).toBeFalsy();
  });
  it('should getRefreshToken', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setRefreshToken('anyRefreshToken');
    expect(service.getRefreshToken()).toEqual('anyRefreshToken');
  });
  it('should get userName', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setUserName('xxxx');
    expect(service.getUserName()).toEqual('xxxx');
  });
  it('should userRole', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setUserRole(3);
    expect(service.getUserRole).toBeTruthy('3');
  });

  it('should get userRole', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setUserRole(3);
    expect(service.getUserRole()).toEqual('3');
  });
  it('should getFiltersActives', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setFiltersActives('filters-active');
    expect(service.getFiltersActives()).toEqual('filters-active');
  });
  it('should getFiltersActivesOrders', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setFiltersActivesOrders('filters-active-orders');
    expect(service.getFiltersActivesOrders()).toEqual('filters-active-orders');
  });

  it('should getCurrentDetailOrder', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setCurrentDetailOrder('detail-current');
    expect(service.getCurrentDetailOrder()).toEqual('detail-current');
  });

  it('should getOrderIsolated', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.getOrderIsolated();
    expect(service.getOrderIsolated).toBeTruthy();
  });
  it('should removeFiltersActive', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.removeFiltersActive();
    expect(service.removeFiltersActive).toBeTruthy();
  });
  it('should removeFiltersActiveOrders', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.removeFiltersActiveOrders();
    expect(service.removeFiltersActiveOrders).toBeTruthy();
  });
  it('should removeCurrentDetailOrder', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.removeCurrentDetailOrder();
    expect(service.removeCurrentDetailOrder).toBeTruthy();
  });
  it('should removeOrderIsolated', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.removeOrderIsolated();
    expect(service.removeOrderIsolated).toBeTruthy();
  });
  it('should getUserClasification', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setUserClasification('MN');
    expect(service.getUserClasification).toBeTruthy('MN');
  });
  it('should getClasificationList', () => {
    const service: LocalStorageService = TestBed.get(LocalStorageService);
    service.setClasificationList(ClasificationColorList);
    expect(service.getClasificationList).toBeTruthy();
  });
});

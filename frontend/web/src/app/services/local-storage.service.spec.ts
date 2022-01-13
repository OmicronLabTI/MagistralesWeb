import { DatePipe } from '@angular/common';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { Catalogs } from '../model/http/pedidos';

import { LocalStorageService } from './local-storage.service';

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
});

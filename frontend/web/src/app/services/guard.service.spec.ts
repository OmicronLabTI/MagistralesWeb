import { TestBed } from '@angular/core/testing';

import { GuardService } from './guard.service';
import { RouterTestingModule } from '@angular/router/testing';
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlSegment} from '@angular/router';
import {DatePipe} from '@angular/common';
import { LocalStorageService } from './local-storage.service';

describe('GuardService', () => {
  let routerSpy: jasmine.SpyObj<Router>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  beforeEach(() => {
    routerSpy = jasmine.createSpyObj<Router>('Router', [
      'navigate'
    ]);
    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'userIsAuthenticated',
      'getUserRole'
    ]);

    localStorageServiceSpy.userIsAuthenticated.and.returnValue(true);
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      providers: [
        DatePipe,
        { provide: Router, useValue: routerSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy}
      ]
    });
  });

  it('should be created', () => {
    const service: GuardService = TestBed.get(GuardService);
    expect(service).toBeTruthy();
  });

  it('should can activate true', () => {
      localStorageServiceSpy.getUserRole.and.returnValue('1');
      const service: GuardService = TestBed.get(GuardService);
      const route = {url: [{
        path: 'userList'
      }]} as ActivatedRouteSnapshot;
      const state = {} as RouterStateSnapshot;
      expect(service.canActivate(route, state)).toBeTruthy();
      expect(routerSpy.navigate).not.toHaveBeenCalledWith(['/login']);
  });

  it('should can activate false', () => {
    localStorageServiceSpy.getUserRole.and.returnValue('1');
    const service: GuardService = TestBed.get(GuardService);
    localStorageServiceSpy.userIsAuthenticated.and.returnValue(false);
    const route = {url: [{
      path: 'pedidos'
    }]} as ActivatedRouteSnapshot;
    const state = {} as RouterStateSnapshot;
    expect(service.canActivate(route, state)).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['login']);
  });

  it('should can activate true', () => {
    localStorageServiceSpy.getUserRole.and.returnValue('3');
    const service: GuardService = TestBed.get(GuardService);
    const route = {url: [{
      path: 'pedidos'
    }]} as ActivatedRouteSnapshot;
    const state = {} as RouterStateSnapshot;
    expect(service.canActivate(route, state)).toBeTruthy();
    expect(routerSpy.navigate).not.toHaveBeenCalledWith(['/login']);
});

  it('should can activate false', () => {
    localStorageServiceSpy.getUserRole.and.returnValue('3');
    const service: GuardService = TestBed.get(GuardService);
    localStorageServiceSpy.userIsAuthenticated.and.returnValue(false);
    const route = {url: [{
      path: 'userList'
    }]} as ActivatedRouteSnapshot;
    const state = {} as RouterStateSnapshot;
    expect(service.canActivate(route, state)).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['login']);
  });
});

import { TestBed } from '@angular/core/testing';

import { GuardService } from './guard.service';
import { DataService } from './data.service';
import { RouterTestingModule } from '@angular/router/testing';
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlSegment} from '@angular/router';
import {DatePipe} from '@angular/common';

describe('GuardService', () => {
  let routerSpy: jasmine.SpyObj<Router>;
  let dataSpyService: jasmine.SpyObj<DataService>;
  beforeEach(() => {
    routerSpy = jasmine.createSpyObj<Router>('Router', [
      'navigate'
    ]);
    dataSpyService = jasmine.createSpyObj<DataService>('DataService', [
      'userIsAuthenticated', 'getUserRole'
    ]);
    dataSpyService.userIsAuthenticated.and.returnValue(true);
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      providers: [
        DataService,
        DatePipe,
        { provide: Router, useValue: routerSpy },
        { provide: DataService, useValue: dataSpyService }
      ]
    });
  });

  it('should be created', () => {
    const service: GuardService = TestBed.get(GuardService);
    expect(service).toBeTruthy();
  });

  it('should can activate true', () => {
      dataSpyService.getUserRole.and.returnValue('1');
      const service: GuardService = TestBed.get(GuardService);
      const route = {url: [{
        path: 'userList'
      }]} as ActivatedRouteSnapshot;
      const state = {} as RouterStateSnapshot;
      expect(service.canActivate(route, state)).toBeTruthy();
      expect(routerSpy.navigate).not.toHaveBeenCalledWith(['/login']);
  });

  it('should can activate false', () => {
    dataSpyService.getUserRole.and.returnValue('1');
    const service: GuardService = TestBed.get(GuardService);
    dataSpyService.userIsAuthenticated.and.returnValue(false);
    const route = {url: [{
      path: 'pedidos'
    }]} as ActivatedRouteSnapshot;
    const state = {} as RouterStateSnapshot;
    expect(service.canActivate(route, state)).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['login']);
  });

  it('should can activate true', () => {
    dataSpyService.getUserRole.and.returnValue('3');
    const service: GuardService = TestBed.get(GuardService);
    const route = {url: [{
      path: 'pedidos'
    }]} as ActivatedRouteSnapshot;
    const state = {} as RouterStateSnapshot;
    expect(service.canActivate(route, state)).toBeTruthy();
    expect(routerSpy.navigate).not.toHaveBeenCalledWith(['/login']);
});

  it('should can activate false', () => {
    dataSpyService.getUserRole.and.returnValue('3');
    const service: GuardService = TestBed.get(GuardService);
    dataSpyService.userIsAuthenticated.and.returnValue(false);
    const route = {url: [{
      path: 'userList'
    }]} as ActivatedRouteSnapshot;
    const state = {} as RouterStateSnapshot;
    expect(service.canActivate(route, state)).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['login']);
  });
});

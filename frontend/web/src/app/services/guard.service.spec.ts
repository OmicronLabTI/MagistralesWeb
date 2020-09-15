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
      'userIsAuthenticated', 'getUserRole', 'setUserRole'
    ]);
    dataSpyService.userIsAuthenticated.and.returnValue(true);
    dataSpyService.getUserRole.and.returnValue('1');
    dataSpyService.setUserRole('1');
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

  /*it('should can activate true', () => {
      const service: GuardService = TestBed.get(GuardService);
      const route = {} as ActivatedRouteSnapshot;
      const state = {} as RouterStateSnapshot;
      route.url.push({
        path: 'pedidos',
        parameters: {
          [
          ]
        },
        parameterMap: 
      });
      expect(service.canActivate(route, state)).toBeTruthy();
      expect(routerSpy.navigate).not.toHaveBeenCalledWith(['/login']);
  });*/

  /*it('should can activate false', () => {
    const service: GuardService = TestBed.get(GuardService);
    dataSpyService.userIsAuthenticated.and.returnValue(false);
    const route = {} as ActivatedRouteSnapshot;
    const state = {} as RouterStateSnapshot;
    route.url[0].path = 'pedidos';
    expect(service.canActivate(route, state)).toBe(false);
    expect(routerSpy.navigate).toHaveBeenCalledWith(['login']);
  });*/
});

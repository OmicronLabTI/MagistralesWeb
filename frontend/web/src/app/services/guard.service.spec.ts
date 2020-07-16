import { TestBed, inject } from '@angular/core/testing';

import { GuardService } from './guard.service';
import { DataService } from './data.service';
import { RouterTestingModule } from '@angular/router/testing';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

describe('GuardService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      RouterTestingModule
    ],
    providers: [
      DataService
    ]
  }));

  it('should be created', () => {
    const service: GuardService = TestBed.get(GuardService);
    expect(service).toBeTruthy();
  });

  it('should can activate true', () => {
    inject([DataService], (dataService) => {
      const service: GuardService = TestBed.get(GuardService);
      const route = {} as ActivatedRouteSnapshot;
      const state = {} as RouterStateSnapshot;
      dataService.setToken('token');
      expect(service.canActivate(route, state)).toBeTruthy();
    });
  });

  it('should can activate false', () => {
    const service: GuardService = TestBed.get(GuardService);
    inject([DataService], (dataService) => {
      const route = {} as ActivatedRouteSnapshot;
      const state = {} as RouterStateSnapshot;
      dataService.setToken(undefined);
      expect(service.canActivate(route, state)).toBeFalsy();
    });
  });
});

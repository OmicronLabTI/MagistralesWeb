import { TestBed } from '@angular/core/testing';

import { ErrorService } from './error.service';
import { DatePipe } from '@angular/common';
import { ErrorHttpInterface } from '../model/http/commons';
import { ObservableService } from './observable.service';
import { LocalStorageService } from './local-storage.service';
import { RouterTestingModule } from '@angular/router/testing';
describe('ErrorService', () => {
  // let dataServiceSpy;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  beforeEach(() => {
    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'setToken',
      'userIsAuthenticated',
      'setUserName',
    ]);
    observableServiceSpy = jasmine.createSpyObj<ObservableService>
      ('ObservableService',
        [
          'setGeneralNotificationMessage',
          'setIsLogin',
          'setIsLogout',
          'setMessageGeneralCallHttp',
        ]
      );
    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        DatePipe,
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy}
      ]
    });
  });

  it('should be created', () => {
    const service: ErrorService = TestBed.get(ErrorService);
    expect(service).toBeTruthy();
  });
  it('should httpError', () => {
    const service: ErrorService = TestBed.get(ErrorService);
    const errorHttpTest = new ErrorHttpInterface();
    errorHttpTest.status = 401;
    service.httpError(errorHttpTest);
    expect(observableServiceSpy.setIsLogout).toHaveBeenCalledWith(true);
    errorHttpTest.status = 500;
    service.httpError(errorHttpTest);
    expect(observableServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
    errorHttpTest.status = 504;
    service.httpError(errorHttpTest);
    expect(observableServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
    errorHttpTest.status = 0;
    service.httpError(errorHttpTest);
    expect(observableServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
  });
});

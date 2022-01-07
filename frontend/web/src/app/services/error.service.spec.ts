import { TestBed } from '@angular/core/testing';

import { ErrorService } from './error.service';
import { DatePipe } from '@angular/common';
import { ErrorHttpInterface } from '../model/http/commons';
import { DataService } from './data.service';
import { ObservableService } from './observable.service';
import { LocalStorageService } from './local-storage.service';
describe('ErrorService', () => {
  let dataServiceSpy;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  beforeEach(() => {
    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'setToken',
      'userIsAuthenticated'
    ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>
      ('DataService',
        [
          'setUserName',
        ]
      );
    //  --- Observable Service
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
      providers: [
        DatePipe,
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
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

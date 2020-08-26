import { TestBed } from '@angular/core/testing';

import { ErrorService } from './error.service';
import {DatePipe} from '@angular/common';
import {ErrorHttpInterface} from '../model/http/commons';
import {DataService} from './data.service';
describe('ErrorService', () => {
  let dataServiceSpy;
  beforeEach(() => {
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'setToken', 'setIsLogin', 'setUserName', 'userIsAuthenticated', 'setGeneralNotificationMessage',
        'setIsLogout', 'setMessageGeneralCallHttp'
    ]);
    TestBed.configureTestingModule({
      providers: [DatePipe,
        { provide: DataService, useValue: dataServiceSpy }]
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
    expect(dataServiceSpy.setIsLogout).toHaveBeenCalledWith(true);
    errorHttpTest.status = 500;
    service.httpError(errorHttpTest);
    expect(dataServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
  });
});

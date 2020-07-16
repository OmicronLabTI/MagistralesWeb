import { TestBed } from '@angular/core/testing';

import { DataService } from './data.service';

describe('DataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

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
});

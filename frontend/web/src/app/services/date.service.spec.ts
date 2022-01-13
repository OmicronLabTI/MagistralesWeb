import { DatePipe } from '@angular/common';
import { TestBed } from '@angular/core/testing';

import { DateService } from './date.service';

describe('DateService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: DateService = TestBed.get(DateService);
    expect(service).toBeTruthy();
  });
  it('should transformDate', () => {
    const service: DateService = TestBed.get(DateService);
    expect(service.transformDate(new Date(), true).includes('-')).toBeTruthy();
  });
  it('should getDateArray', () => {
    const service: DateService = TestBed.get(DateService);
    expect(service.getDateArray(new Date()).length > 0).toBeTruthy();
  });
  it('should getMaxMinDate', () => {
    const service: DateService = TestBed.get(DateService);
    expect(service.getMaxMinDate(new Date(), 3, false).getMonth()
            !== new Date().getMonth()).toBeTruthy();
    expect(service.getMaxMinDate(new Date(), 3, true).getMonth()
        !== new Date().getMonth()).toBeTruthy();

  });
  it('should getDateFormatted', () => {
    const service: DateService = TestBed.get(DateService);
    expect(service.getDateFormatted(new Date(), new Date(), true).includes('/')).toBeTruthy();
    expect(service.getDateFormatted(new Date(), new Date(), false).includes('/')).toBeTruthy();
  });
});

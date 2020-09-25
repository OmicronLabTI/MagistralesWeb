import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ConsumeService } from './consume.service';
import { DatePipe } from '@angular/common';

import { ReportingService } from './reporting.service';

describe('ReportingService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [DatePipe, ConsumeService]
  }));

  it('should be created', () => {
    const service: ReportingService = TestBed.get(ReportingService);
    expect(service).toBeTruthy();
  });
});

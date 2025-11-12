import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { autoBillingMock } from 'src/mocks/autoBillingMock';
import { AutoBillingService } from './autoBilling.service';
import { ConsumeService } from './consume.service';

describe('AutoBillingService', () => {
  let service: AutoBillingService;
  let consumeServiceMock: jasmine.SpyObj<ConsumeService>;

  beforeEach(() => {
    consumeServiceMock = jasmine.createSpyObj<ConsumeService>('ConsumeService', ['httpGet']);
    consumeServiceMock.httpGet.and.returnValue(of({ response: autoBillingMock }));

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
      providers: [
        DatePipe,
        { provide: ConsumeService, useValue: consumeServiceMock }
      ]
    });

    service = TestBed.get(AutoBillingService);
  });

  /* ==========================================================
     ✅ CORE CREATION & TYPE CHECKS
  ========================================================== */
  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call getAllAutoBilling and trigger httpGet', () => {
    service.getAllAutoBilling();
    expect(consumeServiceMock.httpGet).toHaveBeenCalled();
  });
});

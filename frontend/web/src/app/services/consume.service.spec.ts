import { inject, TestBed } from '@angular/core/testing';

import { ConsumeService } from './consume.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Observable } from 'rxjs';
import { DatePipe } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';

fdescribe('ConsumeService', () => {
  const ENDPOINT = 'http://google.com';
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientTestingModule, RouterTestingModule
    ],
    providers: [
      DatePipe
    ]
  }));

  it('should be created', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    expect(service).toBeTruthy();
  });

  it('should create get observable', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpGet<any>(ENDPOINT);
    expect(obs instanceof Observable).toBeTruthy();
  });

  it('should create post observable', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPost<any>(ENDPOINT, {});
    expect(obs instanceof Observable).toBeTruthy();
  });

  it('should create put observable', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPut<any>(ENDPOINT, {});
    expect(obs instanceof Observable).toBeTruthy();
  });
  it('should create patch observable', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPatch<any>(ENDPOINT, {});
    expect(obs instanceof Observable).toBeTruthy();
  });
  it('should create get observable', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpGet<any>(ENDPOINT, { param: 1 }, { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(res => {
      expect(res).toBeDefined();
    });

    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('GET');

    req.flush({});
  }));
});

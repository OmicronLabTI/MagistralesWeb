import { fakeAsync, inject, TestBed, tick } from '@angular/core/testing';

import { ConsumeService } from './consume.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Observable } from 'rxjs';
import { DatePipe } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';
import { environment } from 'src/environments/environment';

describe('ConsumeService', () => {
  const ENDPOINT = environment.baseUrl;
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

  it('should create get observable', inject([HttpTestingController], fakeAsync((httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpGet<any>(ENDPOINT, { params: 1 }, { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(res => {
      expect(res).toBeDefined();
    });
    const req = httpMock.expectOne({method: 'GET'});
    req.flush({});
  })));

  it('should create get observable with error', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpGet<any>(ENDPOINT, { param: 1 }, { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(() => {}, error => {
      expect(error).toBeDefined();
    });

    const req = httpMock.expectOne({method: 'GET'});
    req.error(new ErrorEvent('err'));
  }));

  it('should create post observable', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPost<any>(ENDPOINT, {}, { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(res => {
      expect(res).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('POST');
    req.flush({});
  }));
  it('should create post observable with error', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPost<any>(ENDPOINT, {}, { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(() => {}, error => {
      expect(error).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('POST');
    req.error(new ErrorEvent('err'));
  }));

  it('should create put observable', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPut<any>(ENDPOINT, { param: 1 }, { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(res => {
      expect(res).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('PUT');
    req.flush({});
  }));
  it('should create put observable with error', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPut<any>(ENDPOINT, { param: 1 }, { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(() => {}, error => {
      expect(error).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('PUT');
    req.error(new ErrorEvent('err'));
  }));
  it('should create patch observable', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPatch<any>(ENDPOINT, {},  { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(res => {
      expect(res).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('PATCH');
    req.flush({});
  }));
  it('should create patch observable with error', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPatch<any>(ENDPOINT, {},  { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(() => {}, error => {
      expect(error).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('PATCH');
    req.error(new ErrorEvent('err'));
  }));
  it('should create httpDownloadFilePost observable', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpDownloadFilePost(ENDPOINT, {},  { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(res => {
      expect(res).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('POST');
    req.flush(new Blob());
  }));
  it('should create httpDownloadFilePost observable with error', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpDownloadFilePost(ENDPOINT, {},  { header: 1 });
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(() => {}, error => {
      expect(error).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('POST');
    req.error(new ErrorEvent('err'));
  }));
  it('should create httpDelete observable', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpDelete(ENDPOINT);
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(res => {
      expect(res).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('DELETE');
    req.flush({});
  }));
  it('should create httpDelete observable', inject([HttpTestingController], (httpMock: HttpTestingController) => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpDelete(ENDPOINT);
    expect(obs instanceof Observable).toBeTruthy();
    obs.subscribe(() => {}, error => {
      expect(error).toBeDefined();
    });
    const req = httpMock.expectOne(ENDPOINT);
    expect(req.request.method).toEqual('DELETE');
    req.error(new ErrorEvent('err'));
  }));
});

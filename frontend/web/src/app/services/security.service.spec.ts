import { TestBed, inject } from '@angular/core/testing';

import { SecurityService } from './security.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { DataService } from './data.service';
import { ConsumeService } from './consume.service';
import { ILoginReq } from '../model/http/security.model';
import { Observable } from 'rxjs';
import { Endpoints } from 'src/environments/endpoints';
import {DatePipe} from '@angular/common';
import {RouterTestingModule} from '@angular/router/testing';
describe('SecurityService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientTestingModule, RouterTestingModule
    ],
    providers: [
      DataService,
      ConsumeService,
        DatePipe
    ]
  }));

  it('should be created', () => {
    const service: SecurityService = TestBed.get(SecurityService);
    expect(service).toBeTruthy();
  });

  it('should create login observable', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpGet<ILoginReq>(Endpoints.security.login);
    expect(obs  ).toBeTruthy();
  });

  it('should login',
    inject([HttpTestingController, SecurityService], (httpMock: HttpTestingController, service: SecurityService) => {
      const data = {
        user: 'admin',
        password: '12345'
      } as ILoginReq;
      service.login(data).subscribe(response => {
        expect(response).toBeDefined();
        // tslint:disable-next-line: max-line-length
        expect(response.access_token).toBe('eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY3MzM1NTgsInVzZXIiOiJzZXJnaW8ifQ.W9kstVRF9qm_s2diVt-Ki0xb4FwkXIA0QtEFSDAlXCM');
        // tslint:disable-next-line: max-line-length
        expect(response.refresh_token).toBe('eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjbGllbnRJZCI6IiIsInByb2ZpbGUiOiJhZG1pbiIsImV4cCI6MTU5NjgyNjU2MiwidXNlciI6Imd1eiJ9.2O4TsKp1uGqBRJ5dobk7xZHsSe5TvXVhxRTPu0oviYY');
        expect(response.token_type).toBe('Bearer');
        expect(response.expires_in).toEqual(3600);
        expect(response.scope).toBe('');
      });

      const req = httpMock.expectOne(Endpoints.security.login);
      expect(req.request.method).toEqual('POST');

      const res: any = require('../../mocks/login.json');
      req.flush(res);
    })
  );
  it('should refreshToken', () => {
      const service: SecurityService = TestBed.get(SecurityService);
      expect(service.refreshToken() instanceof Observable).toBeTruthy();
    });
  it('should getUser', () => {
        const service: SecurityService = TestBed.get(SecurityService);
        expect(service.getUser('anyUsername') instanceof Observable).toBeTruthy();
    });
});

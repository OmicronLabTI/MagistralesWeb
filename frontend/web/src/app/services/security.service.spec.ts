import { TestBed } from '@angular/core/testing';

import { SecurityService } from './security.service';
import { HttpClientModule } from '@angular/common/http';
import { DataService } from './data.service';
import { ConsumeService } from './consume.service';
import { ILoginReq } from '../model/http/security.model';
import { Observable } from 'rxjs';

describe('SecurityService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientModule
    ],
    providers: [
      DataService,
      ConsumeService
    ]
  }));

  it('should be created', () => {
    const service: SecurityService = TestBed.get(SecurityService);
    expect(service).toBeTruthy();
  });

  it('should create login observable', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpGet<ILoginReq>('http://google.com');
    expect(obs instanceof Observable).toBeTruthy();
  });
});

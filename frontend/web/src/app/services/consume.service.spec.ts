import { TestBed } from '@angular/core/testing';

import { ConsumeService } from './consume.service';
import { HttpClientModule } from '@angular/common/http';
import { DataService } from './data.service';
import { ILoginRes } from '../model/http/security.model';
import { Observable } from 'rxjs';

describe('ConsumeService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientModule
    ],
    providers: [
      DataService
    ]
  }));

  it('should be created', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    expect(service).toBeTruthy();
  });

  it('should create get observable', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpGet<any>('http://google.com');
    expect(obs instanceof Observable).toBeTruthy();
  });

  it('should create post observable', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPost<any>('http://google.com', {});
    expect(obs instanceof Observable).toBeTruthy();
  });

  it('should create put observable', () => {
    const service: ConsumeService = TestBed.get(ConsumeService);
    const obs = service.httpPut<any>('http://google.com', {});
    expect(obs instanceof Observable).toBeTruthy();
  });
});

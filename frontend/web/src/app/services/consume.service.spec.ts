import { TestBed } from '@angular/core/testing';

import { ConsumeService } from './consume.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DataService } from './data.service';
import { Observable } from 'rxjs';
import {DatePipe} from '@angular/common';
describe('ConsumeService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientTestingModule
    ],
    providers: [
      DataService,
        DatePipe
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

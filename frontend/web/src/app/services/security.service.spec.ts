import { TestBed } from '@angular/core/testing';

import { SecurityService } from './security.service';
import { HttpClientModule } from '@angular/common/http';
import { DataService } from './data.service';
import { ConsumeService } from './consume.service';

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
});

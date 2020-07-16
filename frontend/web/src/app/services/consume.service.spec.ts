import { TestBed } from '@angular/core/testing';

import { ConsumeService } from './consume.service';
import { HttpClientModule } from '@angular/common/http';
import { DataService } from './data.service';

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
});

import { TestBed } from '@angular/core/testing';

import { IncidentsService } from './incidents.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {DatePipe} from '@angular/common';

describe('IncidentsService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: IncidentsService = TestBed.get(IncidentsService);
    expect(service).toBeTruthy();
  });
});

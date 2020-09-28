import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ConsumeService } from './consume.service';
import { DatePipe } from '@angular/common';
import { MaterialRequestService } from './material-request.service';

describe('MaterialRequestService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [DatePipe, ConsumeService]
  }));

  it('should be created', () => {
    const service: MaterialRequestService = TestBed.get(MaterialRequestService);
    expect(service).toBeTruthy();
  });
});

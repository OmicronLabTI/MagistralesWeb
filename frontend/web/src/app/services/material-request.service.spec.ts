import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ConsumeService } from './consume.service';
import { DatePipe } from '@angular/common';
import { MaterialRequestService } from './material-request.service';
import {Observable} from 'rxjs';
import {RawRequest} from '../model/http/materialReques';

describe('MaterialRequestService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [DatePipe, ConsumeService]
  }));

  it('should be created', () => {
    const service: MaterialRequestService = TestBed.get(MaterialRequestService);
    expect(service).toBeTruthy();
  });
  it('should getPreMaterialRequest', () => {
    const service: MaterialRequestService = TestBed.get(MaterialRequestService);
    expect(service.getPreMaterialRequest([], false) instanceof Observable).toBeTruthy();
    expect(service.getPreMaterialRequest([], true) instanceof Observable).toBeTruthy();
  });
  it('should postMaterialRequest', () => {
    const service: MaterialRequestService = TestBed.get(MaterialRequestService);
    expect(service.postMaterialRequest({data: {} as RawRequest, userId: 'userId'}) instanceof Observable).toBeTruthy();
  });
});

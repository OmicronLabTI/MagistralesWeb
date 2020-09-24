import { TestBed } from '@angular/core/testing';

import { MaterialRequestService } from './material-request.service';

describe('MaterialRequestService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MaterialRequestService = TestBed.get(MaterialRequestService);
    expect(service).toBeTruthy();
  });
});

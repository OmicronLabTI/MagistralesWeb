import { TestBed } from '@angular/core/testing';
import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Observable } from 'rxjs';

import { ProductivityService } from './productivity.service';

describe('ProductivityService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: ProductivityService = TestBed.get(ProductivityService);
    expect(service).toBeTruthy();
  });

  it('should getProductivity', () => {
    const service: ProductivityService = TestBed.get(ProductivityService);
    expect(service.getProductivity('anyQueryString') instanceof Observable).toBeTruthy();
  });
});

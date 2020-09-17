import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { OrdersService } from './orders.service';
import { DatePipe } from '@angular/common';
import {Observable} from 'rxjs';

describe('OrdersService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: OrdersService = TestBed.get(OrdersService);
    expect(service).toBeTruthy();
  });
  it('should saveMyListComponent', () => {
    const service: OrdersService = TestBed.get(OrdersService);
    expect(service.saveMyListComponent({}) instanceof Observable).toBeTruthy();
  });
});

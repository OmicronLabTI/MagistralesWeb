import { TestBed } from '@angular/core/testing';
import { HttpClientModule } from '@angular/common/http';
import { PedidosService } from './pedidos.service';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('PedidosService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: PedidosService = TestBed.get(PedidosService);
    expect(service).toBeTruthy();
  });
});

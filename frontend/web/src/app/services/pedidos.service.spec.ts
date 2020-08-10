import { TestBed } from '@angular/core/testing';
import { HttpClientModule } from '@angular/common/http';
import { PedidosService } from './pedidos.service';
import {DatePipe} from '@angular/common';

describe('PedidosService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientModule],
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: PedidosService = TestBed.get(PedidosService);
    expect(service).toBeTruthy();
  });
});

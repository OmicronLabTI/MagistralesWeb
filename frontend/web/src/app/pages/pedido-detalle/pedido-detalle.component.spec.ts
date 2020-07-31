import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PedidoDetalleComponent } from './pedido-detalle.component';

describe('PedidoDetalleComponent', () => {
  let component: PedidoDetalleComponent;
  let fixture: ComponentFixture<PedidoDetalleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PedidoDetalleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PedidoDetalleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

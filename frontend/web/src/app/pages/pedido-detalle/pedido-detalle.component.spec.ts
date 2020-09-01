import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PedidoDetalleComponent } from './pedido-detalle.component';
import {RouterTestingModule} from '@angular/router/testing';
import { IPedidoDetalleReq } from 'src/app/model/http/detallepedidos.model';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {PedidosService} from '../../services/pedidos.service';
import {of} from 'rxjs';
import {DetailOrderMock} from '../../../mocks/detailOrder.Mock';

describe('PedidoDetalleComponent', () => {
  let component: PedidoDetalleComponent;
  let fixture: ComponentFixture<PedidoDetalleComponent>;
  let pedidosServiceSpy;
  beforeEach(async(() => {
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getPedidos', 'processOrders', 'getDetallePedido'
    ]);
    pedidosServiceSpy.getDetallePedido.and.callFake(() => {
      return of(DetailOrderMock);
    });
    TestBed.configureTestingModule({
      imports: [
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        RouterTestingModule,
        FormsModule,
        BrowserAnimationsModule],
      declarations: [ PedidoDetalleComponent ],
      providers: [DatePipe,
        { provide: PedidosService, useValue: pedidosServiceSpy }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PedidoDetalleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    component.dataSource.data = null;
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should getDetallePedido', () => {
    component.docNum = '60026';
    component.getDetallePedido();
    expect(pedidosServiceSpy.getDetallePedido).toHaveBeenCalledWith(component.docNum);
    expect(component.isThereOrdersDetailToPlan).toBeFalsy();
    expect(component.isThereOrdersDetailToPlace).toBeFalsy();
    expect(component.isThereOrdersDetailToCancel).toBeFalsy();
    expect(component.isThereOrdersDetailToFinalize).toBeFalsy();
  });

  it('should someComplete return false', () => {
    expect(component.someComplete()).toBeFalsy();
  });
  it('should someComplete return true', () => {
    component.dataSource.data = [
      {
        isChecked: true,
      } as IPedidoDetalleReq
    ];
    expect(component.someComplete()).toBeTruthy();
  });

  it('should someCompletereturn false when allcomplete is true', () => {
    component.dataSource.data = [
      {
        isChecked: false,
      } as IPedidoDetalleReq
    ];
    component.allComplete = true;
    expect(component.someComplete()).toBeFalsy();
  });
  it('should updateAllComplete', () => {
    component.dataSource.data = DetailOrderMock.response;
    component.updateAllComplete();
    expect(component.allComplete).toBeFalsy();
    // component.dataSource.data = DetailOrderMock.response;
    component.dataSource.data.forEach(detail => detail.isChecked = true);
    component.updateAllComplete();
    expect(component.allComplete).toBeTruthy();
  });
  it('should setAll', () => {
    component.dataSource.data.forEach(detail => detail.isChecked = false);
    component.dataSource.data = null;
    component.setAll(false);
    expect(component.allComplete).toBe(false);
    component.dataSource.data = DetailOrderMock.response;
    component.setAll(true);
    expect(component.allComplete).toBe(true);
    expect(component.dataSource.data.every(detail => detail.isChecked)).toBeTruthy();
    component.setAll(false);
    expect(component.allComplete).toBe(false);
    expect(component.dataSource.data.every(detail => !detail.isChecked)).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { PedidosComponent } from './pedidos.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {DatePipe} from '@angular/common';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {PedidosService} from '../../services/pedidos.service';
import {of, throwError} from 'rxjs';
import {PedidosListMock} from '../../../mocks/pedidosListMock';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {ConstStatus} from '../../constants/const';

describe('PedidosComponent', () => {
  let component: PedidosComponent;
  let fixture: ComponentFixture<PedidosComponent>;
  let pedidosServiceSpy;

  beforeEach(async(() => {
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getPedidos'
    ]);
    pedidosServiceSpy.getPedidos.and.callFake(() => {
      return of(PedidosListMock);
    });
    TestBed.configureTestingModule({
      declarations: [ PedidosComponent ],
      imports: [RouterTestingModule, MATERIAL_COMPONENTS, HttpClientTestingModule, BrowserAnimationsModule],
      providers: [
        DatePipe,
        { provide: PedidosService, useValue: pedidosServiceSpy },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PedidosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.displayedColumns)
        .toEqual(['seleccion', 'cons', 'codigo', 'cliente', 'medico', 'asesor', 'f_inicio', 'f_fin', 'status', 'qfb_asignado', 'actions']);
    expect(component.limit).toEqual(10);
    expect(component.offset).toEqual(0);
  });
  it('should call getPedidos ok', () => {
    component.offset = 0;
    component.limit = 10;
    component.queryString = 'rango de fechas';
    component.getFullQueryString();
    expect(component.fullQueryString).toEqual(`${component.queryString}&offset=0&limit=10`);
    component.getPedidos();
    expect(pedidosServiceSpy.getPedidos).toHaveBeenCalledWith(`${component.queryString}&offset=0&limit=10`);
    expect(component.lengthPaginator).toEqual(PedidosListMock.comments);
    expect(component.dataSource.data).toEqual(PedidosListMock.response);
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.abierto)
        .forEach(pedido => expect(pedido.class).toEqual('green'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.planificado)
        .forEach(pedido => expect(pedido.class).toEqual('mat-primary'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.liberado)
        .forEach(pedido => expect(pedido.class).toEqual('liberado'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.cancelado)
        .forEach(pedido => expect(pedido.class).toEqual('cancelado'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.enProceso)
        .forEach(pedido => expect(pedido.class).toEqual('proceso'));
    expect(component.isThereOrdersToPlan).toBeFalsy();
    expect(component.isThereOrdersToPlace).toBeFalsy();
    expect(component.isThereOrdersToCancel).toBeFalsy();
    expect(component.isThereOrdersToFinalize).toBeFalsy();
  });
  it('should should call getPedidos error', () => {
    pedidosServiceSpy.getPedidos.and.callFake(() => {
      return throwError({ status: 500 });
    });
    component.getPedidos();
    expect(component.dataSource.data.length).toEqual(0);
  });
});

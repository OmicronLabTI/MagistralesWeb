import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { PedidosComponent } from './pedidos.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {DatePipe} from '@angular/common';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {PedidosService} from '../../services/pedidos.service';
import {of} from 'rxjs';
import {UserListMock} from '../../../mocks/userListMock';

describe('PedidosComponent', () => {
  let component: PedidosComponent;
  let fixture: ComponentFixture<PedidosComponent>;
  let pedidosServiceSpy;

  beforeEach(async(() => {
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getPedidos'
    ]);
    pedidosServiceSpy.getPedidos.and.callFake(() => {
      return of(UserListMock);
    });
    TestBed.configureTestingModule({
      declarations: [ PedidosComponent ],
      imports: [RouterTestingModule, MATERIAL_COMPONENTS, HttpClientTestingModule],
      providers: [
        DatePipe
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
    expect(component.pageSize).toEqual(10);
    expect(component.limit).toEqual(10);
    expect(component.offset).toEqual(0);
  });
  it('should call getPedidos() ok', () => {
    component.offset = 0;
    component.limit = 10;
    component.queryString = 'rango de fechas';
    component.getFullQueryString();
    expect(component.fullQueryString).toEqual(`${component.queryString}&offset=0&limit=10`);
    component.getPedidos();
  });
});

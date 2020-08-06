import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PedidoDetalleComponent } from './pedido-detalle.component';
import {CUSTOM_ELEMENTS_SCHEMA} from "@angular/core";
import {RouterModule} from "@angular/router";
import {RouterTestingModule} from "@angular/router/testing";
import {MatTableModule} from "@angular/material";
import {HttpClientModule} from "@angular/common/http";
import { IPedidoDetalleReq } from 'src/app/model/http/detallepedidos.model';

describe('PedidoDetalleComponent', () => {
  let component: PedidoDetalleComponent;
  let fixture: ComponentFixture<PedidoDetalleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [RouterModule, MatTableModule, HttpClientModule, RouterTestingModule],
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

  it('should return false', () => {
    expect(component.someComplete()).toBeFalsy();
  })

  it('should return true', () => {
    component.dataSource.data = [
      {
        isChecked: true,
      }as IPedidoDetalleReq
    ]
    expect(component.someComplete()).toBeTruthy();
  })

  it('should return true when allcomplete is true', () => {
    component.dataSource.data = [
      {
        isChecked: true,
      }as IPedidoDetalleReq
    ]
    component.allComplete = true;
    expect(component.someComplete()).toBeTruthy();
  })
});

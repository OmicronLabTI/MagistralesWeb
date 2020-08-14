import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PedidoDetalleComponent } from './pedido-detalle.component';
import {RouterModule} from "@angular/router";
import {RouterTestingModule} from "@angular/router/testing";
import {HttpClientModule} from "@angular/common/http";
import { IPedidoDetalleReq } from 'src/app/model/http/detallepedidos.model';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('PedidoDetalleComponent', () => {
  let component: PedidoDetalleComponent;
  let fixture: ComponentFixture<PedidoDetalleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        RouterTestingModule,
        FormsModule,
        BrowserAnimationsModule],
      declarations: [ PedidoDetalleComponent ],
      providers: [DatePipe]
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
  });

  it('should return true', () => {
    component.dataSource.data = [
      {
        isChecked: true,
      } as IPedidoDetalleReq
    ];
    expect(component.someComplete()).toBeTruthy();
  });

  it('should return false when allcomplete is true', () => {
    component.dataSource.data = [
      {
        isChecked: false,
      } as IPedidoDetalleReq
    ];
    component.allComplete = true;
    expect(component.someComplete()).toBeFalsy();
  });
});

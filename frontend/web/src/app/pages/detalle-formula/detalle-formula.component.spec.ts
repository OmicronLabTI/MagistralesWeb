import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { DetalleFormulaComponent } from './detalle-formula.component';
import { RouterTestingModule } from '@angular/router/testing';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { PedidosService } from 'src/app/services/pedidos.service';
import { DetalleFormulaMock } from 'src/mocks/pedidosListMock';
import { of, throwError } from 'rxjs';

describe('DetalleFormulaComponent', () => {
  let component: DetalleFormulaComponent;
  let fixture: ComponentFixture<DetalleFormulaComponent>;
  let pedidosServiceSpy;

  beforeEach(async(() => {
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getFormulaDetail'
    ]);
    pedidosServiceSpy.getFormulaDetail.and.callFake(() => {
      return of(DetalleFormulaMock);
    });
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        ReactiveFormsModule,
        FormsModule,
        BrowserAnimationsModule
      ],
      declarations: [ DetalleFormulaComponent ],
      providers: [
        DatePipe, {
          provide: PedidosService, useValue: pedidosServiceSpy
        }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetalleFormulaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.displayedColumns).toEqual([
      'seleccion',
      'cons',
      'numero',
      'descripcion',
      'cantbase',
      'cantreq',
      'consumido',
      'disponible',
      'unidad',
      'almacen',
      'cantpend',
      'enstock',
      'cantalmacen'
    ]);
  });

  it('should call getDetalleFormula ok', () => {
    component.ordenFabricacionId = '1234';
    component.getDetalleFormula();
    expect(pedidosServiceSpy.getFormulaDetail).toHaveBeenCalledWith(component.ordenFabricacionId);
  });

  it('should call getFormulaDetail error', () => {
    pedidosServiceSpy.getFormulaDetail.and.callFake(() => {
      return throwError({ status: 500 });
    });
    component.getDetalleFormula();
    expect(component.dataSource.data.length).toEqual(1);
  });

  it('should updateAllComplete', () => {
    component.dataSource.data = DetalleFormulaMock.response.details;
    component.dataSource.data.forEach( element => element.isChecked = false);
    component.updateAllComplete();
    expect(component.allComplete).toBeFalsy();
    component.dataSource.data.forEach( element => element.isChecked = true);
    component.updateAllComplete();
    expect(component.allComplete).toBeTruthy();

  });
  it('should someComplete', () => {
    component.dataSource.data = [];
    component.dataSource.data = DetalleFormulaMock.response.details;
    component.dataSource.data.forEach( element => element.isChecked = false);
    component.allComplete = false;
    expect(component.someComplete()).toBeFalsy();
    component.dataSource.data.forEach( element => element.isChecked = true);
    expect(component.someComplete()).toBeTruthy();
  });
  it('should setAll', () => {
    component.dataSource.data = null;
    component.setAll(true);
    expect(component.allComplete).toBeTruthy();
    component.dataSource.data = DetalleFormulaMock.response.details;
    component.setAll(true);
    expect(component.allComplete).toBeTruthy();
    expect(component.dataSource.data.every(element => element.isChecked)).toBeTruthy();
    component.setAll(false);
    expect(component.allComplete).toBeFalsy();
    expect(component.dataSource.data.every(element => element.isChecked)).toBeFalsy();
  });
});

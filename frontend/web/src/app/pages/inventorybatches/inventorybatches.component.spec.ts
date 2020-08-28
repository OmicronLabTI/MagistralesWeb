import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InventorybatchesComponent } from './inventorybatches.component';
import { RouterTestingModule } from '@angular/router/testing';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

describe('InventorybatchesComponent', () => {
  let component: InventorybatchesComponent;
  let fixture: ComponentFixture<InventorybatchesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, MATERIAL_COMPONENTS, HttpClientTestingModule, ReactiveFormsModule, FormsModule, BrowserAnimationsModule],
      declarations: [ InventorybatchesComponent ],
      providers: [DatePipe]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InventorybatchesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.lotesSelectedColumns).toEqual([
      'lote',
      'seleccionada',
      'opciones'
    ]);
    expect(component.detailsColumns).toEqual([
      'cons',
      'codigoProducto',
      'descripcionProducto',
      'almacen',
      'totalNecesario',
      'totalSeleccionado'
    ]);
    expect(component.lotesColumns).toEqual([
      'cons',
      'disponible',
      'seleccionada',
      'asignada',
      'opciones'
    ]);
  });

  it('should call getInventoryBatches() ok', () => {
    expect(component.getInventoryBatches()).toBeTruthy();
  });

  it('should return true', () =>{
    expect(component.setSelectedTr()).toBeTruthy();
  });

  it('should return false', () =>{
    expect(component.getBatchesFromSelected()).toBeFalsy();
  });

  it('should return false', () =>{
    expect(component.deleteLotes()).toBeFalsy();
  });

  it('should return false', () =>{
    expect(component.deleteDetails()).toBeFalsy();
  });

  it('should return false', () =>{
    expect(component.setSelectedQuantity()).toBeFalsy();
  });

  it('should return false', () =>{
    expect(component.setTotales()).toBeFalsy();
  });
});

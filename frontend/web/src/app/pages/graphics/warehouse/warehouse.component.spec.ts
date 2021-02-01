import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WarehouseComponent } from './warehouse.component';
import {DatePipe} from '@angular/common';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('WarehouseComponent', () => {
  let component: WarehouseComponent;
  let fixture: ComponentFixture<WarehouseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      declarations: [ WarehouseComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [DatePipe]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WarehouseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    component.localPackages = [
      {
        fieldKey: 'Empaquetado',
        totalCount: 5,
        graphType: 'PackageLocal'
      },
      {
        fieldKey: 'Asignado',
        totalCount: 6,
        graphType: 'PackageLocal'
      },
      {
        fieldKey: 'En Camino',
        totalCount: 7,
        graphType: 'PackageLocal'
      },
      {
        fieldKey: 'Entregado',
        totalCount: 8,
        graphType: 'PackageLocal'
      },
      {
        fieldKey: 'No Entregado',
        totalCount: 9,
        graphType: 'PackageLocal'
      }
    ];
    component.foreignPackages = [
      {
        fieldKey: 'Empaquetado',
        totalCount: 10,
        graphType: 'PackageForeign'
      },
      {
        fieldKey: 'Enviado',
        totalCount: 6,
        graphType: 'PackageForeign'
      }
    ];
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

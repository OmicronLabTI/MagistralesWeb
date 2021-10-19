import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WarehouseComponent } from './warehouse.component';
import {DatePipe} from '@angular/common';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {WarehouseMock} from '../../../../mocks/warehouseListMock';
import {IncidentsService} from '../../../services/incidents.service';
import {of} from 'rxjs';

describe('WarehouseComponent', () => {
  let component: WarehouseComponent;
  let fixture: ComponentFixture<WarehouseComponent>;
  let incidentsServiceSpy;

  beforeEach(async(() => {
    incidentsServiceSpy = jasmine.createSpyObj<IncidentsService>('IncidentsService', [
      'getWarehouseGraph'
    ]);
    incidentsServiceSpy.getWarehouseGraph.and.callFake(() => {
      return of(WarehouseMock);
    });
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      declarations: [ WarehouseComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [DatePipe,
        { provide: IncidentsService, useValue: incidentsServiceSpy },
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WarehouseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

/*  it('should create', () => {
    component.localPackages = WarehouseMock.response.filter(
        itemGraph => itemGraph.graphType.toLowerCase() === GraphType.packageLocal.toLowerCase());
    component.foreignPackages = WarehouseMock.response.filter(
        itemGraph => itemGraph.graphType.toLowerCase() === GraphType.foreignPackage.toLowerCase());

   /!* expect(component).toBeTruthy();*!/
  });*/
 /* it('should generateDataWarehouse', () => {
    component.checkNewRange('20/02/21-22/05/21');
    /!*expect(component.itemsGraph.length > 0).toBeTruthy();*!/
  });*/
});

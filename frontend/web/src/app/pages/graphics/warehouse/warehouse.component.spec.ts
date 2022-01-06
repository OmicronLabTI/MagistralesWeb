import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WarehouseComponent } from './warehouse.component';
import {DatePipe} from '@angular/common';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {WarehouseMock} from '../../../../mocks/warehouseListMock';
import {IncidentsService} from '../../../services/incidents.service';
import {of} from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { DataService } from 'src/app/services/data.service';
import { ErrorService } from 'src/app/services/error.service';

fdescribe('WarehouseComponent', () => {
  let component: WarehouseComponent;
  let fixture: ComponentFixture<WarehouseComponent>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let errorServiceSpy;
  let incidentsServiceSpy;

  beforeEach(async(() => {
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'setUrlActive',
      'getPercentageByItem'
    ]);
    incidentsServiceSpy = jasmine.createSpyObj<IncidentsService>('IncidentsService', [
      'getWarehouseGraph'
    ]);
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    dataServiceSpy.getPercentageByItem.and.callFake(() => {
      return '5';
    });
    incidentsServiceSpy.getWarehouseGraph.and.callFake(() => {
      return of(WarehouseMock);
    });
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule,
      RouterTestingModule],
      declarations: [ WarehouseComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [DatePipe,
        { provide: IncidentsService, useValue: incidentsServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WarehouseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    // component.localPackages = WarehouseMock.response.filter(
    //     itemGraph => itemGraph.graphType.toLowerCase() === GraphType.packageLocal.toLowerCase());
    // component.foreignPackages = WarehouseMock.response.filter(
    //     itemGraph => itemGraph.graphType.toLowerCase() === GraphType.foreignPackage.toLowerCase());

  //  /!* expect(component).toBeTruthy();*!/
  });
 /* it('should generateDataWarehouse', () => {
    component.checkNewRange('20/02/21-22/05/21');
    /!*expect(component.itemsGraph.length > 0).toBeTruthy();*!/
  });*/
});

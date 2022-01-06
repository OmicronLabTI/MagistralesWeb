import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WarehouseComponent } from './warehouse.component';
import { DatePipe } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { WarehouseMock } from '../../../../mocks/warehouseListMock';
import { IncidentsService } from '../../../services/incidents.service';
import { of, throwError } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { DataService } from 'src/app/services/data.service';
import { ErrorService } from 'src/app/services/error.service';
import { ColorsReception, TypeReception } from 'src/app/constants/const';

describe('WarehouseComponent', () => {
  let component: WarehouseComponent;
  let fixture: ComponentFixture<WarehouseComponent>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  let incidentsServiceSpy: jasmine.SpyObj<IncidentsService>;

  beforeEach(async(() => {
    // -------------DATA SERVICE -----------------
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'setUrlActive',
      'getPercentageByItem'
    ]);
    dataServiceSpy.getPercentageByItem.and.callFake(() => {
      return '5';
    });

    // -------------INCIDENTS SERVICE -----------------
    incidentsServiceSpy = jasmine.createSpyObj<IncidentsService>('IncidentsService', [
      'getWarehouseGraph'
    ]);
    incidentsServiceSpy.getWarehouseGraph.and.callFake(() => {
      return of(WarehouseMock);
    });
    // -------------ERROR SERVICE -----------------
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);

    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        RouterTestingModule],
      declarations: [WarehouseComponent],
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
  });

  it('getWarehouseGraph service success', () => {
    incidentsServiceSpy.getWarehouseGraph.and.returnValue(of(WarehouseMock));
    component.checkNewRange('12/01/21 - 12/02/21');
    expect(incidentsServiceSpy.getWarehouseGraph).toHaveBeenCalledWith('12/01/21 - 12/02/21');
    expect(component.itemsGraphReceive.length).toBeGreaterThanOrEqual(4);
    expect(component.itemsGraph.length).toBeGreaterThanOrEqual(4);
  });

  it('getWarehouseGraph service failed', () => {
    incidentsServiceSpy.getWarehouseGraph.and.returnValue(throwError({ status: 500 }));
    component.checkNewRange('12/01/21 - 12/02/21');
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('getNewReceptionData should change color items', () => {
    const incidentsGraphicsMatrices = WarehouseMock.response;
    const result = component.getNewReceptionData(incidentsGraphicsMatrices);
    expect(result.length).toBeGreaterThanOrEqual(15);
    expect(result[0].color).toBe('#007AFF');
    expect(result[0].fieldKey).toBe('Por Recibir');
  });

  it('should getColorItem backOrder', () => {
    const color = component.getColorItem(TypeReception.backOrder);
    expect(color).toBe(ColorsReception.backOrder);
  });

  it('should getColorItem byReceive', () => {
    const color = component.getColorItem(TypeReception.byReceive);
    expect(color).toBe(ColorsReception.byReceive);
  });

  it('should getColorItem pending', () => {
    const color = component.getColorItem(TypeReception.pending);
    expect(color).toBe(ColorsReception.pending);
  });

  it('should getColorItem warehoused', () => {
    const color = component.getColorItem(TypeReception.warehoused);
    expect(color).toBe(ColorsReception.warehoused);
  });
});

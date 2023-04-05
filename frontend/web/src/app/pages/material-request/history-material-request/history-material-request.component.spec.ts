import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { MaterialRequestService } from 'src/app/services/material-request.service';
import { of, throwError } from 'rxjs';
import { HistoryMaterialRequestComponent } from './history-material-request.component';
import { MaterialRequestHistoryMock } from 'src/mocks/materialRequest';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { SatNativeDateModule, SatDatepickerModule } from 'saturn-datepicker';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ErrorService } from 'src/app/services/error.service';

describe('HistoryMaterialRequestComponent', () => {
  let component: HistoryMaterialRequestComponent;
  let fixture: ComponentFixture<HistoryMaterialRequestComponent>;
  let materialReServiceSpy: jasmine.SpyObj<MaterialRequestService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  beforeEach(async(() => {
    materialReServiceSpy = jasmine.createSpyObj<MaterialRequestService>
      ('MaterialRequestService',
        [
          'gethistoryMaterial'
        ]);

    materialReServiceSpy.gethistoryMaterial.and.callFake(() => of(MaterialRequestHistoryMock));
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    errorServiceSpy.httpError.and.returnValue();
    TestBed.configureTestingModule({
      declarations: [HistoryMaterialRequestComponent],
      imports: [
        HttpClientTestingModule,
        RouterTestingModule.withRoutes([]),
        SatDatepickerModule,
        SatNativeDateModule,
        MATERIAL_COMPONENTS,
        FormsModule,
        BrowserAnimationsModule,
        ReactiveFormsModule
      ],
      schemas: [
        CUSTOM_ELEMENTS_SCHEMA
      ],
      providers: [
        {
          provide: MaterialRequestService, useValue: materialReServiceSpy
        }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HistoryMaterialRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    component.date = new FormControl({ begin: new Date(), end: new Date() });
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should filter change is not null', () => {
    component.filterChange();
  });

  it('should filter change is  null', () => {
    component.date.setValue(null);
    component.filterChange();
    expect(component.date.value).toBeNull();
  });

  it('should on OpenDate', () => {
    component.onOpenDate();
    expect(component.minDate).toBeNull();
  });

  it('should on update maxDate today', () => {
    component.updateMaxDate(new Date());
    expect(component.maxDate).toBe(component.today);
  });

  it('should on update maxDate', () => {
    const datePrev = component.getWeekOneWeekDate(component.today, -10);
    component.updateMaxDate(datePrev);
  });

  it('should on change page event', () => {
    component.changeDataEvent({
      pageIndex: 1,
      pageSize: 10,
      previousPageIndex: 0,
      length: 10
    });
    expect(component.limit).toBe(10);
  });

  it('should on change page event', () => {
    materialReServiceSpy.gethistoryMaterial.and.callFake(() => throwError('error'));
  });
  it('should getIsDisabled true', () => {
    component.statusControl.setValue(['Abierto']);
    expect(component.getIsDisabled('Abierto')).toBeTruthy();
  });
  it('should getIsDisabled false', () => {
    component.statusControl.setValue(['Abierto', 'Cerrado']);
    expect(component.getIsDisabled('Abierto')).toBeFalsy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductivityComponent } from './productivity.component';
import { RouterTestingModule } from '@angular/router/testing';
import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ProductivityService } from 'src/app/services/productivity.service';
import { of } from 'rxjs';
import { ProductivityListMock } from 'src/mocks/productivityMock';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ObservableService } from 'src/app/services/observable.service';

describe('ProductivityComponent', () => {
  let component: ProductivityComponent;
  let fixture: ComponentFixture<ProductivityComponent>;
  let productivityServiceSpy;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;

  beforeEach(async(() => {
    productivityServiceSpy = jasmine.createSpyObj<ProductivityService>(
      'ProductivityService', [
      'getProductivity'
    ]
    );
    productivityServiceSpy.getProductivity.and.callFake(() => {
      return of(ProductivityListMock);
    });

    // --- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'setUrlActive'
      ]);

    TestBed.configureTestingModule({
      declarations: [ProductivityComponent],
      imports: [
        RouterTestingModule,
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        ReactiveFormsModule,
        FormsModule,
        BrowserAnimationsModule
      ],
      providers: [
        DatePipe,
        { provide: ProductivityService, useValue: productivityServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductivityComponent);
    fixture.componentInstance.ngAfterViewInit();
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component.dataSourceDetails).toEqual(ProductivityListMock.response.matrix);
    expect(component.dataSource.data).toEqual(
      ProductivityListMock.response.matrix.filter(element => ProductivityListMock.response.matrix.indexOf(element) > 0)
    );
    expect(component.monthColumns).toEqual(ProductivityListMock.response.matrix[0]);
    expect(component).toBeTruthy();
  });

  it('should call getProductivityData ok', () => {
    component.queryString = 'rango de fechas';
    expect(component.queryString).toEqual(component.queryString);
    component.getProductivityData();
    expect(productivityServiceSpy.getProductivity).toHaveBeenCalledWith(component.queryString);
  });

});

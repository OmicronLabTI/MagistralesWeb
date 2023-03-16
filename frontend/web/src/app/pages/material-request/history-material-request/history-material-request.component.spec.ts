import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { MaterialRequestService } from 'src/app/services/material-request.service';
import { of, throwError } from 'rxjs';
import { HistoryMaterialRequestComponent } from './history-material-request.component';
import { MaterialRequestHistoryMock } from 'src/mocks/materialRequest';
import { ComponentsModule } from 'src/app/components/components.module';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { SatNativeDateModule, SatDatepickerModule } from 'saturn-datepicker';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material';

describe('HistoryMaterialRequestComponent', () => {
  let component: HistoryMaterialRequestComponent;
  let fixture: ComponentFixture<HistoryMaterialRequestComponent>;
  let materialReServiceSpy: jasmine.SpyObj<MaterialRequestService>;

  beforeEach(async(() => {
    materialReServiceSpy = jasmine.createSpyObj<MaterialRequestService>
      ('MaterialRequestService',
        [
          'gethistoryMaterial'
        ]);

    materialReServiceSpy.gethistoryMaterial.and.returnValue(of(MaterialRequestHistoryMock));

    TestBed.configureTestingModule({
      declarations: [HistoryMaterialRequestComponent],
      imports: [
        HttpClientTestingModule,
        RouterTestingModule.withRoutes([]),
        SatDatepickerModule,
        SatNativeDateModule,
        MatInputModule,
        FormsModule,
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
    component.date  = new FormControl({begin: new Date(), end: new Date()});
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});

import { Location } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, async } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule, MatSelectModule } from '@angular/material';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxMaskModule } from 'ngx-mask';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { MaterialRequestComponent } from './material-request.component';
import { MaterialRequestData } from 'src/app/model/http/materialReques';
describe('MaterialRequestComponent', () => {
  let component: MaterialRequestComponent;
  let fixture: ComponentFixture<MaterialRequestComponent>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;

  beforeEach(async(() => {

    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'getUserId',
      'getUserName',
      'setMaterialHistoryQuery',
      'setMaterialRequestData'
    ]);

    localStorageServiceSpy.getUserName.and.returnValue('benny benny');
    localStorageServiceSpy.getUserId.and.returnValue('35642b3a-9471-4b89-9862-8bee6d98c361');
    localStorageServiceSpy.setMaterialHistoryQuery.and.returnValue();
    localStorageServiceSpy.setMaterialRequestData.and.returnValue();
    TestBed.configureTestingModule({
      declarations: [MaterialRequestComponent],
      imports: [
        HttpClientTestingModule,
        MatTabsModule,
        MatCheckboxModule,
        MatTableModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        FormsModule,
        RouterTestingModule,
        BrowserAnimationsModule,
        MatInputModule,
        MatSelectModule,
        NgxMaskModule.forRoot()
      ],
      providers: [
        { provide: LocalStorageService, useValue: localStorageServiceSpy },
      ],
      schemas: [
        CUSTOM_ELEMENTS_SCHEMA
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

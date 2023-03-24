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
import { of, throwError } from 'rxjs';
import { ErrorService } from 'src/app/services/error.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { MaterialRequestService } from 'src/app/services/material-request.service';
import { MessagesService } from 'src/app/services/messages.service';
import { ObservableService } from 'src/app/services/observable.service';
import { ReportingService } from 'src/app/services/reporting.service';
import { MaterialPostResMock } from 'src/mocks/materialPost';
import { MaterialPostResFailedMock } from 'src/mocks/materialPostResponseFailed';
import { MaterialRequestMock } from '../../../mocks/materialRequest';
import { DataService } from '../../services/data.service';
import { MaterialRequestComponent } from './material-request.component';
describe('MaterialRequestComponent', () => {
  let component: MaterialRequestComponent;
  let fixture: ComponentFixture<MaterialRequestComponent>;

  let materialReServiceSpy: jasmine.SpyObj<MaterialRequestService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let reportingServiceSpy: jasmine.SpyObj<ReportingService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;
  const getPreMaterialRequestMock = MaterialRequestMock;
  const postMaterialRequestMock = MaterialPostResMock;
  const blobResponse = new Blob();
  const locationStub = {
    back: jasmine.createSpy('back')
  };

  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom',
      'getMessageTitle',
    ]);

    //  ------------------ MaterialRequestService
    materialReServiceSpy = jasmine.createSpyObj<MaterialRequestService>
      ('MaterialRequestService',
        [
          'getPreMaterialRequest',
          'postMaterialRequest',
          'getDestinationStore'
        ]);
    materialReServiceSpy.getDestinationStore.and.returnValue(
      of({
        response: [
          {
            id: 50,
            value: 'MG',
            type: 'string',
            field: 'DestinationWarehouse'
          },
          {
            id: 51,
            value: 'BE',
            type: 'string',
            field: 'DestinationWarehouse'
          },
          {
            id: 52,
            value: 'MN',
            type: 'string',
            field: 'DestinationWarehouse'
          }
        ]
      })
    );
    materialReServiceSpy.getPreMaterialRequest.and.returnValue(of(getPreMaterialRequestMock));
    materialReServiceSpy.postMaterialRequest.and.returnValue(of(postMaterialRequestMock));

    // ------------------ ErrorService
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', ['httpError']);
    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'getUserId',
      'getUserName',
    ]);


    // ------------------ DataService
    dataServiceSpy = jasmine.createSpyObj<DataService>
      ('DataService',
        [
          'setIsToSaveAnything',
          'calculateTernary',
          'openNewTapByUrl'
        ]);
    dataServiceSpy.calculateTernary.and.callFake(<T, U>(validation: boolean, firstValue: T, secondaValue: U): T | U => {
      return validation ? firstValue : secondaValue;
    });
    messagesServiceSpy.getMessageTitle.and.returnValue('Title');
    localStorageServiceSpy.getUserName.and.returnValue('benny benny');
    localStorageServiceSpy.getUserId.and.returnValue('35642b3a-9471-4b89-9862-8bee6d98c361');

    //  -------------------- ReportingService
    reportingServiceSpy = jasmine.createSpyObj<ReportingService>
      ('ReportingService',
        [
          'downloadPreviewMaterial'
        ]
      );
    reportingServiceSpy.downloadPreviewMaterial.and.returnValue(of({
      response: ['', '']
    }));

    // -------------------- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'getNewMaterialComponent',
        'getNewDataSignature',
        'setSearchComponentModal',
        'setOpenSignatureDialog',
        'setMessageGeneralCallHttp',
        'setIsLoading'
      ]
    );
    observableServiceSpy.getNewMaterialComponent.and.returnValue(of({}));
    observableServiceSpy.getNewDataSignature.and.returnValue(of({}));

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
        { provide: MaterialRequestService, useValue: materialReServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ReportingService, useValue: reportingServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy },
        { provide: Location, useValue: locationStub },
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

  it('getPreMaterialRequestH service is success', () => {
    component.getPreMaterialRequestH();
    expect(materialReServiceSpy.getPreMaterialRequest).toHaveBeenCalled();
    expect(component.dataSource.data.length).toBeGreaterThanOrEqual(0);
  });

  it('getPreMaterialRequestH service failed', () => {
    materialReServiceSpy.getPreMaterialRequest.and.returnValue(throwError({ status: 500 }));
    component.getPreMaterialRequestH();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('addNewComponent should call observable', () => {
    component.addNewComponent();
    expect(observableServiceSpy.setSearchComponentModal).toHaveBeenCalled();
  });
  it('updateAllComplete', () => {
    materialReServiceSpy.getPreMaterialRequest.and.returnValue(of(MaterialRequestMock));
    component.updateAllComplete();
    expect(component.allComplete).toBeFalsy();
  });
  it('someComplete should be success ', () => {
    materialReServiceSpy.getPreMaterialRequest.and.returnValue(of(MaterialRequestMock));
    component.getPreMaterialRequestH();
    const isSuccess = component.someComplete();
    expect(isSuccess).toBeTruthy();
  });
  it('someComplete should be false', () => {
    component.dataSource.data = null;
    const isSuccess = component.someComplete();
    expect(isSuccess).toBeFalsy();
  });
  it('setAll method, isThereToDelete property should be false', () => {
    component.dataSource.data = null;
    component.setAll(true);
    expect(component.isThereToDelete).toBeFalsy();
  });
  it('set All method, isThereToDelete property should be true', () => {
    materialReServiceSpy.getPreMaterialRequest.and.returnValue(of(MaterialRequestMock));
    component.getPreMaterialRequestH();
    component.setAll(true);
    expect(component.isThereToDelete = true);
  });
  it('should call setOpenSignatureDialog', () => {
    component.signUser();
    expect(observableServiceSpy.setOpenSignatureDialog).toHaveBeenCalled();
  });
  it('sendRequest service should be success', () => {
    materialReServiceSpy.getPreMaterialRequest.and.returnValue(of(MaterialRequestMock));
    component.getPreMaterialRequestH();
    component.sendRequest();
    expect(materialReServiceSpy.postMaterialRequest).toHaveBeenCalled();
    expect(observableServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
  });
  it('sendRequest service should be success but some failed', () => {
    materialReServiceSpy.getPreMaterialRequest.and.returnValue(of(MaterialRequestMock));
    const mock = MaterialPostResFailedMock;
    materialReServiceSpy.postMaterialRequest.and.returnValue(of(mock));
    component.getPreMaterialRequestH();
    component.sendRequest();
    expect(materialReServiceSpy.postMaterialRequest).toHaveBeenCalled();
  });
  it('sendRequest service should be failed', () => {
    materialReServiceSpy.getPreMaterialRequest.and.returnValue(of(MaterialRequestMock));
    materialReServiceSpy.postMaterialRequest.and.returnValue(throwError({ status: 500 }));
    component.sendRequest();
    expect(materialReServiceSpy.postMaterialRequest).toHaveBeenCalled();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });
  it('onRequestQuantityChange method should change quantity success', () => {
    materialReServiceSpy.getPreMaterialRequest.and.returnValue(of(MaterialRequestMock));
    component.getPreMaterialRequestH();
    component.onRequestQuantityChange(100, 0);
    expect(component.isCorrectData).toBeTruthy();
    expect(dataServiceSpy.setIsToSaveAnything).toHaveBeenCalled();
  });
  it('should download Preview', () => {
    reportingServiceSpy.downloadPreviewMaterial.and.returnValue(of({
      response: ['url1', 'url2']
    }));
    component.downloadPreview();
    expect(dataServiceSpy.openNewTapByUrl).toHaveBeenCalled();
  });
});

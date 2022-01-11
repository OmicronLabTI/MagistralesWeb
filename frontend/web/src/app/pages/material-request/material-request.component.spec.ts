import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialRequestComponent } from './material-request.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MaterialRequestService } from 'src/app/services/material-request.service';
import { of, throwError } from 'rxjs';
import { ErrorService } from 'src/app/services/error.service';
import { DataService } from '../../services/data.service';
import { FileDownloaderService } from 'src/app/services/file.downloader.service';
import { ReportingService } from 'src/app/services/reporting.service';
import { HttpResponse } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatInputModule } from '@angular/material';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ObservableService } from 'src/app/services/observable.service';
import { MessagesService } from 'src/app/services/messages.service';
import { MaterialRequestMock } from '../../../mocks/materialRequest';
import { MaterialPostResMock } from 'src/mocks/materialPost';
import { MaterialPostResFailedMock } from 'src/mocks/materialPostResponseFailed';
import { Location } from '@angular/common';

describe('MaterialRequestComponent', () => {
  let component: MaterialRequestComponent;
  let fixture: ComponentFixture<MaterialRequestComponent>;

  let materialReServiceSpy: jasmine.SpyObj<MaterialRequestService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let fileDownloaderServiceSpy: jasmine.SpyObj<FileDownloaderService>;
  let reportingServiceSpy: jasmine.SpyObj<ReportingService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;

  const getPreMaterialRequestMock = MaterialRequestMock;
  const postMaterialRequestMock = MaterialPostResMock;
  const blobResponse = new HttpResponse<Blob>();
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
          'postMaterialRequest'
        ]);
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
        ]);
    messagesServiceSpy.getMessageTitle.and.returnValue('Title');
    localStorageServiceSpy.getUserName.and.returnValue('benny benny');
    localStorageServiceSpy.getUserId.and.returnValue('35642b3a-9471-4b89-9862-8bee6d98c361');
    // -------------------- FileDownloaderService
    fileDownloaderServiceSpy = jasmine.createSpyObj<FileDownloaderService>
      ('FileDownloaderService', ['downloadFile']);

    //  -------------------- ReportingService
    reportingServiceSpy = jasmine.createSpyObj<ReportingService>
      ('ReportingService',
        [
          'downloadPreviewRawMaterialRequest'
        ]
      );
    reportingServiceSpy.downloadPreviewRawMaterialRequest.and.returnValue(of(blobResponse));

    // -------------------- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'getNewMaterialComponent',
        'getNewDataSignature',
        'setSearchComponentModal',
        'setOpenSignatureDialog',
        'setMessageGeneralCallHttp',
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
      ],
      providers: [
        { provide: MaterialRequestService, useValue: materialReServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: FileDownloaderService, useValue: fileDownloaderServiceSpy },
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
    component.downloadPreview();
    expect(fileDownloaderServiceSpy.downloadFile).toHaveBeenCalled();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialRequestComponent } from './material-request.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MaterialRequestService } from 'src/app/services/material-request.service';
import { IMaterialRequestRes, IMaterialPostRes } from '../../model/http/materialReques';
import { of } from 'rxjs';
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


  const getPreMaterialRequestMock = new IMaterialRequestRes();
  const postMaterialRequestMock = new IMaterialPostRes();
  const blobResponse = new HttpResponse<Blob>();

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
    messagesServiceSpy.getMessageTitle.and.returnValue('');
    localStorageServiceSpy.getUserName.and.returnValue('');
    localStorageServiceSpy.getUserId.and.returnValue('');
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
        { provide: LocalStorageService, useValue: localStorageServiceSpy}
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

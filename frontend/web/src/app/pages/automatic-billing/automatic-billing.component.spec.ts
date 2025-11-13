import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AutomaticBillingComponent } from './automatic-billing.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { InvoicesService } from 'src/app/services/invoices.service';
import { ErrorService } from 'src/app/services/error.service';
import { ObservableService } from 'src/app/services/observable.service';
import { DataService } from 'src/app/services/data.service';
import { of, Subject } from 'rxjs';
import { MatDialog, MatDialogRef, MatMenuModule, MatMenuTrigger, MatTableModule, MatTooltipModule, PageEvent } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpServiceTOCall } from 'src/app/constants/const';
import { automaticBillingResponseMock, changesAppliedConfirmMock, changesAppliedFailedMock,
  invoicesMock, manualRetryResponseMock } from 'src/mocks/invoicesMock';
import { LocalStorageService } from 'src/app/services/local-storage.service';

export class MatDialogMock {
  open() {
    return {
      afterClosed: () => of({}),
    };
  }
}

describe('AutomaticBillingComponent', () => {
  let component: AutomaticBillingComponent;
  let fixture: ComponentFixture<AutomaticBillingComponent>;
  let invoiceServiceSpy: jasmine.SpyObj<InvoicesService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;

  let matDialog: MatDialogMock;
  const invoicesDashboardMock = invoicesMock;

  beforeEach(async(() => {
    matDialog = new MatDialogMock();

    invoiceServiceSpy = jasmine.createSpyObj<InvoicesService>('InvoicesService', [
      'getAutomaticBillingTableData',
      'adjustmentMade',
      'sendManualRetry'
    ]);

    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService', [
      'setUrlActive',
    ]);

    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'calculateTernary',
      'validateValidString',
      'calculateOrValueList'
    ]);

    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);

    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'getUserId'
    ]);

    localStorageServiceSpy.getUserId.and.returnValue('00197298-0e73-4299-b004-092de34c5aaa');

    invoiceServiceSpy.getAutomaticBillingTableData.and.callFake(() => {
      return of(automaticBillingResponseMock);
    });
    invoiceServiceSpy.adjustmentMade.and.callFake(() => {
      return of(changesAppliedConfirmMock);
    });
    invoiceServiceSpy.sendManualRetry.and.callFake(() => {
      return of(manualRetryResponseMock);
    });
    TestBed.configureTestingModule({
      declarations: [AutomaticBillingComponent],
      imports: [
        MatTableModule,
        MatTooltipModule,
        MatMenuModule,
        BrowserAnimationsModule
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        { provide: InvoicesService, useValue: invoiceServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: MatDialog, useValue: matDialog },
      ],
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutomaticBillingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should call changeDataEvent', () => {
    expect(component.changeDataEvent({ pageSize: 10, pageIndex: 0 } as PageEvent)).toEqual({ pageSize: 10, pageIndex: 0 } as PageEvent);
    expect(component.offset).toEqual(0);
    expect(component.limit).toEqual(10);
  });
  it('should sendManualChangesConfirmation', () => {
    component.sendManualChangesConfirmation(invoicesDashboardMock[0]);
    expect(invoiceServiceSpy.adjustmentMade).toHaveBeenCalled();
  });
  it('should sendManualChangesConfirmation', () => {
    invoiceServiceSpy.adjustmentMade.and.callFake(() => {
      return of(changesAppliedFailedMock);
    });
    component.sendManualChangesConfirmation(invoicesDashboardMock[0]);
    expect(invoiceServiceSpy.adjustmentMade).toHaveBeenCalled();
  });
  it('should manualRetry', () => {
    component.manualRetry('c6d2b66c-4a9b-4e24-9a3f-4f6f7a1a730e');
    expect(invoiceServiceSpy.sendManualRetry).toHaveBeenCalled();
  });
  it('should manualChangesApplied', () => {
    spyOn(matDialog, 'open').and.returnValue({
          afterClosed: () => of(true),
        } as MatDialogRef<typeof component>);
    component.manualChangesApplied(invoicesDashboardMock[0]);
    expect(invoiceServiceSpy.adjustmentMade).toHaveBeenCalled();
  });
  it('should manualChangesApplied', () => {
    spyOn(matDialog, 'open').and.returnValue({
          afterClosed: () => of(false),
        } as MatDialogRef<typeof component>);
    component.manualChangesApplied(invoicesDashboardMock[0]);
    expect(invoicesDashboardMock[0].manualChangeApplied).toEqual(false);
  });
});
 
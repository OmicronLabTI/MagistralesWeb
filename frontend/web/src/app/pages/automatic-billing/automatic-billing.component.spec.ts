import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

import { AutomaticBillingComponent } from './automatic-billing.component';
import { InvoicesService } from 'src/app/services/invoices.service';
import { ErrorService } from 'src/app/services/error.service';
import { ObservableService } from 'src/app/services/observable.service';
import { DataService } from 'src/app/services/data.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';

import { of } from 'rxjs';

import {
  MatDialog,
  MatDialogRef,
  MatMenuModule,
  MatTableModule,
  MatTooltipModule,
  PageEvent
} from '@angular/material';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import {
  automaticBillingResponseMock,
  changesAppliedConfirmMock,
  changesAppliedFailedMock,
  invoicesMock,
  manualRetryResponseMock
} from 'src/mocks/invoicesMock';
import { automaticBillingBillingTypeConst, automaticBillingInvoiceTypeConst,
  automaticBillingStatusConst } from 'src/app/constants/automatic_billing_constants';

/**
 * Mock seguro para evitar errores tslint y tipado correcto.
 */
export class MatDialogMock {
  open(): MatDialogRef<any> {
    return {
      afterClosed: () => of({})
    } as MatDialogRef<any>;
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

    invoiceServiceSpy = jasmine.createSpyObj('InvoicesService', [
      'getAutomaticBillingTableData',
      'adjustmentMade',
      'sendManualRetry'
    ]);

    observableServiceSpy = jasmine.createSpyObj('ObservableService', [
      'setUrlActive'
    ]);

    dataServiceSpy = jasmine.createSpyObj('DataService', [
      'calculateTernary',
      'validateValidString',
      'calculateOrValueList'
    ]);

    errorServiceSpy = jasmine.createSpyObj('ErrorService', ['httpError']);

    localStorageServiceSpy = jasmine.createSpyObj('LocalStorageService', [
      'getUserId'
    ]);

    localStorageServiceSpy.getUserId.and.returnValue(
      '00197298-0e73-4299-b004-092de34c5aaa'
    );

    invoiceServiceSpy.getAutomaticBillingTableData.and.returnValue(
      of(automaticBillingResponseMock)
    );

    invoiceServiceSpy.adjustmentMade.and.returnValue(
      of(changesAppliedConfirmMock)
    );

    invoiceServiceSpy.sendManualRetry.and.returnValue(
      of(manualRetryResponseMock)
    );

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
        { provide: LocalStorageService, useValue: localStorageServiceSpy },
        { provide: MatDialog, useValue: matDialog }
      ]
    }).compileComponents();
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
    const event = { pageSize: 10, pageIndex: 0 } as PageEvent;

    expect(component.changeDataEvent(event)).toEqual(event);
    expect(component.offset).toBe(0);
    expect(component.limit).toBe(10);
  });

  it('should sendManualChangesConfirmation', () => {
    component.sendManualChangesConfirmation(invoicesDashboardMock[0]);
    expect(invoiceServiceSpy.adjustmentMade).toHaveBeenCalled();
  });

  it('should sendManualChangesConfirmation (failure)', () => {
    invoiceServiceSpy.adjustmentMade.and.returnValue(
      of(changesAppliedFailedMock)
    );
    component.sendManualChangesConfirmation(invoicesDashboardMock[0]);
    expect(invoiceServiceSpy.adjustmentMade).toHaveBeenCalled();
  });

  it('should manualRetry', () => {
    component.manualRetry('c6d2b66c-4a9b-4e24-9a3f-4f6f7a1a730e');
    expect(invoiceServiceSpy.sendManualRetry).toHaveBeenCalled();
  });

  it('should manualChangesApplied (confirmed)', () => {
    spyOn(matDialog, 'open').and.returnValue({
      afterClosed: () => of(true)
    } as MatDialogRef<any>);

    component.manualChangesApplied(invoicesDashboardMock[0]);
    expect(invoiceServiceSpy.adjustmentMade).toHaveBeenCalled();
  });

  it('should manualChangesApplied (cancel)', () => {
    spyOn(matDialog, 'open').and.returnValue({
      afterClosed: () => of(false)
    } as MatDialogRef<any>);

    component.manualChangesApplied(invoicesDashboardMock[0]);
    expect(invoicesDashboardMock[0].manualChangeApplied).toBe(false);
  });
  it('should seeDetail for SAP orders', () => {
    spyOn(matDialog, 'open');
    component.seeDetail(invoicesDashboardMock[0], true);
    expect(matDialog.open).toHaveBeenCalled();
  });
  it('should seeDetail for remissions', () => {
    spyOn(matDialog, 'open');
    component.seeDetail(invoicesDashboardMock[0], false);
    expect(matDialog.open).toHaveBeenCalled();
  });
  it('should onSelectionChangeStatus', () => {
    component.statusColumnSelectedOptions = [
      automaticBillingStatusConst.error
    ];
    component.onSelectionChangeStatus();
    expect(component.lastOptionStatus).toEqual(automaticBillingStatusConst.error);
  });
  it('should onSelectionChangeInvoiceType', () => {
    component.invoiceTypeColumnSelectedOptions = [
      automaticBillingInvoiceTypeConst.generic
    ];
    component.onSelectionChangeInvoiceType();
    expect(component.lastOptionInvoiceType).toEqual(automaticBillingInvoiceTypeConst.generic);
  });
  it('should onSelectionChangeBillingType', () => {
    component.billingTypeColumSelectedOptions = [
      automaticBillingBillingTypeConst.parcial
    ];
    component.onSelectionChangeBillingType();
    expect(component.lastOptionBillingType).toEqual(automaticBillingBillingTypeConst.parcial);
  });
});

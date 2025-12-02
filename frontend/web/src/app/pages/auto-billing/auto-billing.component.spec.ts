import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { MatDialog } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCheckboxModule } from '@angular/material/checkbox';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';

import { AutoBillingComponent } from './auto-billing.component';
import { AutoBillingService } from 'src/app/services/autoBilling.service';
import { ObservableService } from 'src/app/services/observable.service';
import { AutoBillingModel } from 'src/app/model/http/autoBilling.model';

describe('AutoBillingComponent', () => {
  let component: AutoBillingComponent;
  let fixture: ComponentFixture<AutoBillingComponent>;
  let matDialogMock: jasmine.SpyObj<MatDialog>;
  let observableServiceMock: jasmine.SpyObj<ObservableService>;
  let autoBillingServiceMock: jasmine.SpyObj<AutoBillingService>;

  const result: AutoBillingModel = {
    requestId: 'REQ001',
    sapInvoiceId: 'SAP001',
    sapCreationDate: '2025-11-12',
    invoiceType: 'A',
    billingMode: 'Manual',
    originUser: 'TEST_USER',
    shopOrder: 'ORD001',
    sapOrder: 2,
    shipments: 1,
    retries: 0,
    sapOrders: [{ id: 1, idinvoice: 'INV001', idpedidosap: 'SO001' }],
    remissions: [{ id: 1, idinvoice: 'INV001', idremission: 'REM001' }]
  };

  beforeEach(async () => {
    matDialogMock = jasmine.createSpyObj('MatDialog', ['open']);
    observableServiceMock = jasmine.createSpyObj('ObservableService', ['setUrlActive']);
    autoBillingServiceMock = jasmine.createSpyObj('AutoBillingService', ['getAllAutoBilling']);

    autoBillingServiceMock.getAllAutoBilling.and.returnValue(
      of({ items: [result], total: 1 })
    );

    await TestBed.configureTestingModule({
      declarations: [AutoBillingComponent],
      imports: [
        BrowserAnimationsModule,
        FormsModule,
        MatCheckboxModule,
        MatDialogModule,
        MatTableModule,
        MatTooltipModule,
        MatButtonModule,
        MatIconModule,
        MatPaginatorModule,
        MatMenuModule
      ],
      providers: [
        { provide: MatDialog, useValue: matDialogMock },
        { provide: ObservableService, useValue: observableServiceMock },
        { provide: AutoBillingService, useValue: autoBillingServiceMock }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AutoBillingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load data on init with correct parameters', fakeAsync(() => {
    tick();
    const args = autoBillingServiceMock.getAllAutoBilling.calls.mostRecent().args;

    expect(args[0]).toBe(0);
    expect(args[1]).toBe(10);
    expect(args[2] instanceof Date).toBe(true);
    expect(args[3] instanceof Date).toBe(true);
    expect(args[4]).toBe('Completa,Parcial');
    expect(args[5]).toBe('Genérica,Con datos fiscales');

    expect(component.dataSource.data.length).toBe(1);
  }));

  it('should reload data when paginator triggers pagination', fakeAsync(() => {
    spyOn(component, 'loadPageData').and.callThrough();
    component.loadPageData(10, 10);
    tick();
    expect(component.loadPageData).toHaveBeenCalledWith(10, 10);
  }));

  it('should call service with correct pagination arguments', fakeAsync(() => {
    component.loadPageData(20, 50);
    tick();
    const args = autoBillingServiceMock.getAllAutoBilling.calls.mostRecent().args;

    expect(args[0]).toBe(20);
    expect(args[1]).toBe(50);
    expect(args[4]).toContain('Completa');
    expect(args[5]).toContain('Genérica');
  }));

  it('should open SAP Orders dialog when sapOrders exist', () => {
    component.openSapOrdersDialog(result);
    expect(matDialogMock.open).toHaveBeenCalled();
  });

  it('should not open SAP Orders dialog when sapOrders list is empty', () => {
    const data: AutoBillingModel = { ...result, sapOrders: [], remissions: [] };
    component.openSapOrdersDialog(data);
    expect(matDialogMock.open).not.toHaveBeenCalled();
  });

  it('should open Remissions dialog when remissions exist', () => {
    component.openShipmentsDialog(result);
    expect(matDialogMock.open).toHaveBeenCalled();
  });

  it('should not open Remissions dialog when remissions list is empty', () => {
    const data: AutoBillingModel = { ...result, sapOrders: [], remissions: [] };
    component.openShipmentsDialog(data);
    expect(matDialogMock.open).not.toHaveBeenCalled();
  });
});

import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import {
  MatButtonModule,
  MatDialog,
  MatDialogModule,
  MatIconModule,
  MatMenuModule,
  MatPaginatorModule,
  MatTableModule,
  MatTooltipModule
} from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
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
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;

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
    sapOrders: [
      { id: 1, idinvoice: 'INV001', idpedidosap: 'SO001' }
    ],
    remissions: [
      { id: 1, idinvoice: 'INV001', idremission: 'REM001' }
    ]
  };

  beforeEach(async(() => {
    matDialogMock = jasmine.createSpyObj('MatDialog', ['open']);
    observableServiceMock = jasmine.createSpyObj('ObservableService', ['setUrlActive']);
    autoBillingServiceMock = jasmine.createSpyObj('AutoBillingService', ['getAllAutoBilling']);

    observableServiceSpy = jasmine.createSpyObj('ObservableService', [
      'setUrlActive'
    ]);

    // **AQUI EL FIX IMPORTANTE**
    autoBillingServiceMock.getAllAutoBilling.and.returnValue(
      of({
        items: [result],
        total: 1
      })
    );

    TestBed.configureTestingModule({
      declarations: [AutoBillingComponent],
      imports: [
        BrowserAnimationsModule,
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
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutoBillingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // =====================================
  // CORE TESTS
  // =====================================

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load data and populate table on init', fakeAsync(() => {
    component.ngOnInit();
    tick();

    expect(autoBillingServiceMock.getAllAutoBilling).toHaveBeenCalledWith(0, 20);
    expect(component.dataSource.data.length).toBe(1);
    expect(component.dataSource.data[0].sapInvoiceId).toBe('SAP001');
  }));

  // Test de paginador
  it('should reload data when paginator emits page event', fakeAsync(() => {
    spyOn(component, 'loadPageData').and.callThrough();

    const mockPageEvent = { pageIndex: 1, pageSize: 10, length: 100 };
    const offset = mockPageEvent.pageIndex * mockPageEvent.pageSize;

    component.loadPageData(offset, mockPageEvent.pageSize);
    tick();

    expect(component.loadPageData).toHaveBeenCalledWith(10, 10);
  }));

  it('should call AutoBillingService with correct pagination parameters', fakeAsync(() => {
    component.loadPageData(10, 50);
    tick();
    expect(autoBillingServiceMock.getAllAutoBilling).toHaveBeenCalledWith(10, 50);
  }));

  // =====================================
  // DIALOG TESTS
  // =====================================

  it('should open SAP Orders dialog when sapOrders exist', () => {
    component.openSapOrdersDialog(result);
    expect(matDialogMock.open).toHaveBeenCalled();
  });

  it('should not open SAP Orders dialog when sapOrders are empty', () => {
    const data: AutoBillingModel = { ...result, sapOrders: [], remissions: [] };
    component.openSapOrdersDialog(data);
    expect(matDialogMock.open).not.toHaveBeenCalled();
  });

  it('should open Remissions dialog when remissions exist', () => {
    component.openShipmentsDialog(result);
    expect(matDialogMock.open).toHaveBeenCalled();
  });

  it('should not open Remissions dialog when remissions are empty', () => {
    const data: AutoBillingModel = { ...result, sapOrders: [], remissions: [] };
    component.openShipmentsDialog(data);
    expect(matDialogMock.open).not.toHaveBeenCalled();
  });
});

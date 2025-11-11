import { async, ComponentFixture, TestBed } from '@angular/core/testing';
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
import { AutoBillingComponent } from './auto-billing.component';
import { ObservableService } from 'src/app/services/observable.service';
import { AutoBillingService } from 'src/app/services/autoBilling.service';
import { of } from 'rxjs';
import { AutoBillingModel } from 'src/app/model/http/autoBilling.model';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('AutoBillingComponent', () => {
  let component: AutoBillingComponent;
  let fixture: ComponentFixture<AutoBillingComponent>;
  let matDialogMock: jasmine.SpyObj<MatDialog>;
  let observableServiceMock: jasmine.SpyObj<ObservableService>;
  let autoBillingServiceMock: jasmine.SpyObj<AutoBillingService>;

  const result: AutoBillingModel = {
    requestId: 'XXXXX',
    sapInvoiceId: 'XXXXX',
    sapCreationDate: '12/12/2025',
    invoiceType: 'A',
    billingMode: 'Manual',
    originUser: 'XXXXXX',
    shopOrder: 'DHJ12-XDF23',
    shopTransaction: 'DHJ12-XDF23',
    sapOrder: 12,
    shipments: 12,
    retries: 1,
    sapOrders: [
      {
        id: 1,
        idinvoice: '123',
        idpedidosap: '123'
      }
    ],
    remissions: [
      {
        id: 1,
        idinvoice: '123',
        idremission: '123'
      }
    ]
  };

  beforeEach(async(() => {
    matDialogMock = jasmine.createSpyObj<MatDialog>('MatDialog', ['open']);
    observableServiceMock = jasmine.createSpyObj<ObservableService>('ObservableService', ['setUrlActive']);
    autoBillingServiceMock = jasmine.createSpyObj<AutoBillingService>('AutoBillingService', ['getAllAutoBilling']);

    autoBillingServiceMock.getAllAutoBilling.and.returnValue(of([result]));

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

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should openSapOrdersDialog', () => {
    component.openSapOrdersDialog(result);
    expect(matDialogMock.open).toHaveBeenCalled();
  });

  it('should not openSapOrdersDialog when sapOrders is empty', () => {
    const data: AutoBillingModel = {
      ...result,
      sapOrders: [],
      remissions: []
    };
    component.openSapOrdersDialog(data);
    expect(matDialogMock.open).not.toHaveBeenCalled();
  });

  it('should openShipmentsDialog', () => {
    component.openShipmentsDialog(result);
    expect(matDialogMock.open).toHaveBeenCalled();
  });

  it('should not openShipmentsDialog when remissions is empty', () => {
    const data: AutoBillingModel = {
      ...result,
      sapOrders: [],
      remissions: []
    };
    component.openShipmentsDialog(data);
    expect(matDialogMock.open).not.toHaveBeenCalled();
  });
});

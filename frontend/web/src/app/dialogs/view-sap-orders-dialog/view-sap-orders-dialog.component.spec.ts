import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewSapOrdersDialogComponent } from './view-sap-orders-dialog.component';
import { MAT_DIALOG_DATA, MatButtonModule, MatDialogModule, MatDialogRef, MatIconModule, MatTableModule } from '@angular/material';

describe('ViewSapOrdersDialogComponent', () => {
  let component: ViewSapOrdersDialogComponent;
  let fixture: ComponentFixture<ViewSapOrdersDialogComponent>;

  const mockDialogRef = { close: jasmine.createSpy('close') };
  const dataMock = {
    invoiceId: 'c6d2b66c-4a9b-4e24-9a3f-4f6f7a1a730e',
    orders: [
      { id: 1, idpedidosap: '500001', idinvoice: 'INV-001' }
    ]
  };

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ViewSapOrdersDialogComponent],
      imports: [MatDialogModule, MatIconModule, MatTableModule, MatButtonModule],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef },
        { provide: MAT_DIALOG_DATA, useValue: dataMock },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewSapOrdersDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should close', () => {
    component.close();
    expect(mockDialogRef.close).toHaveBeenCalled();
  });
});

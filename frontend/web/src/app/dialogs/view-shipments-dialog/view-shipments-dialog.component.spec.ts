import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewShipmentsDialogComponent } from './view-shipments-dialog.component';
import { MAT_DIALOG_DATA, MatButtonModule, MatDialogModule, MatDialogRef, MatIconModule, MatTableModule, MatTooltipModule } from '@angular/material';

describe('ViewShipmentsDialogComponent', () => {
  let component: ViewShipmentsDialogComponent;
  let fixture: ComponentFixture<ViewShipmentsDialogComponent>;

  const mockDialogRef = { close: jasmine.createSpy('close') };
  const dataMock = {
    invoiceId: 'c6d2b66c-4a9b-4e24-9a3f-4f6f7a1a730e',
    remissions: [
      { id: 1, idremission: '500001', idinvoice: 'INV-001' }
    ]
  };

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ViewShipmentsDialogComponent],
      imports: [MatDialogModule, MatIconModule, MatTableModule, MatButtonModule, MatTooltipModule],
      providers: [
        { provide: MatDialogRef, useValue: mockDialogRef },
        { provide: MAT_DIALOG_DATA, useValue: dataMock },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewShipmentsDialogComponent);
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

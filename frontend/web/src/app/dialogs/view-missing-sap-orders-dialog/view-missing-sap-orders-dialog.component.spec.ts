import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewMissingSapOrdersDialogComponent } from './view-missing-sap-orders-dialog.component';
import { MatDialogModule, MatIconModule, MatTableModule,
  MatButtonModule, MatTooltipModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

describe('ViewMissingSapOrdersDialogComponent', () => {
  let component: ViewMissingSapOrdersDialogComponent;
  let fixture: ComponentFixture<ViewMissingSapOrdersDialogComponent>;

  const mockDialogRef = { close: jasmine.createSpy('close') };
  const dataMock = {
    dxpOrder: 'c6d2b66c-4a9b-4e24-9a3f-4f6f7a1a730e',
    orders: [
      123456,
      789012,
      345678,
      123456,
      789012,
      345678,
    ]
  };

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ViewMissingSapOrdersDialogComponent],
      imports: [MatDialogModule, MatIconModule, MatTableModule, MatButtonModule, MatTooltipModule],
      providers: [
              { provide: MatDialogRef, useValue: mockDialogRef },
              { provide: MAT_DIALOG_DATA, useValue: dataMock },
            ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewMissingSapOrdersDialogComponent);
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

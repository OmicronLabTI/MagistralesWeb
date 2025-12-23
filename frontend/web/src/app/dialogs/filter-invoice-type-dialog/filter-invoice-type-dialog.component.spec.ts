import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';

import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

import { FilterInvoiceTypeDialogComponent } from './filter-invoice-type-dialog.component';

describe('FilterInvoiceTypeDialogComponent', () => {
  let component: FilterInvoiceTypeDialogComponent;
  let fixture: ComponentFixture<FilterInvoiceTypeDialogComponent>;
  let dialogRefMock: jasmine.SpyObj<MatDialogRef<FilterInvoiceTypeDialogComponent>>;
  const dataMock = true;

  beforeEach(async(() => {
    dialogRefMock = jasmine.createSpyObj('MatDialogRef', ['close']);

    TestBed.configureTestingModule({
      declarations: [FilterInvoiceTypeDialogComponent],
      imports: [
        FormsModule,
        NoopAnimationsModule,
        MatFormFieldModule,
        MatSelectModule,
        MatInputModule,
        MatIconModule,
        MatButtonModule,
        MatDatepickerModule,
        MatNativeDateModule
      ],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefMock },
        { provide: MAT_DIALOG_DATA, useValue: dataMock }
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FilterInvoiceTypeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create component', () => {
    expect(component).toBeTruthy();
  });

  it('should start with form invalid and clear disabled', () => {
    expect(component.formValid).toBe(false);
    expect(component.canClear).toBe(false);
  });

  it('should validate form when ID Factura SAP and value are filled', () => {
    component.selectedSearchType = 'ID Factura SAP';
    component.searchValue = '123';

    component.validateForm();

    expect(component.formValid).toBe(true);
  });

  it('should enable clear when any field has value', () => {
    component.selectedSearchType = 'Pedido SAP';

    component.validateForm();

    expect(component.canClear).toBe(true);
  });

  it('should clear all fields and reset state', () => {
    component.selectedSearchType = 'Pedido SAP';
    component.searchValue = '999';
    component.dateFrom = new Date(2025, 11, 1);
    component.dateTo = new Date(2025, 11, 5);
    component.formValid = true;
    component.canClear = true;

    component.clear();

    expect(component.selectedSearchType).toBe('');
    expect(component.searchValue).toBe('');

    expect(component.dateFrom).not.toBeNull();
    expect(component.dateTo).not.toBeNull();

    expect(component.dateFrom instanceof Date).toBe(true);
    expect(component.dateTo instanceof Date).toBe(true);

    if (component.dateFrom && component.dateTo) {
      expect(component.dateFrom.getTime())
        .toBeLessThanOrEqual(component.dateTo.getTime());
    }

    expect(component.formValid).toBe(false);
    expect(component.canClear).toBe(false);
  });

  it('should NOT close dialog when form is invalid', () => {
    component.formValid = false;

    component.apply();

    expect(dialogRefMock.close).not.toHaveBeenCalled();
  });

  it('should close dialog with correct data when form is valid for ID Factura SAP', () => {
    component.selectedSearchType = 'ID Factura SAP';
    component.searchValue = '777';
    component.formValid = true;

    const expectedFrom = component.dateFrom;
    const expectedTo = component.dateTo;

    component.apply();

    expect(dialogRefMock.close).toHaveBeenCalledWith({
      type: 'ID Factura SAP',
      value: '777',
      from: expectedFrom,
      to: expectedTo
    });
  });

  it('should close dialog when calling close()', () => {
    component.close();
    expect(dialogRefMock.close).toHaveBeenCalled();
  });
});

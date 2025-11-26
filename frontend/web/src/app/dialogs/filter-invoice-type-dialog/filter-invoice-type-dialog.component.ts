import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-filter-invoice-type-dialog',
  templateUrl: './filter-invoice-type-dialog.component.html',
  styleUrls: ['./filter-invoice-type-dialog.component.scss']
})
export class FilterInvoiceTypeDialogComponent {

  searchTypes = [
    { label: 'Cliente', value: 'CLIENTE' },
    { label: 'Correo', value: 'EMAIL' },
    { label: 'Referencia', value: 'REF' },
    { label: 'Folio', value: 'FOLIO' }
  ];

  selectedSearchType = '';
  searchValue = '';
  dateFrom: Date | null = null;
  dateTo: Date | null = null;

  formValid = false;
  canClear = false;

  constructor(public dialogRef: MatDialogRef<FilterInvoiceTypeDialogComponent>) {}

  validateForm() {
    this.formValid =
      !!this.selectedSearchType &&
      !!this.searchValue &&
      !!this.dateFrom &&
      !!this.dateTo;

    this.canClear =
      !!this.selectedSearchType ||
      !!this.searchValue ||
      !!this.dateFrom ||
      !!this.dateTo;
  }

  clear() {
    this.selectedSearchType = '';
    this.searchValue = '';
    this.dateFrom = null;
    this.dateTo = null;

    this.formValid = false;
    this.canClear = false;
  }

  apply() {
    if (!this.formValid) return;

    this.dialogRef.close({
      type: this.selectedSearchType,
      value: this.searchValue,
      from: this.dateFrom,
      to: this.dateTo
    });
  }

  close() {
    this.dialogRef.close();
  }
}

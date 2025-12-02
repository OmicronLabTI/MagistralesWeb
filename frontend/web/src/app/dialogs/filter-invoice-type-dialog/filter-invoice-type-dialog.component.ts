import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { InvoiceFilterResult } from 'src/app/model/dialog/InvoiceFilterResult';

@Component({
  selector: 'app-filter-invoice-type-dialog',
  templateUrl: './filter-invoice-type-dialog.component.html',
  styleUrls: ['./filter-invoice-type-dialog.component.scss']
})
export class FilterInvoiceTypeDialogComponent {
  searchTypes = [
    { label: 'ID Factura SAP', value: 'ID Factura SAP' },
    { label: 'Pedido SAP', value: 'NO Pedido SAP' },
    { label: 'Pedido shop', value: 'Pedido shop' }
  ];

  selectedSearchType = '';
  searchValue = '';
  dateFrom: Date | null = null;
  dateTo: Date | null = null;

  formValid = false;
  canClear = false;

  constructor(public dialogRef: MatDialogRef<FilterInvoiceTypeDialogComponent>) {}

  validateForm(): void {
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

  clear(): void {
    this.selectedSearchType = '';
    this.searchValue = '';
    this.dateFrom = null;
    this.dateTo = null;
    this.formValid = false;
    this.canClear = false;
  }

  apply(): void {
    if (!this.formValid) {
      return;
    }

    const result: InvoiceFilterResult = {
      type: this.selectedSearchType,
      value: this.searchValue,
      from: this.dateFrom,
      to: this.dateTo
    };

    this.dialogRef.close(result);
  }

  close(): void {
    this.dialogRef.close();
  }
}

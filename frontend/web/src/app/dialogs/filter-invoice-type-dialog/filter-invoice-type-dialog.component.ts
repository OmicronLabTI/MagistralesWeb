import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { InvoiceFilterResult } from 'src/app/model/dialog/InvoiceFilterResult';

@Component({
  selector: 'app-filter-invoice-type-dialog',
  templateUrl: './filter-invoice-type-dialog.component.html',
  styleUrls: ['./filter-invoice-type-dialog.component.scss']
})
export class FilterInvoiceTypeDialogComponent {
  searchTypes = [
    { label: 'ID Factura SAP', value: 'ID Factura SAP' },
    { label: 'Pedido SAP', value: 'Pedido SAP' },
    { label: 'Pedido shop', value: 'Pedido shop' }
  ];

  selectedSearchType = '';
  searchValue = '';
  dateFrom: Date | null = null;
  dateTo: Date | null = null;

  formValid = false;
  canClear = false;

  constructor(
    public dialogRef: MatDialogRef<FilterInvoiceTypeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: boolean
  ) {
    this.setDefaultDateRange();
  }

  get isIdFacturaSap(): boolean {
    return this.selectedSearchType === 'ID Factura SAP';
  }

  get isPedidoSap(): boolean {
    return this.selectedSearchType === 'Pedido SAP';
  }

  get isPedidoShop(): boolean {
    return this.selectedSearchType === 'Pedido shop';
  }

  onSearchTypeChange(): void {
    this.searchValue = '';
    this.validateForm();
  }

  private setDefaultDateRange(): void {
    const today = new Date();
    const currentYear = today.getFullYear();
    const yearStart = new Date(currentYear, 0, 1);

    const fromCandidate = new Date(today);
    fromCandidate.setDate(today.getDate() - 4);

    this.dateFrom = fromCandidate < yearStart ? yearStart : fromCandidate;
    this.dateTo = today;
  }

  private isDateRangeValid(): boolean {
    if (!this.dateFrom || !this.dateTo) {
      return false;
    }

    const from = this.dateFrom;
    const to = this.dateTo;

    const currentYear = new Date().getFullYear();
    if (from.getFullYear() !== currentYear || to.getFullYear() !== currentYear) {
      return false;
    }

    if (from > to) {
      return false;
    }

    const msPerDay = 1000 * 60 * 60 * 24;
    const diffMs = to.getTime() - from.getTime();
    const diffDays = Math.floor(diffMs / msPerDay) + 1;

    return diffDays >= 1 && diffDays <= 15;
  }

  validateForm(): void {
    this.canClear =
      !!this.selectedSearchType ||
      !!this.searchValue ||
      !!this.dateFrom ||
      !!this.dateTo;

    if (this.isIdFacturaSap) {
      this.formValid = this.searchValue.trim().length > 0;
      return;
    }

    if (this.isPedidoSap) {
      const pedidos = this.searchValue
        .split(',')
        .map(p => p.trim())
        .filter(p => p.length > 0);

      this.formValid = pedidos.length > 0 && pedidos.length <= 10;
      return;
    }

    if (this.isPedidoShop) {
      this.formValid = this.searchValue.trim().length >= 6;
      return;
    }

    this.formValid = this.isDateRangeValid();
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

    const usesOnlyTextSearch = this.isIdFacturaSap || this.isPedidoSap || this.isPedidoShop;

    const result: InvoiceFilterResult = {
      type: this.selectedSearchType || null,
      value: this.searchValue || null,
      from: usesOnlyTextSearch ? null : this.dateFrom,
      to: usesOnlyTextSearch ? null : this.dateTo
    };

    this.dialogRef.close(result);
  }

  close(): void {
    this.dialogRef.close();
  }
}

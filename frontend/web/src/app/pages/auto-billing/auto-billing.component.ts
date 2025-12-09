import {
  Component,
  OnInit,
  AfterViewInit,
  ViewChild,
  HostListener
} from '@angular/core';

import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';

import { AutoBillingService } from 'src/app/services/autoBilling.service';
import { AutoBillingModel } from 'src/app/model/http/autoBilling.model';
import { ViewSapOrdersDialogComponent } from 'src/app/dialogs/view-sap-orders-dialog/view-sap-orders-dialog.component';
import { ViewShipmentsDialogComponent } from 'src/app/dialogs/view-shipments-dialog/view-shipments-dialog.component';

import { ObservableService } from 'src/app/services/observable.service';
import { HttpServiceTOCall } from 'src/app/constants/const';
import { FilterInvoiceTypeDialogComponent } from 'src/app/dialogs/filter-invoice-type-dialog/filter-invoice-type-dialog.component';

@Component({
  selector: 'app-auto-billing',
  templateUrl: './auto-billing.component.html',
  styleUrls: ['./auto-billing.component.scss']
})
export class AutoBillingComponent implements OnInit, AfterViewInit {

  displayedColumns: string[] = [
    'sapInvoiceId',
    'sapCreationDate',
    'invoiceType',
    'billingMode',
    'originUser',
    'shopOrder',
    'sapOrder',
    'shipments',
    'retries',
    'actions'
  ];

  dataSource = new MatTableDataSource<AutoBillingModel>([]);
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  invoiceOptions = [
    { label: 'Genérica', selected: true, disabled: false },
    { label: 'Con datos fiscales', selected: true, disabled: false }
  ];

  billingOptions = [
    { label: 'Completa', selected: true, disabled: false },
    { label: 'Parcial', selected: true, disabled: false }
  ];

  idtype = '';
  id = '';

  showFilter = false;
  popupPosition: any = {};
  currentOptions: any[] = [];

  startDate: Date;
  endDate: Date;

  isAdvancedSearchActive = false;

  lastFilterState = { invoices: '', billing: '' };

  constructor(
    private autoBillingService: AutoBillingService,
    private dialog: MatDialog,
    private observableService: ObservableService
  ) {
    this.observableService.setUrlActive(HttpServiceTOCall.HISTORY_BILLING);
  }

  ngOnInit(): void {
    const today = new Date();
    const past5 = new Date();
    past5.setDate(today.getDate() - 4);

    this.startDate = past5;
    this.endDate = today;

    this.updateAdvancedSearchState();
    this.loadPageData(0, 10);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.showFilter) {
      return;
    }

    const clicked = event.target as HTMLElement;

    const inside =
      clicked.closest('.filter-box') ||
      clicked.closest('.header-filter');

    if (!inside) {
      this.showFilter = false;

      const currentInvoices = this.getSelectedInvoiceTypes();
      const currentBilling = this.getSelectedBillingTypes();

      const changed =
        currentInvoices !== this.lastFilterState.invoices ||
        currentBilling !== this.lastFilterState.billing;

      if (changed && !this.isAdvancedSearchActive) {
        let size = 10;

        if (this.paginator && this.paginator.pageSize) {
          size = this.paginator.pageSize;
        }

        this.loadPageData(0, size);
      }
    }
  }

  ngAfterViewInit(): void {
    this.paginator.page.subscribe(event => {
      const changed = event.pageSize !== this.paginator.pageSize;

      if (changed) {
        this.paginator.pageIndex = 0;
      }

      const offset = this.paginator.pageIndex * event.pageSize;
      const limit = event.pageSize;

      this.loadPageData(offset, limit);
    });
  }

  getSelectedInvoiceTypes(): string {
    return this.invoiceOptions
      .filter(x => x.selected)
      .map(x => x.label)
      .join(',');
  }

  getSelectedBillingTypes(): string {
    return this.billingOptions
      .filter(x => x.selected)
      .map(x => x.label)
      .join(',');
  }

  updateAdvancedSearchState(): void {
    this.isAdvancedSearchActive =
      this.id.trim() !== '' &&
      this.idtype.trim() !== '';

    this.invoiceOptions.forEach(x => {
      x.disabled = this.isAdvancedSearchActive;
    });

    this.billingOptions.forEach(x => {
      x.disabled = this.isAdvancedSearchActive;
    });
  }

  loadPageData(offset: number, limit: number): void {
    const invoices = this.getSelectedInvoiceTypes();
    const billing = this.getSelectedBillingTypes();

    this.updateAdvancedSearchState();

    this.autoBillingService
      .getAllAutoBilling(
        offset,
        limit,
        this.startDate,
        this.endDate,
        billing,
        invoices,
        this.idtype,
        this.id
      )
      .subscribe(response => {
        this.dataSource.data = response.items;

        if (this.paginator) {
          this.paginator.length = response.total;
        }
      });
  }

  openFilter(event: MouseEvent, type: string): void {
    if (this.isAdvancedSearchActive) {
      return;
    }

    const rect = (event.target as HTMLElement).getBoundingClientRect();

    this.popupPosition = {
      top: rect.bottom + 'px',
      left: rect.left + 'px'
    };

    this.currentOptions =
      type === 'invoice' ? this.invoiceOptions : this.billingOptions;

    this.lastFilterState = {
      invoices: this.getSelectedInvoiceTypes(),
      billing: this.getSelectedBillingTypes()
    };

    this.showFilter = true;
  }

  toggleOption(option: any): void {
    if (!option.disabled) {
      option.selected = !option.selected;
    }
  }

  openSapOrdersDialog(row: AutoBillingModel): void {
    if (!row.sapOrders || row.sapOrders.length === 0) {
      return;
    }

    this.dialog.open(ViewSapOrdersDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: row
    });
  }

  openShipmentsDialog(row: AutoBillingModel): void {
    if (!row.remissions || row.remissions.length === 0) {
      return;
    }

    this.dialog.open(ViewShipmentsDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: row
    });
  }

  onClear(): void {
    this.invoiceOptions.forEach(x => {
      x.selected = true;
      x.disabled = false;
    });

    this.billingOptions.forEach(x => {
      x.selected = true;
      x.disabled = false;
    });

    this.id = '';
    this.idtype = '';
    this.isAdvancedSearchActive = false;

    const today = new Date();
    const past5 = new Date();
    past5.setDate(today.getDate() - 4);

    this.startDate = past5;
    this.endDate = today;

    this.loadPageData(0, 10);
  }

  openAdvancedFiltersDialog(): void {
    const dialogRef = this.dialog.open(FilterInvoiceTypeDialogComponent, {
      panelClass: 'advanced-filter-dialog',
      disableClose: true,
      width: 'auto',
      maxWidth: '95vw'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      this.idtype = result.type
        .replace('ID Factura SAP', 'invoice')
        .replace('Pedido SAP', 'pedidosap')
        .replace('Pedido shop', 'pedidodxp');

      this.id = result.value;

      this.updateAdvancedSearchState();
      this.loadPageData(0, 10);
    });
  }
}

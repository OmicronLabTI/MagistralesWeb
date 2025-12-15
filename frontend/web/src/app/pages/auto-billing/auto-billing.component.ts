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
import { InvoicesService } from 'src/app/services/invoices.service';
import {
  ViewMissingSapOrdersDialogComponent
} from 'src/app/dialogs/view-missing-sap-orders-dialog/view-missing-sap-orders-dialog.component';
import { ErrorService } from 'src/app/services/error.service';

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

  lastFilterState = { invoices: '', billing: '' };
  isEmptyResults = false;

  constructor(
    private autoBillingService: AutoBillingService,
    private dialog: MatDialog,
    private observableService: ObservableService,
    private invoiceService: InvoicesService,
    private errorService: ErrorService,
  ) {
    this.observableService.setUrlActive(HttpServiceTOCall.HISTORY_BILLING);
  }

  ngOnInit(): void {
    const today = new Date();
    const past5 = new Date();
    past5.setDate(today.getDate() - 4);

    this.startDate = past5;
    this.endDate = today;
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

      if (changed) {
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


  loadPageData(offset: number, limit: number): void {
    const invoices = this.getSelectedInvoiceTypes();
    const billing = this.getSelectedBillingTypes();

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
        this.isEmptyResults = response.items.length === 0;

        if (this.paginator) {
          this.paginator.length = response.total;
        }
      });
  }

  openFilter(event: MouseEvent, type: string): void {

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
      width: 'auto',
      panelClass: 'custom-dialog-container',
      data: {
        invoiceId: row.sapInvoiceId,
        orders: row.sapOrders,
        status: row.status,
        updateDate: row.lastUpdateDate ? row.lastUpdateDate : 'N/A',
        isFromAutomaticBilling: true
      }
    });
  }

  openShipmentsDialog(row: AutoBillingModel): void {
    if (!row.remissions || row.remissions.length === 0) {
      return;
    }

    this.dialog.open(ViewShipmentsDialogComponent, {
      width: 'auto',
      panelClass: 'custom-dialog-container',
      data: {
        invoiceId: row.sapInvoiceId,
        remissions: row.remissions,
        status: row.status,
        updateDate: row.lastUpdateDate,
        isFromAutomaticBilling: true
      }
    });
  }

  seeMissingSAPOrders(row: AutoBillingModel): void {
    const params = `pedidodxp=${row.shopOrder}`;
    this.invoiceService.getMissingSAPOrders(params).subscribe(res => {
      this.dialog.open(ViewMissingSapOrdersDialogComponent, {
        width: 'auto',
        panelClass: 'custom-dialog-container',
        data: {
          dxpOrder: row.shopOrder,
          orders: res.response
        }
      });
    }, error => {
      this.errorService.httpError(error);
    }
    );
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

    const today = new Date();
    const past5 = new Date();
    past5.setDate(today.getDate() - 4);

    this.startDate = past5;
    this.endDate = today;

    this.paginator.pageIndex = 0;
    this.loadPageData(0, 10);
  }

  openAdvancedFiltersDialog(): void {
    const dialogRef = this.dialog.open(FilterInvoiceTypeDialogComponent, {
      panelClass: 'advanced-filter-dialog',
      disableClose: true,
      width: 'auto',
      maxWidth: '95vw',
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }
      if (result.type == null || result.value == null) {
        this.startDate = result.from;
        this.endDate = result.to;
        this.invoiceOptions.forEach(x => x.disabled = false);
        this.billingOptions.forEach(x => x.disabled = false);
        this.loadPageData(0, 10);
        return;
      }
      this.idtype = result.type
        .replace('ID Factura SAP', 'invoice')
        .replace('Pedido SAP', 'pedidosap')
        .replace('Pedido shop', 'pedidodxp');
      this.id = result.value;
      const today = new Date();
      const past5 = new Date();
      past5.setDate(today.getDate() - 4);
      this.invoiceOptions.forEach(x => x.selected = true);
      this.billingOptions.forEach(x => x.selected = true);
      if (result.to.toDateString() === today.toDateString() && result.from.toDateString() === past5.toDateString()) {
        this.startDate = result.from;
        this.endDate = result.to;
        this.invoiceOptions.forEach(x => x.disabled = true);
        this.billingOptions.forEach(x => x.disabled = true);
      } else {
        this.invoiceOptions.forEach(x => x.disabled = false);
        this.billingOptions.forEach(x => x.disabled = false);
      }
      this.loadPageData(0, 10);
    });
  }

  onOptionChanged(index: number): void {
    const selectedCount = this.currentOptions.filter(o => o.selected).length;

    if (selectedCount === 1) {
      this.currentOptions
        .filter(o => o.selected)
        .forEach(o => o.disabled = true);

      return;
    }

    this.currentOptions.forEach(o => o.disabled = false);
  }
}

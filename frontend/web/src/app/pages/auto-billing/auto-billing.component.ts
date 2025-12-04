import {
  Component, OnInit, AfterViewInit, ViewChild, ElementRef
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
    { label: 'Genérica', selected: true },
    { label: 'Con datos fiscales', selected: true }
  ];

  billingOptions = [
    { label: 'Completa', selected: true },
    { label: 'Parcial', selected: true }
  ];

  previousInvoiceSelection = '';
  previousBillingSelection = '';
  idtype = '';
  id = '';

  showFilter = false;
  popupPosition: any = {};
  currentOptions: any[] = [];

  constructor(
    private autoBillingService: AutoBillingService,
    private dialog: MatDialog,
    private observableService: ObservableService,
    private host: ElementRef
  ) {
    this.observableService.setUrlActive(HttpServiceTOCall.HISTORY_BILLING);
  }

  ngOnInit(): void {
    this.loadPageData(0, 10);

    document.addEventListener('click', (event: any) => {
      if (!this.showFilter) {
        return;
      }

      const clickedInsidePopup =
        event.target.closest('.filter-box') ||
        event.target.closest('.header-filter');

      if (!clickedInsidePopup) {
        this.showFilter = false;

        const currentInvoices = this.getSelectedInvoiceTypes();
        const currentBilling = this.getSelectedBillingTypes();

        const filtersChanged =
          currentInvoices !== this.previousInvoiceSelection ||
          currentBilling !== this.previousBillingSelection;

        if (filtersChanged) {
          this.loadPageData(0, this.paginator ? this.paginator.pageSize : 10);
        }
      }
    });
  }

  ngAfterViewInit(): void {
    this.paginator.page.subscribe(event => {
      const sizeChanged = event.pageSize !== this.paginator.pageSize;

      if (sizeChanged) {
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
    const today = new Date();
    const past5 = new Date();
    past5.setDate(today.getDate() - 5);

    const billing = this.getSelectedBillingTypes();
    const invoices = this.getSelectedInvoiceTypes();
    const id = this.id;
    const idType = this.idtype;

    this.autoBillingService
      .getAllAutoBilling(
        offset,
        limit,
        past5,
        today,
        billing,
        invoices,
        idType,
        id
      )
      .subscribe(response => {
        this.dataSource.data = response.items;

        if (this.paginator) {
          this.paginator.length = response.total;
        }
      });
  }

  openFilter(event: MouseEvent, type: string): void {
    const target = event.target as HTMLElement;
    const rect = target.getBoundingClientRect();

    this.popupPosition = {
      top: rect.bottom + 'px',
      left: rect.left + 'px'
    };

    this.currentOptions = type === 'invoice'
      ? this.invoiceOptions
      : this.billingOptions;

    this.previousInvoiceSelection = this.getSelectedInvoiceTypes();
    this.previousBillingSelection = this.getSelectedBillingTypes();

    this.showFilter = true;
  }

  toggleOption(option: any): void {
    option.selected = !option.selected;
  }

  openSapOrdersDialog(row: AutoBillingModel): void {
    if (!row.sapOrders || row.sapOrders.length === 0) {
      return;
    }

    this.dialog.open(ViewSapOrdersDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: { invoiceId: row.sapInvoiceId, orders: row.sapOrders }
    });
  }

  openShipmentsDialog(row: AutoBillingModel): void {
    if (!row.remissions || row.remissions.length === 0) {
      return;
    }

    this.dialog.open(ViewShipmentsDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: { invoiceId: row.sapInvoiceId, remissions: row.remissions }
    });
  }

  onClear(): void {
    this.invoiceOptions.forEach(x => x.selected = true);
    this.billingOptions.forEach(x => x.selected = true);
    this.idtype = '';
    this.id = '';
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
      this.loadPageData(0, 10);
    });
  }
}

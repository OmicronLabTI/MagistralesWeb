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
    { label: 'Genérica', selected: false },
    { label: 'Con datos fiscales', selected: false }
  ];

  billingOptions = [
    { label: 'Completa', selected: false },
    { label: 'Parcial', selected: false }
  ];

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
      if (!this.showFilter) return;

      const clickedInsidePopup =
        event.target.closest('.filter-box') ||
        event.target.closest('.header-filter');

      if (!clickedInsidePopup) {
        this.showFilter = false;
        console.log('Seleccionado:', this.currentOptions);
      }
    });
  }

  ngAfterViewInit(): void {
    this.paginator.page.subscribe(event => {
      const sizeChanged = event.pageSize !== this.paginator.pageSize;
      if (sizeChanged) this.paginator.pageIndex = 0;

      const offset = this.paginator.pageIndex * event.pageSize;
      const limit = event.pageSize;

      this.loadPageData(offset, limit);
    });
  }

  loadPageData(offset: number, limit: number): void {
    this.autoBillingService.getAllAutoBilling(offset, limit)
      .subscribe(response => {
        this.dataSource.data = response.items;
        if (this.paginator) this.paginator.length = response.total;
      });
  }

  openFilter(event: MouseEvent, type: string) {
    const target = event.target as HTMLElement;
    const rect = target.getBoundingClientRect();

    this.popupPosition = {
      top: rect.bottom + 'px',
      left: rect.left + 'px'
    };

    this.currentOptions = type === 'invoice'
      ? this.invoiceOptions
      : this.billingOptions;

    this.showFilter = true;
  }

  openSapOrdersDialog(row: AutoBillingModel): void {
    if (!row.sapOrders || row.sapOrders.length === 0) return;

    this.dialog.open(ViewSapOrdersDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: {
        invoiceId: row.sapInvoiceId,
        orders: row.sapOrders
      }
    });
  }

  openShipmentsDialog(row: AutoBillingModel): void {
    if (!row.remissions || row.remissions.length === 0) return;

    this.dialog.open(ViewShipmentsDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: {
        invoiceId: row.sapInvoiceId,
        remissions: row.remissions
      }
    });
  }

  onClear(): void {
    this.invoiceOptions.forEach(x => x.selected = false);
    this.billingOptions.forEach(x => x.selected = false);
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

      console.log('Filtro seleccionado:', result);

      console.log(result);
    });
  }
}

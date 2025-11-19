// ============================================================================
// AutoBillingComponent
// Provides real backend-driven pagination using offset/limit and total count.
// Fully compatible with Angular 8.
// ============================================================================

import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { AutoBillingService } from 'src/app/services/autoBilling.service';
import { AutoBillingModel } from 'src/app/model/http/autoBilling.model';
import { ViewSapOrdersDialogComponent } from 'src/app/dialogs/view-sap-orders-dialog/view-sap-orders-dialog.component';
import { ViewShipmentsDialogComponent } from 'src/app/dialogs/view-shipments-dialog/view-shipments-dialog.component';
import { ObservableService } from 'src/app/services/observable.service';
import { HttpServiceTOCall } from 'src/app/constants/const';

@Component({
  selector: 'app-auto-billing',
  templateUrl: './auto-billing.component.html',
  styleUrls: ['./auto-billing.component.scss']
})
export class AutoBillingComponent implements OnInit, AfterViewInit {

  /**
   * Columns to display in the Material table.
   * These match the backend AutoBillingRowDto properties.
   */
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

  /**
   * Material table datasource for AutoBillingModel items.
   */
  dataSource = new MatTableDataSource<AutoBillingModel>([]);

  /**
   * Reference to Angular Material paginator.
   * Allows listening to pagination events and updating data accordingly.
   */
  @ViewChild(MatPaginator, { static: false }) paginator!: MatPaginator;

  constructor(
    private autoBillingService: AutoBillingService,
    private dialog: MatDialog,
    private observableService: ObservableService,
  ) {
    this.observableService.setUrlActive(HttpServiceTOCall.HISTORY_BILLING);
  }

  /**
   * Initializes component and loads the first page (offset 0, limit 20).
   */
  ngOnInit(): void {
    this.loadPageData(0, 20);
  }

  /**
   * Subscribes to paginator events once the view is initialized.
   * Whenever the user changes the page, new backend data is requested.
   */
  ngAfterViewInit(): void {
    this.paginator.page.subscribe(() => {
      const offset = this.paginator.pageIndex * this.paginator.pageSize;
      const limit = this.paginator.pageSize;
      this.loadPageData(offset, limit);
    });
  }

  /**
   * Loads AutoBilling data from the backend using offset and limit.
   * The backend response contains:
   *    - items: the current page rows
   *    - total: full row count used for paginator.length
   *
   * @param offset Starting index for pagination.
   * @param limit Number of rows per page.
   */
  loadPageData(offset: number, limit: number): void {
    this.autoBillingService.getAllAutoBilling(offset, limit).subscribe(res => {
      this.dataSource.data = res.items;

      // Fix para pruebas unitarias:
      // paginator no existe hasta AfterViewInit
      if (this.paginator) {
        this.paginator.length = res.total;
      }
    });
  }


  /**
   * Opens the SAP Orders dialog for the selected invoice row.
   * The dialog is only opened if the record contains SAP orders.
   *
   * @param row AutoBillingModel representing the selected row.
   */
  openSapOrdersDialog(row: AutoBillingModel): void {
    if (!row.sapOrders || row.sapOrders.length === 0) {
      return;
    }

    this.dialog.open(ViewSapOrdersDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: {
        invoiceId: row.sapInvoiceId,
        orders: row.sapOrders
      }
    });
  }

  /**
   * Opens the Shipments (Remissions) dialog for the selected invoice row.
   * The dialog is shown only if remissions are available.
   *
   * @param row AutoBillingModel representing the selected row.
   */
  openShipmentsDialog(row: AutoBillingModel): void {
    if (!row.remissions || row.remissions.length === 0) {
      return;
    }

    this.dialog.open(ViewShipmentsDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: {
        invoiceId: row.sapInvoiceId,
        remissions: row.remissions
      }
    });
  }
}

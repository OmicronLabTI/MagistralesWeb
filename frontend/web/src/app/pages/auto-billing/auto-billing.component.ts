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

/**
 * AutoBillingComponent
 *
 * This component displays the historical automatic billing records using
 * an Angular Material table with server-driven pagination (offset/limit).
 * It initializes the first page on load, listens to paginator events,
 * and requests new data from the backend whenever the page index or
 * page size changes.
 *
 * It also provides modal dialogs for viewing detailed SAP orders and
 * shipment information associated with each record.
 */
@Component({
  selector: 'app-auto-billing',
  templateUrl: './auto-billing.component.html',
  styleUrls: ['./auto-billing.component.scss']
})
export class AutoBillingComponent implements OnInit, AfterViewInit {

  /**
   * Columns displayed in the Material table.
   * These names must match the column definitions defined in the template.
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
   * Data source for the Material data table.
   * Receives data returned from the backend service.
   */
  dataSource = new MatTableDataSource<AutoBillingModel>([]);

  /**
   * Reference to the Material paginator.
   * Used to subscribe to pagination events and update data accordingly.
   */
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  /**
   * Constructor
   * Injects required services:
   * - AutoBillingService: Fetches paginated backend data.
   * - MatDialog: Opens modal dialogs.
   * - ObservableService: Registers the active API route in the application.
   */
  constructor(
    private autoBillingService: AutoBillingService,
    private dialog: MatDialog,
    private observableService: ObservableService
  ) {
    // Register the active API being used by this page.
    this.observableService.setUrlActive(HttpServiceTOCall.HISTORY_BILLING);
  }

  /**
   * OnInit lifecycle hook.
   * Loads the first page of data using default page size (10).
   */
  ngOnInit(): void {
    this.loadPageData(0, 10);
  }

  /**
   * AfterViewInit lifecycle hook.
   * Subscribes to paginator events after the paginator is available.
   * When the user changes the page size, the page index is reset to 0.
   * Every paginator event triggers a new backend request with updated
   * offset and limit values.
   */
  ngAfterViewInit(): void {
    this.paginator.page.subscribe(event => {
      const pageSizeChanged = event.pageSize !== this.paginator.pageSize;

      // Reset to first page if the page size changes.
      if (pageSizeChanged) {
        this.paginator.pageIndex = 0;
      }

      const offset = this.paginator.pageIndex * event.pageSize;
      const limit = event.pageSize;

      this.loadPageData(offset, limit);
    });
  }

  /**
   * loadPageData
   *
   * Requests paginated data from the backend using the service.
   * After receiving the response:
   * - Updates the data table source.
   * - Updates the paginator length with the total number of records.
   *
   * @param offset Starting index of the page (offset)
   * @param limit Number of items per page (limit)
   */
  loadPageData(offset: number, limit: number): void {
    this.autoBillingService
      .getAllAutoBilling(offset, limit)
      .subscribe(response => {
        this.dataSource.data = response.items;

        if (this.paginator) {
          this.paginator.length = response.total;
        }
      });
  }

  /**
   * openSapOrdersDialog
   *
   * Opens a modal showing the list of SAP orders related to the selected record.
   * If no SAP orders are available, the dialog is not opened.
   *
   * @param row The selected AutoBillingModel row.
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
   * openShipmentsDialog
   *
   * Opens a modal displaying shipment/remission details for the selected record.
   * If no remissions are available, the dialog is not opened.
   *
   * @param row The selected AutoBillingModel row.
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

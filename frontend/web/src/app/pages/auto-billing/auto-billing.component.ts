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
  displayedColumns: string[] = [
    'sapInvoiceId', 'sapCreationDate', 'invoiceType', 'billingMode',
    'originUser', 'shopOrder', 'shopTransaction', 'sapOrder', 'shipments', 'retries', 'actions'
  ];

  dataSource = new MatTableDataSource<AutoBillingModel>([]);

  @ViewChild(MatPaginator, { static: false }) paginator!: MatPaginator;

  constructor(
    private autoBillingService: AutoBillingService,
    private dialog: MatDialog,
    private observableService: ObservableService
  ) {
    this.observableService.setUrlActive(HttpServiceTOCall.HISTORY_BILLING);
  }

  ngOnInit(): void {
    this.loadData(0, 20);
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;

    this.paginator.page.subscribe(() => {
      const offset = this.paginator.pageIndex * this.paginator.pageSize;
      const limit = this.paginator.pageSize;
      this.loadData(offset, limit);
    });
  }

  /** Loads records from backend with pagination */
  loadData(offset: number, limit: number): void {
    this.autoBillingService.getAllAutoBilling(offset, limit).subscribe({
      next: data => {
        this.dataSource.data = data;
      }
    });
  }

  /** Opens SAP Orders dialog */
  openSapOrdersDialog(row: AutoBillingModel): void {
    if (!row || !row.sapOrders || row.sapOrders.length === 0) {
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

  /** Opens Remissions dialog */
  openShipmentsDialog(row: AutoBillingModel): void {
    if (!row || !row.remissions || row.remissions.length === 0) {
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

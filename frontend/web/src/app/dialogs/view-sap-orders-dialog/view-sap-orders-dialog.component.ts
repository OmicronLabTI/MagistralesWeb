import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SapOrderModel } from 'src/app/model/http/autoBilling.model';

@Component({
  selector: 'app-view-sap-orders-dialog',
  templateUrl: './view-sap-orders-dialog.component.html',
  styleUrls: ['./view-sap-orders-dialog.component.scss']
})
export class ViewSapOrdersDialogComponent implements OnInit {
  /** Table data source for displaying SAP Orders */
  dataSource: SapOrderModel[] = [];

  /** Table columns */
  displayedColumns: string[] = ['id', 'idpedidosap'];

  /** Concatenated list of SAP Order IDs (for tooltip/export) */
  sapOrderListText = '';

  constructor(
    public dialogRef: MatDialogRef<ViewSapOrdersDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { invoiceId: string; orders: SapOrderModel[] }
  ) {}

  ngOnInit(): void {
    if (this.data && this.data.orders) {
      this.dataSource = this.data.orders;
    } else {
      this.dataSource = [];
    }

    // ✅ Using arrow function, no extra space before parens
    this.sapOrderListText = this.dataSource
      .map(o => o.idpedidosap)
      .join(', ');
  }

  /** Close the dialog */
  close(): void {
    this.dialogRef.close();
  }
}

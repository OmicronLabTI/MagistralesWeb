import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { RemissionModel } from 'src/app/model/http/autoBilling.model';

@Component({
  selector: 'app-view-shipments-dialog',
  templateUrl: './view-shipments-dialog.component.html',
  styleUrls: ['./view-shipments-dialog.component.scss']
})
export class ViewShipmentsDialogComponent implements OnInit {
  /** Table data source for displaying related remissions */
  dataSource: RemissionModel[] = [];

  /** Table columns */
  displayedColumns: string[] = ['id', 'idremission'];

  /** Concatenated remission IDs for tooltip/export */
  remissionListText = '';

  constructor(
    public dialogRef: MatDialogRef<ViewShipmentsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { invoiceId: string; remissions: RemissionModel[] }
  ) {}

  ngOnInit(): void {
    if (this.data && this.data.remissions) {
      this.dataSource = this.data.remissions;
    } else {
      this.dataSource = [];
    }

    // ✅ Arrow function, no extra space before parens
    this.remissionListText = this.dataSource
      .map(r => r.idremission)
      .join(', ');
  }

  /** Close the dialog */
  close(): void {
    this.dialogRef.close();
  }
}

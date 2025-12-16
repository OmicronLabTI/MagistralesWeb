import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

@Component({
  selector: 'app-view-missing-sap-orders-dialog',
  templateUrl: './view-missing-sap-orders-dialog.component.html',
  styleUrls: ['./view-missing-sap-orders-dialog.component.scss']
})
export class ViewMissingSapOrdersDialogComponent implements OnInit {
  dataSource: number[] = [];
  displayedColumns: string[] = ['sapOrder'];

  constructor(
    public dialogRef: MatDialogRef<ViewMissingSapOrdersDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: {
      dxpOrder: string,
      orders: number[]
    }
  ) { }

  ngOnInit() {
    this.dataSource = this.data.orders;
  }

  close(): void {
    this.dialogRef.close();
  }

}

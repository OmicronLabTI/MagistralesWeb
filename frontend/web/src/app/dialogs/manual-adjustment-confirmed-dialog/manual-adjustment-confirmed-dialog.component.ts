import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

@Component({
  selector: 'app-manual-adjustment-confirmed-dialog',
  templateUrl: './manual-adjustment-confirmed-dialog.component.html',
  styleUrls: ['./manual-adjustment-confirmed-dialog.component.scss']
})
export class ManualAdjustmentConfirmedDialogComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<ManualAdjustmentConfirmedDialogComponent>,
  ) { }

  ngOnInit() {
  }

  confirm() {
    this.dialogRef.close(true);
  }

}

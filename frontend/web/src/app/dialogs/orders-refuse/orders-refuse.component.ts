import { Component, OnInit } from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';
import {CONST_STRING} from '../../constants/const';

@Component({
  selector: 'app-orders-refuse',
  templateUrl: './orders-refuse.component.html',
  styleUrls: ['./orders-refuse.component.scss']
})
export class OrdersRefuseComponent implements OnInit {
  commentsRefuse = CONST_STRING.empty;
  constructor(private dialogRef: MatDialogRef<OrdersRefuseComponent>) { }

  ngOnInit() {
  }

  saveCommentsResult() {
    this.dialogRef.close({isOk: true, comments: this.commentsRefuse});
  }
}

import { Component, OnInit, Inject, ViewChild, AfterViewInit } from '@angular/core';

import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import { IMyNewListReq, BaseComponent, IMyCustomListRes } from 'src/app/model/http/listacomponentes';
import { DataService } from 'src/app/services/data.service';
import { FormControl, Validators } from '@angular/forms';
import { OrdersService } from 'src/app/services/orders.service';
import { Messages } from 'src/app/constants/messages';
import { MatPaginator, MatTableDataSource} from '@angular/material';

@Component({
  selector: 'app-componentslist',
  templateUrl: './componentslist.component.html',
  styleUrls: ['./componentslist.component.scss']
})
export class ComponentslistComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['codigo', 'nombre'];
  dataSource = new MatTableDataSource<BaseComponent>();
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  constructor(
    private dialogRef: MatDialogRef<ComponentslistComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dataService: DataService,
    private orderService: OrdersService
  ) { }

  ngOnInit() {
    this.getCustomList();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  getCustomList() {
    console.log(this.data.code);
    this.orderService.getCustomList('?productId=' + this.data.code).subscribe( result => {
      this.dataSource.data = result.response;
    });
  }

}

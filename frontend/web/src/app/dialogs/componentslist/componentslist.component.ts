import { Component, OnInit, Inject } from '@angular/core';

import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import { IMyNewListReq, BaseComponent, IMyCustomListRes } from 'src/app/model/http/listacomponentes';
import { DataService } from 'src/app/services/data.service';
import { FormControl, Validators } from '@angular/forms';
import { OrdersService } from 'src/app/services/orders.service';
import { Messages } from 'src/app/constants/messages';
import { MatTableDataSource} from '@angular/material';

@Component({
  selector: 'app-componentslist',
  templateUrl: './componentslist.component.html',
  styleUrls: ['./componentslist.component.scss']
})
export class ComponentslistComponent implements OnInit {
  displayedColumns: string[] = ['codigo', 'nombre'];
  dataSource = new MatTableDataSource<BaseComponent>();
  constructor(
    private dialogRef: MatDialogRef<ComponentslistComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dataService: DataService,
    private orderService: OrdersService
  ) { }

  ngOnInit() {
    this.getCustomList();
  }

  getCustomList() {
    this.orderService.getCustomList(this.data.code).subscribe( result => {
      this.dataSource.data = result.response;
    });
  }

}

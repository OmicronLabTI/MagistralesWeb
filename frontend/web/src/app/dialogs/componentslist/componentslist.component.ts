import { Component, OnInit, Inject, ViewChild, AfterViewInit } from '@angular/core';

import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import { BaseComponent } from 'src/app/model/http/listacomponentes';
import { DataService } from 'src/app/services/data.service';
import { OrdersService } from 'src/app/services/orders.service';
import { Messages } from 'src/app/constants/messages';
import { MatTableDataSource} from '@angular/material';
import {ErrorService} from '../../services/error.service';

@Component({
  selector: 'app-componentslist',
  templateUrl: './componentslist.component.html',
  styleUrls: ['./componentslist.component.scss']
})
export class ComponentslistComponent implements AfterViewInit {
  displayedColumns: string[] = ['nombre', 'actions'];
  dataSource = new MatTableDataSource<BaseComponent>();

  constructor(
    private dialogRef: MatDialogRef<ComponentslistComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dataService: DataService,
    private orderService: OrdersService,
    private errorService: ErrorService
  ) { }

  ngAfterViewInit() {
    this.getCustomList();
  }

  getCustomList() {
    this.orderService.getCustomList('?productId=' + this.data.code).subscribe( result => {
      console.log('response', result.response)
      this.dataSource.data = result.response;
    });
  }

  selectComponent(element: BaseComponent) {
    console.log('element: ', element)
    this.dataService.presentToastCustom(Messages.confirmReplaceWithListComponents, 'question', '', true, true).then( (res: any) => {
      if (res.isConfirmed) {
        this.dialogRef.close({componentes: element.components});
      }
    });
  }
  removeCustomList(element: BaseComponent) {
    console.log('element: ', element)

    this.dataService.presentToastCustom(`${Messages.removeListComponents} ${element.name.toUpperCase()}?`,
        'question', '', true, true).then( (res: any) => {
      if (res.isConfirmed) {
        this.orderService.deleteCustomList({productId: element.productId, name: element.name}).subscribe( deleteListResult => {
          console.log('deleteResult: ', deleteListResult)
          this.getCustomList();
        }, error =>  this.errorService.httpError(error));
      }});
  }

}

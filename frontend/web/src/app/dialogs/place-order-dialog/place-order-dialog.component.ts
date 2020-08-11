import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {PedidosService} from '../../services/pedidos.service';
import {ErrorService} from '../../services/error.service';
import {QfbWithNumber} from '../../model/http/users';
import {DataService} from '../../services/data.service';

@Component({
  selector: 'app-place-order-dialog',
  templateUrl: './place-order-dialog.component.html',
  styleUrls: ['./place-order-dialog.component.scss']
})
export class PlaceOrderDialogComponent implements OnInit {
    isPlaceManual = false;
    idQfbSelected = '';
    qfbs: QfbWithNumber[] =  [];
  constructor(private dialogRef: MatDialogRef<PlaceOrderDialogComponent>,
              private ordersServices: PedidosService, private errorService: ErrorService,
              @Inject(MAT_DIALOG_DATA) public placeData: any, private dataService: DataService) {
    console.log('dataReceipt: ', this.placeData)
    if (this.placeData.placeOrdersData !== '') {
      this.idQfbSelected = this.placeData.placeOrdersData.userId ? this.placeData.placeOrdersData.userId : '';
      this.isPlaceManual = true;
    }
  }

  async ngOnInit() {
    await this.ordersServices.getQfbsWithOrders().toPromise().then(resultQfbs => this.qfbs = resultQfbs.response)
        .catch(error => this.errorService.httpError(error));
  }

  changePlaceManual() {
    this.isPlaceManual = true;
  }

  placeOrder(userId: string, userName: string) {
    this.dataService.setQbfToPlace({userId, userName});
    this.dialogRef.close();
  }
}

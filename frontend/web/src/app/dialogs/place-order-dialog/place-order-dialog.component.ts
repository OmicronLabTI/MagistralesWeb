import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {PedidosService} from '../../services/pedidos.service';
import {ErrorService} from '../../services/error.service';
import {QfbWithNumber} from '../../model/http/users';
import {DataService} from '../../services/data.service';
import {PlaceOrders} from '../../model/device/orders';
import {MODAL_NAMES} from '../../constants/const';

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
              @Inject(MAT_DIALOG_DATA) public placeData: PlaceOrders, private dataService: DataService) {
        console.log('dataReceive: ', this.placeData);
        if (this.placeData.placeOrdersData) {
          this.idQfbSelected = this.placeData.placeOrdersData.userId ? this.placeData.placeOrdersData.userId : '';
        }
        this.isPlaceManual = this.idQfbSelected !== '';
  }

  async ngOnInit() {
    await this.ordersServices.getQfbsWithOrders().toPromise().then(resultQfbs => this.qfbs = resultQfbs.response)
        .catch(error => this.errorService.httpError(error));
  }

  changePlaceManual() {
    this.isPlaceManual = true;
  }

  placeOrder(userId: string, userName: string) {
   this.dataService.setQbfToPlace({userId, userName,
      modalType: this.placeData.placeOrdersData.modalType, list: this.placeData.placeOrdersData.list,
       assignType: MODAL_NAMES.assignManual});
   this.dialogRef.close();
  }

  placeOrderAutomatic() {
      this.dataService.setQbfToPlace({
          modalType: this.placeData.placeOrdersData.modalType,
          list: this.placeData.placeOrdersData.list,
          assignType: MODAL_NAMES.assignAutomatic
      });
      this.dialogRef.close();
  }
}

import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {PedidosService} from '../../services/pedidos.service';
import {ErrorService} from '../../services/error.service';
import {QfbWithNumber} from '../../model/http/users';
import {DataService} from '../../services/data.service';
import {CONST_NUMBER, CONST_STRING, MODAL_NAMES} from '../../constants/const';

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
        if (this.placeData.placeOrdersData) {
          this.idQfbSelected = this.placeData.placeOrdersData.userId ? this.placeData.placeOrdersData.userId : '';
        }
        this.isPlaceManual = this.idQfbSelected !== CONST_STRING.empty;
  }

  async ngOnInit() {
    await this.ordersServices.getQfbsWithOrders().toPromise().then(resultQfbs => {
        const newResponse = resultQfbs.response.filter(qfb =>
            !this.placeData.placeOrdersData.isFromReassign ? qfb.asignable === CONST_NUMBER.one :
                qfb.asignable === CONST_NUMBER.one || qfb.asignable === CONST_NUMBER.zero );
        newResponse.forEach( qfbNew => {
            qfbNew.countTotalOrders = new Intl.NumberFormat().format(Number(qfbNew.countTotalOrders));
            qfbNew.countTotalFabOrders = new Intl.NumberFormat().format(Number(qfbNew.countTotalFabOrders));
            qfbNew.countTotalPieces = new Intl.NumberFormat().format(Number(qfbNew.countTotalPieces));
        });

        this.qfbs = newResponse;
    })
        .catch(error => {
            this.errorService.httpError(error);
            this.dialogRef.close();
        });
  }

  changePlaceManual() {
    this.isPlaceManual = true;
  }

  placeOrder(userId: string, userName: string) {
   this.dataService.setQbfToPlace({userId, userName,
      modalType: this.placeData.placeOrdersData.modalType, list: this.placeData.placeOrdersData.list,
       assignType: MODAL_NAMES.assignManual, isFromOrderIsolated: this.placeData.placeOrdersData.isFromOrderIsolated,
       isFromReassign: this.placeData.placeOrdersData.isFromReassign});
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

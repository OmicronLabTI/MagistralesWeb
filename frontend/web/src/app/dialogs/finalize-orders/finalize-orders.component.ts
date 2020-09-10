import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {IOrdersReq} from '../../model/http/ordenfabricacion';
import {MatTableDataSource} from '@angular/material/table';
import {PedidosService} from '../../services/pedidos.service';
import {ErrorService} from '../../services/error.service';
import {ClassNames, CONST_NUMBER, HttpServiceTOCall, MessageType} from '../../constants/const';
import {CancelOrderReq} from '../../model/http/pedidos';
import {DataService} from '../../services/data.service';
import {Messages} from '../../constants/messages';
@Component({
  selector: 'app-finalize-orders',
  templateUrl: './finalize-orders.component.html',
  styleUrls: ['./finalize-orders.component.scss']
})
export class FinalizeOrdersComponent implements OnInit {
  displayedColumns: string[] =  ['nOrder', 'codProduct', 'desProduct', 'lote', 'quantity', 'unit', 'fabDate', 'endDate'];
  dataSource = new MatTableDataSource<IOrdersReq>();
  ordersNoIsolated: IOrdersReq [] = [];
  ordersIsolated: IOrdersReq [] = [];
  toDay = new Date();
  isCorrectData = false;
  constructor(@Inject(MAT_DIALOG_DATA) public finalizeData: any,
              private orderService: PedidosService,
              private errorService: ErrorService,
              private dataService: DataService,
              private dialogRef: MatDialogRef<FinalizeOrdersComponent>) {

  }

  async ngOnInit() {
    let index = 0;
    this.ordersIsolated = [...this.finalizeData.finalizeOrdersData.filter(order => order.docNum === 0)];
    for ( const order of this.ordersIsolated) {
      await this.orderService.getNextBatchCode(order.itemCode).toPromise().then(
          resNewBatchCode => this.ordersIsolated[index].batche = resNewBatchCode.response
      ).catch(error => {
        this.errorService.httpError(error);
        this.dialogRef.close();
      });
      index ++;
    }
    this.dataSource.data = this.ordersIsolated;
    this.ordersNoIsolated = [...this.finalizeData.finalizeOrdersData.filter(order => order.docNum !== 0)];
    if (this.ordersIsolated.length === 0 && this.ordersNoIsolated.length > 0) {
      this.finalizeOrderSend();
    }

  }

  onBatchesChange(batchesValue: string, index: number) {
    this.dataSource.data[index].batche = batchesValue;
    this.isCorrectDataToFinalize();
  }

  onQuantityFinishChange(quantityValue: number, index: number) {
    if ( quantityValue > 0 ) {
      this.dataSource.data[index].quantityFinish = Number(quantityValue.toFixed(CONST_NUMBER.seven));
    }
    this.dataSource.data.forEach( order => order.isWithError = order.quantityFinish === null || order.quantityFinish <= 0);
    this.isCorrectDataToFinalize();
  }

  onFabDateChange(fabDateValue: any, index: number) {
    this.dataSource.data[index].fabDate = fabDateValue;
    this.isCorrectDataToFinalize();
  }

  onEndDateChange(endDateValue: any, index: number) {
    this.dataSource.data[index].endDate = endDateValue;
    this.isCorrectDataToFinalize();
  }

  focusOutLote(index: number) {
    this.orderService.getIfExistsBatchCode(this.ordersIsolated[index].itemCode, this.ordersIsolated[index].batche).subscribe(
        resultExistsBatchCode => {
          this.ordersIsolated[index].isWithErrorBatch = resultExistsBatchCode.response;
        }
        , error => {
          this.dialogRef.close();
          this.errorService.httpError(error);
        });
  }
 isCorrectDataToFinalize() {
    this.isCorrectData = this.dataSource.data.filter(order => order.batche === '' || order.fabDate === undefined ||
        order.endDate === undefined || (order.quantityFinish === undefined || order.quantityFinish <= 0 || order.quantityFinish === null)
        || order.isWithError || order.isWithErrorBatch).length === 0;
 }

  finalizeOrderSend() {
      const fullData = [].concat(this.dataSource.data, this.ordersNoIsolated);
      const finalizeOrderReq: CancelOrderReq[] = [];
      fullData.forEach( orderIsolated => {
        const finalizeOrder = new CancelOrderReq();
        finalizeOrder.orderId = orderIsolated.fabOrderId;
        finalizeOrder.userId = this.dataService.getUserId();
        if (orderIsolated.docNum === 0) {
          finalizeOrder.batches = [{ batchCode: orderIsolated.batche,
            quantity: orderIsolated.quantityFinish.toString(),
            manufacturingDate: this.dataService.transformDate(orderIsolated.fabDate, true),
            expirationDate: this.dataService.transformDate(orderIsolated.endDate, true)
          }];
        } else {
          finalizeOrder.batches = [];
        }
        finalizeOrderReq.push(finalizeOrder);
      });
      this.orderService.putFinalizeOrders(finalizeOrderReq, false).subscribe( finalizeResult => {
        if (finalizeResult.success && finalizeResult.response.failed.length > 0) {
          const titleFinalizeWithError = this.dataService.getMessageTitle(
              finalizeResult.response.failed, MessageType.finalizeOrder, true);
          this.dialogRef.close();
          this.dataService.setCallHttpService(HttpServiceTOCall.ORDERS_ISOLATED);
          this.dataService.presentToastCustom(titleFinalizeWithError, 'error',
              Messages.errorToAssignOrderAutomaticSubtitle, true, false, ClassNames.popupCustom);
        } else {
          this.dialogRef.close();
          this.dataService.setCallHttpService(HttpServiceTOCall.ORDERS_ISOLATED);
          this.dataService.setMessageGeneralCallHttp({title: Messages.success, isButtonAccept: false, icon: 'success'});
        }
      }, error => {
        this.errorService.httpError(error);
        this.dialogRef.close();
      });
  }
}

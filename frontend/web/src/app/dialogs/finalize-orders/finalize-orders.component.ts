import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {IOrdersReq} from '../../model/http/ordenfabricacion';
import {MatTableDataSource} from '@angular/material/table';
import {PedidosService} from '../../services/pedidos.service';
import {ErrorService} from '../../services/error.service';
import {
    ClassNames,
    CONST_NUMBER,
    CONST_STRING,
    HttpServiceTOCall,
    MessageType,
    MODAL_FIND_ORDERS
} from '../../constants/const';
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
    this.finalizeData = finalizeData || new Array();
  }

  async ngOnInit() {
    let index = 0;
    let ordersIsolatedFull: IOrdersReq[] = [];
    this.ordersIsolated = [...this.finalizeData.finalizeOrdersData.filter(order => order.docNum === 0)];

    if (this.ordersIsolated.length > 0) {
      const groupedList = this.groupBy(this. ordersIsolated, order => order.itemCode);

      for ( const order of groupedList) {
        await this.orderService.getNextBatchCode(order[0]).toPromise().then(
            resNewBatchCode => {
              const fullBatchCode = resNewBatchCode.response.split('-');
              let count = 0;
              order[1][0].batche = this.getZfFll(fullBatchCode[0], fullBatchCode[1], count);
              if (order[1].length > 1) {
                for ( const objectOrder of order[1]) {
                  // set default batch code
                  order[1][count].batche = this.getZfFll(fullBatchCode[0], fullBatchCode[1], count);
                  count ++;
                }
              }
              ordersIsolatedFull.push(...order[1]);
            }
        ).catch(error => {
          this.errorService.httpError(error);
          this.dialogRef.close();
        });
        index ++;
      }
    }
    
    ordersIsolatedFull.forEach(x =>  {
      // set default dates
      x.fabDate = this.toDay;
      x.endDate = new Date(new Date().setMonth(this.toDay.getMonth() + 6));
    });
    
    this.dataSource.data = ordersIsolatedFull;
    this.ordersNoIsolated = [...this.finalizeData.finalizeOrdersData.filter(order => order.docNum !== 0)];
    if (this.ordersIsolated.length === 0 && this.ordersNoIsolated.length > 0) {
      this.finalizeOrderSend();
    }
  }

  onBatchesChange(batchesValue: string, index: number) {
      this.dataSource.data[index].isWithErrorBatch = batchesValue === CONST_STRING.empty;
      this.dataSource.data[index].batche = batchesValue;
      this.isCorrectDataToFinalize();
  }

  onQuantityFinishChange(quantityValueAsString: string, index: number) {
    let quantityValue : number = Number.parseFloat(quantityValueAsString);

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
      if (this.dataSource.data.filter(order => order.batche === this.dataSource.data[index].batche
          && order.itemCode === this.dataSource.data[index].itemCode). length > 1) {
          this.ordersIsolated[index].isWithErrorBatch = true;
          this.isCorrectDataToFinalize();
      } else {
          if (this.dataSource.data.filter(order => order.batche === this.dataSource.data[index].batche
              && order.itemCode === this.dataSource.data[index].itemCode). length > 1) {
              this.dataSource.data.filter(order => order.itemCode === this.dataSource.data[index].itemCode)
                  .forEach(order => order.isWithErrorBatch = false);
          }
          this.orderService.getIfExistsBatchCode(this.ordersIsolated[index].itemCode, this.ordersIsolated[index].batche).subscribe(
              resultExistsBatchCode => {
                  this.ordersIsolated[index].isWithErrorBatch = resultExistsBatchCode.response;
                  this.isCorrectDataToFinalize();
              }
              , error => {
                  this.dialogRef.close();
                  this.errorService.httpError(error);
              });
      }

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

  groupBy(list, keyGetter) {
    const map = new Map();
    list.forEach((item) => {
      const key = keyGetter(item);
      const collection = map.get(key);
      if (!collection) {
        map.set(key, [item]);
      } else {
        collection.push(item);
      }
    });
    return map;
  }
  getZfFll = (letterBatchCode: string, nextBatchCode: string, count: number) => (
      `${letterBatchCode}-${this.zFill((Number(nextBatchCode) + count).toString(), CONST_NUMBER.seven)}`
  )
  zFill = (str: string, max: number) => str.length < max ? this.zFill ( '0' + str, max) : str;

  keyDownFunction(event: KeyboardEvent) {
        if (event.key === MODAL_FIND_ORDERS.keyEnter && this.isCorrectData) {
            this.finalizeOrderSend();
        }
    }
}

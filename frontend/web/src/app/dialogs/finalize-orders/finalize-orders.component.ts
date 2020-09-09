import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {IOrdersReq} from '../../model/http/ordenfabricacion';
import {MatTableDataSource} from '@angular/material/table';
import {PedidosService} from '../../services/pedidos.service';
import {ErrorService} from '../../services/error.service';
@Component({
  selector: 'app-finalize-orders',
  templateUrl: './finalize-orders.component.html',
  styleUrls: ['./finalize-orders.component.scss']
})
export class FinalizeOrdersComponent implements OnInit {
  displayedColumns: string[] =  ['nOrder', 'codProduct', 'desProduct', 'lote', 'quantity', 'unit', 'fabDate', 'endDate'];
  dataSource = new MatTableDataSource<IOrdersReq>();
  constructor(@Inject(MAT_DIALOG_DATA) public finalizeData: any,
              private orderService: PedidosService,
              private errorService: ErrorService) {
    console.log('dataFinalizeDialog: ', finalizeData.finalizeOrdersData)

  }

  async ngOnInit() {
    let index = 0;
    console.log('dadta: ', this.finalizeData.finalizeOrdersData[0].itemCode)
 /*   this.orderService.getNextBatchCode(this.finalizeData.finalizeOrdersData[0].itemCode).subscribe(
        resNewBatchCode => console.log('resNew: ', resNewBatchCode)
    , error => this.errorService.httpError(error));*/
    for ( const order of this.finalizeData.finalizeOrdersData) {
      await this.orderService.getNextBatchCode(order.itemCode).toPromise().then(
          resNewBatchCode => this.finalizeData.finalizeOrdersData[index].batche = resNewBatchCode.response
      ).catch(this.errorService.httpError);
      index ++;
    }
  /*  this.finalizeData.finalizeOrdersData.forEach( (order) => {
    });*/
    console.log('afterHttpRequest')
    this.dataSource.data = this.finalizeData.finalizeOrdersData;

  }

  onBatchesChange(batchesValue: string, index: number) {
    console.log('batches: ', batchesValue, ' index: ', index)
    this.dataSource.data[index].batche = batchesValue;
    console.log('data changing: ', this.dataSource.data)
  }

  onQuantityFinishChange(quantityValue: number, index: number) {
    console.log('qeuantity: ', quantityValue, ' index: ', index)
    this.dataSource.data[index].quantityFinish = quantityValue;
    console.log('data changing: ', this.dataSource.data)
  }

  onFabDateChange(fabDateValue: any, index: number) {
    this.dataSource.data[index].fabDate = fabDateValue;
    console.log('data changing: ', this.dataSource.data)
  }

  onEndDateChange(endDateValue: any, index: number) {
    this.dataSource.data[index].endDate = endDateValue;
    console.log('data changing: ', this.dataSource.data)
  }
}

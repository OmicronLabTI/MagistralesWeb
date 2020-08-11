import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import { DataService } from '../../services/data.service';
import {CONST_STRING, MODAL_FIND_ORDERS} from '../../constants/const';
import {Messages} from '../../constants/messages';
import {ErrorService} from '../../services/error.service';
import {IPedidoReq, IPedidosListRes, ParamsPedidos, ProcessOrders} from '../../model/http/pedidos';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {CONST_NUMBER} from '../../constants/const';
import {MatDialog} from '@angular/material/dialog';
import {FindOrdersDialogComponent} from '../../dialogs/find-orders-dialog/find-orders-dialog.component';
import {PlaceOrderDialogComponent} from '../../dialogs/place-order-dialog/place-order-dialog.component';
import {Subscription} from 'rxjs';
import {QfbWithNumber} from '../../model/http/users';

@Component({
  selector: 'app-pedidos',
  templateUrl: './pedidos.component.html',
  styleUrls: ['./pedidos.component.scss']
})
export class PedidosComponent implements OnInit, OnDestroy {
  allComplete = false;
  ordersToProcess = new ProcessOrders();
  // tslint:disable-next-line:max-line-length
  displayedColumns: string[] = ['seleccion', 'cons', 'codigo', 'cliente', 'medico', 'asesor', 'f_inicio', 'f_fin', 'status', 'qfb_asignado', 'actions'];
  dataSource = new MatTableDataSource<IPedidoReq>();
  pageSize = CONST_NUMBER.ten;
  pageEvent: PageEvent;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;
  queryString = CONST_STRING.empty;
  rangeDate = CONST_STRING.empty;
  isDateInit =  true;
  isSearchWithFilter = false;
  filterDataOrders = new ParamsPedidos();
  isThereOrdersToProcess = false;
  subscriptionQfbToPlace = new Subscription();
  constructor(
    private pedidosService: PedidosService,
    private dataService: DataService,
    private errorService: ErrorService,
    private dialog: MatDialog
  ) {
    this.rangeDate = this.getDateFormatted(new Date(), new Date(), true);
    this.filterDataOrders.dateType = '0';
    this.filterDataOrders.dateFull = this.rangeDate;
    this.queryString = `?fini=${this.rangeDate}&offset=${this.offset}&limit=${this.limit}`;
  }

  ngOnInit() {
    this.getPedidos();
    this.dataSource.paginator = this.paginator;
    this.subscriptionQfbToPlace = this.dataService.getQfbToPlace().subscribe(qfbTOPlace => this.onSuccessPlaceToOrder(qfbTOPlace));
  }

  getPedidos() {
    this.pedidosService.getPedidos(this.queryString).subscribe(
      (pedidoRes: IPedidosListRes) => {
        this.lengthPaginator = pedidoRes.comments;
        this.dataSource.data = pedidoRes.response;
        this.dataSource.data.forEach(element => {
          // element.pedidoStatus = element.pedidoStatus === 'O' ? 'Abierto' : 'Cerrado';
          element.class = element.pedidoStatus === 'Abierto' ? 'green' : 'mat-primary';
        });
      },
      error => {
        console.log(error);
        this.errorService.httpError(error);
      }
    );
  }

  updateAllComplete() {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked);
    this.getThereOrdersToProcess();
  }

  someComplete(): boolean {
    if (this.dataSource.data == null) {
      return false;
    }
    return this.dataSource.data.filter(t => t.isChecked).length > 0 && !this.allComplete;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (this.dataSource.data == null) {
      return;
    }
    this.dataSource.data.forEach(t => t.isChecked = completed);
    this.getThereOrdersToProcess();
  }

  processOrders() {
    this.dataService.presentToastCustom(Messages.processOrders, 'warning', CONST_STRING.empty, true, true)
    .then((result: any) => {
      if (result.isConfirmed) {
        this.ordersToProcess.listIds = this.dataSource.data.filter(t => (t.isChecked && t.pedidoStatus === 'Abierto')).map(t => t.docNum);
        this.ordersToProcess.user = this.dataService.getUserId();
        this.pedidosService.processOrders(this.ordersToProcess).subscribe(
          () => {
            this.dataService.presentToastCustom(Messages.success, 'success', CONST_STRING.empty, false, false);
            this.getPedidos();
          },
          error => {
            this.errorService.httpError(error);
          }
        );
      }
    });
  }
  changeDataEvent(event: PageEvent) {
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getPedidos();
    return event;
  }
  openFindOrdersDialog() {
    const dialogRef = this.dialog.open(FindOrdersDialogComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        modalType: 'orders',
        filterOrdersData: this.filterDataOrders
      }
    });
    dialogRef.afterClosed().subscribe((result: ParamsPedidos) => {
      if (result) {
        this.filterDataOrders = new  ParamsPedidos();
      }
      if (result.docNum) {
        this.filterDataOrders.docNum = result.docNum;
        this.filterDataOrders.dateFull = this.getDateFormatted(new Date(), new Date(), true);
        this.queryString = `?docNum=${result.docNum}`;
      } else {
        if (result.dateType) {
          this.filterDataOrders.dateType = result.dateType;
          this.rangeDate = this.getDateFormatted(result.fini, result.ffin, false);
          if ( result.dateType === '0') {
            this.isDateInit = true;
            this.queryString = `?fini=${this.rangeDate}`;
          } else {
            this.isDateInit = false;
            this.queryString = `?ffin=${this.rangeDate}`;
          }
          this.filterDataOrders.dateFull = this.rangeDate;
       }
        if (result.status !== '' && result.status) {
          this.queryString = `${this.queryString}&status=${result.status}`;
          this.filterDataOrders.status = result.status;
        }
        if (result.qfb !== '' && result.qfb) {
          this.queryString = `${this.queryString}&qfb=${result.qfb}`;
          this.filterDataOrders.qfb = result.qfb;
        }
      }
      this.isSearchWithFilter = !!(result.docNum || (result.status && result.status !== '') || (result.qfb && result.qfb !== ''));
      this.queryString = `${this.queryString}&offset=${this.offset}&limit=${this.limit}`;
      if (result) {
        this.getPedidos();
      }
    });
  }
  getDateFormatted(initDate: Date, finishDate: Date, isBeginDate: boolean) {
    if (isBeginDate) {
      initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.thirtyDays);
    }
    return `${this.dataService.transformDate(initDate)}-${this.dataService.transformDate(finishDate)}`;
  }

  openPlaceOrdersDialog() {
    this.createPlaceOrderDialog('');
  }
  onSuccessPlaceToOrder(qfbToPlace: QfbWithNumber) {
    this.dataService.presentToastCustom(`${Messages.placeOrder} ${qfbToPlace.userName} ?`,
        'warning',
        CONST_STRING.empty,
        true, true)
        .then((result: any) => {
          if (result.isConfirmed) {
            console.log('sendQfbToPlace: ');
          } else {
            this.createPlaceOrderDialog(qfbToPlace);
          }
        });
  }

  createPlaceOrderDialog(dataToDialog: any) {
    this.dialog.open(PlaceOrderDialogComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        modalType: 'placeOrders',
        placeOrdersData: dataToDialog
      }
    });
  }
  getThereOrdersToProcess() {
    this.isThereOrdersToProcess = this.dataSource.data.filter(t => (t.isChecked && t.pedidoStatus === 'Abierto')).length > 0;
  }

  ngOnDestroy() {
    this.subscriptionQfbToPlace.unsubscribe();
  }
}

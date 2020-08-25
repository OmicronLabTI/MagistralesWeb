import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import {DataService} from '../../services/data.service';
import {
  ClassNames,
  CONST_NUMBER,
  CONST_STRING, ConstStatus,
  HttpServiceTOCall, MessageType,
  MODAL_FIND_ORDERS,
  MODAL_NAMES
} from '../../constants/const';
import {Messages} from '../../constants/messages';
import {ErrorService} from '../../services/error.service';
import {CancelOrderReq, IPedidoReq, ParamsPedidos, ProcessOrders} from '../../model/http/pedidos';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatDialog} from '@angular/material/dialog';
import {FindOrdersDialogComponent} from '../../dialogs/find-orders-dialog/find-orders-dialog.component';
import {Subscription} from 'rxjs';
import {Title} from '@angular/platform-browser';

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
  pageEvent: PageEvent;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;
  queryString = CONST_STRING.empty;
  fullQueryString = CONST_STRING.empty;
  rangeDate = CONST_STRING.empty;
  isDateInit =  true;
  isSearchWithFilter = false;
  filterDataOrders = new ParamsPedidos();
  isThereOrdersToPlan = false;
  isThereOrdersToPlace = false;
  subscriptionCallHttp = new Subscription();
  isThereOrdersToCancel = false;
  constructor(
    private pedidosService: PedidosService,
    private dataService: DataService,
    private errorService: ErrorService,
    private dialog: MatDialog,
    private titleService: Title
  ) {
    this.dataService.setUrlActive(HttpServiceTOCall.ORDERS);
    this.rangeDate = this.getDateFormatted(new Date(), new Date(), true);
    this.filterDataOrders.dateType = '0';
    this.filterDataOrders.dateFull = this.rangeDate;
    this.queryString = `?fini=${this.rangeDate}`;
    this.getFullQueryString();
  }

  ngOnInit() {
    this.subscriptionCallHttp = this.dataService.getCallHttpService().subscribe(callHttpService => {
      if (callHttpService === HttpServiceTOCall.ORDERS) {
        this.getPedidos();
      }
    });
    this.titleService.setTitle('OmicronLab - Pedidos');
    this.getPedidos();
    this.dataSource.paginator = this.paginator;
  }

  getPedidos() {
    this.pedidosService.getPedidos(this.fullQueryString).subscribe(
      pedidoRes => {
        this.lengthPaginator = pedidoRes.comments;
        this.dataSource.data = pedidoRes.response;
        this.dataSource.data.forEach(element => {
          element.class = element.pedidoStatus === 'Abierto' ? 'green' : 'mat-primary';
        });
        this.isThereOrdersToPlan = false;
        this.isThereOrdersToPlace = false;
        this.isThereOrdersToCancel = false;
      },
      error => {/// checar con gus para manejar errores
        this.errorService.httpError(error);
        this.dataSource.data = [];
      }
    );
  }

  updateAllComplete() {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked);
    this.getButtonsToUnLooked();
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
    this.getButtonsToUnLooked();
  }

  processOrders() {
    this.dataService.presentToastCustom(Messages.processOrders, 'warning', CONST_STRING.empty, true, true)
    .then((result: any) => {
      if (result.isConfirmed) {
        this.ordersToProcess.listIds = this.dataSource.data.filter(t =>
            (t.isChecked && t.pedidoStatus === ConstStatus.abierto)).map(t => t.docNum);
        this.ordersToProcess.user = this.dataService.getUserId();
        this.pedidosService.processOrders(this.ordersToProcess).subscribe(
          resProcessOrder => {
            if (resProcessOrder.success && resProcessOrder.response.length > 0) {
              const titleProcessWithError = this.dataService.getMessageTitle(resProcessOrder.response, MessageType.processOrder);
              this.getPedidos();
              this.dataService.presentToastCustom(titleProcessWithError, 'info',
                  Messages.errorToAssignOrderAutomaticSubtitle, true, false, ClassNames.popupCustom);
            } else {
              this.getPedidos();
              this.dataService.setMessageGeneralCallHttp({title: Messages.success , icon: 'success', isButtonAccept: false});
            }
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
    this.getFullQueryString();
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
        this.offset = 0;
        this.limit = 10;
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
      if ((result && result.dateType === '0') && (result && result.status === '' || result.qfb === '')) {
        this.isSearchWithFilter = false;
      }
      if ((result && result.dateType === '0') && (result && result.status !== '' || result.qfb !== '')) {
         this.isSearchWithFilter = true;
      }
      if ((result && result.dateType === '1') && (result && result.status !== '' || result.qfb !== '')) {
        this.isSearchWithFilter = true;
      }
      if ((result && result.dateType === '1') && (result && result.status === '' || result.qfb === '')) {
        this.isSearchWithFilter = true;
      }
      if (result && result.docNum !== '') {
        this.isSearchWithFilter = true;
      }
      this.getFullQueryString();
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
    this.dataService.setQbfToPlace(
        {
          modalType: MODAL_NAMES.placeOrders,
          list: this.dataSource.data.filter(t => (t.isChecked && t.pedidoStatus === ConstStatus.planificado)).map(t => t.docNum)
        });
  }
  getButtonsToUnLooked() {
    this.isThereOrdersToPlan = this.getIsThereOnData(ConstStatus.abierto);
    this.isThereOrdersToPlace = this.getIsThereOnData(ConstStatus.planificado);
    this.isThereOrdersToCancel = this.getIsThereOnData(ConstStatus.finalizado , true);
  }
  getIsThereOnData(status: string, isFromCancelOrder = false) {
    if (!isFromCancelOrder) {
      return this.dataSource.data.filter(t => (t.isChecked && t.pedidoStatus === status)).length > 0;
    } else {
      return this.dataSource.data.filter(t => (t.isChecked &&
          (t.pedidoStatus !== status && t.pedidoStatus !== ConstStatus.cancelado))).length > 0;
    }
  }
  getFullQueryString() {
    this.fullQueryString = `${this.queryString}&offset=${this.offset}&limit=${this.limit}`;
  }

  ngOnDestroy() {
    this.subscriptionCallHttp.unsubscribe();
  }

  cancelOrders() {
    this.dataService.setCancelOrders({list: this.dataSource.data.filter
      (t => (t.isChecked && t.pedidoStatus !== ConstStatus.finalizado)).map(order => {
        const cancelOrder = new CancelOrderReq();
        cancelOrder.orderId = order.docNum;
        return cancelOrder;
      }),
        cancelType: MODAL_NAMES.placeOrders});
  }
}

import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {
  ComponentSearch,
  CONST_NUMBER,
  CONST_STRING,
  ConstOrders,
  ConstStatus,
  FromToFilter,
  HttpServiceTOCall,
  HttpStatus,
  MODAL_FIND_ORDERS,
  MODAL_NAMES
} from '../../constants/const';
import {DataService} from '../../services/data.service';
import {ErrorService} from '../../services/error.service';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {Title} from '@angular/platform-browser';
import {MatTableDataSource} from '@angular/material';
import {IOrdersReq} from 'src/app/model/http/ordenfabricacion';
import {ErrorHttpInterface} from 'src/app/model/http/commons';
import {OrdersService} from 'src/app/services/orders.service';
import {CancelOrderReq, ParamsPedidos} from 'src/app/model/http/pedidos';
import {Subscription} from 'rxjs';
import {MatDialog} from '@angular/material/dialog';
import {FinalizeOrdersComponent} from '../../dialogs/finalize-orders/finalize-orders.component';

@Component({
  selector: 'app-faborders-list',
  templateUrl: './faborders-list.component.html',
  styleUrls: ['./faborders-list.component.scss']
})
export class FabordersListComponent implements OnInit, OnDestroy {
  allComplete = false;
  displayedColumns: string[] = [
    'seleccion',
    'cons',
    'pedido',
    'orden',
    'codigoproducto',
    'descripcion',
    'cantidadplanificada',
    'fechaorden',
    'fechatermino',
    'qfbasignado',
    'estatus',
    'actions'
  ];
  dataSource = new MatTableDataSource<IOrdersReq>();
  pageEvent: PageEvent;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;
  queryString = CONST_STRING.empty;
  fullQueryString = CONST_STRING.empty;
  isDateInit =  true;
  filterDataOrders = new ParamsPedidos();
  pageIndex = 0;
  subscriptionObservables = new Subscription();
  isSearchOrderWithFilter = false;
  isThereOrdersIsolatedToCancel = false;
  isAssignOrderIsolated = false;
  isReAssignOrderIsolated = false;
  isFinalizeOrderIsolated = false;
  constructor(
    private ordersService: OrdersService,
    private dataService: DataService,
    private errorService: ErrorService,
    private titleService: Title,
    private dialog: MatDialog,
  ) {
    this.dataService.setUrlActive(HttpServiceTOCall.ORDERS_ISOLATED);
    this.filterDataOrders.isFromOrders = false;
    this.filterDataOrders.dateType = ConstOrders.defaultDateInit;
    this.filterDataOrders.dateFull = this.getDateFormatted(new Date(), new Date(), true);
    this.queryString = `?fini=${this.filterDataOrders.dateFull}`;
    this.getFullQueryString();
  }

  ngOnInit() {
    this.titleService.setTitle('OmicronLab - Órdenes de fabricación');
    this.getOrders();
    this.dataSource.paginator = this.paginator;
    this.subscriptionObservables.add(this.dataService.getNewSearchOrdersModal().subscribe( resultSearchOrdersModal => {
      if (!resultSearchOrdersModal.isFromOrders) {
        this.onSuccessSearchOrdersModal(resultSearchOrdersModal);
      }
    }));
    this.subscriptionObservables.add(this.dataService.getCallHttpService().subscribe(detailHttpCall => {
          if (detailHttpCall === HttpServiceTOCall.ORDERS_ISOLATED) {
            this.getOrders();
          }
        }));
  }

  updateAllComplete() {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked);
    this.getButtonsOrdersIsolatedToUnLooked();
  }

  someComplete(): boolean {
    return this.dataSource.data.filter(t => t.isChecked).length > 0 && !this.allComplete;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (this.dataSource.data == null) {
      return;
    }
    this.dataSource.data.forEach(t => t.isChecked = completed);
    this.getButtonsOrdersIsolatedToUnLooked();
  }

  getOrders() {
    this.ordersService.getOrders(this.fullQueryString).subscribe(
      ordersRes => {
        this.lengthPaginator = ordersRes.comments;
        this.dataSource.data = ordersRes.response;
        this.dataSource.data.forEach(element => {
          switch (element.status) {
            case ConstStatus.abierto:
              element.class = 'abierto';
              break;
            case ConstStatus.planificado:
              element.class = 'planificado';
              break;
            case ConstStatus.asignado:
              element.class = 'asignado';
              break;
            case ConstStatus.pendiente:
              element.class = 'pendiente';
              break;
            case ConstStatus.terminado:
              element.class = 'terminado';
              break;
            case ConstStatus.enProceso:
              element.class = 'proceso';
              break;
            case ConstStatus.reasingado:
              element.class = 'reasignado';
              break;
            case ConstStatus.finalizado:
              element.class = 'finalizado';
              break;
            case ConstStatus.cancelado.toUpperCase():
              element.class = 'cancelado';
              break;
          }
          element.description = element.description.toUpperCase();
        });
        this.isThereOrdersIsolatedToCancel = false;
        this.isAssignOrderIsolated = false;
        this.isReAssignOrderIsolated = false;
        this.isFinalizeOrderIsolated = false;
      },
        (error: ErrorHttpInterface) => {
        if (error.status !== HttpStatus.notFound) {
          this.errorService.httpError(error);
        }
        this.dataSource.data = [];
      }
    );
  }

  getFullQueryString() {
    this.fullQueryString = `${this.queryString}&offset=${this.offset}&limit=${this.limit}`;
  }

  getDateFormatted(initDate: Date, finishDate: Date, isBeginDate: boolean) {
    if (isBeginDate) {
      initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.thirtyDays);
    }
    return `${this.dataService.transformDate(initDate)}-${this.dataService.transformDate(finishDate)}`;
  }

  changeDataEvent(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getFullQueryString();
    this.getOrders();
    return event;
  }

  createOrderIsolated() {
    this.dataService.setSearchComponentModal({modalType: ComponentSearch.createOrderIsolated});
  }

  openSearchOrders() {
    this.dataService.setSearchOrdersModal({modalType: ConstOrders.modalOrdersIsolated, filterOrdersData: this.filterDataOrders });

  }

  onSuccessSearchOrdersModal(resultSearchOrdersModal: ParamsPedidos) {
    this.filterDataOrders = this.dataService.getNewDataToFilter(resultSearchOrdersModal)[0];
    this.queryString = this.dataService.getNewDataToFilter(resultSearchOrdersModal)[1];
    this.isSearchOrderWithFilter = this.dataService.getIsWithFilter(resultSearchOrdersModal);
    this.pageIndex = 0;
    this.offset = 0;
    this.limit = 10;
    this.getFullQueryString();
    this.getOrders();
    this.isDateInit = resultSearchOrdersModal.dateType === ConstOrders.defaultDateInit;
  }

  cancelOrder() {
    this.dataService.setCancelOrders({list: this.dataSource.data.filter
      (t => (t.isChecked && t.status !== ConstStatus.finalizado)).map(order => {
        const cancelOrder = new CancelOrderReq();
        cancelOrder.orderId = Number(order.fabOrderId);
        return cancelOrder;
      }),
      cancelType: MODAL_NAMES.placeOrdersDetail, isFromCancelIsolated: true});
  }
  assignOrderIsolated() {
    this.dataService.setQbfToPlace({modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataSource.data.filter(t => t.isChecked && t.status === ConstStatus.planificado).map(order => Number(order.fabOrderId))
      , isFromOrderIsolated: true});
  }
  private getButtonsOrdersIsolatedToUnLooked() {
    this.isFinalizeOrderIsolated = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.terminado, FromToFilter.fromDefault);
    this.isThereOrdersIsolatedToCancel = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.finalizado,
                                                                           FromToFilter.fromOrdersIsolatedCancel);
    this.isAssignOrderIsolated = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.planificado,
        FromToFilter.fromDefault);
    this.isReAssignOrderIsolated = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.reasingado,
                                                                      FromToFilter.fromOrderIsolatedReassign);
  }
  reAssignOrder() {
    this.dataService.setQbfToPlace({modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataService.getItemOnDateWithFilter(this.dataSource.data,
                                FromToFilter.fromOrderIsolatedReassignItems).map(order => Number(order.fabOrderId))
      , isFromOrderIsolated: true, isFromReassign: true});
  }
  ngOnDestroy() {
    this.subscriptionObservables.unsubscribe();
  }

  finalizeOrder() {
    this.dialog.open(FinalizeOrdersComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        finalizeOrdersData: this.dataService.getItemOnDateWithFilter(this.dataSource.data, FromToFilter.fromDefault, ConstStatus.terminado)
      }
    }).afterClosed().subscribe(() => this.getOrders());
  }
}

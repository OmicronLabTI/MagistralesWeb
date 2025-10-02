import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
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
  MODAL_NAMES, orderRelationTypes, RouterPaths
} from '../../constants/const';
import { DataService } from '../../services/data.service';
import { ErrorService } from '../../services/error.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Title } from '@angular/platform-browser';
import { MatTableDataSource } from '@angular/material';
import { IOrdersReq } from 'src/app/model/http/ordenfabricacion';
import { ErrorHttpInterface } from 'src/app/model/http/commons';
import { OrdersService } from 'src/app/services/orders.service';
import { CancelOrderReq, ParamsPedidos } from 'src/app/model/http/pedidos';
import { Subscription } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { FinalizeOrdersComponent } from '../../dialogs/finalize-orders/finalize-orders.component';
import { Router } from '@angular/router';
import { PedidosService } from '../../services/pedidos.service';
import { ObservableService } from '../../services/observable.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { DateService } from '../../services/date.service';
import { FiltersService } from '../../services/filters.service';
import { MessagesService } from 'src/app/services/messages.service';
import { HttpParams } from '@angular/common/http';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { childrenOrdersMock } from 'src/mocks/pedidosListMock';

@Component({
  selector: 'app-faborders-list',
  templateUrl: './faborders-list.component.html',
  styleUrls: ['./faborders-list.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4,0.0,0.2,1)')),
    ]),
  ],
})
export class FabordersListComponent implements OnInit, OnDestroy {
  allComplete = false;
  displayedColumns: string[] = [
    'seleccion',
    'pedido',
    'orden',
    'codigoproducto',
    'descripcion',
    'cantidadplanificada',
    'lote',
    'fechaorden',
    'fechatermino',
    'piezasDisponibles',
    'ordenesHija',
    'qfbasignado',
    'estatus',
    'actions'
  ];
  dataSource = new MatTableDataSource<IOrdersReq>();
  pageEvent: PageEvent;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;
  userClasification = CONST_STRING.empty;
  queryString = CONST_STRING.empty;
  fullQueryString = CONST_STRING.empty;
  isDateInit = true;
  filterDataOrders = new ParamsPedidos();
  pageIndex = 0;
  subscriptionObservables = new Subscription();
  isSearchOrderWithFilter = false;
  isThereOrdersIsolatedToCancel = false;
  isAssignOrderIsolated = false;
  isReAssignOrderIsolated = false;
  isFinalizeOrderIsolated = false;
  isOnInit = true;
  initialSearch = true;
  expandedElement: IOrdersReq | null;
  constructor(
    private ordersService: OrdersService,
    private dataService: DataService,
    private localStorageService: LocalStorageService,
    private errorService: ErrorService,
    private titleService: Title,
    private dialog: MatDialog,
    private router: Router,
    private pedidosService: PedidosService,
    private observableService: ObservableService,
    private dateService: DateService,
    private filtersService: FiltersService,
    private messagesService: MessagesService,
  ) {
    this.observableService.setUrlActive(HttpServiceTOCall.ORDERS_ISOLATED);
  }

  ngOnInit() {
    this.userClasification = this.localStorageService.getUserClasification();
    if (this.localStorageService.getOrderIsolated()) {
      this.filterDataOrders.docNum = this.localStorageService.getOrderIsolated();
      this.queryString = `?docNum=${this.localStorageService.getOrderIsolated()}`;
      this.localStorageService.removeOrderIsolated();
    }
    if (this.localStorageService.getFiltersActivesAsModelOrders()) {
      this.onSuccessSearchOrdersModal(this.localStorageService.getFiltersActivesAsModelOrders());
    } else {
      this.createInitRageOrders();
    }
    this.titleService.setTitle('OmicronLab - Órdenes de fabricación');
    this.dataSource.paginator = this.paginator;
    this.subscriptionObservables.add(this.observableService.getNewSearchOrdersModal().subscribe(resultSearchOrdersModal => {
      if (!resultSearchOrdersModal.isFromOrders) {
        this.onSuccessSearchOrdersModal(resultSearchOrdersModal);
      }
    }));
    this.subscriptionObservables.add(this.observableService.getCallHttpService().subscribe(detailHttpCall => {
      if (detailHttpCall === HttpServiceTOCall.ORDERS_ISOLATED) {
        this.getOrdersAction();
      }
    }));
    this.localStorageService.removeFiltersActiveOrders();
  }
  createInitRageOrders() {
    this.pedidosService.getInitRangeDate().subscribe(({ response }) => this.getInitRange(response.filter(
      catalog => catalog.field === 'MagistralesDaysToLook')[0].value), error => this.errorService.httpError(error));
  }
  getInitRange(daysInitRange: string) {
    this.filterDataOrders.isFromOrders = false;
    this.filterDataOrders.dateType = ConstOrders.defaultDateInit;
    this.filterDataOrders.dateFull = this.dateService.getDateFormatted(new Date(), new Date(), false, false, Number(daysInitRange));
    this.queryString = `?fini=${this.filterDataOrders.dateFull}`;  // init search
    this.getFullQueryString();
    this.getOrdersAction();
  }
  updateAllComplete(event: boolean) {
    const allChildrenChecked = this.getIfAllChildrenOrdersIsChecked();
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked) && allChildrenChecked;
    this.showOnSplitProcessMessage(event);
    this.getButtonsOrdersIsolatedToUnLooked();
  }

  getIfAllChildrenOrdersIsChecked(): boolean {
    const allChlidrenOrdersChecked = this.dataSource.data
      .every(parentOrder => parentOrder.childOrdersDetail.every(childOrder => childOrder.isChecked));
    return allChlidrenOrdersChecked;
  }

  someComplete(): boolean {
    const someChildrenChecked = this.someChildrenOrderIsChecked();
    return (this.dataSource.data.filter(t => t.isChecked).length > 0 || someChildrenChecked) && !this.allComplete;
  }

  someChildrenOrderIsChecked(): boolean {
    const someChlidrenOrdersChecked = this.dataSource.data
      .some(parentOrder => parentOrder.childOrdersDetail.some(childOrder => childOrder.isChecked));
    return someChlidrenOrdersChecked;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (this.dataSource.data == null) {
      return;
    }
    this.dataSource.data.forEach(t => t.isChecked = completed);
    this.showOnSplitProcessMessage(completed);
    this.getButtonsOrdersIsolatedToUnLooked();
  }

  getOrdersAction() {
    this.ordersService.getOrders(this.fullQueryString).subscribe(
      ordersRes => {
        this.lengthPaginator = ordersRes.comments;
        this.dataSource.data = ordersRes.response;
        this.dataSource.data.forEach((element, i) => {
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
            case ConstStatus.cancelado:
              element.class = 'cancelado';
              break;
            case ConstStatus.entregado:
            case ConstStatus.almacenado:
              element.class = ConstStatus.almacenado.toLowerCase();
              break;
          }
          element.description = element.description.toUpperCase();
          element.childOrdersDetail = [];
          element.style = this.dataService.calculateTernary(i % 2 === 0, '#f1f2f3', '#fff');
        });
        this.isThereOrdersIsolatedToCancel = false;
        this.isAssignOrderIsolated = false;
        this.isReAssignOrderIsolated = false;
        this.isFinalizeOrderIsolated = false;
        this.allComplete = false;
        this.isOnInit = false;
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
    const extraParam = this.initialSearch ? '&parent=true' : '';
    // tslint:disable-next-line:max-line-length
    this.fullQueryString = `${this.queryString}&offset=${this.offset}&limit=${this.limit}&classifications=${this.userClasification}${extraParam}`;
  }

  getDateFormatted(initDate: Date, finishDate: Date, isBeginDate: boolean) {
    if (isBeginDate) {
      initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.thirtyDays);
    }
    return `${this.dateService.transformDate(initDate)}-${this.dateService.transformDate(finishDate)}`;
  }

  changeDataEvent(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getFullQueryString();
    this.getOrdersAction();
    return event;
  }

  createOrderIsolated() {
    this.localStorageService.setFiltersActivesOrders(JSON.stringify(this.filterDataOrders));
    this.observableService.setSearchComponentModal({ modalType: ComponentSearch.createOrderIsolated });
  }

  openSearchOrders() {
    this.observableService.setSearchOrdersModal({ modalType: ConstOrders.modalOrdersIsolated, filterOrdersData: this.filterDataOrders });

  }

  onSuccessSearchOrdersModal(resultSearchOrdersModal: ParamsPedidos) {
    this.filterDataOrders = new ParamsPedidos();
    this.filterDataOrders = this.filtersService.getNewDataToFilter(resultSearchOrdersModal)[0];
    this.queryString = this.filtersService.getNewDataToFilter(resultSearchOrdersModal)[1];
    this.initialSearch = this.validateParameters(this.queryString);
    this.isSearchOrderWithFilter = this.filtersService.getIsWithFilter(resultSearchOrdersModal);
    this.pageIndex = resultSearchOrdersModal.pageIndex || 0;
    this.offset = resultSearchOrdersModal.offset || 0;
    this.limit = resultSearchOrdersModal.limit || 10;
    this.getFullQueryString();
    this.getOrdersAction();
    this.isDateInit = resultSearchOrdersModal.dateType === ConstOrders.defaultDateInit;
  }

  validateParameters(url: string): boolean {
    const queryString = url.split('?')[1];
    if (!queryString) {
      return false;
    }

    const params = new HttpParams({ fromString: queryString });
    const permitidos = ['fini', 'ffin'];

    let valido = true;
    params.keys().forEach(key => {
      if (!permitidos.includes(key)) {
        valido = false;
      }
    });

    return valido;
  }

  getChildrenOrdersData(parentOrderID: number, indexToSetData: number) {
    this.pedidosService.getChildrenOrders(parentOrderID).subscribe(
      res => {
        this.dataSource.data[indexToSetData].childOrdersDetail = [...res.response];
      },
      error => this.errorService.httpError(error));
  }

  toggleExpand(order: IOrdersReq) {
    const parentOrderId = order.fabOrderId;
    const indice = this.dataSource.data.indexOf(order);
    if (this.dataSource.data[indice].childOrdersDetail === undefined || this.dataSource.data[indice].childOrdersDetail.length === 0) {
      this.getChildrenOrdersData(parentOrderId, indice);
    }
    this.expandedElement = this.expandedElement === order ? null : order;
  }

  cancelOrder() {
    this.observableService.setCancelOrders({
      list: this.getDataCancel(ConstStatus.finalizado),
      cancelType: MODAL_NAMES.placeOrdersDetail, isFromCancelIsolated: true
    });
  }

  getDataCancel(status: string) {
    const childrenOrders = this.getChildrenOrdersToCancel(status);
    const parentsOrders = this.dataSource.data.filter
      (t => (t.isChecked && (t.status !== status && t.status !== ConstStatus.almacenado))).map(order => {
        return this.getCancelOrderReq(order.fabOrderId);
      });
    const dataRequest = parentsOrders.concat(childrenOrders);
    return dataRequest;
  }

  getChildrenOrdersToCancel(status: string) {
    const orders = this.getChildrenOrdersCheckedIdsToCancel(status);
    return orders.map(order => this.getCancelOrderReq(order));
  }

  getCancelOrderReq(ordenFabricacionId: number): CancelOrderReq {
    const cancelOrder = new CancelOrderReq();
    cancelOrder.orderId = ordenFabricacionId;
    return cancelOrder;
  }

  getChildrenOrdersCheckedIdsToCancel(status: string) {
    const childrenChecked = this.dataSource.data.map(parentOrder =>
      parentOrder.childOrdersDetail.filter(childOrder =>
        this.dataService.calculateAndValueList([
          childOrder.isChecked,
          childOrder.status !== status,
          childOrder.status !== ConstStatus.almacenado
        ])
      ).map(order => order.fabOrderId))
      .reduce((acc, ids) => acc.concat(ids), []);
    return childrenChecked;
  }

  assignOrderIsolated() {
    this.observableService.setQbfToPlace({
      modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromOrdersIsolated)
      , isFromOrderIsolated: true
    });
  }
  private getButtonsOrdersIsolatedToUnLooked() {
    this.isFinalizeOrderIsolated = this.filtersService.
      getIsThereOnData(this.dataSource.data, ConstStatus.terminado, FromToFilter.fromDefault);
    this.isThereOrdersIsolatedToCancel = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.finalizado,
      FromToFilter.fromOrdersIsolatedCancel);
    this.isAssignOrderIsolated = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.planificado,
      FromToFilter.fromDefault);
    this.isReAssignOrderIsolated = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.reasingado,
      FromToFilter.fromOrderIsolatedReassign);
  }

  showOnSplitProcessMessage(check: boolean) {
    const orders = this.dataSource.data.filter(order =>
      this.dataService.calculateAndValueList([
        order.isChecked,
        order.onSplitProcess
      ])).map(t => t.fabOrderId);
    const someOnSplitProcess = orders.length > 0;
    const showMessage = this.dataService.calculateAndValueList([someOnSplitProcess, check]);
    if (showMessage) {
      const mssg = `No es posible modificar el estatus de órdenes en proceso de división: ${orders.join(', ')}.`;
      this.messagesService.presentToastCustom('', 'error', mssg, false, false);
    }
  }

  reAssignOrder() {
    const parentOrdersReasign = this.filtersService.getItemOnDateWithFilter(this.dataSource.data,
      FromToFilter.fromOrderIsolatedReassignItems).map(order => Number(order.ordenFabricacionId));

    const childrenOrders = this.getChildrenOrdersChecked();
    const childrenOrderToReasign = this.filtersService.getItemOnDateWithFilter(childrenOrders,
      FromToFilter.fromOrderIsolatedReassignItems).map(order => Number(order.ordenFabricacionId));

    const dataRequest = parentOrdersReasign.concat(childrenOrderToReasign);
    this.observableService.setQbfToPlace({
      modalType: MODAL_NAMES.placeOrdersDetail,
      list: dataRequest
      , isFromOrderIsolated: true, isFromReassign: true
    });
  }
  ngOnDestroy() {
    this.subscriptionObservables.unsubscribe();
  }

  finalizeOrder() {
    const finalizeParentOrders = this.filtersService.
      getItemOnDateWithFilter(this.dataSource.data, FromToFilter.fromDefault, ConstStatus.terminado);

    const childrenOrdersChecked = this.getChildrenOrdersChecked();
    const finalizeChildrenOrders = this.filtersService.
      getItemOnDateWithFilter(childrenOrdersChecked, FromToFilter.fromDefault, ConstStatus.terminado);
    const finalizeOrderData = finalizeParentOrders.concat(finalizeChildrenOrders);
    console.log(finalizeOrderData);
    this.dialog.open(FinalizeOrdersComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        finalizeOrdersData: finalizeOrderData
      }
    }).afterClosed().subscribe(() => this.getOrdersAction());
  }

  getChildrenOrdersChecked() {
    const childrenChecked = this.dataSource.data
      .map(parentOrder => parentOrder.childOrdersDetail.filter(childOrder => childOrder.isChecked))
      .reduce((acc, children) => acc.concat(children), []);
    return childrenChecked;
  }

  materialRequestIsolatedOrder() {
    this.filterDataOrders.offset = this.offset;
    this.filterDataOrders.limit = this.limit;
    this.filterDataOrders.pageIndex = this.pageIndex;
    this.localStorageService.setFiltersActivesOrders(JSON.stringify(this.filterDataOrders));
    this.router.navigate([RouterPaths.materialRequest,
    this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromOrdersIsolated).toString() || CONST_NUMBER.zero
      , CONST_NUMBER.zero]);
  }

  goToFormulaDetail(fabOrderId: string) {
    this.filterDataOrders.offset = this.offset;
    this.filterDataOrders.limit = this.limit;
    this.filterDataOrders.pageIndex = this.pageIndex;
    this.localStorageService.setFiltersActivesOrders(JSON.stringify(this.filterDataOrders));
    this.dataService.changeRouterForFormula(fabOrderId,
      this.dataSource.data.map(order => order.fabOrderId).toString(),
      CONST_NUMBER.zero);
  }
}

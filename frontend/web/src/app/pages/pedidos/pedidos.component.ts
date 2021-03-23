import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import {DataService} from '../../services/data.service';
import {
    ClassCssOrderType,
    ClassNames,
    CONST_NUMBER,
    CONST_STRING,
    ConstOrders,
    ConstStatus,
    FromToFilter,
    HttpServiceTOCall,
    HttpStatus,
    MessageType,
    MODAL_NAMES, OrderType,
    RouterPaths,
    TypeToSeeTap,
} from '../../constants/const';
import {Messages} from '../../constants/messages';
import {ErrorService} from '../../services/error.service';
import {
    CancelOrderReq,
    ICreatePdfOrdersRes,
    IPedidoReq,
    IRecipesRes,
    ParamsPedidos,
    ProcessOrders
} from '../../model/http/pedidos';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatDialog} from '@angular/material/dialog';
import {Subscription} from 'rxjs';
import {Title} from '@angular/platform-browser';
import {ErrorHttpInterface} from '../../model/http/commons';
import {Router} from '@angular/router';
import {IOrdersRefuseReq, ReasonRefuse} from '../../model/http/detallepedidos.model';
import {CommentsConfig} from '../../model/device/incidents.model';

@Component({
  selector: 'app-pedidos',
  templateUrl: './pedidos.component.html',
  styleUrls: ['./pedidos.component.scss']
})
export class PedidosComponent implements OnInit, OnDestroy {
  allComplete = false;
  ordersToProcess = new ProcessOrders();
  // tslint:disable-next-line:max-line-length
  displayedColumns: string[] = ['seleccion', 'codigo', 'cliente', 'medico', 'asesor', 'orderType', 'f_inicio', 'f_fin', 'qfb_asignado', 'status', 'actions'];
  dataSource = new MatTableDataSource<IPedidoReq>();
  pageEvent: PageEvent;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;
  queryString = CONST_STRING.empty;
  fullQueryString = CONST_STRING.empty;
  isDateInit =  true;
  isSearchWithFilter = false;
  filterDataOrders = new ParamsPedidos();
  isThereOrdersToPlan = false;
  isCheckedOrders = false;
  isThereOrdersToPlace = false;
  subscriptionCallHttp = new Subscription();
  isThereOrdersToCancel = false;
  isThereOrdersToFinalize = false;
  isThereOrdersToReassign = false;
  pageIndex = CONST_NUMBER.zero;
  isThereOrdersToRequest = false;
  isOnInit = true;
  isTherePedidosToViewPdf = false;
  constructor(
    private pedidosService: PedidosService,
    public dataService: DataService,
    private errorService: ErrorService,
    private dialog: MatDialog,
    private titleService: Title,
    private router: Router,
  ) {
    this.dataService.setUrlActive(HttpServiceTOCall.ORDERS);

  }

  ngOnInit() {
    this.titleService.setTitle('OmicronLab - Pedidos');
    this.dataSource.paginator = this.paginator;
    if (this.dataService.getFiltersActives()) {
          this.onSuccessSearchOrderModal(this.dataService.getFiltersActivesAsModel());
    } else {
          this.createInitRage();
    }
    this.subscriptionCallHttp.add(this.dataService.getCallHttpService().subscribe(callHttpService => {
      if (callHttpService === HttpServiceTOCall.ORDERS) {
        this.getPedidos();
      }
    }));
    this.subscriptionCallHttp.add(this.dataService.getNewSearchOrdersModal().subscribe(resultSearchOrderModal => {
      if (resultSearchOrderModal.isFromOrders) {
        this.onSuccessSearchOrderModal(resultSearchOrderModal);
      }
    }));
    this.subscriptionCallHttp.add(this.dataService.getNewCommentsResult().subscribe(newCommentsResult =>
        this.successNewComments(newCommentsResult)));
    this.dataService.removeFiltersActive();
  }
  createInitRage() {

    this.pedidosService.getInitRangeDate().subscribe(({response}) =>
            this.getInitRange(response.filter(catalog => catalog.field === 'MagistralesDaysToLook')[0].value),
            error => this.errorService.httpError(error));
  }
  getInitRange(rangeDateResult: string ) {
      this.filterDataOrders = new ParamsPedidos();
      this.filterDataOrders.isFromOrders = true;
      this.filterDataOrders.dateType = ConstOrders.defaultDateInit;
      this.filterDataOrders.dateFull = this.dataService.getDateFormatted(new Date(), new Date(), false, false, Number(rangeDateResult));
      this.queryString = `?fini=${this.filterDataOrders.dateFull}`;
      this.getFullQueryString();
      this.getPedidos();
  }
  getPedidos() {
    this.pedidosService.getPedidos(this.fullQueryString).subscribe(
      pedidoRes => {
        this.lengthPaginator = pedidoRes.comments;
        this.dataSource.data = pedidoRes.response;
        this.dataSource.data.forEach(element => {
              switch (element.pedidoStatus) {
                  case ConstStatus.abierto:
                      element.class = 'abierto';
                      break;
                  case ConstStatus.planificado:
                      element.class = 'planificado';
                      break;
                  case ConstStatus.liberado:
                      element.class = 'liberado';
                      break;
                  case ConstStatus.cancelado:
                      element.class = 'cancelado';
                      break;
                  case ConstStatus.enProceso:
                      element.class = 'proceso';
                      break;
                  case ConstStatus.finalizado:
                      element.class = 'finalizado';
                      break;
                  case ConstStatus.terminado:
                      element.class = 'terminado';
                      break;
                  case ConstStatus.rechazado:
                      element.class = 'rechazado';
                      break;
                  case ConstStatus.entregado:
                  case ConstStatus.almacenado:
                      element.class = ConstStatus.almacenado.toLowerCase();
                      break;
              }
              element.classClasification = this.getClassClasification(element.orderType);
          });
        this.isTherePedidosToViewPdf = false;
        this.isCheckedOrders = false;
        this.isThereOrdersToPlan = false;
        this.isThereOrdersToPlace = false;
        this.isThereOrdersToCancel = false;
        this.isThereOrdersToFinalize = false;
        this.isThereOrdersToReassign = false;
        this.isThereOrdersToRequest = false;
        this.allComplete = false;
        this.isOnInit = false;
      },
        (error: ErrorHttpInterface) => {
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

  processOrdersService() {
    this.dataService.presentToastCustom(Messages.processOrders, 'warning', CONST_STRING.empty, true, true)
    .then((result: any) => {
      if (result.isConfirmed) {
        this.ordersToProcess.listIds = this.getOrdersOnlyOpen();
        this.ordersToProcess.user = this.dataService.getUserId();
        this.pedidosService.processOrders(this.ordersToProcess).subscribe(
          resProcessOrder => {
            if (resProcessOrder.success && resProcessOrder.response.length > 0) {
              const titleProcessWithError = this.dataService.getMessageTitle(resProcessOrder.response, MessageType.processOrder);
              this.getPedidos();
              this.dataService.presentToastCustom(titleProcessWithError, 'error',
                  Messages.errorToAssignOrderAutomaticSubtitle, true, false, ClassNames.popupCustom);
            } else {
              this.showMessagesAndRefresh();
            }
            this.dataService.setIsLoading(false);
          },
          error => {
            this.errorService.httpError(error);
          }
        );
      }
    });
  }
  showMessagesAndRefresh() {
      this.getPedidos();
      this.dataService.setMessageGeneralCallHttp({title: Messages.success , icon: 'success', isButtonAccept: false});
  }
  changeDataEvent(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getFullQueryString();
    this.getPedidos();
    return event;
  }
  openFindOrdersDialog() {
    this.dataService.setSearchOrdersModal({modalType: ConstOrders.modalOrders, filterOrdersData: this.filterDataOrders });
 }

  openPlaceOrdersDialog() {
    this.dataService.setQbfToPlace(
        {
          modalType: MODAL_NAMES.placeOrders,
          list: this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromOrders)
        });
  }
  getButtonsToUnLooked() {
    this.isCheckedOrders = this.dataSource.data.filter( order => order.isChecked).length > CONST_NUMBER.zero;
    this.isThereOrdersToCancel = this.dataService.getIsThereOnData(this.dataSource.data,
        ConstStatus.finalizado, FromToFilter.fromOrdersCancel);
    this.isThereOrdersToFinalize = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.terminado, FromToFilter.fromOrders);
    this.isThereOrdersToPlan = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.abierto, FromToFilter.fromOrders);
    this.isThereOrdersToPlace = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.planificado, FromToFilter.fromOrders);
    this.isThereOrdersToReassign =
        this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.liberado, FromToFilter.fromOrdersReassign);
    this.isTherePedidosToViewPdf = this.dataSource.data.filter( order => order.isChecked).length > CONST_NUMBER.zero;

  }
  getFullQueryString() {
    this.fullQueryString = `${this.queryString}&offset=${this.offset}&limit=${this.limit}`;
  }

  ngOnDestroy() {
    this.subscriptionCallHttp.unsubscribe();
  }

  cancelOrders() {
    this.dataService.setCancelOrders({list: this.dataSource.data.filter
      (t => (t.isChecked && t.pedidoStatus !== ConstStatus.finalizado && t.pedidoStatus !== ConstStatus.almacenado)).map(order => {
        const cancelOrder = new CancelOrderReq();
        cancelOrder.orderId = order.docNum;
        return cancelOrder;
      }),
        cancelType: MODAL_NAMES.placeOrders});
  }

  finalizeOrders() {
    this.dataService.setFinalizeOrders({list: this.dataSource.data.filter
      (t => (t.isChecked && t.pedidoStatus === ConstStatus.terminado)).map(order => {
        const finalizeOrder = new CancelOrderReq();
        finalizeOrder.orderId = order.docNum;
        return finalizeOrder;
      }), cancelType: MODAL_NAMES.placeOrders});
  }

  private onSuccessSearchOrderModal(resultSearchOrderModal: ParamsPedidos) {
    this.isDateInit = resultSearchOrderModal.dateType === ConstOrders.defaultDateInit;
    this.pageIndex = 0;
    this.offset = 0;
    this.limit = 10;
    this.filterDataOrders = new ParamsPedidos();
    this.filterDataOrders = this.dataService.getNewDataToFilter(resultSearchOrderModal)[0];
    this.queryString = this.dataService.getNewDataToFilter(resultSearchOrderModal)[1];
    this.isSearchWithFilter = this.dataService.getIsWithFilter(resultSearchOrderModal);
    this.getFullQueryString();
    this.getPedidos();
  }

  reassignOrders() {
    this.dataService.setQbfToPlace({modalType: MODAL_NAMES.placeOrders,
      list: this.dataService.getItemOnDateWithFilter(this.dataSource.data,
          FromToFilter.fromOrdersReassign, ConstStatus.liberado).map(order => order.docNum)
      , isFromReassign: true});
  }

  toSeeRecipes(docNum: number) {
    this.pedidosService.getRecipesByOrder(docNum).subscribe(recipeByOrderRes => {
          this.onSuccessHttpGetRecipes(recipeByOrderRes);
          this.dataService.setIsLoading(false);
        }
    , error => this.errorService.httpError(error));

  }

  onSuccessHttpGetRecipes(resultGetRecipes: IRecipesRes) {
    if (resultGetRecipes.response.length === CONST_NUMBER.zero) {
      this.dataService.setMessageGeneralCallHttp({title: Messages.noHasRecipes, icon: 'info', isButtonAccept: true});
    } else {
      resultGetRecipes.response.forEach(urlPdf => this.dataService.openNewTapByUrl(urlPdf.recipe, TypeToSeeTap.receipt, urlPdf.order));
    }
  }

  requestMaterial() {
        this.dataService.setFiltersActives(JSON.stringify(this.filterDataOrders));
        this.router.navigate([RouterPaths.materialRequest,
            this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromOrders).toString() || CONST_NUMBER.zero,
            CONST_NUMBER.one]);
    }

  printOrderAsPdfFile() {
    if (this.isCheckedOrders) {
      this.dataService.presentToastCustom(Messages.confirmCreateOrderPdf, 'question', '', true, true)
      .then( (res: any) => {
        if (res.isConfirmed) {
          this.printOrderAsPdfFileConfirmedAction();
        }
      });
    }
  }

  printOrderAsPdfFileConfirmedAction() {
    let documentNumbers = this.dataSource.data.filter(t => (t.isChecked && t.pedidoStatus !== ConstStatus.cancelado)).map(i => { return i.docNum });
    this.pedidosService.createPdfOrders(documentNumbers)
    .subscribe((response : ICreatePdfOrdersRes) => {
      if (response.userError) {
        let errorNumbers = response.response.filter(x => !isNaN(x as any));
        let formatedNumbers = errorNumbers.join(', ');
        let message = '';
        if (response.response.length > 1) {
          message = `${Messages.errorMessageCreateOrdersPdf}${formatedNumbers}`;
        }
        else {
          message = `${Messages.errorMessageCreateOrderPdf}${formatedNumbers}`;
        }
        this.dataService.presentToastCustom(Messages.errorTitleCreateOrderPdf, 'error', message, true, false, ClassNames.popupCustom);
      } else {
        this.dataService.presentToastCustom(Messages.successTitleCreateOrderPdf, 'success', null, true, false);
      }
      this.getPedidos();
    },
    (error: ErrorHttpInterface) => {
      if (error.status !== HttpStatus.notFound) {
        this.errorService.httpError(error);
      }
      this.getPedidos();
    });
  }

  openNewTabByOrder(order: number) {
      this.dataService.setFiltersActives(JSON.stringify(this.filterDataOrders));
      this.router.navigate([RouterPaths.orderDetail, order]);
  }
    viewPedidosWithPdf() {
        this.pedidosService.getOrdersPdfViews(this.dataSource.data.filter(order => order.isChecked).map( order => order.docNum))
            .subscribe( viewPdfResult => {
                viewPdfResult.response.forEach( pdfUrl => {
                    this.dataService.openNewTapByUrl(
                        pdfUrl, TypeToSeeTap.order,
                        Number(pdfUrl.split('/').slice(-1)[0].split('.')[0].slice(5, 10) // to get number Order from link
                        ));
                });
                }
                , error => this.errorService.httpError(error));
    }
    getOrdersOnlyOpen() {
        return this.dataSource.data.filter(t =>
            (t.isChecked && t.pedidoStatus === ConstStatus.abierto)).map(t => t.docNum);
    }

    ordersToRefuse() {

        this.dataService.presentToastCustom(Messages.refuseOrders, 'warning', CONST_STRING.empty, true, true)
            .then((result: any) => {
                if (result.isConfirmed) {
                     this.showCommentsToRefuse();
                }
            });
    }

    showCommentsToRefuse() {
        this.dataService.setOpenCommentsDialog({comments: CONST_STRING.empty, isForRefuseOrders: true});
    }
    successNewComments(newCommentsResult: CommentsConfig) {
        const ordersToRefuseReq = new IOrdersRefuseReq();
        ordersToRefuseReq.comments = newCommentsResult.comments;
        ordersToRefuseReq.userId = this.dataService.getUserId();
        ordersToRefuseReq.ordersId  = this.getOrdersOnlyOpen();
        this.pedidosService.putRefuseOrders(ordersToRefuseReq).subscribe(({response}) =>
            this.successRefuseResult(response.failed), error => this.errorService.httpError(error));
    }
    successRefuseResult(failed: ReasonRefuse[]) {
        if (failed.length === CONST_NUMBER.zero) {
            this.showMessagesAndRefresh();
            return;
        }
        this.dataService.presentToastCustom(this.dataService.getMessageTitle(failed, MessageType.default, true)
                , 'info', CONST_STRING.empty, true, false, ClassNames.popupCustom);
        this.getPedidos();
    }


    getClassClasification(orderType: string) {
        switch (orderType) {
            case OrderType.bioElite:
                return ClassCssOrderType.mn;
            case OrderType.bioEqual:
                return  ClassCssOrderType.be;
            case OrderType.magistral:
                return ClassCssOrderType.mg;
            case OrderType.mixto:
                return ClassCssOrderType.mx;
        }
    }


}

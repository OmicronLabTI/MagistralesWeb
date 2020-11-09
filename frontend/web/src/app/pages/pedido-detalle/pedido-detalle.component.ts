import {Component, OnDestroy, OnInit} from '@angular/core';
import {MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import {IPedidoDetalleLabelReq, IPedidoDetalleReq, LabelToFinish} from '../../model/http/detallepedidos.model';
import {ActivatedRoute, Router} from '@angular/router';
import {DataService} from '../../services/data.service';
import {
  ClassNames,
  CONST_NUMBER,
  CONST_STRING,
  ConstStatus,
  FromToFilter,
  HttpServiceTOCall,
  MessageType,
  MODAL_NAMES,
  RolesType,
  RouterPaths,
  TypeToSeeTap
} from '../../constants/const';
import {Subscription} from 'rxjs';
import {Title} from '@angular/platform-browser';
import {CancelOrderReq, OrderToDelivered, ProcessOrdersDetailReq} from '../../model/http/pedidos';
import {Messages} from '../../constants/messages';
import {ErrorService} from '../../services/error.service';
import {MatDialog} from '@angular/material/dialog';
import {AddCommentsDialogComponent} from '../../dialogs/add-comments-dialog/add-comments-dialog.component';

@Component({
  selector: 'app-pedido-detalle',
  templateUrl: './pedido-detalle.component.html',
  styleUrls: ['./pedido-detalle.component.scss']
})
export class PedidoDetalleComponent implements OnInit, OnDestroy {
  allComplete = false;
  docNum: string;
  docStatus: string;
  displayedColumns: string[] = [
    'seleccion',
    'ordenFabricacionId',
    'codigoProducto',
    'descripcionProducto',
    'qtyPlanned',
    'fechaOF',
    'fechaOFFin',
    'qfb',
    'label',
    'statusOF',
    'actions'
  ];
  dataSource = new MatTableDataSource<IPedidoDetalleReq>();
  isThereOrdersDetailToPlan = false;
  isThereOrdersDetailToPlace = false;
  subscriptionCallHttpDetail = new Subscription();
  detailsOrderToProcess = new ProcessOrdersDetailReq();
  isThereOrdersDetailToCancel = false;
  isThereOrdersDetailToFinalize = false;
  isThereOrdersDetailToReassign = false;
  isOnInit = true;
  isThereOrdersDetailToDelivered = false;
  isThereOrdersToFinishLabel = false;
  signatureData = CONST_STRING.empty;
  isThereOrdersToViewPdf = false;
  isCorrectToAddComments = false;
  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              private dataService: DataService,
              private titleService: Title, private errorService: ErrorService,
              private router: Router, private dialog: MatDialog) {
    this.dataService.setUrlActive(HttpServiceTOCall.DETAIL_ORDERS);
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.docNum = params.get('id');
      this.titleService.setTitle('Pedido ' + this.docNum);
    });
    this.getDetallePedido();
    this.subscriptionCallHttpDetail.add(this.dataService.getCallHttpService().subscribe(detailHttpCall => {
      if (detailHttpCall === HttpServiceTOCall.DETAIL_ORDERS) {
        this.getDetallePedido();
      }
    }));
    this.subscriptionCallHttpDetail.add(this.dataService.getNewDataSignature().subscribe( newDataSignature => {
      this.signatureData = newDataSignature;
      this.sendToLabelsFinish();
    }));
  }

  getDetallePedido() {
    this.pedidosService.getDetallePedido(this.docNum).subscribe(
      (pedidoDetalleRes) => {
        this.dataSource.data = pedidoDetalleRes.response;
        this.dataSource.data.forEach(element => {
          this.docStatus = element.pedidoStatus;
          element.fechaOf = element.fechaOf == null ? '' : element.fechaOf.substring(10, 0);
          element.fechaOfFin = element.fechaOfFin == null ? '' : element.fechaOfFin.substring(10, 0);
          element.status = element.status === '' ? ConstStatus.abierto : element.status;
          switch (element.status.toUpperCase()) {
            case ConstStatus.abierto.toUpperCase():
              element.class = 'abierto';
              break;
            case ConstStatus.planificado.toUpperCase():
              element.class = 'planificado';
              break;
            case ConstStatus.asignado.toUpperCase():
              element.class = 'asignado';
              break;
            case ConstStatus.pendiente.toUpperCase():
              element.class = 'pendiente';
              break;
            case ConstStatus.terminado.toUpperCase():
              element.class = 'terminado';
              break;
            case ConstStatus.enProceso.toUpperCase():
              element.class = 'proceso';
              break;
            case ConstStatus.finalizado.toUpperCase():
              element.class = 'finalizado';
              break;
            case ConstStatus.cancelado.toUpperCase():
              element.class = 'cancelado';
              break;
            case ConstStatus.reasingado.toUpperCase():
              element.class = 'reasignado';
              break;
            case ConstStatus.entregado.toUpperCase():
              element.class = 'entregado';
              break;
          }
          element.descripcionProducto = element.descripcionProducto.toUpperCase();
        });
        this.isThereOrdersToViewPdf = false;
        this.isThereOrdersDetailToDelivered = false;
        this.isThereOrdersToFinishLabel = false;
        this.isThereOrdersDetailToPlan = false;
        this.isThereOrdersDetailToPlace = false;
        this.isThereOrdersDetailToCancel = false;
        this.isThereOrdersDetailToFinalize = false;
        this.isThereOrdersDetailToReassign = false;
        this.allComplete = false;
        this.isOnInit = false;
        this.signatureData = CONST_STRING.empty;
        this.isCorrectToAddComments = this.dataSource.data.every(order => order.status === ConstStatus.abierto
                                                                 && order.ordenFabricacionId === CONST_NUMBER.zero);
      }, error => this.errorService.httpError(error));
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

  openPlaceOrderDialog() {
    this.dataService.setQbfToPlace({modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromDetailOrder)});
  }

  getButtonsToUnLooked() {
    this.isThereOrdersDetailToDelivered = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.finalizado,
        FromToFilter.fromDefault);
    this.isThereOrdersToViewPdf = this.dataSource.data.filter(order => order.isChecked).length > CONST_NUMBER.zero;

    this.isThereOrdersToFinishLabel = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.abierto,
        FromToFilter.fromOrderDetailLabel);

    this.isThereOrdersDetailToCancel = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.finalizado,
        FromToFilter.fromDetailOrder);
    this.isThereOrdersDetailToPlace = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.planificado,
                                                                        FromToFilter.fromDefault);
    this.isThereOrdersDetailToFinalize = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.terminado,
                                                                           FromToFilter.fromDefault);
    this.isThereOrdersDetailToPlan = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.abierto, FromToFilter.fromDefault);
    this.isThereOrdersDetailToReassign = this.dataService.getIsThereOnData(this.dataSource.data, ConstStatus.reasingado,
        FromToFilter.fromOrderIsolatedReassign);
  }
  ngOnDestroy() {
    this.subscriptionCallHttpDetail.unsubscribe();
  }

  processOrdersDetail() {
    this.dataService.presentToastCustom(Messages.processOrdersDetail, 'warning', CONST_STRING.empty, true, true)
        .then((result: any) => {
          if (result.isConfirmed) {
            this.detailsOrderToProcess.pedidoId = Number(this.docNum);
            this.detailsOrderToProcess.userId = this.dataService.getUserId();
            this.detailsOrderToProcess.productId =
                this.dataSource.data.filter(t => (t.isChecked && t.status === ConstStatus.abierto)).map(detail => detail.codigoProducto);
            this.pedidosService.postPlaceOrdersDetail(this.detailsOrderToProcess).subscribe(resultProcessDetail => {
              if (resultProcessDetail.success && resultProcessDetail.response.length > 0) {
                const titleProcessDetailWithError = this.dataService.getMessageTitle(
                    resultProcessDetail.response, MessageType.processDetailOrder);
                this.getDetallePedido();
                this.dataService.presentToastCustom(titleProcessDetailWithError, 'error',
                    Messages.errorToAssignOrderAutomaticSubtitle, true, false,  ClassNames.popupCustom);
              } else {
                this.reloadOrderDetail();
              }
            }, error => this.errorService.httpError(error));
          }
        } );
  }

  cancelOrders() {
    this.dataService.setCancelOrders({list: this.getDataCancelFinalize(ConstStatus.finalizado) ,
      cancelType: MODAL_NAMES.placeOrdersDetail});
  }

  finalizeOrdersDetail() {
    this.dataService.setFinalizeOrders({list: this.getDataCancelFinalize(ConstStatus.terminado, true),
      cancelType: MODAL_NAMES.placeOrdersDetail});
  }

  reassignOrderDetail() {
    this.dataService.setQbfToPlace({modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataService.getItemOnDateWithFilter(this.dataSource.data,
          FromToFilter.fromOrderIsolatedReassignItems).map(order => Number(order.ordenFabricacionId))
      , isFromReassign: true});
  }

    goToOrders(urlPath: string[]) {
      this.dataService.setPathUrl(urlPath);
    }

    materialRequestDetail() {
        this.router.navigate([RouterPaths.materialRequest,
          this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromDetailOrder).toString(),
          CONST_NUMBER.zero.toString()]);
    }
    getDataCancelFinalize(status: string, isFromFinalize: boolean = false) {
    return this.dataSource.data.filter
      (t => (t.isChecked && (isFromFinalize ? t.status === status : t.status !== status))).map(order => {
        const cancelOrder = new CancelOrderReq();
        cancelOrder.orderId = order.ordenFabricacionId;
        return cancelOrder;
      });
    }

  addCommentsDialog() {
    if (!this.isCorrectToAddComments) {
      this.dialog.open(AddCommentsDialogComponent, {
        panelClass: 'custom-dialog-container',
        data: this.dataSource.data[0].comments
      }).afterClosed().subscribe(addCommentsResult => {
        if ( addCommentsResult) {
          this.addCommentsOnService(addCommentsResult);
        }
      });
    }
  }

  addCommentsOnService(addCommentsResult: string) {
    this.pedidosService.savedComments( Number(this.docNum), addCommentsResult).subscribe(() => {
          this.reloadOrderDetail();
        },
        error => this.errorService.httpError(error));
  }

  reloadOrderDetail() {
    this.getDetallePedido();
    this.dataService.setMessageGeneralCallHttp({title: Messages.success, icon: 'success', isButtonAccept: false });
  }

  ordersToDelivered() {
    this.dataService.presentToastCustom(Messages.deliveredOrders, 'question', CONST_STRING.empty, true, true)
        .then((result: any) => {
          if (result.isConfirmed) {
            this.pedidosService.putOrdersToDelivered(
                this.dataService.getItemOnDateWithFilter(this.dataSource.data, FromToFilter.fromDefault, ConstStatus.finalizado)
                    .map(order => {
                      const orderToDelivered = new OrderToDelivered();
                      orderToDelivered.orderId = order.ordenFabricacionId;
                      orderToDelivered.status = ConstStatus.entregado;
                      return orderToDelivered;
                    })).subscribe(() => {
                      this.reloadOrderDetail();
                }
                , error => this.errorService.httpError((error)));
          }});

  }

  finishOrdersLabels() {
    this.dataService.setOpenSignatureDialog(this.signatureData);
  }
  sendToLabelsFinish() {
           this.createConsumeService();
  }

  createConsumeService(isFromRemoveSignature: boolean = false, index?: number) {
    const labelToFinishReq = new IPedidoDetalleLabelReq();
    labelToFinishReq.details = this.getArrayToFinishLabel(isFromRemoveSignature, index);
    labelToFinishReq.designerSignature = isFromRemoveSignature ? null : this.signatureData;
    labelToFinishReq.userId = this.dataService.getUserId();
    this.pedidosService.finishLabels(labelToFinishReq).subscribe(() => {
      this.reloadOrderDetail();
    }, error => this.errorService.httpError(error));
  }

  getArrayToFinishLabel(isFromRemoveSignature: boolean, index?: number ) {
    if (!isFromRemoveSignature) {
      return this.dataSource.data.filter( order => order.isChecked && (order.status !== ConstStatus.abierto &&
          order.status !== ConstStatus.cancelado))
          .map(order => {
            const labelToFinish = new LabelToFinish();
            labelToFinish.orderId = order.ordenFabricacionId;
            labelToFinish.checked = !isFromRemoveSignature;
            return labelToFinish;
          });
    } else {
      const labelsToFinish: LabelToFinish[] = [];
      labelsToFinish.push({orderId: this.dataSource.data[index].ordenFabricacionId, checked: false});
      return labelsToFinish;
    }
  }

  removeSignature(index: number) {
    if (this.dataService.getUserRole() === RolesType.design) {
      this.dataService.presentToastCustom(`${Messages.removeLabelFinish } ${this.dataSource.data[index].label.toLowerCase() }?`,
          'question', CONST_STRING.empty, true, true)
          .then((result: any) => {
            if (result.isConfirmed) {
              this.createConsumeService(true, index);
            }
          });
    }
  }

  viewOrdersWithPdf() {
    this.pedidosService.getOrdersPdfViews([Number(this.docNum)])
        .subscribe( viewPdfResult => {
          viewPdfResult.response.forEach( pdfUrl => this.dataService.openNewTapByUrl( pdfUrl, TypeToSeeTap.order, Number(this.docNum)));
          }
            , error => this.errorService.httpError(error));
  }
}

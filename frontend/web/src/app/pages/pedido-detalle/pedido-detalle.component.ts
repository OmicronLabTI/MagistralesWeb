import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { PedidosService } from '../../services/pedidos.service';
import { IPedidoDetalleLabelReq, IPedidoDetalleReq, LabelToFinish } from '../../model/http/detallepedidos.model';
import { ActivatedRoute, Router } from '@angular/router';
import { DataService } from '../../services/data.service';
import {
  CarouselOption, CarouselOptionString,
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
import { Subscription } from 'rxjs';
import { Title } from '@angular/platform-browser';
import { CancelOrderReq, OrderToDelivered, ParamsPedidos, ProcessOrdersDetailReq } from '../../model/http/pedidos';
import { Messages } from '../../constants/messages';
import { ErrorService } from '../../services/error.service';
import { MatDialog } from '@angular/material/dialog';
import { DownloadImagesService } from '../../services/download-images.service';
import { CommentsConfig } from '../../model/device/incidents.model';

@Component({
  selector: 'app-pedido-detalle',
  templateUrl: './pedido-detalle.component.html',
  styleUrls: ['./pedido-detalle.component.scss']
})
export class PedidoDetalleComponent implements OnInit, OnDestroy {
  allComplete = false;
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
  isThereOrdersToFinishLabel = false;
  signatureData = CONST_STRING.empty;
  isThereOrdersToViewPdf = false;
  isCorrectToAddComments = false;
  ordersToSendAndDownloadQR: number[] = [];
  ordersReceivedFromRequest: number[] = [];
  paramsDetailOrder = new ParamsPedidos();
  baseQueryString = CONST_STRING.empty;
  isThereOrdersDetailToDelivered = false;
  patientName = CONST_STRING.empty;
  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              public dataService: DataService,
              private titleService: Title, private errorService: ErrorService,
              private router: Router, private dialog: MatDialog,
              private downloadImagesService: DownloadImagesService) {
    this.dataService.setUrlActive(HttpServiceTOCall.DETAIL_ORDERS);
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.validateToGetCurrentDetail(params.get('id'));
      this.titleService.setTitle('Pedido ' + params.get('id'));
    });
    this.subscriptionCallHttpDetail.add(this.dataService.getCallHttpService().subscribe(detailHttpCall => {
      if (detailHttpCall === HttpServiceTOCall.DETAIL_ORDERS) {
        this.getDetallePedido();
      }
    }));
    this.subscriptionCallHttpDetail.add(this.dataService.getNewDataSignature().subscribe(newDataSignature => {
      this.signatureData = newDataSignature;
      this.sendToLabelsFinish();
    }));
    this.subscriptionCallHttpDetail.add(this.dataService.getNewCommentsResult().subscribe(newCommentsResult =>
      this.successNewComments(newCommentsResult)));
  }

  getDetallePedido() {
    this.pedidosService.getDetallePedido(this.paramsDetailOrder.current).subscribe(
      ({ response }) => this.onSuccessDetailPedido(response), error => this.errorService.httpError(error));
  }
  onSuccessDetailPedido(response: IPedidoDetalleReq[]) {
    this.paramsDetailOrder.current = response[CONST_NUMBER.zero].pedidoId.toString();
    this.dataSource.data = response;
    this.dataSource.data.forEach(element => {
      const patientName = element.patientName !== CONST_STRING.empty && element.patientName !== undefined ?
        element.patientName.split(':')[1]
        : CONST_STRING.empty;
      this.patientName = patientName;
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
        case ConstStatus.rechazado.toUpperCase():
          element.class = 'rechazado';
          break;
        case ConstStatus.entregado.toUpperCase():
        case ConstStatus.almacenado.toUpperCase():
          element.class = ConstStatus.almacenado.toLowerCase();
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
    this.dataService.setQbfToPlace({
      modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromDetailOrder)
    });
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
          this.detailsOrderToProcess.pedidoId = Number(this.paramsDetailOrder.current);
          this.detailsOrderToProcess.userId = this.dataService.getUserId();
          this.detailsOrderToProcess.productId =
            this.dataSource.data.filter(t => (t.isChecked && t.status === ConstStatus.abierto)).map(detail => detail.codigoProducto);
          this.pedidosService.postPlaceOrdersDetail(this.detailsOrderToProcess).subscribe(resultProcessDetail => {
            if (resultProcessDetail.success && resultProcessDetail.response.length > 0) {
              const titleProcessDetailWithError = this.dataService.getMessageTitle(
                resultProcessDetail.response, MessageType.processDetailOrder);
              this.getDetallePedido();
              this.dataService.presentToastCustom(titleProcessDetailWithError, 'error',
                Messages.errorToAssignOrderAutomaticSubtitle, true, false, ClassNames.popupCustom);
            } else {
              this.reloadOrderDetail();
            }
          }, error => this.errorService.httpError(error));
        }
      });
  }

  cancelOrders() {
    this.dataService.setCancelOrders({
      list: this.getDataCancel(ConstStatus.finalizado),
      cancelType: MODAL_NAMES.placeOrdersDetail
    });
  }

  finalizeOrdersDetail() {
    this.dataService.setFinalizeOrders({
      list: this.getDataCancelFinalize(ConstStatus.terminado, true),
      cancelType: MODAL_NAMES.placeOrdersDetail
    });
  }

  reassignOrderDetail() {
    this.dataService.setQbfToPlace({
      modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataService.getItemOnDateWithFilter(this.dataSource.data,
        FromToFilter.fromOrderIsolatedReassignItems).map(order => Number(order.ordenFabricacionId))
      , isFromReassign: true
    });
  }

  goToOrders(urlPath: string[]) {
    this.dataService.setPathUrl(urlPath);
  }

  materialRequestDetail() {
    this.dataService.setCurrentDetailOrder(this.paramsDetailOrder.current);
    this.router.navigate([RouterPaths.materialRequest,
    this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromDetailOrder).toString() || CONST_NUMBER.zero,
    CONST_NUMBER.zero]);
  }
  getDataCancel(status: string) {
    return this.dataSource.data.filter
      (t => (t.isChecked && (t.status !== status && t.status !== ConstStatus.almacenado))).map(order => {
        const cancelOrder = new CancelOrderReq();
        cancelOrder.orderId = order.ordenFabricacionId;
        return cancelOrder;
      });
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
      this.dataService.setOpenCommentsDialog({ comments: this.dataSource.data[0].comments });
    }
  }

  addCommentsOnService(addCommentsResult: string) {
    this.pedidosService.savedComments(Number(this.paramsDetailOrder.current), addCommentsResult).subscribe(() => {
      this.reloadOrderDetail();
    },
      error => this.errorService.httpError(error));
  }

  reloadOrderDetail() {
    this.getDetallePedido();
    this.dataService.setMessageGeneralCallHttp({ title: Messages.success, icon: 'success', isButtonAccept: false });
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

  getArrayToFinishLabel(isFromRemoveSignature: boolean, index?: number) {
    if (!isFromRemoveSignature) {
      return this.dataSource.data.filter(order => order.isChecked && (order.status !== ConstStatus.abierto &&
        order.status !== ConstStatus.cancelado))
        .map(order => {
          const labelToFinish = new LabelToFinish();
          labelToFinish.orderId = order.ordenFabricacionId;
          labelToFinish.checked = false;
          return labelToFinish;
        });
    } else {
      const labelsToFinish: LabelToFinish[] = [];
      labelsToFinish.push({ orderId: this.dataSource.data[index].ordenFabricacionId, checked: false });
      return labelsToFinish;
    }
  }

  removeSignature(index: number) {
    if (this.dataService.getUserRole() === RolesType.design) {
      this.dataService.presentToastCustom(`${Messages.removeLabelFinish} ${this.dataSource.data[index].label.toLowerCase()}?`,
        'question', CONST_STRING.empty, true, true)
        .then((result: any) => {
          if (result.isConfirmed) {
            this.createConsumeService(true, index);
          }
        });
    }
  }

  viewOrdersWithPdf() {
    this.pedidosService.getOrdersPdfViews([Number(this.paramsDetailOrder.current)])
      .subscribe(viewPdfResult => {
        viewPdfResult.response.forEach(pdfUrl =>
          this.dataService.openNewTapByUrl(pdfUrl, TypeToSeeTap.order, Number(this.paramsDetailOrder.current)));
      }
        , error => this.errorService.httpError(error));
  }

  ordersToDownloadQr() {
    this.ordersReceivedFromRequest = [];
    this.ordersToSendAndDownloadQR = this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromDetailOrderQr);
    this.pedidosService.qrByEachOrder(this.ordersToSendAndDownloadQR)
      .subscribe(({ response }) => this.downloadQrByUrl(response), error => this.errorService.httpError(error));

  }
  downloadQrByUrl(urlsOfQrs: string[]) {
    urlsOfQrs.forEach(urlOfQr => {
      this.addToOrderReceived(urlOfQr);
      this.downloadImagesService.downloadImageFromUrl(urlOfQr, urlOfQr.split('/').slice(-1)[0]);
    });
    this.checkIfThereOrdersWithoutQr();
  }
  addToOrderReceived(urlOfQr: string) {
    this.ordersReceivedFromRequest.push(Number(urlOfQr.split('/').slice(-1)[0].split('.')[0].slice(0, -2)));
  }
  checkIfThereOrdersWithoutQr() {
    const ordersWithoutQr: string[] = [];
    this.ordersToSendAndDownloadQR.forEach(orderSend => {
      if (!this.ordersReceivedFromRequest.includes(orderSend)) {
        ordersWithoutQr.push(String(orderSend));
      }
    });
    if (ordersWithoutQr.length > CONST_NUMBER.zero) {
      this.createMessageWithOrdersWithoutQr(ordersWithoutQr);
    }
  }
  createMessageWithOrdersWithoutQr(ordersWithoutQr: string[]) {
    this.dataService.presentToastCustom(
      this.dataService.getMessageTitle(ordersWithoutQr, MessageType.ordersWithoutQr, false), 'error',
      CONST_STRING.empty, true, false, ClassNames.popupCustom);
  }

  successNewComments(newCommentsResult: CommentsConfig) {
    this.addCommentsOnService(newCommentsResult.comments);
  }
  generateParamsToGetDetail(order: string) {
    this.paramsDetailOrder = JSON.parse(this.dataService.getFiltersActives());
    this.paramsDetailOrder = { ...this.paramsDetailOrder, current: order };
    this.baseQueryString = this.dataService.getNewDataToFilter(this.paramsDetailOrder)[1];
    this.getDetallePedido();

  }

  carouselDetail(typeCarousel: number) {
    switch (typeCarousel) {
      case CarouselOption.backDetail:
        this.generateFullQueryString(CarouselOptionString.backDetail);
        break;
      case CarouselOption.nextDetail:
        this.generateFullQueryString(CarouselOptionString.nextDetail);
        break;
    }
  }
  generateFullQueryString(optionCarouselDetail: string) {
    this.carouselDetailService(this.dataService.getFullStringForCarousel(
      this.baseQueryString, this.paramsDetailOrder.current, optionCarouselDetail));
  }
  carouselDetailService(queryStringFull: string) {
    this.pedidosService.getDetailCarousel(queryStringFull).subscribe(({ response }) =>
      this.onSuccessDetailPedido(response), error => this.errorService.httpError(error));
  }

  goToDetailFormula(ordenFabricacionId: string) {
    this.dataService.changeRouterForFormula(ordenFabricacionId,
      this.dataSource.data.map(detail => detail.ordenFabricacionId).toString(),
      CONST_NUMBER.one);
  }

  validateToGetCurrentDetail(paramsOrder: string) {
    if (this.dataService.getCurrentDetailOrder()) {
      this.generateParamsToGetDetail(this.dataService.getCurrentDetailOrder());
      this.dataService.removeCurrentDetailOrder();
    } else {
      this.generateParamsToGetDetail(paramsOrder);
    }
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
        }
      });

  }
}


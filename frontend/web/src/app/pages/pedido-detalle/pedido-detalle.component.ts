import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { PedidosService } from '../../services/pedidos.service';
import { ChildrenOrders, IPedidoDetalleLabelReq, IPedidoDetalleReq, LabelToFinish } from '../../model/http/detallepedidos.model';
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
  TypeToSeeTap,
  constRealLabel,
  CatalogTypes,
  orderRelationTypes
} from '../../constants/const';
import { Subscription } from 'rxjs';
import { Title } from '@angular/platform-browser';
import {
  CancelOrderReq, Catalogs,
  CreatePdfOrder, OrderToDelivered, ParamsPedidos, ProcessOrdersDetailReq
} from '../../model/http/pedidos';
import { Messages } from '../../constants/messages';
import { ErrorService } from '../../services/error.service';
import { DownloadImagesService } from '../../services/download-images.service';
import { CommentsConfig } from '../../model/device/incidents.model';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ObservableService } from '../../services/observable.service';
import { MessagesService } from 'src/app/services/messages.service';
import { FiltersService } from '../../services/filters.service';
import { animate, state, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-pedido-detalle',
  templateUrl: './pedido-detalle.component.html',
  styleUrls: ['./pedido-detalle.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4,0.0,0.2,1)')),
    ]),
  ],
})
export class PedidoDetalleComponent implements OnInit, OnDestroy {
  allComplete = false;
  docStatus: string;
  ProductNoLabel: Catalogs;
  productCodeSplit = [];
  realLabel: string;
  displayedColumns: string[] = [
    'seleccion',
    'ordenFabricacionId',
    'codigoProducto',
    'descripcionProducto',
    'qtyPlanned',
    'fechaOF',
    'fechaOFFin',
    'piezasDisponibles',
    'childrenOrdersqty',
    'qfb',
    'label',
    'statusOF',
    'actions',
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
  OrderToGenerateQR = false;
  signatureData = CONST_STRING.empty;
  isThereOrdersToViewPdf = false;
  isCorrectToAddComments = false;
  ordersToSendAndDownloadQR: number[] = [];
  ordersReceivedFromRequest: number[] = [];
  paramsDetailOrder = new ParamsPedidos();
  baseQueryString = CONST_STRING.empty;
  isThereOrdersDetailToDelivered = false;
  patientName = CONST_STRING.empty;
  expandedElement: IPedidoDetalleReq | null;
  expandedElementList: IPedidoDetalleReq[] = [];
  constructor(
    private pedidosService: PedidosService,
    private route: ActivatedRoute,
    public dataService: DataService,
    private titleService: Title,
    private errorService: ErrorService,
    private router: Router,
    private downloadImagesService: DownloadImagesService,
    private observableService: ObservableService,
    public localStorageService: LocalStorageService,
    private messagesService: MessagesService,
    private filtersService: FiltersService,
  ) {
    this.observableService.setUrlActive(HttpServiceTOCall.DETAIL_ORDERS);
  }

  ngOnInit() {
    this.getProductoNoLabel();
    this.route.paramMap.subscribe(params => {
      this.validateToGetCurrentDetail(params.get('id'));
      this.titleService.setTitle('Pedido ' + params.get('id'));
    });
    this.subscriptionCallHttpDetail.add(this.observableService.getCallHttpService().subscribe(detailHttpCall => {
      if (detailHttpCall === HttpServiceTOCall.DETAIL_ORDERS) {
        this.getDetallePedidoService();
      }
    }));
    this.subscriptionCallHttpDetail.add(this.observableService.getNewDataSignature().subscribe(newDataSignature => {
      this.signatureData = newDataSignature;
      this.sendToLabelsFinish();
    }));
    this.subscriptionCallHttpDetail.add(this.observableService.getNewCommentsResult().subscribe(newCommentsResult =>
      this.successNewComments(newCommentsResult)));
  }

  getDetallePedidoService() {
    this.pedidosService.getDetallePedido(this.paramsDetailOrder.current).subscribe(
      ({ response }) => this.onSuccessDetailPedido(response), error => this.errorService.httpError(error));
  }
  onSuccessDetailPedido(response: IPedidoDetalleReq[]) {
    this.productCodeSplit = [];
    this.paramsDetailOrder.current = response[CONST_NUMBER.zero].pedidoId.toString();
    this.dataSource.data = response;
    this.dataSource.data.forEach((orders, i) => {
      const productCodeSplit = orders.codigoProducto.split(' ');
      this.productCodeSplit.push(productCodeSplit[0]);
      this.realLabel = constRealLabel.impresaCliente;
      const patientName = orders.patientName !== CONST_STRING.empty && orders.patientName !== undefined ?
        orders.patientName.split(':')[1]
        : CONST_STRING.empty;
      this.patientName = patientName;
      this.docStatus = orders.pedidoStatus;
      orders.fechaOf = orders.fechaOf == null ? '' : orders.fechaOf.substring(10, 0);
      orders.fechaOfFin = orders.fechaOfFin == null ? '' : orders.fechaOfFin.substring(10, 0);
      orders.status = orders.status === '' ? ConstStatus.abierto : orders.status;
      switch (orders.status.toUpperCase()) {
        case ConstStatus.abierto.toUpperCase():
          orders.class = 'abierto';
          break;
        case ConstStatus.planificado.toUpperCase():
          orders.class = 'planificado';
          break;
        case ConstStatus.asignado.toUpperCase():
          orders.class = 'asignado';
          break;
        case ConstStatus.pendiente.toUpperCase():
          orders.class = 'pendiente';
          break;
        case ConstStatus.terminado.toUpperCase():
          orders.class = 'terminado';
          break;
        case ConstStatus.enProceso.toUpperCase():
          orders.class = 'proceso';
          break;
        case ConstStatus.finalizado.toUpperCase():
          orders.class = 'finalizado';
          break;
        case ConstStatus.cancelado.toUpperCase():
          orders.class = 'cancelado';
          break;
        case ConstStatus.reasingado.toUpperCase():
          orders.class = 'reasignado';
          break;
        case ConstStatus.rechazado.toUpperCase():
          orders.class = 'rechazado';
          break;
        case ConstStatus.entregado.toUpperCase():
        case ConstStatus.almacenado.toUpperCase():
          orders.class = ConstStatus.almacenado.toLowerCase();
          break;
      }
      orders.descripcionProducto = orders.descripcionProducto.toUpperCase();
      // orders.childOrders = [];
      orders.style = this.dataService.calculateTernary(i % 2 === 0, '#f1f2f3', '#fff');
    });
    this.isThereOrdersToViewPdf = false;
    this.isThereOrdersDetailToDelivered = false;
    this.isThereOrdersToFinishLabel = false;
    this.OrderToGenerateQR = false;
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

  toggleExpand(order: IPedidoDetalleReq) {
    const index = this.expandedElementList.indexOf(order);
    if (index >= 0) {
      this.expandedElementList.splice(index, 1);
    } else {
      this.expandedElementList.push(order);
    }
  }

  isExpanded(row: IPedidoDetalleReq): boolean {
    return this.expandedElementList.includes(row);
  }

  updateAllComplete(event: boolean) {
    this.OrderToGenerateQR = false;
    const allChildrenChecked = this.getIfAllChildrenOrdersIsChecked();
    const someChildrenChecked = this.someChildrenOrderIsChecked();
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked) && allChildrenChecked;
    this.OrderToGenerateQR = this.dataSource.data != null && (this.dataSource.data.some(t => t.isChecked) || someChildrenChecked);
    this.showOnSplitProcessMessage(event);
    this.getButtonsToUnLooked();
  }

  getIfAllChildrenOrdersIsChecked(): boolean {
    const allChlidrenOrdersChecked = this.dataSource.data
      .every(parentOrder => parentOrder.childOrders.every(childOrder => childOrder.isChecked));
    return allChlidrenOrdersChecked;
  }

  someComplete(): boolean {
    if (this.dataSource.data == null) {
      return false;
    }
    const someChildrenChecked = this.someChildrenOrderIsChecked();
    return (this.dataSource.data.filter(t => t.isChecked).length > 0 || someChildrenChecked) && !this.allComplete;
  }

  someChildrenOrderIsChecked(): boolean {
    const someChlidrenOrdersChecked = this.dataSource.data
      .some(parentOrder => parentOrder.childOrders.some(childOrder => childOrder.isChecked));
    return someChlidrenOrdersChecked;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (this.dataSource.data == null) {
      return;
    }
    this.dataSource.data.forEach(t => {
      t.isChecked = completed;
      this.setAllChildrenOrdersChecked(t, completed);
    });
    this.OrderToGenerateQR = this.dataService.calculateAndValueList([
      this.dataSource.data != null,
      this.dataSource.data.some(t => t.isChecked),
      this.someChildrenOrderIsChecked()
    ]);
    this.showOnSplitProcessMessage(completed);
    this.getButtonsToUnLooked();
  }

  setAllChildrenOrdersChecked(order: IPedidoDetalleReq, completed: boolean) {
    order.childOrders.forEach(childOrder => {
      childOrder.isChecked = completed;
    });
  }

  openPlaceOrderDialog() {
    this.observableService.setQbfToPlace({
      modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromDetailOrder)
    });
  }

  getButtonsToUnLooked() {
    this.isThereOrdersDetailToDelivered = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.finalizado,
      FromToFilter.fromDefault);
    this.isThereOrdersToViewPdf = this.dataSource.data.filter(order => order.isChecked).length > CONST_NUMBER.zero;

    this.isThereOrdersToFinishLabel = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.abierto,
      FromToFilter.fromOrderDetailLabel);

    this.isThereOrdersDetailToCancel = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.finalizado,
      FromToFilter.fromDetailOrder);
    this.isThereOrdersDetailToPlace = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.planificado,
      FromToFilter.fromDefault);
    this.isThereOrdersDetailToFinalize = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.terminado,
      FromToFilter.fromDefault);
    this.isThereOrdersDetailToPlan = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.abierto,
      FromToFilter.fromDefault);
    this.isThereOrdersDetailToReassign = this.filtersService.getIsThereOnData(this.dataSource.data, ConstStatus.reasingado,
      FromToFilter.fromOrderIsolatedReassign);
  }

  showOnSplitProcessMessage(check: boolean) {
    const dataChecked = this.dataSource.data.filter(t => t.isChecked);
    const someOnSplitProcess = dataChecked.some(order => order.onSplitProcess);
    const showMessage = this.dataService.calculateAndValueList([someOnSplitProcess, check]);
    if (showMessage) {
      const orders = this.dataSource.data.filter(order =>
        this.dataService.calculateAndValueList([
          order.isChecked,
          order.onSplitProcess
        ])).map(t => t.ordenFabricacionId);
      const mssg = `No es posible modificar el estatus de órdenes en proceso de división: ${orders.join(', ')}.`;
      this.messagesService.presentToastCustom('', 'error', mssg, false, false);
    }
  }

  ngOnDestroy() {
    this.subscriptionCallHttpDetail.unsubscribe();
  }

  processOrdersDetail() {
    this.messagesService.presentToastCustom(Messages.processOrdersDetail, 'warning', CONST_STRING.empty, true, true)
      .then((result: any) => {
        if (result.isConfirmed) {
          this.detailsOrderToProcess.pedidoId = Number(this.paramsDetailOrder.current);
          this.detailsOrderToProcess.userId = this.localStorageService.getUserId();
          this.detailsOrderToProcess.productId =
            this.dataSource.data.filter(t => (t.isChecked && t.status === ConstStatus.abierto)).map(detail => detail.codigoProducto);
          const childrenProducts = this.getChildrenOrdersProductToPlan(ConstStatus.abierto);
          this.detailsOrderToProcess.productId = this.detailsOrderToProcess.productId.concat(childrenProducts);
          this.pedidosService.postPlaceOrdersDetail(this.detailsOrderToProcess).subscribe(resultProcessDetail => {
            if (resultProcessDetail.success && resultProcessDetail.response.length > 0) {
              const titleProcessDetailWithError = this.messagesService.getMessageTitle(
                resultProcessDetail.response, MessageType.processDetailOrder);
              this.getDetallePedidoService();
              this.messagesService.presentToastCustom(titleProcessDetailWithError, 'error',
                Messages.errorToAssignOrderAutomaticSubtitle, true, false, ClassNames.popupCustom);
            } else {
              this.reloadOrderDetail();
            }
          }, error => this.errorService.httpError(error));
        }
      });
  }
  getChildrenOrdersProductToPlan(status: string) {
    return this.dataSource.data.map(parentOrder =>
      parentOrder.childOrders.filter(childOrder =>
        this.dataService.calculateAndValueList([
          childOrder.isChecked,
          childOrder.status === status
        ])
      ).map(order => order.codigoProducto))
      .reduce((acc, ids) => acc.concat(ids), []);
  }

  cancelOrders() {
    this.observableService.setCancelOrders({
      list: this.getDataCancel(ConstStatus.finalizado),
      cancelType: MODAL_NAMES.placeOrdersDetail
    });
  }

  finalizeOrdersDetail() {
    this.observableService.setFinalizeOrders({
      list: this.getDataCancelFinalize(ConstStatus.terminado, true),
      cancelType: MODAL_NAMES.placeOrdersDetail
    });
  }

  reassignOrderDetail() {
    const parentOrdersReasign = this.filtersService.getItemOnDateWithFilter(this.dataSource.data,
      FromToFilter.fromOrderIsolatedReassignItems).map(order => Number(order.ordenFabricacionId));


    const childrenOrders = this.getChildrenOrdersChecked();
    const childrenOrderToReasign = this.filtersService.getItemOnDateWithFilter(childrenOrders,
      FromToFilter.fromOrderIsolatedReassignItems).map(order => Number(order.ordenFabricacionId));

    const dataRequest = parentOrdersReasign.concat(childrenOrderToReasign);
    this.observableService.setQbfToPlace({
      modalType: MODAL_NAMES.placeOrdersDetail,
      list: dataRequest,
      isFromReassign: true
    });
  }

  getChildrenOrdersChecked() {
    const childrenChecked = this.dataSource.data
      .map(parentOrder => parentOrder.childOrders.filter(childOrder => childOrder.isChecked))
      .reduce((acc, children) => acc.concat(children), []);
    return childrenChecked;
  }

  goToOrders(urlPath: string[]) {
    this.observableService.setPathUrl(urlPath);
  }

  materialRequestDetail() {
    this.localStorageService.setCurrentDetailOrder(this.paramsDetailOrder.current);
    this.router.navigate([RouterPaths.materialRequest,
    this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromDetailOrder).toString() || CONST_NUMBER.zero,
    CONST_NUMBER.zero]);
  }
  getDataCancel(status: string) {
    const childrenOrders = this.getChildrenOrdersToCancel(status);
    const parentsOrders = this.dataSource.data.filter
      (t => (t.isChecked && (t.status !== status && t.status !== ConstStatus.almacenado))).map(order => {
        return this.getCancelOrderReq(order.ordenFabricacionId);
      });
    const dataRequest = parentsOrders.concat(childrenOrders);
    return dataRequest;
  }

  getChildrenOrdersToCancel(status: string) {
    const orders = this.getChildrenOrdersCheckedIdsToCancel(status);
    return orders.map(order => this.getCancelOrderReq(order));
  }

  getChildrenOrdersCheckedIdsToCancel(status: string) {
    const childrenChecked = this.dataSource.data.map(parentOrder =>
      parentOrder.childOrders.filter(childOrder =>
        this.dataService.calculateAndValueList([
          childOrder.isChecked,
          childOrder.status !== status,
          childOrder.status !== ConstStatus.almacenado
        ])
      ).map(order => order.ordenFabricacionId))
      .reduce((acc, ids) => acc.concat(ids), []);
    return childrenChecked;
  }

  getDataCancelFinalize(status: string, isFromFinalize: boolean = false) {
    const childrenOrders = this.getChildrenOrdersTiFinalize(status, isFromFinalize);
    const parentOrders = this.dataSource.data.filter(t =>
      (t.isChecked && (isFromFinalize ? t.status === status : t.status !== status))).map(order => {
        return this.getCancelOrderReq(order.ordenFabricacionId);
      });
    const dataRequest = parentOrders.concat(childrenOrders);
    return dataRequest;
  }

  getChildrenOrdersTiFinalize(status: string, isFromFinalize: boolean = false) {
    const orders = this.getChildrenOrdersCheckedIds(status, isFromFinalize);
    return orders.map(order => this.getCancelOrderReq(order));
  }

  getChildrenOrdersCheckedIds(status: string, isFromFinalize: boolean = false) {
    const childrenChecked = this.dataSource.data.map(parentOrder =>
      parentOrder.childOrders.filter(childOrder =>
        this.dataService.calculateAndValueList([
          childOrder.isChecked,
          this.dataService.calculateTernary(
            isFromFinalize,
            childOrder.status === status,
            childOrder.status !== status,
          )
        ])
      ).map(order => order.ordenFabricacionId))
      .reduce((acc, ids) => acc.concat(ids), []);
    return childrenChecked;
  }

  getCancelOrderReq(ordenFabricacionId: number): CancelOrderReq {
    const cancelOrder = new CancelOrderReq();
    cancelOrder.orderId = ordenFabricacionId;
    return cancelOrder;
  }
  addCommentsDialog() {
    if (!this.isCorrectToAddComments) {
      this.observableService.setOpenCommentsDialog({ comments: this.dataSource.data[0].comments });
    }
  }

  addCommentsOnService(addCommentsResult: string) {
    this.pedidosService.savedComments(Number(this.paramsDetailOrder.current), addCommentsResult).subscribe(() => {
      this.reloadOrderDetail();
    },
      error => this.errorService.httpError(error));
  }

  reloadOrderDetail() {
    this.getDetallePedidoService();
    this.observableService.setMessageGeneralCallHttp({ title: Messages.success, icon: 'success', isButtonAccept: false });
  }

  finishOrdersLabels() {
    this.observableService.setOpenSignatureDialog(this.signatureData);
  }
  sendToLabelsFinish() {
    this.createConsumeService();
  }

  createConsumeService(isFromRemoveSignature: boolean = false, index?: number) {
    const labelToFinishReq = new IPedidoDetalleLabelReq();
    labelToFinishReq.details = this.getArrayToFinishLabel(isFromRemoveSignature, index);
    labelToFinishReq.designerSignature = isFromRemoveSignature ? null : this.signatureData;
    labelToFinishReq.userId = this.localStorageService.getUserId();
    this.pedidosService.finishLabels(labelToFinishReq).subscribe(() => {
      this.reloadOrderDetail();
    }, error => this.errorService.httpError(error));
  }

  getArrayToFinishLabel(isFromRemoveSignature: boolean, index?: number) {
    const invalidCatalogGroups = [CatalogTypes.dermazone.toUpperCase(),
    CatalogTypes.magistralDermocos.toUpperCase(),
    CatalogTypes.magistralMedicament.toUpperCase()];
    if (!isFromRemoveSignature) {
      const parentOrdersToFinishLabel = this.getParentOrdersToFinishLabels(invalidCatalogGroups);
      const childrenOrderToFinishLabel = this.getChildrenOrdersToFinisLabel(invalidCatalogGroups);

      const dataRequest = parentOrdersToFinishLabel.concat(childrenOrderToFinishLabel);
      return dataRequest;
    } else {
      const labelsToFinish: LabelToFinish[] = [];
      labelsToFinish.push({ orderId: this.dataSource.data[index].ordenFabricacionId, checked: false });
      return labelsToFinish;
    }
  }

  getChildrenOrdersToFinisLabel(invalidCatalogGroups: string[]): LabelToFinish[] {
    const childrenOrdersChecked = this.getChildrenOrdersChecked();

    return childrenOrdersChecked.filter(order =>
      this.dataService.calculateAndValueList([
        order.isChecked,
        (order.status !== ConstStatus.abierto && order.status !== ConstStatus.cancelado),
        (order.codigoProducto.split(' ')[0] !== this.ProductNoLabel.value
          || !invalidCatalogGroups.includes(order.catalogGroup.toUpperCase())),
        order.finishedLabel !== 1
      ])).map(order => {
        const labelToFinish = new LabelToFinish();
        labelToFinish.orderId = order.ordenFabricacionId;
        labelToFinish.checked = true;
        return labelToFinish;
      });
  }

  getParentOrdersToFinishLabels(invalidCatalogGroups: string[]): LabelToFinish[] {
    return this.dataSource.data.filter(order =>
      this.dataService.calculateAndValueList([
        order.isChecked,
        (order.status !== ConstStatus.abierto && order.status !== ConstStatus.cancelado),
        (order.codigoProducto.split(' ')[0] !== this.ProductNoLabel.value
          || !invalidCatalogGroups.includes(order.catalogGroup.toUpperCase())),
        order.finishedLabel !== 1
      ]))
      .map(order => {
        const labelToFinish = new LabelToFinish();
        labelToFinish.orderId = order.ordenFabricacionId;
        labelToFinish.checked = true;
        return labelToFinish;
      });
  }

  removeSignature(index: number) {
    if (this.localStorageService.getUserRole() === RolesType.design) {
      this.messagesService.presentToastCustom(`${Messages.removeLabelFinish} ${this.dataSource.data[index].label.toLowerCase()}?`,
        'question', CONST_STRING.empty, true, true)
        .then((result: any) => {
          if (result.isConfirmed) {
            this.createConsumeService(true, index);
          }
        });
    }
  }

  removeSignatureFromChildren(childOrder: ChildrenOrders) {
    if (this.localStorageService.getUserRole() === RolesType.design) {
      const label = childOrder.label;
      this.messagesService.presentToastCustom(`${Messages.removeLabelFinish} ${label.toLowerCase()}?`,
        'question', CONST_STRING.empty, true, true)
        .then((result: any) => {
          if (result.isConfirmed) {
            const labelToFinishReq = new IPedidoDetalleLabelReq();
            labelToFinishReq.details = [{
              orderId: childOrder.ordenFabricacionId,
              checked: false
            }];
            labelToFinishReq.designerSignature = this.signatureData;
            labelToFinishReq.userId = this.localStorageService.getUserId();
            this.finishChilOrder(labelToFinishReq);
          }
        });
    }
  }

  finishChilOrder(req: IPedidoDetalleLabelReq) {
    this.pedidosService.finishLabels(req).subscribe(() => {
      this.reloadOrderDetail();
    }, error => this.errorService.httpError(error));
  }

  viewOrdersWithPdf() {
    const data = new CreatePdfOrder();
    data.orderId = Number(this.paramsDetailOrder.current);
    data.clientType = this.paramsDetailOrder.clientType;
    this.pedidosService.getOrdersPdfViews([data])
      .subscribe(viewPdfResult => {
        viewPdfResult.response.forEach(pdfUrl =>
          this.dataService.openNewTapByUrl(pdfUrl, TypeToSeeTap.order, Number(this.paramsDetailOrder.current)));
      }
        , error => this.errorService.httpError(error));
  }

  ordersToDownloadQr() {
    this.ordersReceivedFromRequest = [];
    const parentOrders = this.dataService.getItemOnDataOnlyIds(this.dataSource.data, FromToFilter.fromDetailOrderQr);
    const childrenOrders = this.dataService.getItemOnDataOnlyIds(this.getChildrenOrdersChecked(), FromToFilter.fromDetailOrderQr);
    this.ordersToSendAndDownloadQR = parentOrders.concat(childrenOrders);
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
    this.messagesService.presentToastCustom(
      this.messagesService.getMessageTitle(ordersWithoutQr, MessageType.ordersWithoutQr, false), 'error',
      CONST_STRING.empty, true, false, ClassNames.popupCustom);
  }

  successNewComments(newCommentsResult: CommentsConfig) {
    this.addCommentsOnService(newCommentsResult.comments);
  }

  generateParamsToGetDetail(order: string) {
    this.paramsDetailOrder = JSON.parse(this.localStorageService.getFiltersActives());
    this.paramsDetailOrder = { ...this.paramsDetailOrder, current: order };
    this.baseQueryString = this.filtersService.getNewDataToFilter(this.paramsDetailOrder)[1];
    this.getDetallePedidoService();

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
    if (this.localStorageService.getCurrentDetailOrder()) {
      this.generateParamsToGetDetail(this.localStorageService.getCurrentDetailOrder());
      this.localStorageService.removeCurrentDetailOrder();
    } else {
      this.generateParamsToGetDetail(paramsOrder);
    }
  }

  ordersToDelivered() {
    this.messagesService.presentToastCustom(Messages.deliveredOrders, 'question', CONST_STRING.empty, true, true)
      .then((result: any) => {
        if (result.isConfirmed) {
          this.pedidosService.putOrdersToDelivered(
            this.filtersService.getItemOnDateWithFilter(this.dataSource.data, FromToFilter.fromDefault, ConstStatus.finalizado)
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

  getProductoNoLabel() {
    this.ProductNoLabel = this.localStorageService.getProductNoLabel();
  }

  donothing() { }
}


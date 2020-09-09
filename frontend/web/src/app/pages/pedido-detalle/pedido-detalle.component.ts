import {Component, OnDestroy, OnInit} from '@angular/core';
import {MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import {IPedidoDetalleReq} from '../../model/http/detallepedidos.model';
import {ActivatedRoute} from '@angular/router';
import {DataService} from '../../services/data.service';
import {
  ClassNames,
  CONST_STRING,
  ConstStatus,
  FromToFilter,
  HttpServiceTOCall,
  MessageType,
  MODAL_NAMES
} from '../../constants/const';
import {Subscription} from 'rxjs';
import {Title} from '@angular/platform-browser';
import {CancelOrderReq, ProcessOrdersDetailReq} from '../../model/http/pedidos';
import {Messages} from '../../constants/messages';
import {ErrorService} from '../../services/error.service';

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
    'cons',
    'ordenFabricacionId',
    'codigoProducto',
    'descripcionProducto',
    'qtyPlanned',
    'fechaOF',
    'fechaOFFin',
    'qfb',
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
  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              private dataService: DataService,
              private titleService: Title, private errorService: ErrorService) {
    this.dataService.setUrlActive(HttpServiceTOCall.DETAIL_ORDERS);
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.docNum = params.get('id');
      this.titleService.setTitle('Pedido ' + this.docNum);
    });
    this.getDetallePedido();
    this.subscriptionCallHttpDetail = this.dataService.getCallHttpService().subscribe(detailHttpCall => {
      if (detailHttpCall === HttpServiceTOCall.DETAIL_ORDERS) {
        this.getDetallePedido();
      }
    });
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
              element.class = 'pdabierto';
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
          }
        });
        this.isThereOrdersDetailToPlan = false;
        this.isThereOrdersDetailToPlace = false;
        this.isThereOrdersDetailToCancel = false;
        this.isThereOrdersDetailToFinalize = false;
        this.isThereOrdersDetailToReassign = false;
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
      list: this.dataSource.data.filter(t => t.isChecked && t.status === ConstStatus.planificado).map(order => order.ordenFabricacionId)});
  }

  getButtonsToUnLooked() {
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
                this.getDetallePedido();
                this.dataService.setMessageGeneralCallHttp({title: Messages.success, icon: 'success', isButtonAccept: false });
              }
            }, error => this.errorService.httpError(error));
          }
        } );
  }

  cancelOrders() {
    this.dataService.setCancelOrders({list: this.dataSource.data.filter
      (t => (t.isChecked && t.status !== ConstStatus.finalizado)).map(order => {
        const cancelOrder = new CancelOrderReq();
        cancelOrder.orderId = order.ordenFabricacionId;
        return cancelOrder;
      }),
      cancelType: MODAL_NAMES.placeOrdersDetail});
  }

  finalizeOrdersDetail() {
    this.dataService.setFinalizeOrders({list: this.dataSource.data.filter
      (t => (t.isChecked && t.status === ConstStatus.terminado)).map(order => {
        const cancelOrder = new CancelOrderReq();
        cancelOrder.orderId = order.ordenFabricacionId;
        return cancelOrder;
      }), cancelType: MODAL_NAMES.placeOrdersDetail});
  }

  reassignOrderDetail() {
    this.dataService.setQbfToPlace({modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataService.getItemOnDateWithFilter(this.dataSource.data,
          FromToFilter.fromOrderIsolatedReassignItems).map(order => Number(order.ordenFabricacionId))
      , isFromReassign: true});
  }
}

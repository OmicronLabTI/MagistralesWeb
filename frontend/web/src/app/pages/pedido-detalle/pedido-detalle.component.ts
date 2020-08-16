import {Component, OnDestroy, OnInit} from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import { IPedidoDetalleReq} from '../../model/http/detallepedidos.model';
import { ActivatedRoute } from '@angular/router';
import {DataService} from '../../services/data.service';
import {CONST_STRING, HttpServiceTOCall, MODAL_NAMES} from '../../constants/const';
import {Subscription} from 'rxjs';
import { Title } from '@angular/platform-browser';
import {ProcessOrdersDetailReq} from '../../model/http/pedidos';
import {Messages} from '../../constants/messages';

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
  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              private dataService: DataService,
              private titleService: Title) { }

  ngOnInit() {
    this.subscriptionCallHttpDetail = this.dataService.getCallHttpService().subscribe(detailHttpCall => {
      if (detailHttpCall === HttpServiceTOCall.DETAIL_ORDERS) {
        this.getDetallePedido();
      }
    });
    this.route.paramMap.subscribe(params => {
      this.docNum = params.get('id');
      this.docStatus = params.get('status');
      this.titleService.setTitle('Pedido ' + this.docNum);
    });
    this.getDetallePedido();
  }

  getDetallePedido() {
    this.pedidosService.getDetallePedido(this.docNum).subscribe(
      (pedidoDetalleRes) => {
        this.dataSource.data = pedidoDetalleRes.response;
        this.dataSource.data.forEach(element => {
          element.fechaOf = element.fechaOf == null ? '----------' : element.fechaOf.substring(10, 0);
          element.fechaOfFin = element.fechaOfFin == null ? '----------' : element.fechaOfFin.substring(10, 0);
          element.status = element.status === '' ? 'Abierto' : element.status;
        });
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

  openPlaceOrderDialog() {
    this.dataService.setQbfToPlace({modalType: MODAL_NAMES.placeOrdersDetail,
      list: this.dataSource.data.filter(t => t.status === 'Planificado').map(order => order.ordenFabricacionId)});
  }

  getButtonsToUnLooked() {
    this.isThereOrdersDetailToPlan = this.getIsThereOnData('Abierto');
    this.isThereOrdersDetailToPlace = this.getIsThereOnData('Planificado');
  }
  getIsThereOnData(status: string) {
    return this.dataSource.data.filter(t => (t.isChecked && t.status === status)).length > 0;
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
                this.dataSource.data.filter(t => (t.isChecked && t.status === 'Abierto')).map(detail => detail.codigoProducto);
            this.pedidosService.postPlaceOrdersDetail(this.detailsOrderToProcess).subscribe(() => {
              this.getDetallePedido();
              this.dataService.presentToastCustom(Messages.success, 'success', CONST_STRING.empty, false, false);
            }, () => this.dataService.presentToastCustom(Messages.generic, 'info', CONST_STRING.empty, false, false)
          );
            console.log('toProcess: ', this.detailsOrderToProcess);
          }
        } );
  }
}

import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import { IPedidoDetalleReq} from '../../model/http/detallepedidos.model';
import { ActivatedRoute } from '@angular/router';
import {DataService} from '../../services/data.service';
import {MODAL_NAMES} from '../../constants/const';

@Component({
  selector: 'app-pedido-detalle',
  templateUrl: './pedido-detalle.component.html',
  styleUrls: ['./pedido-detalle.component.scss']
})
export class PedidoDetalleComponent implements OnInit {
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
  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              private dataService: DataService) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.docNum = params.get('id');
      this.docStatus = params.get('status');
    });
    this.getDetallePedido();
  }

  getDetallePedido() {
    this.pedidosService.getDetallePedido(this.docNum).subscribe(
      (pedidoDetalleRes) => {
        pedidoDetalleRes.response.forEach(element => {
          element.fechaOf = element.fechaOf == null ? '----------' : element.fechaOf.substring(10, 0);
          element.fechaOfFin = element.fechaOfFin == null ? '----------' : element.fechaOfFin.substring(10, 0);
          element.status = element.status === '' ? 'Abierto' : element.status;
          this.dataSource.data.push(element);
        });
        this.dataSource._updateChangeSubscription();
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

}

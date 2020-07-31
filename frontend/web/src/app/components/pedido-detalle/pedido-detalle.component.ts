import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from "../../services/pedidos.service";
import {IPedidoDetalleReq, IPedidoDetalleListRes} from "../../model/http/detallepedidos.model";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: 'app-pedido-detalle',
  templateUrl: './pedido-detalle.component.html',
  styleUrls: ['./pedido-detalle.component.scss']
})
export class PedidoDetalleComponent implements OnInit {
  allComplete: boolean = false;
  actualPage: number = 0;
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
  ]
  dataSource = new MatTableDataSource()

  constructor(private pedidosService: PedidosService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.docNum = params.get("id")
      this.docStatus = params.get("status")
    })
    this.getDetallePedido();
  }

  getDetallePedido() {
    this.pedidosService.getDetallePedido(this.docNum).subscribe(
      (pedidoDetalleRes: IPedidoDetalleListRes) => {
        pedidoDetalleRes.response.forEach(element => {
          element.fechaOf = element.fechaOf == null ? "----------" : element.fechaOf.substring(10, 0);
          element.fechaOfFin = element.fechaOfFin == null ? "----------" : element.fechaOfFin.substring(10, 0);
          this.dataSource.data.push(element);
        })
        this.dataSource._updateChangeSubscription();
      }
    );
  }

  updateAllComplete() {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t['isChecked']);
  }

  someComplete(): boolean {
    if (this.dataSource.data == null) {
      return false;
    }
    return this.dataSource.data.filter(t => t['isChecked']).length > 0 && !this.allComplete;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (this.dataSource.data == null) {
      return;
    }
    this.dataSource.data.forEach(t => t['isChecked'] = completed);
  }

}
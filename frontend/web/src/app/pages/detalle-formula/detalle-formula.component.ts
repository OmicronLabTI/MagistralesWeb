import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from "../../services/pedidos.service";
import {IFormulaRes, IFormulaDetalleReq, IFormulaReq} from "../../model/http/detalleformula";
import { ActivatedRoute } from "@angular/router";
import {ErrorService} from "../../services/error.service";

@Component({
  selector: 'app-detalle-formula',
  templateUrl: './detalle-formula.component.html',
  styleUrls: ['./detalle-formula.component.scss']
})

export class DetalleFormulaComponent implements OnInit {
  allComplete = false;
  dataFormulaDetail = new IFormulaReq();
  ordenFabricacionId: string;
  displayedColumns: string[] = [
    'seleccion',
    'cons',
    'numero',
    'descripcion',
    'cantbase',
    'cantreq',
    'consumido',
    'disponible',
    'unidad',
    'almacen',
    'cantpend',
    'enstock',
    'cantalmacen'
  ]
  dataSource = new MatTableDataSource<IFormulaDetalleReq>();

  constructor(private pedidosService: PedidosService, private route: ActivatedRoute, private errorService: ErrorService) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.ordenFabricacionId = params.get('ordenid');
    });
    // this.dataSource.data = ELEMENT_DATA;
    this.getDetalleFormula();
  }

  getDetalleFormula() {
    this.pedidosService.getFormulaDetail(this.ordenFabricacionId).subscribe(
      (formulaRes: IFormulaRes) => {
        this.dataFormulaDetail = formulaRes.response;
        this.dataSource.data = this.dataFormulaDetail.details;
        this.dataSource.data.forEach(detail => {detail.isChecked = false; });
/*        this.dataFormulaDetail.details.forEach(element => {
      /!*    element.fechaOf = element.fechaOf == null ? "----------" : element.fechaOf.substring(10, 0);
          element.fechaOfFin = element.fechaOfFin == null ? "----------" : element.fechaOfFin.substring(10, 0);*!/
          this.dataSource.data.push(element);
        })
        this.dataSource._updateChangeSubscription();*/
      }, error => this.errorService.httpError(error));
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


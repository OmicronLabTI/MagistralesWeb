import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from "../../services/pedidos.service";
import { IFormulaDetalleRes, IFormulaDetalleReq} from "../../model/http/detalleformula";
import { ActivatedRoute } from "@angular/router";
import {TooltipPosition} from "@angular/material/tooltip";

const ELEMENT_DATA: IFormulaDetalleReq[] = [
  {isChecked: false, productId: "MP-0001", description: "BENZOTO DE POTASIO 150 ML 0.3%", baseQuantity: 0.0013, requiredQuantity: 0.0003, consumed: 0, available: 2.089, unit: 'GR', warehouse: 'MG', pendingQuantity: 0.0013, stock: 3.99, warehouseQuantity: 10.09},
  {isChecked: false, productId: "MP-0002", description: "CLORATO DE POTASIO 150 ML 0.3%", baseQuantity: 0.0013, requiredQuantity: 0.0003, consumed: 0, available: 2.089, unit: 'GR', warehouse: 'MG', pendingQuantity: 0.0013, stock: 3.99, warehouseQuantity: 10.09},
  {isChecked: false, productId: "MP-0003", description: "CLORIDRATO DE POTASIO 150 ML 0.3%", baseQuantity: 0.0013, requiredQuantity: 0.0003, consumed: 0, available: 2.089, unit: 'GR', warehouse: 'MG', pendingQuantity: 0.0013, stock: 3.99, warehouseQuantity: 10.09},
];

@Component({
  selector: 'app-detalle-formula',
  templateUrl: './detalle-formula.component.html',
  styleUrls: ['./detalle-formula.component.scss']
})

export class DetalleFormulaComponent implements OnInit {
  allComplete: boolean = false;
  actualPage: number = 0;
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
  dataSource = new MatTableDataSource();

  constructor(private pedidosService: PedidosService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.ordenFabricacionId = params.get("ordenid")
    })
    this.dataSource.data = ELEMENT_DATA;
    this.getDetalleFormula();
  }

  getDetalleFormula() {
    this.pedidosService.getDetallePedido(this.docNum).subscribe(
      (formulaDetalleRes: IFormulaDetalleRes) => {
        formulaDetalleRes.response.forEach(element => {
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


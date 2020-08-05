import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from "../../services/pedidos.service";
import { IFormulaDetalleRes, IFormulaDetalleReq} from "../../model/http/detalleformula";
import { ActivatedRoute } from "@angular/router";
import {TooltipPosition} from "@angular/material/tooltip";
import {MatFormFieldModule} from '@angular/material/form-field';


const ELEMENT_DATA: IFormulaDetalleReq[] = [
  {isChecked: false, numero: "MP-0001", descripcion: "BENZOTO DE POTASIO 150 ML 0.3%", cantidadBase: 0.0013, cantidadRequerida: 0.0003, consumido: 0, disponible: 2.089, unidad: 'GR', almacen: 'MG', cantidadPendiente: 0.0013, stock: 3.99, cantidadAlmacen: 10.09},
  {isChecked: false, numero: "MP-0002", descripcion: "CLORATO DE POTASIO 150 ML 0.3%", cantidadBase: 0.0013, cantidadRequerida: 0.0003, consumido: 0, disponible: 2.089, unidad: 'GR', almacen: 'MG', cantidadPendiente: 0.0013, stock: 3.99, cantidadAlmacen: 10.09},
  {isChecked: false, numero: "MP-0003", descripcion: "CLORIDRATO DE POTASIO 150 ML 0.3%", cantidadBase: 0.0013, cantidadRequerida: 0.0003, consumido: 0, disponible: 2.089, unidad: 'GR', almacen: 'MG', cantidadPendiente: 0.0013, stock: 3.99, cantidadAlmacen: 10.09},
];

@Component({
  selector: 'app-detalle-formula',
  templateUrl: './detalle-formula.component.html',
  styleUrls: ['./detalle-formula.component.scss']
})

export class DetalleFormulaComponent implements OnInit {
  allComplete: boolean = false;
  actualPage: number = 0;
  docNum: string;
  docStatus: string;
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
    this.dataSource.data = ELEMENT_DATA;
    console.log(this.dataSource.data);
  }

  getDetallePedido() {
    
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


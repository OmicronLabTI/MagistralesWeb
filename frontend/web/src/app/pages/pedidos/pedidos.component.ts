import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from "../../services/pedidos.service";
import { IPedidosListRes} from "../../model/http/pedidos";
import { createElementCssSelector } from '@angular/compiler';

@Component({
  selector: 'app-pedidos',
  templateUrl: './pedidos.component.html',
  styleUrls: ['./pedidos.component.scss']
})
export class PedidosComponent implements OnInit {
  allComplete: boolean = false;
  actualPage: number = 0;
  params = {
    date: "month",
    offset: this.actualPage,
    limit: 5
  };
  displayedColumns: string[] = ['seleccion', 'cons', 'codigo', 'cliente', 'medico', 'asesor', 'f_inicio', 'f_fin', 'status', 'qfb_asignado', 'actions']
  dataSource = new MatTableDataSource()

  constructor(private pedidosService: PedidosService) { }

  ngOnInit() {
    this.getPedidos();
  }

  getPedidos() {
    this.params['offset'] = this.actualPage;
    this.pedidosService.getPedidos(this.params).subscribe(
      (pedidoRes: IPedidosListRes) => {
        pedidoRes.response.forEach(element => {
          element.pedidoStatus = element.pedidoStatus == "O" ? "Abierto" : "Cerrado";
          console.log(element.pedidoStatus);
          element.class = element.pedidoStatus == "Abierto" ? "green": "mat-primary";
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

  setSpanTitle(menuValue: any, title: string, date: string){
    menuValue.textContent = title;
    this.params['date'] = date;
    this.params['offset'] = 0;
    this.dataSource.data = [];
    this.dataSource._updateChangeSubscription();
    this.getPedidos();
  }

}

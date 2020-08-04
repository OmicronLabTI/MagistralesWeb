import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import { IPedidosListRes} from '../../model/http/pedidos';
import { createElementCssSelector } from '@angular/compiler';
import { DataService } from '../../services/data.service';
import { CONST_STRING} from '../../constants/const';
import {Messages} from '../../constants/messages';
import {ErrorService} from '../../services/error.service';

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

  constructor(
    private pedidosService: PedidosService,
    private dataService: DataService,
    private errorService: ErrorService
  ) { }

  ngOnInit() {
    this.getPedidos();
  }

  getPedidos() {
    this.params['offset'] = this.actualPage;
    this.pedidosService.getPedidos(this.params).subscribe((pedidoRes: IPedidosListRes) => {
      pedidoRes.response.forEach(element => {
        element.pedidoStatus = element.pedidoStatus == "O" ? "Abierto" : "Cerrado";
        element.class = element.pedidoStatus == "Abierto" ? "green": "mat-primary";
        this.dataSource.data.push(element);
      })
      this.dataSource._updateChangeSubscription();
    },
    error => {
      console.log(error);
      this.errorService.httpError(error);
    });
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

  processOrders(){
    this.dataService.presentToastCustom(Messages.processOrders, 'warning', CONST_STRING.empty, true, true)
    .then((result: any) => {
      if (result.isConfirmed) {
        this.pedidosService.processOrders(this.dataSource.data.filter(t => (t['isChecked'] && t['pedidoStatus']=='Abierto')).map(t => t['docNum'])).subscribe(
          () => {
            this.getPedidos();
          },
          error => {
            this.errorService.httpError(error);
          }
        );
      }
    });
  }

}

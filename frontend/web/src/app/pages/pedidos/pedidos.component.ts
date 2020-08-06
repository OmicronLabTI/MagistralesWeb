import {Component, OnInit, ViewChild} from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import { createElementCssSelector } from '@angular/compiler';
import { DataService } from '../../services/data.service';
import { CONST_STRING} from '../../constants/const';
import {Messages} from '../../constants/messages';
import {ErrorService} from '../../services/error.service';
import {IPedidoReq, IPedidosListRes, ParamsPedidos} from '../../model/http/pedidos';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {CONST_NUMBER} from '../../constants/const';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-pedidos',
  templateUrl: './pedidos.component.html',
  styleUrls: ['./pedidos.component.scss']
})
export class PedidosComponent implements OnInit {
  allComplete = false;
  params = new ParamsPedidos();
  // tslint:disable-next-line:max-line-length
  displayedColumns: string[] = ['seleccion', 'cons', 'codigo', 'cliente', 'medico', 'asesor', 'f_inicio', 'f_fin', 'status', 'qfb_asignado', 'actions'];
  dataSource = new MatTableDataSource<IPedidoReq>();
  pageSize = CONST_NUMBER.ten;
  pageEvent: PageEvent;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;
  fullDate: string[] = [];
  constructor(
    private pedidosService: PedidosService,
    private datePipe: DatePipe,
    private dataService: DataService,
    private errorService: ErrorService
  ) {
    this.fullDate = this.datePipe.transform(new Date(), 'dd-MM-yyyy').split('-');
    this.params.fini = `01/${this.fullDate[1]}/${this.fullDate[2]}-${this.fullDate[0]}/${this.fullDate[1]}/${this.fullDate[2]}`;
    //this.params.ffin = `${this.fullDate[0]}/${this.fullDate[1]}/${this.fullDate[2]}`;
  }

  ngOnInit() {
    this.getPedidos();
    this.dataSource.paginator = this.paginator;
  }

  getPedidos() {
    this.params.offset = this.offset;
    this.params.limit = this.limit;
    this.pedidosService.getPedidos(this.params).subscribe(
      (pedidoRes: IPedidosListRes) => {
        this.lengthPaginator = pedidoRes.comments;
        console.log('pedidos res: ', pedidoRes);
        this.dataSource.data = pedidoRes.response;
        this.dataSource.data.forEach(element => {
          element.pedidoStatus = element.pedidoStatus === 'O' ? 'Abierto' : 'Cerrado';
          element.class = element.pedidoStatus === "Abierto" ? "green": "mat-primary";
        });
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

  setSpanTitle(menuValue: any, title: string) {
    menuValue.textContent = title;
    this.params.offset = this.offset;
    this.params.limit = this.limit;
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
            this.dataService.presentToastCustom(Messages.success, 'success', CONST_STRING.empty, false, false)
            this.getPedidos();
          },
          error => {
            this.errorService.httpError(error);
          }
        );
      }
    });
  }
  changeDataEvent(event: PageEvent) {
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = (event.pageSize * (event.pageIndex + 1));
    this.getPedidos();
    return event;
  }

}

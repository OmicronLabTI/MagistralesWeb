import {Component, OnInit, ViewChild} from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import { DataService } from '../../services/data.service';
import { CONST_STRING} from '../../constants/const';
import {Messages} from '../../constants/messages';
import {ErrorService} from '../../services/error.service';
import {IPedidoReq, IPedidosListRes, ParamsPedidos} from '../../model/http/pedidos';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {CONST_NUMBER} from '../../constants/const';
import {DatePipe} from '@angular/common';
import {MatDialog} from '@angular/material/dialog';
import {FindOrdersDialogComponent} from '../../dialogs/find-orders-dialog/find-orders-dialog.component';

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
  queryString = CONST_STRING.empty;
  rangeDate = CONST_STRING.empty;
  isDateInit =  true;
  isSearchWithFilter = false;
  constructor(
    private pedidosService: PedidosService,
    private datePipe: DatePipe,
    private dataService: DataService,
    private errorService: ErrorService,
    private dialog: MatDialog
  ) {
    this.fullDate = this.getFormatDate(new Date());
    // tslint:disable-next-line:max-line-length
    this.queryString = `?fini=${this.fullDate[0]}/${this.fullDate[1]}/${this.fullDate[2]}-${this.fullDate[0]}/${this.fullDate[1]}/${this.fullDate[2]}&offset=${this.offset}&limit=${this.limit}`;
    this.rangeDate =
        `${this.fullDate[0]}/${this.fullDate[1]}/${this.fullDate[2]}-${this.fullDate[0]}/${this.fullDate[1]}/${this.fullDate[2]}`;
  }

  ngOnInit() {
    this.getPedidos();
    this.dataSource.paginator = this.paginator;
  }

  getPedidos() {
    this.pedidosService.getPedidos(this.queryString).subscribe(
      (pedidoRes: IPedidosListRes) => {
        this.lengthPaginator = pedidoRes.comments;
        this.dataSource.data = pedidoRes.response;
        this.dataSource.data.forEach(element => {
          element.pedidoStatus = element.pedidoStatus === 'O' ? 'Abierto' : 'Cerrado';
          element.class = element.pedidoStatus === 'Abierto' ? 'green' : 'mat-primary';
        });
      },
      error => {
        console.log(error);
        this.errorService.httpError(error);
      }
    );
  }

  updateAllComplete() {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked);
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
  }

/*  setSpanTitle(menuValue: any, title: string) {
    menuValue.textContent = title;
    this.params.offset = this.offset;
    this.params.limit = this.limit;
    this.dataSource.data = [];
    this.dataSource._updateChangeSubscription();
    this.getPedidos();
  }*/

  processOrders() {
    this.dataService.presentToastCustom(Messages.processOrders, 'warning', CONST_STRING.empty, true, true)
    .then((result: any) => {
      if (result.isConfirmed) {
        this.pedidosService.processOrders(this.dataSource.data.filter(t => (t.isChecked && t.pedidoStatus == 'Abierto')).map(t => t.docNum)).subscribe(
          () => {
            this.dataService.presentToastCustom(Messages.success, 'success', CONST_STRING.empty, false, false);
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
    this.limit = event.pageSize;
    this.getPedidos();
    return event;
  }
  openFindOrdersDialog() {
    this.isSearchWithFilter = true;
    const dialogRef = this.dialog.open(FindOrdersDialogComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        modalType: 'orders',
      }
    });
    dialogRef.afterClosed().subscribe((result: ParamsPedidos) => {
      if (result.docNum) {
        this.queryString = `?docNum=${result.docNum}`;
      } else {
        if (result.dateType) {
          const dateInit: string[] = this.getFormatDate(result.fini);
          const dateFinish: string[] = this.getFormatDate(result.ffin);
          this.rangeDate = `${dateInit[0]}/${dateInit[1]}/${dateInit[2]}-${dateFinish[0]}/${dateFinish[1]}/${dateFinish[2]}`;
          if ( result.dateType === '0') {
            this.isDateInit = true;
            this.queryString = `?fini=${this.rangeDate}`;
          } else {
            this.isDateInit = false;
            this.queryString = `?ffin=${this.rangeDate}`;
          }
        }
        if (result.status !== '') {
          this.queryString = `${this.queryString}&status=${result.status}`;
        }
        if (result.qfb !== '') {
          this.queryString = `${this.queryString}&qfb=${result.qfb}`;
        }
      }
      this.queryString = `${this.queryString}&offset=${this.offset}&limit=${this.limit}`;
      if (result) {
        this.getPedidos();
      }
    });
  }

  getFormatDate(date: Date) {
      return this.datePipe.transform(date, 'dd-MM-yyyy').split('-');
  }
}

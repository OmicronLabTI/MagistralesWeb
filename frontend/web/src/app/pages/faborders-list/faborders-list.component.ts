import { Component, OnInit, ViewChild } from '@angular/core';
import {
  ComponentSearch,
  CONST_NUMBER,
  CONST_STRING, ConstStatus, HttpServiceTOCall,
  HttpStatus,
  MODAL_FIND_ORDERS
} from '../../constants/const';
import {DataService} from '../../services/data.service';
import {ErrorService} from '../../services/error.service';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {Title} from '@angular/platform-browser';
import {MatTableDataSource} from '@angular/material';
import { IOrdersReq } from 'src/app/model/http/ordenfabricacion';
import { ErrorHttpInterface } from 'src/app/model/http/commons';
import { OrdersService } from 'src/app/services/orders.service';
import { ParamsPedidos } from 'src/app/model/http/pedidos';

@Component({
  selector: 'app-faborders-list',
  templateUrl: './faborders-list.component.html',
  styleUrls: ['./faborders-list.component.scss']
})
export class FabordersListComponent implements OnInit {
  allComplete = false;
  displayedColumns: string[] = [
    'seleccion',
    'cons',
    'pedido',
    'orden',
    'codigoproducto',
    'descripcion',
    'cantidadplanificada',
    'fechaorden',
    'fechatermino',
    'qfbasignado',
    'estatus'
  ];
  dataSource = new MatTableDataSource<IOrdersReq>();
  pageEvent: PageEvent;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;
  queryString = CONST_STRING.empty;
  fullQueryString = CONST_STRING.empty;
  rangeDate = CONST_STRING.empty;
  isDateInit =  true;
  isSearchWithFilter = false;
  filterDataOrders = new ParamsPedidos();
  pageIndex = 0;

  constructor(
    private ordersService: OrdersService,
    private dataService: DataService,
    private errorService: ErrorService,
    private titleService: Title
  ) {
    this.dataService.setUrlActive(HttpServiceTOCall.ORDERS_ISOLATED);
    this.rangeDate = this.getDateFormatted(new Date(), new Date(), true);
    this.filterDataOrders.dateType = '0';
    this.filterDataOrders.dateFull = this.rangeDate;
    this.queryString = `?fini=${this.rangeDate}`;
    this.getFullQueryString();
  }

  ngOnInit() {
    this.titleService.setTitle('OmicronLab - Órdenes de fabricación');
    this.getOrders();
    this.dataSource.paginator = this.paginator;
  }

  updateAllComplete() {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked);
  }

  someComplete(): boolean {
    return this.dataSource.data.filter(t => t.isChecked).length > 0 && !this.allComplete;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (this.dataSource.data == null) {
      return;
    }
    this.dataSource.data.forEach(t => t.isChecked = completed);
  }

  getOrders() {
    this.ordersService.getOrders(this.fullQueryString).subscribe(
      ordersRes => {
        this.lengthPaginator = ordersRes.comments;
        this.dataSource.data = ordersRes.response;
        this.dataSource.data.forEach(element => {
          switch (element.status) {
            case ConstStatus.abierto:
              element.class = 'green';
              break;
            case ConstStatus.planificado:
              element.class = 'mat-primary';
              break;
            case ConstStatus.liberado:
              element.class = 'liberado';
              break;
            case ConstStatus.cancelado:
              element.class = 'cancelado';
              break;
            case ConstStatus.enProceso:
              element.class = 'proceso';
              break;
          }
        });
      },
        (error: ErrorHttpInterface) => {
        if (error.status !== HttpStatus.notFound) {
          this.errorService.httpError(error);
        }
        this.dataSource.data = [];
      }
    );
  }

  getFullQueryString() {
    this.fullQueryString = `${this.queryString}&offset=${this.offset}&limit=${this.limit}`;
  }

  getDateFormatted(initDate: Date, finishDate: Date, isBeginDate: boolean) {
    if (isBeginDate) {
      initDate = new Date(initDate.getTime() - MODAL_FIND_ORDERS.thirtyDays);
    }
    return `${this.dataService.transformDate(initDate)}-${this.dataService.transformDate(finishDate)}`;
  }

  changeDataEvent(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getFullQueryString();
    this.getOrders();
    return event;
  }

  createOrderIsolated() {
    this.dataService.setSearchComponentModal({modalType: ComponentSearch.createOrderIsolated});
  }

  openSearchOrders() {
    console.log('searching')
  }

  cancelOrder() {
    // this.dataService.setCancelOrders({lis})
  }
}

import { Component, OnDestroy, OnInit } from '@angular/core';
import { DataService } from '../../services/data.service';
import {
  ClassButton,
  CONST_NUMBER,
  CONST_STRING,
  ConstOrders,
  HttpServiceTOCall,
  TypeStatusIncidents
} from '../../constants/const';
import { Subscription } from 'rxjs';
import { ParamsPedidos } from '../../model/http/pedidos';
import { IncidentsService } from '../../services/incidents.service';
import { ErrorService } from '../../services/error.service';
import { ChangeStatusIncidentReq, IIncidentsListRes, IncidentItem } from '../../model/http/incidents.model';
import { MatTableDataSource } from '@angular/material/table';
import { CommentsConfig } from '../../model/device/incidents.model';
import { Messages } from '../../constants/messages';
import { PageEvent } from '@angular/material/paginator';
import { Title } from '@angular/platform-browser';
import { ObservableService } from '../../services/observable.service';
import { DateService } from '../../services/date.service';
import { FiltersService } from '../../service/filters.service';

@Component({
  selector: 'app-incidents-list',
  templateUrl: './incidents-list.component.html',
  styleUrls: ['./incidents-list.component.scss']
})
export class IncidentsListComponent implements OnInit, OnDestroy {
  subscriptionObservables = new Subscription();
  filterDataIncidents = new ParamsPedidos();
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;
  pageIndex = CONST_NUMBER.zero;
  queryIncidentsString = CONST_STRING.empty;
  isSearchWithFilter = false;
  fullQueryStringIncidents = CONST_STRING.empty;
  displayedColumns: string[] = [
    'cons',
    'createDate',
    'saleOrder',
    'delivery',
    'invoice',
    'itemCode',
    'type',
    'incident',
    'batches',
    'stage',
    'comments',
    'status'];
  dataSource = new MatTableDataSource<IncidentItem>();
  currentIndex = CONST_NUMBER.zero;
  isOnInit = true;
  pageEvent: PageEvent;
  lengthPaginator = CONST_NUMBER.zero;
  constructor(
    private dataService: DataService,
    private incidentsService: IncidentsService,
    private errorService: ErrorService,
    private titleService: Title,
    private observableService: ObservableService,
    private dateService: DateService,
    private filtersService: FiltersService,
    ) {
    this.observableService.setUrlActive(HttpServiceTOCall.INCIDENTS_LIST);
    this.filterDataIncidents.dateType = ConstOrders.defaultDateInit;
    this.filterDataIncidents.dateFull = this.dateService.getDateFormatted(new Date(), new Date(), true);
    this.filterDataIncidents.isFromIncidents = true;


  }

  ngOnInit() {
    this.titleService.setTitle('OmicronLab - Incidents');
    this.subscriptionObservables.add(this.observableService.getNewSearchOrdersModal().subscribe(resultSearchOrderModal => {
      if (resultSearchOrderModal.isFromIncidents) {
        this.onSuccessSearchOrderModal(resultSearchOrderModal);
      }
    }));
    this.subscriptionObservables.add(this.observableService.getNewCommentsResult().subscribe(newCommentsResult =>
      this.onSuccessResult(newCommentsResult)));
    this.queryIncidentsString = `?fini=${this.filterDataIncidents.dateFull}`;
    this.getFullQueryString();
    this.updateIncidentList();
  }

  onSuccessSearchOrderModal(resultSearchOrderModal: ParamsPedidos) {
    this.pageIndex = 0;
    this.offset = 0;
    this.limit = 10;
    this.filterDataIncidents = this.filtersService.getNewDataToFilter(resultSearchOrderModal)[0];
    this.queryIncidentsString = this.filtersService.getNewDataToFilter(resultSearchOrderModal)[1];
    this.isSearchWithFilter = this.filtersService.getIsWithFilter(resultSearchOrderModal);
    this.getFullQueryString();
    this.updateIncidentList();
  }

  openFindOrdersDialog() {
    this.observableService.setSearchOrdersModal({ modalType: ConstOrders.modalIncidents, filterOrdersData: this.filterDataIncidents });
  }

  ngOnDestroy() {
    this.subscriptionObservables.unsubscribe();
  }
  getFullQueryString() {
    this.fullQueryStringIncidents = `${this.queryIncidentsString}&offset=${this.offset}&limit=${this.limit}`;
  }
  updateIncidentList() {
    this.incidentsService.getIncidentsList(this.fullQueryStringIncidents).subscribe(incidentsListResult =>
      this.onSuccessIncidentsList(incidentsListResult), error => this.errorService.httpError(error));
  }

  onSuccessIncidentsList(incidentsListResult: IIncidentsListRes) {
    this.lengthPaginator = incidentsListResult.comments;
    this.getDataWithClass(incidentsListResult.response);
    this.isOnInit = false;
  }

  getDataWithClass(response: IncidentItem[]) {
    response.forEach(itemIncident => {
      switch (itemIncident.status.toLowerCase()) {
        case TypeStatusIncidents.open.toLowerCase():
          itemIncident.classButton = ClassButton.openIncident;
          break;
        case TypeStatusIncidents.close.toLowerCase():
          itemIncident.classButton = ClassButton.closeIncident;
          break;
        case TypeStatusIncidents.attending.toLowerCase():
          itemIncident.classButton = ClassButton.attendingIncident;
          break;
      }
      itemIncident.batchesDisplay = this.getDisplayBatchesData(itemIncident, false);
      itemIncident.batchesTooltip = String(this.getDisplayBatchesData(itemIncident, true));
    });
    this.dataSource.data = response;
  }

  onSuccessResult(newCommentsResult: CommentsConfig) {
    if (newCommentsResult.isForClose) {
      this.changeStatusIncidentService({
        saleOrderId: this.dataSource.data[this.currentIndex].saleOrder,
        status: TypeStatusIncidents.close,
        comments: newCommentsResult.comments,
        itemCode: this.dataSource.data[this.currentIndex].itemCode
      });
    }
  }

  reloadIncidentsList() {
    this.updateIncidentList();
    this.observableService.setMessageGeneralCallHttp({ title: Messages.success, icon: 'success', isButtonAccept: false });
  }

  openCommentsDialog(index: number) {
    this.currentIndex = index;
    this.setOpenCommentsDialog(index);
  }
  openChangeComments(index: number) {
    this.currentIndex = index;
    if (this.dataSource.data[index].status.toLowerCase() === TypeStatusIncidents.attending.toLowerCase()) {
      this.setOpenCommentsDialog(index, true);
    } else if (this.dataSource.data[index].status.toLowerCase() === TypeStatusIncidents.open.toLowerCase()) {
      this.changeStatusComments();
    }
  }
  setOpenCommentsDialog(index: number, isForClose: boolean = false) {
    this.observableService.setOpenCommentsDialog({
      comments: this.dataSource.data[index].comments,
      isReadOnly: !isForClose,
      isForClose
    });
  }

  private changeStatusComments() {
    const title = `La incidencia del pedido ${this.dataSource.data[this.currentIndex].saleOrder} - ${this.dataSource.data[this.currentIndex].itemCode} será Atendida`;
    this.dataService.presentToastCustom(title, 'question', CONST_STRING.empty, true, true)
      .then((result: any) => {
        if (result.isConfirmed) {
          this.changeStatusIncidentService({
            saleOrderId: this.dataSource.data[this.currentIndex].saleOrder,
            status: TypeStatusIncidents.attending,
            comments: this.dataSource.data[this.currentIndex].comments,
            itemCode: this.dataSource.data[this.currentIndex].itemCode
          });
        }
      });
  }

  changeStatusIncidentService(changeStatus: ChangeStatusIncidentReq) {
    this.incidentsService.patchStatusIncidents(changeStatus).subscribe(() => {
      this.reloadIncidentsList();
    }, error => this.errorService.httpError(error));
  }
  changeDataEvent(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getFullQueryString();
    this.updateIncidentList();
    return event;
  }

  getDisplayBatchesData(incident: IncidentItem, isForToolTip: boolean) {
    const arrayBatches = this.getArrayBatches(incident);
    if (isForToolTip) {
      return this.getDataToDisplay(arrayBatches).trim().slice(0, -1);
    } else {
      return this.getArrayBatchesToDisplay(arrayBatches);
    }
  }

  getArrayBatches(incident: IncidentItem): string[] {
    return incident.batches.split(',').filter(batche => batche !== CONST_STRING.empty);
  }

  getArrayBatchesToDisplay(arrayBatches: string[]) {
    if (arrayBatches.length > CONST_NUMBER.zero && arrayBatches.length <= CONST_NUMBER.two) {
      return arrayBatches;
    } else {
      return arrayBatches.splice(CONST_NUMBER.zero, CONST_NUMBER.two);
    }
  }
  getDataToDisplay(arrayBatches: string[]) {
    let batchesDataDisplay = CONST_STRING.empty;
    arrayBatches.forEach(
      batche => batchesDataDisplay += `${batche}, `);

    return batchesDataDisplay;
  }
}

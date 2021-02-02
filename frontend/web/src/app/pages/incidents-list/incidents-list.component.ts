import {Component, OnDestroy, OnInit} from '@angular/core';
import {DataService} from '../../services/data.service';
import {
  ClassButton,
  CONST_NUMBER,
  CONST_STRING,
  ConstOrders,
  HttpServiceTOCall,
  TypeStatusIncidents
} from '../../constants/const';
import {Subscription} from 'rxjs';
import {ParamsPedidos} from '../../model/http/pedidos';
import {IncidentsService} from '../../services/incidents.service';
import {ErrorService} from '../../services/error.service';
import {IIncidentsListRes, IncidentItem} from '../../model/http/incidents.model';
import {MatTableDataSource} from '@angular/material/table';

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
    'stage',
    'comments',
    'status'];
  dataSource = new MatTableDataSource<IncidentItem>();

  constructor(private dataService: DataService, private incidentsService: IncidentsService,
              private errorService: ErrorService) {
    this.dataService.setUrlActive(HttpServiceTOCall.INCIDENTS_LIST);
    this.filterDataIncidents.dateType = ConstOrders.defaultDateInit;
    this.filterDataIncidents.dateFull = this.dataService.getDateFormatted(new Date(), new Date(), true);
    this.filterDataIncidents.isFromIncidents = true;


  }

  ngOnInit() {
    this.subscriptionObservables.add(this.dataService.getNewSearchOrdersModal().subscribe(resultSearchOrderModal => {
      if (resultSearchOrderModal.isFromIncidents) {
        this.onSuccessSearchOrderModal(resultSearchOrderModal);
      }
    }));
    this.queryIncidentsString = `?fini=${this.filterDataIncidents.dateFull}`;
    this.getFullQueryString();
    this.updateIncidentList();
  }

  onSuccessSearchOrderModal(resultSearchOrderModal: ParamsPedidos) {
    this.pageIndex = 0;
    this.offset = 0;
    this.limit = 10;
    this.filterDataIncidents = this.dataService.getNewDataToFilter(resultSearchOrderModal)[0];
    this.queryIncidentsString = this.dataService.getNewDataToFilter(resultSearchOrderModal)[1];
    this.isSearchWithFilter = this.dataService.getIsWithFilter(resultSearchOrderModal);
    this.getFullQueryString();
    this.updateIncidentList();
    console.log('filterData', this.filterDataIncidents)
    console.log('fullString: ', this.fullQueryStringIncidents)
  }

  openFindOrdersDialog() {
    this.dataService.setSearchOrdersModal({modalType: ConstOrders.modalIncidents, filterOrdersData: this.filterDataIncidents });
  }

  ngOnDestroy() {
    this.subscriptionObservables.unsubscribe();
  }
  getFullQueryString() {
    this.fullQueryStringIncidents = `${this.queryIncidentsString}&offset=${this.offset}&limit=${this.limit}`;
  }
  updateIncidentList() {
    this.incidentsService.getIncidentsList(this.fullQueryStringIncidents).subscribe( incidentsListResult =>
        this.onSuccessIncidentsList(incidentsListResult), error => this.errorService.httpError(error));
  }

  onSuccessIncidentsList(incidentsListResult: IIncidentsListRes) {

    // paginator init again
    console.log('incidentsListResult: ', incidentsListResult)
    this.getDataWithClass(incidentsListResult.response)
  }

   getDataWithClass(response: IncidentItem[]) {
      response.forEach( itemIncident => {
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
      });
      this.dataSource.data = response;
  }
}

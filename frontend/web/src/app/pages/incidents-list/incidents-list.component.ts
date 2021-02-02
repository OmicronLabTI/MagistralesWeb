import {Component, OnDestroy, OnInit} from '@angular/core';
import {DataService} from '../../services/data.service';
import {CONST_NUMBER, CONST_STRING, ConstOrders, HttpServiceTOCall} from '../../constants/const';
import {Subscription} from 'rxjs';
import {ParamsPedidos} from '../../model/http/pedidos';

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
  queryString = CONST_STRING.empty;
  isSearchWithFilter = false;
  fullQueryStringIncidents = CONST_STRING.empty;
  constructor(private dataService: DataService) {
    this.dataService.setUrlActive(HttpServiceTOCall.INCIDENTS_LIST);
    this.filterDataIncidents.dateType = ConstOrders.defaultDateInit;
    this.filterDataIncidents.dateFull = this.dataService.getDateFormatted(new Date(), new Date(), true);
    this.filterDataIncidents.isFromIncidents = true;

  }

  ngOnInit() {
    this.subscriptionObservables.add(this.dataService.getNewSearchOrdersModal().subscribe(resultSearchOrderModal => {
      console.log('resultObs;: ', resultSearchOrderModal)
      if (resultSearchOrderModal.isFromIncidents) {
        this.onSuccessSearchOrderModal(resultSearchOrderModal);
      }
    }));
  }

  onSuccessSearchOrderModal(resultSearchOrderModal: ParamsPedidos) {
    this.pageIndex = 0;
    this.offset = 0;
    this.limit = 10;
    this.filterDataIncidents = this.dataService.getNewDataToFilter(resultSearchOrderModal)[0];
    this.queryString = this.dataService.getNewDataToFilter(resultSearchOrderModal)[1];
    this.isSearchWithFilter = this.dataService.getIsWithFilter(resultSearchOrderModal);
    this.getFullQueryString();
    // this.getPedidos();
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
    this.fullQueryStringIncidents = `${this.queryString}&offset=${this.offset}&limit=${this.limit}`;
  }
}

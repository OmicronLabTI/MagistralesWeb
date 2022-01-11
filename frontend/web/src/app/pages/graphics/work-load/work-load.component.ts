import { Component, OnInit } from '@angular/core';
import { DataService } from '../../../services/data.service';
import { CONST_NUMBER, HttpServiceTOCall } from '../../../constants/const';
import { PedidosService } from '../../../services/pedidos.service';
import { ErrorService } from '../../../services/error.service';
import { WorkLoad } from '../../../model/http/pedidos';
import { ObservableService } from '../../../services/observable.service';
import { DateService } from '../../../services/date.service';

@Component({
  selector: 'app-work-load',
  templateUrl: './work-load.component.html',
  styleUrls: ['./work-load.component.scss']
})
export class WorkLoadComponent implements OnInit {
  initialDate: string[] = [];
  startDate = new Date();
  minStartDate = new Date();
  finishDate = new Date();
  maxFinishDate = new Date();
  workLoads: WorkLoad[] = [];
  workLoadTotals: WorkLoad[] = [];
  constructor(
    private dataService: DataService,
    private ordersService: PedidosService,
    private errorService: ErrorService,
    private observableService: ObservableService,
    private dateService: DateService) {
    this.observableService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
    this.initialDate = this.dateService.transformDate(this.startDate).split('/');
    this.startDate = new Date(`${this.initialDate[1]}/01/${this.initialDate[2]}`);
    this.getMaxDates();
  }

  ngOnInit() {
  }

  private getMaxDates() {
    this.minStartDate = this.dateService.getMaxMinDate(this.finishDate, CONST_NUMBER.three, false);
    this.maxFinishDate = this.dateService.getMaxMinDate(this.startDate, CONST_NUMBER.three, true);
    this.getWorkLoad();
  }

  onDataChange() {
    this.getMaxDates();
  }

  private getWorkLoad() {
    this.ordersService.getWorLoad(this.dateService.getDateFormatted(this.startDate, this.finishDate, false))
      .subscribe(workLoadRes => {
        workLoadRes.response.forEach(workLoad => {
          workLoad.totalPossibleAssign = this.dataService.getFormattedNumber(workLoad.totalPossibleAssign);
          workLoad.assigned = this.dataService.getFormattedNumber(workLoad.assigned);
          workLoad.processed = this.dataService.getFormattedNumber(workLoad.processed);
          workLoad.pending = this.dataService.getFormattedNumber(workLoad.pending);
          workLoad.finished = this.dataService.getFormattedNumber(workLoad.finished);
          workLoad.reassigned = this.dataService.getFormattedNumber(workLoad.reassigned);
          workLoad.finalized = this.dataService.getFormattedNumber(workLoad.finalized);
          workLoad.totalOrders = this.dataService.getFormattedNumber(workLoad.totalOrders);
          workLoad.totalFabOrders = this.dataService.getFormattedNumber(workLoad.totalFabOrders);
          workLoad.totalPieces = this.dataService.getFormattedNumber(workLoad.totalPieces);
        });
        this.workLoads = workLoadRes.response.filter(workLoad => workLoad.user !== 'Total');
        this.workLoadTotals = workLoadRes.response.filter(workLoad => workLoad.user === 'Total');
      }
        , error => this.errorService.httpError(error));
  }
}

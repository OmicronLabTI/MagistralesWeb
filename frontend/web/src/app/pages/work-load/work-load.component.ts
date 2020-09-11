import { Component, OnInit } from '@angular/core';
import {DataService} from '../../services/data.service';
import {CONST_NUMBER, HttpServiceTOCall} from '../../constants/const';
import {PedidosService} from '../../services/pedidos.service';
import {ErrorService} from '../../services/error.service';
import {WorkLoad} from '../../model/http/pedidos';

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
  constructor(private dataService: DataService, private ordersService: PedidosService,
              private errorService: ErrorService) {
    this.dataService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
    this.initialDate = this.dataService.transformDate(this.startDate).split('/');
    this.startDate = new Date(`${this.initialDate[1]}/01/${this.initialDate[2]}`);
    this.getMaxDates();
  }

  ngOnInit() {
  }

  private getMaxDates() {
    this.minStartDate = new Date(
                this.finishDate.getFullYear(),
          this.finishDate.getMonth() - CONST_NUMBER.three,
                this.finishDate.getDate());
    this.maxFinishDate = new Date(
              this.startDate.getFullYear(),
        this.startDate.getMonth() + CONST_NUMBER.three,
              this.startDate.getDate());
    this.getWorkLoad();
  }

  onDataChange() {
    this.getMaxDates();
  }

  private getWorkLoad() {
    this.ordersService.getWorLoad(this.dataService.getDateFormatted(this.startDate, this.finishDate, false))
        .subscribe(workLoadRes => {
             this.workLoads = workLoadRes.response;
             console.log('workLoads: ', this.workLoads)
            }
        , error => this.errorService.httpError(error));
  }
}

import { Component, OnInit } from '@angular/core';
import {DataService} from '../../services/data.service';
import {CONST_NUMBER, CONST_STRING, MODAL_FIND_ORDERS} from '../../constants/const';

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
  constructor(private dataService: DataService) {
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
  }

  onDataChange() {
    this.getMaxDates();
  }
}

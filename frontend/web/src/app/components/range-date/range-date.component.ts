import {Component, Input, OnInit, Output, EventEmitter} from '@angular/core';
import {CONST_NUMBER, TypeInitialRange} from '../../constants/const';
import {DataService} from '../../services/data.service';

@Component({
  selector: 'app-range-date',
  templateUrl: './range-date.component.html',
  styleUrls: ['./range-date.component.scss']
})
export class RangeDateComponent implements OnInit {
  @Input() maxMonthsRange = CONST_NUMBER.lessOne;
  @Input() typeInitialRange = CONST_NUMBER.lessOne;
  @Output() newRangeEvent = new EventEmitter<string>();
  initialDate: string[] = [];
  startDate = new Date();
  finishDate = new Date();
  minStartDate = new Date();
  maxFinishDate = new Date();
  constructor(private dataService: DataService) {
  }

  ngOnInit() {
    switch (Number(this.typeInitialRange)) {
      case TypeInitialRange.monthCalendar:
        this.initialDate = this.dataService.getDateArray(this.startDate);
        this.startDate = new Date(`${this.initialDate[1]}/01/${this.initialDate[2]}`);
        this.onDataChange();
        break;
    }
  }

  onDataChange() {
    this.minStartDate = this.dataService.getMaxMinDate(this.finishDate, this.maxMonthsRange, false);
    this.maxFinishDate = this.dataService.getMaxMinDate(this.startDate, this.maxMonthsRange, true);
    this.newRangeEvent.emit(this.dataService.getDateFormatted(this.startDate, this.finishDate, false));
  }
}

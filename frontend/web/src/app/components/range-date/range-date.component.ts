import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { CONST_NUMBER, CONST_STRING, TypeInitialRange } from '../../constants/const';
import { DateService } from '../../services/date.service';

@Component({
  selector: 'app-range-date',
  templateUrl: './range-date.component.html',
  styleUrls: ['./range-date.component.scss']
})
export class RangeDateComponent implements OnInit {
  @Input() maxMonthsRange = CONST_NUMBER.lessOne;
  @Input() typeInitialRange = CONST_NUMBER.lessOne;
  @Input() titleRangeDate = CONST_STRING.empty;
  @Output() newRangeEvent = new EventEmitter<string>();
  initialDate: string[] = [];
  startDate = new Date();
  finishDate = new Date();
  minStartDate = new Date();
  maxFinishDate = new Date();
  constructor(
    private dateService: DateService,
  ) { }

  ngOnInit() {
    /*switch (Number(this.typeInitialRange)) {
      case TypeInitialRange.monthCalendar:
        this.initialDate = this.dataService.getDateArray(this.startDate);
        this.startDate = new Date(`${this.initialDate[1]}/01/${this.initialDate[2]}`);
        this.onDataChange();
        break;
    }*/
    if (Number(this.typeInitialRange) === TypeInitialRange.monthCalendar) {
      this.initialDate = this.dateService.getDateArray(this.startDate);
      this.startDate = new Date(`${this.initialDate[1]}/01/${this.initialDate[2]}`);
      this.onDataChange();
    }
  }

  onDataChange() {
    this.minStartDate = this.dateService.getMaxMinDate(this.finishDate, Number(this.maxMonthsRange), false);
    this.maxFinishDate = this.dateService.getMaxMinDate(this.startDate, Number(this.maxMonthsRange), true);
    this.newRangeEvent.emit(this.dateService.getDateFormatted(this.startDate, this.finishDate, false));
  }
}

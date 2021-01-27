import {Component, Input, OnInit, Output, EventEmitter} from '@angular/core';
import {CONST_NUMBER} from '../../constants/const';

@Component({
  selector: 'app-range-date',
  templateUrl: './range-date.component.html',
  styleUrls: ['./range-date.component.scss']
})
export class RangeDateComponent implements OnInit {
  @Input() range = CONST_NUMBER.zero;
  @Output() newRangeEvent = new EventEmitter<number>();
  constructor() { }

  ngOnInit() {
  }

  eventEmitter() {
    const addRange = Number(this.range) + 11;
    console.log('event Emitter; ', addRange);
    this.newRangeEvent.emit(addRange);
  }
}

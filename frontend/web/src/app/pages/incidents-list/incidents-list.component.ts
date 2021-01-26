import {Component, OnInit} from '@angular/core';
import {DataService} from '../../services/data.service';
import {HttpServiceTOCall} from '../../constants/const';

@Component({
  selector: 'app-incidents-list',
  templateUrl: './incidents-list.component.html',
  styleUrls: ['./incidents-list.component.scss']
})
export class IncidentsListComponent implements OnInit {

  constructor(private dataService: DataService) {
    this.dataService.setUrlActive(HttpServiceTOCall.INCIDENTS_LIST);
  }

  ngOnInit() {

  }

}

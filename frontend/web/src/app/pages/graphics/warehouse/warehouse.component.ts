import {Component, OnInit} from '@angular/core';
import {DataService} from '../../../services/data.service';
import {HttpServiceTOCall} from '../../../constants/const';

@Component({
  selector: 'app-warehouse',
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.scss']
})
export class WarehouseComponent implements OnInit {

  constructor(private dataService: DataService) {
    this.dataService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
  }

  ngOnInit() {
  }

}

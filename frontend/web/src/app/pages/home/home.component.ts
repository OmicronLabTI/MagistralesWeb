import { Component, OnInit } from '@angular/core';
import {DataService} from "../../services/data.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  constructor(private _dataService: DataService) {
    console.log('tokenServiceHome: ', this._dataService.userIsAuthenticated())
  }

  ngOnInit() {
  }

}

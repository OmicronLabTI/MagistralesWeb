import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-material-request',
  templateUrl: './material-request.component.html',
  styleUrls: ['./material-request.component.scss']
})
export class MaterialRequestComponent implements OnInit {
  dataToRequest = {};
  displayedColumns: string[] = [
    'code', 'component', 'requestQuantity', 'unit'
  ];
  constructor(private router: Router) {
    this.dataToRequest = this.router.getCurrentNavigation().extras.state;
    console.log('dataRequest2: ', this.dataToRequest);
  }

  ngOnInit() {
  }

    addNewComponent() {
        console.log('added component: ')
    }

  signUser() {
    console.log('signingUser')
  }

  sendRequest() {
    console.log('sendingRequest')
  }

  cancelRequest() {
    console.log('cancelRequest')
  }
}

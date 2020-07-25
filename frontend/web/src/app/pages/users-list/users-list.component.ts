import { Component, OnInit } from '@angular/core';
import {UserList} from "../../model/http/userList";

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent implements OnInit {
  checkedGeneral = false;
  listUserData: UserList [] = [
    {
      "id": 0,
      "name": "benny",
      "lastName": "alvarado",
      "rol": "administrador",
      "status": "inactivo"
    },
    {
      "id": 1,
      "name": "berenice",
      "lastName": "amezaga",
      "rol": "administrador",
      "status": "activo"
    },
    {
      "id": 2,
      "name": "irma",
      "lastName": "hernandez",
      "rol": "gfb",
      "status": "inactivo"
    },
    {
      "id": 3,
      "name": "erika",
      "lastName": "villalba",
      "rol": "administrador",
      "status": "activo"
    },
    {
      "id": 4,
      "name": "mayte",
      "lastName": "rivera",
      "rol": "qfb",
      "status": "inactivo"
    }
  ];
  constructor() { }
  checkGeneralEvent(){
    console.log('value checked general: ', this.checkedGeneral)
    this.checkedGeneral = !this.checkedGeneral;
  }
  ngOnInit() {
  }

}

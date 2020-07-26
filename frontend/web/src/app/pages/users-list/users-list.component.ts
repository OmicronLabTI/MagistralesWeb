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
  constructor() {
    this.listUserData.map(obj =>{
      obj.checkedRow = false;
    });
  }
  checkPrincipalEvent(){
    this.checkedGeneral = !this.checkedGeneral;
   if(this.checkedGeneral){
     this.changeAllUserTCheckRow(true);
   }else {
     this.changeAllUserTCheckRow(false);
   }

  }
  checkGeneralEvent(userId: number, isChecked: boolean){
      this.checkedGeneral = this.checkedGeneral ?!isChecked: this.checkedGeneral;

    this.listUserData.filter(user => user.id === userId)
        .forEach(user => user.checkedRow = !user.checkedRow)
  }

  changeAllUserTCheckRow(isChecked: boolean){
    this.listUserData = this.listUserData.map(user =>{
      user.checkedRow = isChecked;
      return user;
    });
  }
  deleteUser(){
    this.checkedGeneral = false;
    this.listUserData =     this.listUserData.filter(user => user.checkedRow === false);
  }

  ngOnInit() {


  }

}

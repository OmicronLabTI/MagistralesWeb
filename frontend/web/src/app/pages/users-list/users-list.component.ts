import { Component, OnInit } from '@angular/core';
import {UserList} from "../../model/http/userList";
import {ModalService} from "../../_modal";
import {DataService} from "../../services/data.service";

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.scss']
})
export class UsersListComponent implements OnInit {
  checkedGeneral = false;
  finishPage = 5;
  actualPage: number;
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
    },
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
    },
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
    },
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
  constructor(private modalService: ModalService, private dataService: DataService) {
    this.listUserData.map(obj =>{
      obj.checkedRow = false;
    });
    this.actualPage = 1;
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
  openModal(modalName: string) {
    this.dataService.setModalName(modalName);
    this.modalService.open(modalName);
  }

  closeModal(id: string) {
    this.modalService.close(id);
  }
  onScroll(){
    console.log('scrolled XDXDXD');
    if (this.actualPage < this.finishPage) {
      //method to call again to show more users
      this.actualPage ++;
    } else {
      console.log('No more lines. Finish page!');
    }
  }
  ngOnInit() {
  }


}

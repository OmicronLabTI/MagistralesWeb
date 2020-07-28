import { Component, OnInit } from '@angular/core';
import {UserList} from "../../model/http/userList";

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  displayedColumns: string[] = ['names', 'lastName', 'role', 'status'];
  dataSource : UserList[] = [
    {
      "id": 'dsafasdf',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    },
    {
      "id": 'dsafasdf',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    },
    {
      "id": 'dsafasdf',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    },
    {
      "id": 'dsafasdf',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    },
    {
      "id": 'dsafasdf',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    }
  ];
  constructor() {
    this.dataSource.map(user =>{
      var rObj = {};
      rObj[user.firstName] = user.firstName;
      rObj[user.lastName] = user.lastName;
      rObj[user.role] = user.role;
      rObj[user.activo] = user.activo;
      return rObj;
    })
  }
  ngOnInit() {
  }

}

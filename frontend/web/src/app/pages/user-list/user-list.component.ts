import { Component, OnInit } from '@angular/core';
import {IUserListRes, IUserReq} from "../../model/http/users";
import {MatDialog} from "@angular/material/dialog";
import {AddUserDialogComponent} from "../../dialogs/add-user-dialog/add-user-dialog.component";
import {UsersService} from "../../services/users.service";

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  isAllComplete = false;
  displayedColumns: string[] = ['delete','names', 'lastName', 'role', 'status','actions'];
  dataSource : IUserReq[] = [
    {
      "id": 'dsafasdf1',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    },
    {
      "id": 'dsafasdf2',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    },
    {
      "id": 'dsafasdf3',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    },
    {
      "id": 'dsafasdf4',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    },
    {
      "id": 'dsafasdf5',
      "userName": 'benny',
      "firstName":'benny',
      "lastName": "alvarado",
      "role": 1,
      "password":'123',
      "activo": 1
    }
  ];
  constructor(private dialog: MatDialog,private usersService: UsersService) {
    this.dataSource.map(user =>{
      user.isChecked = false;
    });
  }
  ngOnInit() {
    /*this.usersService.getUsers().subscribe((userRes: IUserListRes) => {
      console.log('user list: ',userRes);
      this.dataSource = userRes.response;
    })*/
  }

  updateAllComplete() {
    this.isAllComplete = this.dataSource.every(user => user.isChecked);
    console.log('data aupdate: ', this.dataSource)
  }

  someComplete(): boolean {
    return this.dataSource.filter(t => t.isChecked).length > 0 && !this.isAllComplete;
  }

  setAll(completed: boolean) {
    this.dataSource.forEach(t => t.isChecked = completed);
    console.log('data set all: ', this.dataSource)
  }


  deleteUsers() {
    console.log('to delete: ', this.dataSource.filter(user => user.isChecked).map(user =>{ return user.id}))
    this.dataSource = this.dataSource.filter(user => !user.isChecked)
/*
    this.usersService.deleteUsers(this.dataSource.filter(user => user.isChecked).map(user =>{ return user.id}));
*/
  }

  openDialog(modalTypeOpen: string) {
      this.dialog.open(AddUserDialogComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        modalType: modalTypeOpen
      }
    });

  }
}

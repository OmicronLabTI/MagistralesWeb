import { Component, OnInit } from '@angular/core';
import {UserList} from "../../model/http/userList";
import {MatDialog} from "@angular/material/dialog";
import {AddUserDialogComponent} from "../../dialogs/add-user-dialog/add-user-dialog.component";

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  isAllComplete = false;
  displayedColumns: string[] = ['delete','names', 'lastName', 'role', 'status','actions'];
  userList: UserList[] = [];
  dataSource : UserList[] = [
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
  constructor(private dialog: MatDialog) {
    this.dataSource.map(user =>{
      user.isChecked = false;
    });
  }
  ngOnInit() {
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

  onEdit(id: string) {
    console.log('edit id: ', id)

  }

  onDelete(id: string ) {
    console.log('delete id: ', id)
    this.dataSource = this.dataSource.filter(user => user.id !== id)
  }

  deleteMany() {
    console.log('to delete: ', this.dataSource.filter(user => user.isChecked))
    this.dataSource = this.dataSource.filter(user => !user.isChecked)
  }

  openDialog() {
    const dialogRef = this.dialog.open(AddUserDialogComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        animal: 'panda'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
    });
  }
}

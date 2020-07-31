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
  dataSource : IUserReq[];
  constructor(private dialog: MatDialog,private usersService: UsersService) {
  /*  this.dataSource.map(user =>{
      user.isChecked = false;
    });*/
  }
  ngOnInit() {
  this.getUsers();
  }
  getUsers(){
    this.usersService.getUsers().subscribe((userRes: IUserListRes) => {
      console.log('user list: ',userRes);
      this.dataSource = userRes.response;
      this.dataSource.map(user =>{
        user.isChecked = false;
      });

    })
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
    const idsToDelete = this.dataSource.filter(user => user.isChecked).map(user =>{ return user.id});
    console.log('before: ', idsToDelete)
    this.usersService.deleteUsers(idsToDelete).subscribe(
        resDelete =>{
          console.log('res delete: ',resDelete);
          this.dataSource = this.dataSource.filter(user => !user.isChecked)

        },
        error => {
          console.log('error delete: ', error)
        }
    );

  }

  openDialog(modalTypeOpen: string,userId : any) {
    let userToEdit: {};
    console.log('user: ', userId);
    if(userId !== ''){
      userToEdit = this.dataSource.filter(user => user.id === userId)[0];
      console.log('user: ', userToEdit);
    }

    const dialogRef = this.dialog.open(AddUserDialogComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        modalType: modalTypeOpen,
        userToEditM: userToEdit
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
      this.getUsers();
    });

  }
}

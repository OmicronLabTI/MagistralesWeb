import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {MODAL_NAMES} from "../../../environments/environment";
import {UsersService} from "../../services/users.service";
import {IRolesRes, IUserReq, RoleUser} from "../../model/http/users";

@Component({
  selector: 'app-add-user-dialog',
  templateUrl: './add-user-dialog.component.html',
  styleUrls: ['./add-user-dialog.component.scss']
})
export class AddUserDialogComponent implements OnInit {
  addUserForm: FormGroup;
  isForEditModal: boolean;
  userRoles: RoleUser[] = [];
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private formBuilder: FormBuilder,
              private usersService:UsersService) {
    this.isForEditModal = this.data.modalType === MODAL_NAMES.editUser;
    console.log('data drom: ', this.data.modalType,'modal names: ', MODAL_NAMES)

    this.addUserForm = this.formBuilder.group({
      userName:['',[Validators.required,Validators.maxLength(50)]],
      firstName:['',[Validators.required,Validators.maxLength(50)]],
      lastName:['',[Validators.required,Validators.maxLength(50)]],
      role:['',Validators.required],
      password:['', [Validators.required,Validators.maxLength(8)]],
      activo:['', Validators.required]
    });
  }

  ngOnInit() {
    /*this.usersService.getRoles().subscribe((rolesRes:IRolesRes) => {
      console.log('rolesRes: ', rolesRes);
      this.userRoles = rolesRes.response;
    })*/
    if(!this.isForEditModal){
      this.addUserForm.get('activo').setValue(1);
      console.log('data add: ', this.addUserForm.value)
    }
  }

  saveUser() {
    const user: IUserReq = this.addUserForm.value;
    user.activo = Number(user.activo);
    user.role = Number(user.role);
    console.log('value user: ', user)
    ///this.usersService.createUser(user).subscribe( resUser => console.log('resUser: ', resUser))

  }
}

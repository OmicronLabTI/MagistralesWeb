import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {UsersService} from '../../services/users.service';
import {IRolesRes, IUserReq, RoleUser} from '../../model/http/users';
import {ErrorService} from '../../services/error.service';
import {CONST_USER_DIALOG, MODAL_NAMES} from '../../constants/const';

@Component({
  selector: 'app-add-user-dialog',
  templateUrl: './add-user-dialog.component.html',
  styleUrls: ['./add-user-dialog.component.scss']
})
export class AddUserDialogComponent implements OnInit {
  userToEdit: IUserReq;
  addUserForm: FormGroup;
  isForEditModal: boolean;
  userRoles: RoleUser[] = [];
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private formBuilder: FormBuilder,
              private usersService: UsersService, private errorService: ErrorService) {
    this.isForEditModal = this.data.modalType === MODAL_NAMES.editUser;
    this.userToEdit = this.data.userToEditM;

    this.addUserForm = this.formBuilder.group({
      userName: ['', [Validators.required, Validators.maxLength(50)]],
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      userTypeR: ['', Validators.required],
      password: ['', [Validators.required, Validators.pattern(CONST_USER_DIALOG.patternPassWord)]],
      activo: ['', Validators.required]
    });

  }

  ngOnInit() {
    this.usersService.getRoles().subscribe((rolesRes: IRolesRes) => {
     this.userRoles = rolesRes.response;
     this.addUserForm.get('userTypeR').
        setValue(!this.isForEditModal ? this.userRoles.
        filter(user =>
          CONST_USER_DIALOG.defaultDefault.toLowerCase() === user.description.toLocaleLowerCase())[0].id.toString() :
          this.userToEdit.role.toString());
    }, error => this.errorService.httpError(error));

    if (!this.isForEditModal) {
      this.addUserForm.get('activo').setValue(1);
    } else {
      console.log('edit data: ', this.userToEdit);
      this.addUserForm.get('userName').setValue(this.userToEdit.userName);
      this.addUserForm.get('firstName').setValue(this.userToEdit.firstName);
      this.addUserForm.get('lastName').setValue(this.userToEdit.lastName);
      this.addUserForm.get('password').setValue(this.userToEdit.password);
      this.addUserForm.get('activo').setValue(this.userToEdit.activo.toString());

    }
  }

  saveUser() {


    if (!this.isForEditModal) {
      const user: IUserReq = {
        userName: this.addUserForm.get('userName').value,
        firstName: this.addUserForm.get('firstName').value,
        lastName: this.addUserForm.get('lastName').value,
        role: Number(this.addUserForm.get('userTypeR').value),
        password: this.addUserForm.get('password').value,
        activo: Number(this.addUserForm.get('activo').value)
      };
      this.usersService.createUser(user).subscribe( () => {},
          error => this.errorService.httpError(error));
    } else {
      const user: IUserReq = {
        id: this.userToEdit.id,
        userName: this.addUserForm.get('userName').value,
        firstName: this.addUserForm.get('firstName').value,
        lastName: this.addUserForm.get('lastName').value,
        role: Number(this.addUserForm.get('userTypeR').value),
        password: this.addUserForm.get('password').value,
        activo: Number(this.addUserForm.get('activo').value)
      };
      console.log('value user edit: ', user);
      this.usersService.updateUser(user).subscribe( () => {},
          error => this.errorService.httpError(error));
    }

  }
}

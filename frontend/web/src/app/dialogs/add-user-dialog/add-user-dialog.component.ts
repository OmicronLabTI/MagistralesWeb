import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {UsersService} from '../../services/users.service';
import { IUserReq, RoleUser} from '../../model/http/users';
import {ErrorService} from '../../services/error.service';
import {CONST_USER_DIALOG, HttpServiceTOCall, HttpStatus, MODAL_NAMES} from '../../constants/const';
import {DataService} from '../../services/data.service';
import {Messages} from '../../constants/messages';
import {SweetAlertIcon} from 'sweetalert2';
import {Subscription} from 'rxjs';
import {ErrorHttpInterface} from '../../model/http/commons';

@Component({
  selector: 'app-add-user-dialog',
  templateUrl: './add-user-dialog.component.html',
  styleUrls: ['./add-user-dialog.component.scss']
})
export class AddUserDialogComponent implements OnInit, OnDestroy {
  userToEdit: IUserReq;
  addUserForm: FormGroup;
  isForEditModal: boolean;
  userRoles: RoleUser[] = [];
  subscription = new Subscription();
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private formBuilder: FormBuilder,
              private usersService: UsersService, private errorService: ErrorService,
              private dataService: DataService,
              private dialogRef: MatDialogRef<AddUserDialogComponent>) {
    this.isForEditModal = this.data.modalType === MODAL_NAMES.editUser;
    this.userToEdit = this.data.userToEditM;

    this.addUserForm = this.formBuilder.group({
      userName: ['', [Validators.required, Validators.maxLength(50)]],
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      userTypeR: ['', Validators.required],
      password: ['', [Validators.required, Validators.pattern(CONST_USER_DIALOG.patternPassWord), ]],
      activo: ['', Validators.required]
    });

  }

  ngOnInit() {
    this.subscription = this.addUserForm.valueChanges.subscribe(valueForm => {
      if (valueForm.userName) {
        this.addUserForm.get('userName').setValue(
            valueForm.userName.normalize('NFD').replace(/[\u0300-\u036f]/g, ''), { emitEvent: false });
      }
    });
    this.usersService.getRoles().subscribe(rolesRes => {
     this.userRoles = rolesRes.response;
     this.addUserForm.get('userTypeR').
        setValue(!this.isForEditModal ? this.userRoles.
        filter(user =>
          CONST_USER_DIALOG.defaultQfb.toLowerCase() === user.description.toLocaleLowerCase())[0].id.toString() :
          this.userToEdit.role.toString());
    }, error => {
      this.errorService.httpError(error);
      this.dialogRef.close();
    });

    if (!this.isForEditModal) {
      this.addUserForm.get('activo').setValue(1);
    } else {
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
      this.usersService.createUser(user).subscribe( () => {
            this.createMessageOk(Messages.success, 'success', false);
          },
          error => this.userExistDialog(error));
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
      this.usersService.updateUser(user).subscribe( () => {
        this.createMessageOk(Messages.success, 'success', false);
          },
          error => this.userExistDialog(error));
    }

  }
  createMessageOk(title: string, icon: SweetAlertIcon, isButtonAccept: boolean) {
    this.dataService.setCallHttpService(HttpServiceTOCall.USERS);
    this.dataService.setMessageGeneralCallHttp({title, icon, isButtonAccept});
  }
  ngOnDestroy() {
   this.subscription.unsubscribe();
  }
  userExistDialog(error: ErrorHttpInterface) {
    if (error.status === HttpStatus.badRequest) {
      this.createMessageOk(Messages.userExist, 'info', true);
    } else {
      this.errorService.httpError(error);
    }
  }
}

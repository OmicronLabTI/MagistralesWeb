import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Subscription} from 'rxjs';
import {CONST_STRING, MODAL_FIND_ORDERS} from '../../constants/const';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {UsersService} from '../../services/users.service';
import {RoleUser} from '../../model/http/users';
import {ErrorService} from '../../services/error.service';
import {DataService} from '../../services/data.service';

@Component({
  selector: 'app-search-users-dialog',
  templateUrl: './search-users-dialog.component.html',
  styleUrls: ['./search-users-dialog.component.scss']
})
export class SearchUsersDialogComponent implements OnInit, OnDestroy {
  searchUserForm: FormGroup;
  subscription = new Subscription();
  isCorrectData = false;
  userRoles: RoleUser[] = [];
  constructor(private formBuilder: FormBuilder, private dialogRef: MatDialogRef<SearchUsersDialogComponent>,
              private usersService: UsersService, private errorService: ErrorService,
              @Inject(MAT_DIALOG_DATA) public searchData: any, private dataService: DataService) {
    this.searchUserForm = this.formBuilder.group({
      userNameSe: ['', [Validators.required, Validators.maxLength(50)]],
      firstNameSe: ['', [Validators.required, Validators.maxLength(50)]],
      lastNameSe: ['', [Validators.required, Validators.maxLength(50)]],
      userTypeRSe: ['', Validators.required],
      activoSe: ['', [Validators.required]],
      asignableSe: ['', [Validators.required]]
    });
  }

  ngOnInit() {
    this.subscription.add(this.searchUserForm.valueChanges.subscribe(searchValue => {
      if (searchValue.userNameSe) {
        this.searchUserForm.get('userNameSe').setValue(
            this.dataService.getNormalizeString(searchValue.userNameSe), { emitEvent: false });
      }
      this.isCorrectData = this.searchUserForm.get('userNameSe').value !== CONST_STRING.empty ||
                           this.searchUserForm.get('firstNameSe').value !== CONST_STRING.empty ||
                           this.searchUserForm.get('lastNameSe').value !== CONST_STRING.empty ||
                           this.searchUserForm.get('userTypeRSe').value !== CONST_STRING.empty ||
                           this.searchUserForm.get('activoSe').value !== CONST_STRING.empty ||
                           this.searchUserForm.get('asignableSe').value !== CONST_STRING.empty;
    }));
    this.usersService.getRoles().subscribe(rolesRes => {
      this.userRoles = rolesRes.response;
      this.searchUserForm.get('userTypeRSe').
      setValue(this.searchData.userTypeRSe && this.searchData.userTypeRSe !== CONST_STRING.empty ? this.searchData.userTypeRSe :
          CONST_STRING.empty);
    }, error => {
      this.errorService.httpError(error);
      this.dialogRef.close();
    });
    this.searchUserForm.get('userNameSe').setValue(this.searchData.userNameSe && this.searchData.userNameSe !== '' ?
                                                    this.searchData.userNameSe : CONST_STRING.empty);
    this.searchUserForm.get('firstNameSe').setValue(this.searchData.firstNameSe && this.searchData.firstNameSe !== '' ?
                                                    this.searchData.firstNameSe : CONST_STRING.empty);
    this.searchUserForm.get('lastNameSe').setValue(this.searchData.lastNameSe && this.searchData.lastNameSe !== '' ?
                                                    this.searchData.lastNameSe : CONST_STRING.empty);
    this.searchUserForm.get('activoSe').setValue(this.searchData.activoSe && this.searchData.activoSe !== '' ?
                                                    this.searchData.activoSe : CONST_STRING.empty);
    this.searchUserForm.get('asignableSe').setValue(this.searchData.asignableSe && this.searchData.asignableSe !== '' ?
                                                     this.searchData.asignableSe : CONST_STRING.empty);
  }


  searchUser() {
    this.dialogRef.close(this.searchUserForm.value);
  }

  resetSearchParams() {
    this.searchUserForm.get('userNameSe').setValue('');
    this.searchUserForm.get('firstNameSe').setValue('');
    this.searchUserForm.get('lastNameSe').setValue('');
    this.searchUserForm.get('userTypeRSe').setValue('');
    this.searchUserForm.get('activoSe').setValue('');
    this.searchUserForm.get('asignableSe').setValue('');
    this.isCorrectData = true;
  }
  keyDownUsers(event: KeyboardEvent) {
    if (event.key === MODAL_FIND_ORDERS.keyEnter && ((this.searchUserForm.get('userNameSe').value !== CONST_STRING.empty
        && this.searchUserForm.get('userNameSe').value !== null) || (this.searchUserForm.get('firstNameSe').value !== CONST_STRING.empty
        && this.searchUserForm.get('firstNameSe').value !== null) || (this.searchUserForm.get('lastNameSe').value !== CONST_STRING.empty
        && this.searchUserForm.get('lastNameSe').value !== null) || (this.searchUserForm.get('userTypeRSe').value !== CONST_STRING.empty
        && this.searchUserForm.get('userTypeRSe').value !== null) || (this.searchUserForm.get('activoSe').value !== CONST_STRING.empty
        && this.searchUserForm.get('activoSe').value !== null) || (this.searchUserForm.get('asignableSe').value !== CONST_STRING.empty
        && this.searchUserForm.get('asignableSe').value !== null) )) {
      this.searchUser();
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

}

import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CONST_STRING, MODAL_FIND_ORDERS } from '../../constants/const';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UsersService } from '../../services/users.service';
import { Clasification, RoleUser } from '../../model/http/users';
import { ErrorService } from '../../services/error.service';
import { DataService } from '../../services/data.service';

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
  clasifications: Clasification[] = [];
  constructor(
    private formBuilder: FormBuilder,
    private dialogRef: MatDialogRef<SearchUsersDialogComponent>,
    private usersService: UsersService,
    private errorService: ErrorService,
    @Inject(MAT_DIALOG_DATA) public searchData: any,
    private dataService: DataService) {
    this.searchUserForm = this.formBuilder.group({
      userNameSe: ['', [Validators.maxLength(50)]],
      firstNameSe: ['', [Validators.maxLength(50)]],
      lastNameSe: ['', [Validators.maxLength(50)]],
      userTypeRSe: ['', []],
      activoSe: ['', []],
      asignableSe: ['', []],
      classificationQFBSe: ['', []]
    });
  }

  ngOnInit(): void {
    this.getClassifications();
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
        this.searchUserForm.get('asignableSe').value !== CONST_STRING.empty ||
        this.searchUserForm.get('classificationQFBSe').value !== CONST_STRING.empty;
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
  }

  setFormValues(): void {
    this.setControlValue('userNameSe', this.searchData.userNameSe);
    this.setControlValue('firstNameSe', this.searchData.firstNameSe);
    this.setControlValue('lastNameSe', this.searchData.lastNameSe);
    this.setControlValue('activoSe', this.searchData.activoSe);
    this.setControlValue('asignableSe', this.searchData.asignableSe);
    this.setControlValue('classificationQFBSe', this.searchData.classificationQFBSe);
  }

  setControlValue(formControlName: string, value: string): void {
    this.searchUserForm.get(formControlName).setValue(value !== null && value !== undefined ?
      value :
      CONST_STRING.empty);
  }
  getClassifications(): void {
    this.usersService.getClasifications().subscribe((res) => {
      this.clasifications = this.deleteDuplicateClasifications(res.response);
      this.setFormValues();
    }, error => {
      this.errorService.httpError(error);
      this.dialogRef.close();
    });
  }

  deleteDuplicateClasifications(clasifications: Clasification[]): Clasification[] {
    const clasificationList = Array.from(
      new Map(clasifications.map(clasification => [clasification.value, clasification])).values()
    );
    return clasificationList;
  }

  searchUser(): void {
    this.dialogRef.close(this.searchUserForm.value);
  }

  resetSearchParams(): void {
    this.searchUserForm.get('userNameSe').setValue('');
    this.searchUserForm.get('firstNameSe').setValue('');
    this.searchUserForm.get('lastNameSe').setValue('');
    this.searchUserForm.get('userTypeRSe').setValue('');
    this.searchUserForm.get('activoSe').setValue('');
    this.searchUserForm.get('asignableSe').setValue('');
    this.searchUserForm.get('classificationQFBSe').setValue('');
    this.isCorrectData = true;
  }
  keyDownUsers(event: KeyboardEvent): void {
    const controls = ['userNameSe', 'firstNameSe', 'lastNameSe', 'userTypeRSe', 'activoSe', 'asignableSe', 'classificationQFBSe'];
    if (event.key === MODAL_FIND_ORDERS.keyEnter && this.validateFormHasSomeValue(controls)) {
      this.searchUser();
    }
  }
  validateFormHasSomeValue(formControls: string[]): boolean {
    return formControls.some((control) => this.searchUserForm.get(control).value !== CONST_STRING.empty &&
      this.searchUserForm.get(control).value !== null);
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

}

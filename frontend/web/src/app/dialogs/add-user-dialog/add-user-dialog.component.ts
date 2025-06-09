import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UsersService } from '../../services/users.service';
import { Clasification, IAddUserDialogConfig, IUserReq, RoleUser, TechnicalUser } from '../../model/http/users';
import { ErrorService } from '../../services/error.service';
import {
  AllClasification,
  CONST_NUMBER, CONST_STRING,
  CONST_USER_DIALOG,
  HttpServiceTOCall,
  HttpStatus, MODAL_FIND_ORDERS,
  MODAL_NAMES,
  TypeClasifications
} from '../../constants/const';
import { DataService } from '../../services/data.service';
import { Messages } from '../../constants/messages';
import { SweetAlertIcon } from 'sweetalert2';
import { Subscription } from 'rxjs';
import { ErrorHttpInterface } from '../../model/http/commons';
import { ObservableService } from 'src/app/services/observable.service';
import { userClasificationMock } from 'src/mocks/userListMock';

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
  clasifications: Clasification[] = [];
  activeClasifications: Clasification[] = [];
  qfbRolId = 0;
  technicalUser: TechnicalUser[] = [];
  clasificationArray: string[] = [];
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: IAddUserDialogConfig,
    private formBuilder: FormBuilder,
    private usersService: UsersService,
    private errorService: ErrorService,
    public dataService: DataService,
    private dialogRef: MatDialogRef<AddUserDialogComponent>,
    private observableService: ObservableService) {

    this.isForEditModal = this.data.modalType === MODAL_NAMES.editUser;
    this.userToEdit = this.data.userToEditM;

    this.addUserForm = this.formBuilder.group({
      userName: ['', [Validators.required, Validators.maxLength(50)]],
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      userTypeR: ['', Validators.required],
      password: ['', [Validators.required, Validators.pattern(CONST_USER_DIALOG.patternPassWord)]],
      activo: ['', [Validators.required]],
      piezas: [CONST_USER_DIALOG.defaultNumberOfPieces, [Validators.required, Validators.maxLength(5)]],
      asignable: ['', [Validators.required]],
      classificationQFB: ['', [Validators.required]],
      clasificationLogDis: ['', [Validators.required]],
      requireTechnical: [false, [Validators.required]],
      technical: ['', [Validators.required]]
    });
  }

  ngOnInit() {
    this.subscription = this.addUserForm.valueChanges.subscribe(valueForm => {
      this.formValuesChanges(valueForm);
    });
    this.callServices();
  }

  async getRoles(): Promise<void> {
    return this.usersService.getRoles().toPromise().then((rolesRes) => {
      this.userRoles = rolesRes.response;
    }).catch((error) => this.callErrorOnService(error));
  }

  async getClassifications(): Promise<void> {
    return this.usersService.getClasifications().toPromise().then((res) => {
      const clasificationList = Object.assign(res.response, []);
      this.clasifications = [AllClasification, ...clasificationList];
      this.activeClasifications = this.clasifications;
    }).catch((error) => this.callErrorOnService(error));
  }

  async getTehcnicalUsers(): Promise<void> {
    return this.usersService.getTehcnicalUsers().toPromise().then((res) => {
      this.technicalUser = res.response;
    }).catch((error) => this.callErrorOnService(error));
  }

  callErrorOnService(error: ErrorHttpInterface): void {
    this.errorService.httpError(error);
    this.dialogRef.close();
  }
  async callServices(): Promise<void> {
    await Promise.all([this.getRoles(), this.getClassifications(), this.getTehcnicalUsers()]).then(() => {
      this.setFormValues();
    });
  }

  getDefaultRol(): string {
    return this.userRoles.filter(user => CONST_USER_DIALOG.defaultQfb.toLowerCase()
      === user.description.toLocaleLowerCase())[0].id.toString();
  }
  formValuesChanges(valueForm: any): void {
    if (valueForm.userName) {
      this.addUserForm.get('userName').setValue(
        this.dataService.getNormalizeString(valueForm.userName), { emitEvent: false });
    }
    if (valueForm.piezas) {
      this.addUserForm.get('piezas').setValue(this.getOnlyNumbers(valueForm.piezas), { emitEvent: false });
    }
    if (valueForm.userTypeR) {
      this.changeUserTypeValue(valueForm.userTypeR);
    }
    if (valueForm.requireTechnical) {
      this.requireTechnicalChange();
    }
    if (valueForm.classificationQFB) {
      this.changeClasification(this.addUserForm.get('piezas').value);
    }
  }

  requireTechnicalChange(): void {
    if (this.addUserForm.get('requireTechnical').value !== '1') {
      this.addUserForm.get('technical').setValue('', { onlySelf: true, emitEvent: false });
      this.addUserForm.get('technical').disable({ onlySelf: true, emitEvent: false });
    }
  }

  changeUserTypeValue(userTypeR: string): void {
    if (userTypeR === '9') {
      this.clearTechnical();
      this.addUserForm.get('asignable').enable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('piezas').disable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('classificationQFB').disable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('requireTechnical').disable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('technical').disable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('clasificationLogDis').disable({ onlySelf: true, emitEvent: false });
      this.addUserForm.updateValueAndValidity({ onlySelf: true, emitEvent: false });
    } else if (userTypeR && userTypeR !== '2') {
      this.clearTechnical();
      this.addUserForm.get('piezas').disable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('asignable').disable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('classificationQFB').disable({ onlySelf: true, emitEvent: false });
      this.disableclassificationQFBForm(userTypeR);
      this.addUserForm.get('requireTechnical').disable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('technical').disable({ onlySelf: true, emitEvent: false });
      this.addUserForm.updateValueAndValidity({ onlySelf: true, emitEvent: false });
    }
    if (userTypeR === '2') {
      this.addUserForm.get('piezas').enable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('asignable').enable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('classificationQFB').enable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('requireTechnical').enable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('technical').enable({ onlySelf: true, emitEvent: false });
      this.addUserForm.get('clasificationLogDis').disable({ onlySelf: true, emitEvent: false });
      this.requireTechnicalChange();
      this.addUserForm.updateValueAndValidity({ onlySelf: true, emitEvent: false });
    }
  }

  disableclassificationQFBForm(userTypeR: string) {
    if (this.dataService.calculateOrValueList([userTypeR === '3', userTypeR === '4'])) {
      this.addUserForm.get('clasificationLogDis').enable({ onlySelf: true, emitEvent: false });
    } else {
      this.addUserForm.get('clasificationLogDis').disable({ onlySelf: true, emitEvent: false });
    }
  }

  clearTechnical(): void {
    this.addUserForm.get('requireTechnical').setValue('0', { emitEvent: false });
    this.addUserForm.get('technical').setValue('', { emitEvent: false });
  }

  getRol(id: number): string {
    return String(this.userRoles.find(item => item.id === id).description);
  }


  changeClasification(quantity = 200): void {
    const selection = String(this.addUserForm.get('classificationQFB').value);
    if (selection.toUpperCase() === TypeClasifications.dermazone.toUpperCase()) {
      this.addUserForm.get('piezas').setValue(0, { emitEvent: false });
      this.addUserForm.get('piezas').disable({ emitEvent: false });
    } else {
      this.addUserForm.get('piezas').enable({ emitEvent: false });
      this.addUserForm.get('piezas').setValue(`${quantity}`, { emitEvent: false });
    }
  }

  validateClasification(): void {
    const rolId = Number(this.addUserForm.get('userTypeR').value);
    const rol = this.getRol(rolId);
    if (rol.toUpperCase() === CONST_USER_DIALOG.defaultQfb.toUpperCase()) {
      this.activeClasifications = this.clasifications.filter(clasification => clasification.classificationQfb);
    } else {
      this.activeClasifications = this.clasifications.filter(clasification => !clasification.classificationQfb);
    }
  }
  setFormValues(): void {
    this.qfbRolId = this.userRoles.find(rol => rol.description.toLowerCase() === CONST_USER_DIALOG.defaultQfb.toLowerCase()).id;
    if (!this.isForEditModal) {
      this.addUserForm.get('requireTechnical').setValue('0');
      this.addUserForm.get('userTypeR').setValue(this.getDefaultRol());
      this.addUserForm.get('asignable').setValue(CONST_NUMBER.one.toString());
      this.addUserForm.get('activo').setValue(CONST_NUMBER.one);
    } else {
      this.addUserForm.get('userTypeR').setValue(this.userToEdit.role.toString());
      this.addUserForm.get('userName').setValue(this.userToEdit.userName);
      this.addUserForm.get('firstName').setValue(this.userToEdit.firstName);
      this.addUserForm.get('lastName').setValue(this.userToEdit.lastName);
      this.addUserForm.get('password').setValue(atob(this.userToEdit.password));
      this.addUserForm.get('activo').setValue(this.userToEdit.activo.toString());
      this.addUserForm.get('piezas').setValue(this.userToEdit.piezas);
      this.addUserForm.get('asignable').setValue(this.userToEdit.asignable.toString());
      this.setUpdateClasification(this.userToEdit.role);
      this.addUserForm.get('requireTechnical').setValue(this.userToEdit.technicalRequire ? '1' : '0');
      this.addUserForm.get('technical').setValue(this.userToEdit.tecnicId);
      this.changeClasification(this.userToEdit.piezas);
      this.addUserForm.updateValueAndValidity({ onlySelf: true, emitEvent: false });
    }
    this.validateClasification();
  }

  setUpdateClasification(role: number) {
    if (this.dataService.calculateOrValueList([role === 3, role === 4])) {
      const clasificationsLogDis = this.dataService.calculateTernary(
        this.userToEdit.classification !== '',
        this.userToEdit.classification.split(','),
        []
      );
      this.addUserForm.get('clasificationLogDis').setValue(clasificationsLogDis);
    } else {
      this.addUserForm.get('classificationQFB').setValue(this.userToEdit.classification);
    }
  }

  save() {
    const requireTecnic = String(this.addUserForm.get('requireTechnical').value) === '1';
    const tecnicId = requireTecnic ? this.addUserForm.get('technical').value : null;
    if (!this.isForEditModal) {
      this.saveUser(requireTecnic, tecnicId);
    } else {
      this.updateUser(requireTecnic, tecnicId);
    }
  }

  buildClasification(): string {
    const userTypeR = this.addUserForm.get('userTypeR').value;
    if (this.dataService.calculateOrValueList([userTypeR === '3', userTypeR === '4'])) {
      this.clasificationArray = this.addUserForm.get('clasificationLogDis').value;
      return this.dataService.calculateTernary(
        this.clasificationArray.includes(TypeClasifications.todas),
        TypeClasifications.todas,
        this.clasificationArray.join(',')
      );
    } else {
      return this.addUserForm.get('classificationQFB').value;
    }
  }

  saveUser(requireTecnic: boolean, tecnicId?: string): void {
    const user: IUserReq = {
      ...this.addUserForm.value,
      password: btoa(this.addUserForm.get('password').value),
      role: Number(this.addUserForm.get('userTypeR').value),
      asignable: Number(this.addUserForm.get('asignable').value),
      piezas: Number(this.addUserForm.get('piezas').value),
      classification: this.buildClasification(),
      technicalRequire: requireTecnic,
      tecnicId
    };
    this.usersService.createUserService(user).subscribe(() => {
      this.createMessageOk(Messages.success, 'success', false);
      this.dialogRef.close();
    }, error => this.userExistDialog(error));
  }

  updateUser(requireTecnic: boolean, tecnicId?: string): void {
    const user: IUserReq = {
      ...this.addUserForm.value,
      id: this.userToEdit.id,
      password: btoa(this.addUserForm.get('password').value),
      role: Number(this.addUserForm.get('userTypeR').value),
      asignable: Number(this.addUserForm.get('asignable').value),
      activo: Number(this.addUserForm.get('activo').value),
      piezas: Number(this.addUserForm.get('piezas').value),
      classification: this.buildClasification(),
      technicalRequire: requireTecnic,
      tecnicId
    };
    this.usersService.updateUser(user).subscribe(() => {
      this.createMessageOk(Messages.success, 'success', false);
      this.dialogRef.close();
    }, error => this.userExistDialog(error));
  }

  createMessageOk(title: string, icon: SweetAlertIcon, isButtonAccept: boolean) {
    this.observableService.setCallHttpService(HttpServiceTOCall.USERS);
    this.observableService.setMessageGeneralCallHttp({ title, icon, isButtonAccept });
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

  getOnlyNumbers(pieces: string) {
    const numbers = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
    let newNumbers = CONST_STRING.empty;
    // tslint:disable-next-line:prefer-for-of
    for (let index = 0; index < pieces.length; index++) {
      newNumbers += numbers.includes(pieces.charAt(index)) ? pieces.charAt(index).trim() : CONST_STRING.empty.trim();
    }
    return newNumbers;
  }

  keyDownFunction(event: KeyboardEvent) {
    if (event.key === MODAL_FIND_ORDERS.keyEnter && this.addUserForm.valid) {
      this.save();
    }
  }

}

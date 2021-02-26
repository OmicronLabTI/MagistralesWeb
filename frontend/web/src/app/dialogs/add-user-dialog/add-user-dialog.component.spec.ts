import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AddUserDialogComponent } from './add-user-dialog.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {
  MatCardModule,
  MatFormFieldModule, MatInputModule,
  MatDialogModule,
  MatSelectModule
} from '@angular/material';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {UsersService} from '../../services/users.service';
import {of, throwError} from 'rxjs';
import {RolesMock} from '../../../mocks/rolesMock';
import {ErrorService} from '../../services/error.service';
import {DataService} from '../../services/data.service';

describe('AddUserDialogComponent', () => {
  let component: AddUserDialogComponent;
  let fixture: ComponentFixture<AddUserDialogComponent>;
  let userServiceSpy;
  let errorServiceSpy;
  let dataServiceSpy;
  const close = () => {};
  const userEditSpec = {
    activo: 0,
    asignable: 1,
    firstName: 'a44',
    id: '12516967-1a01-488b-b6ec-4ebd9500d3ee',
    isChecked: false,
    lastName: '34',
    password: 'QXhpdHkyMDIwaGh4eA==',
    piezas: 10,
    role: 3,
    userName: 'a44',
    classification: 'BE'
  };
  beforeEach(async(() => {
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'presentToastCustom', 'getCallHttpService', 'setMessageGeneralCallHttp', 'setUrlActive',
        'setCallHttpService', 'setMessageGeneralCallHttp', 'getNormalizeString'
    ]);
    userServiceSpy = jasmine.createSpyObj<UsersService>('UsersService', [
      'getRoles', 'createUserService' , 'updateUser'
    ]);
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);

    userServiceSpy.getRoles.and.callFake(() => {
      return of(RolesMock);
    });
    userServiceSpy.updateUser.and.callFake(() => {
      return of({});
    });
    userServiceSpy.createUserService.and.callFake(() => {
      return of({});
    });
    TestBed.configureTestingModule({
      imports: [FormsModule, HttpClientTestingModule, MatCardModule, ReactiveFormsModule,
        MatDialogModule,
        BrowserAnimationsModule,
        MatFormFieldModule,
          MatSelectModule,
        MatInputModule],
      declarations: [ AddUserDialogComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        {
          provide: MatDialogRef,
          useValue: {close}
        },
        { provide: MAT_DIALOG_DATA, useValue: {userToEditM: userEditSpec} } ,
        { provide: UsersService, useValue: userServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
          DatePipe
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should formDefined', () => {
    expect(component.addUserForm).toBeDefined();
  });
  it('should ngOnInit create ok', () => {
    component.isForEditModal = false;
    component.ngOnInit();
    expect(userServiceSpy.getRoles).toHaveBeenCalled();
    expect(component.userRoles).toEqual(RolesMock.response);
    expect(component.addUserForm.get('userTypeR').value).toEqual('2');
    expect(component.addUserForm.get('activo').value).toEqual(1);
    component.isForEditModal = true;
    component.ngOnInit();
    expect(component.userRoles).toEqual(RolesMock.response);
    expect(component.addUserForm.get('userTypeR').value).toEqual('3');

    expect(component.addUserForm.get('firstName').value).toEqual(userEditSpec.firstName);
    expect(component.addUserForm.get('lastName').value).toEqual(userEditSpec.lastName);
    expect(component.addUserForm.get('password').value).toEqual( atob('QXhpdHkyMDIwaGh4eA=='));
    expect(component.addUserForm.get('activo').value).toEqual(userEditSpec.activo.toString());
    expect(component.addUserForm.get('piezas').value).toEqual(userEditSpec.piezas);
    expect(component.addUserForm.get('asignable').value).toEqual(userEditSpec.asignable.toString());
    expect(component.addUserForm.get('classificationQFB').value).toEqual(userEditSpec.classification);
  });
  it('should ngOnInit create faild', () => {
    userServiceSpy.getRoles.and.callFake(() => {
      return throwError({ error: true });
    });
    component.ngOnInit();
    expect(errorServiceSpy.httpError).toHaveBeenCalledWith({ error: true });
  });
  it('should createUser defaultValues Form', () => {
    userServiceSpy.getRoles.and.callFake(() => {
      return throwError({ error: true });
    });
    component.ngOnInit();
    expect(errorServiceSpy.httpError).toHaveBeenCalledWith({ error: true });
  });
  it('should saveUser() ', () => {
      component.isForEditModal = false;
      component.saveUser();
      expect(userServiceSpy.createUserService).toHaveBeenCalled();
      expect(dataServiceSpy.setCallHttpService).toHaveBeenCalled();
      expect(dataServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
      component.isForEditModal = false;
      userServiceSpy.createUserService.and.callFake(() => {
          return throwError({ status: 400 });
      });
      component.saveUser();
      expect(dataServiceSpy.setCallHttpService).toHaveBeenCalled();
      expect(dataServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();

      component.isForEditModal = false;
      userServiceSpy.createUserService.and.callFake(() => {
        return throwError({ status: 401 });
      });
      component.saveUser();
      expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });
  it('should editUser() ', () => {
    component.isForEditModal = true;
    component.saveUser();
    expect(userServiceSpy.updateUser).toHaveBeenCalled();
    expect(dataServiceSpy.setCallHttpService).toHaveBeenCalled();
    expect(dataServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
    component.isForEditModal = true;

    userServiceSpy.updateUser.and.callFake(() => {
      return throwError({ status: 400 });
    });
    component.saveUser();
    expect(dataServiceSpy.setCallHttpService).toHaveBeenCalled();
    expect(dataServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();

    component.isForEditModal = true;
    userServiceSpy.updateUser.and.callFake(() => {
      return throwError({ status: 401 });
    });
    component.saveUser();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });
});

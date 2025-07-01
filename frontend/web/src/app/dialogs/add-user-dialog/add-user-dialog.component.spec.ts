import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AddUserDialogComponent } from './add-user-dialog.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import {
  MatCardModule,
  MatFormFieldModule, MatInputModule,
  MatDialogModule,
  MatSelectModule
} from '@angular/material';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { UsersService } from '../../services/users.service';
import { of, throwError } from 'rxjs';
import { RolesMock } from '../../../mocks/rolesMock';
import { ErrorService } from '../../services/error.service';
import { DataService } from '../../services/data.service';
import { ObservableService } from 'src/app/services/observable.service';
import { MessagesService } from 'src/app/services/messages.service';

describe('AddUserDialogComponent', () => {
  let component: AddUserDialogComponent;
  let fixture: ComponentFixture<AddUserDialogComponent>;
  let userServiceSpy: jasmine.SpyObj<UsersService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;

  const close = () => { };
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
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom'
    ]);

    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService',
      [
        'getNormalizeString',
        'calculateOrValueList'
      ]);
    userServiceSpy = jasmine.createSpyObj<UsersService>('UsersService',
      [
        'getRoles',
        'createUserService',
        'updateUser',
        'getClasifications',
        'getTehcnicalUsers'
      ]);
    userServiceSpy.getTehcnicalUsers.and.returnValue(of({
      response: [
        {
          id: '1',
          firstName: 'Daniel',
          lastName: 'Perez'
        },
        {
          id: '2',
          firstName: 'Daniel',
          lastName: 'Perez'
        },
      ]
    }));
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService',
      [
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
    dataServiceSpy.calculateOrValueList.and.returnValue(false);

    userServiceSpy.getClasifications.and.returnValue(of({
      response: [
        { value: 'MN', description: 'Bioelite (MN)', color: '#FBC115' },
        { value: 'BE', description: 'Bioequal (BE)', color: '#FBC115' },
        { value: 'MG', description: 'Magistral (MG)', color: '#FBC115' },
        { value: 'DZ', description: 'Dermazon (DZ)', color: '#FBC115' }
      ]
    }));

    //  --- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'getCallHttpService',
        'setMessageGeneralCallHttp',
        'setUrlActive',
        'setCallHttpService',
      ]);
    TestBed.configureTestingModule({
      imports: [FormsModule, HttpClientTestingModule, MatCardModule, ReactiveFormsModule,
        MatDialogModule,
        BrowserAnimationsModule,
        MatFormFieldModule,
        MatSelectModule,
        MatInputModule],
      declarations: [AddUserDialogComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        {
          provide: MatDialogRef,
          useValue: { close }
        },
        { provide: MAT_DIALOG_DATA, useValue: { userToEditM: userEditSpec } },
        { provide: UsersService, useValue: userServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },
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
  it('should callServices create ok', () => {
    component.isForEditModal = false;
    component.callServices();
    expect(userServiceSpy.getRoles).toHaveBeenCalled();
  });

  it('should create when is edit mode', () => {
    component.isForEditModal = true;
    component.callServices();
    expect(userServiceSpy.getRoles).toHaveBeenCalled();
  });
  it('should save() ', () => {
    component.isForEditModal = false;
    component.save();
    expect(userServiceSpy.createUserService).toHaveBeenCalled();
    expect(observableServiceSpy.setCallHttpService).toHaveBeenCalled();
    expect(observableServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
    component.isForEditModal = false;
    userServiceSpy.createUserService.and.callFake(() => {
      return throwError({ status: 400 });
    });
    component.save();
    expect(observableServiceSpy.setCallHttpService).toHaveBeenCalled();
    expect(observableServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();

    component.isForEditModal = false;
    userServiceSpy.createUserService.and.callFake(() => {
      return throwError({ status: 401 });
    });
    component.save();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });
  it('should editUser() ', () => {
    component.isForEditModal = true;
    component.save();
    expect(userServiceSpy.updateUser).toHaveBeenCalled();
    expect(observableServiceSpy.setCallHttpService).toHaveBeenCalled();
    expect(observableServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
    component.isForEditModal = true;

    userServiceSpy.updateUser.and.callFake(() => {
      return throwError({ status: 400 });
    });
    component.save();
    expect(observableServiceSpy.setCallHttpService).toHaveBeenCalled();
    expect(observableServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();

    component.isForEditModal = true;
    userServiceSpy.updateUser.and.callFake(() => {
      return throwError({ status: 401 });
    });
    component.save();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });
  it('should validateClasification as QFB', () => {
    component.userRoles = RolesMock.response;
    component.clasifications = [
      {
        value: 'MN', description: 'Bioelite (MN)',
        color: '#FBC115'
      },
      {
        value: 'BE', description: 'Bioequal (BE)',
        color: '#FBC115'
      },
      {
        value: 'MG', description: 'Magistral (MG)',
        color: '#FBC115'
      },
      {
        value: 'DZ', description: 'Dermazon (DZ)',
        color: '#FBC115'
      }];
    component.addUserForm.get('userTypeR').setValue(RolesMock.response[0].id, { emitEvent: false });
    component.activeClasifications = [
      {
        value: 'MN', description: 'Bioelite (MN)',
        color: '#FBC115'
      },
      {
        value: 'BE', description: 'Bioequal (BE)',
        color: '#FBC115'
      },
      {
        value: 'MG', description: 'Magistral (MG)',
        color: '#FBC115'
      },
      {
        value: 'DZ', description: 'Dermazon (DZ)',
        color: '#FBC115'
      }];
    component.validateClasification();
    expect(component.activeClasifications.length).toBe(4);
  });
  it('should validateClasification as admin', () => {
    component.userRoles = RolesMock.response;
    component.addUserForm.get('userTypeR').setValue(RolesMock.response[2].id);
    component.activeClasifications = [
      {
        value: 'MN', description: 'Bioelite (MN)',
        color: '#FBC115'
      },
      {
        value: 'BE', description: 'Bioequal (BE)',
        color: '#FBC115'
      },
      {
        value: 'MG', description: 'Magistral (MG)',
        color: '#FBC115'
      },
      {
        value: 'DZ', description: 'Dermazon (DZ)',
        color: '#FBC115'
      }];
    component.validateClasification();
    expect(component.activeClasifications.length).toBe(0);
  });
  it('should changeClasification with DZ value', () => {
    component.addUserForm.get('classificationQFB').setValue('DZ');
    component.changeClasification();
    expect(component.addUserForm.get('piezas').value).toBe(0);
  });
  it('should changeClasification with BE value', () => {
    component.addUserForm.get('classificationQFB').setValue('BE');
    component.changeClasification(200);
    expect(component.addUserForm.get('piezas').value).toBe('200');
  });
});

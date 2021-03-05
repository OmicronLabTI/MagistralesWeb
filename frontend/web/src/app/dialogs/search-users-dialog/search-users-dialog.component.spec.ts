import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchUsersDialogComponent } from './search-users-dialog.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {UsersService} from '../../services/users.service';
import {ErrorService} from '../../services/error.service';
import {DataService} from '../../services/data.service';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatSelectModule} from '@angular/material/select';
import {MatCardModule} from '@angular/material/card';
import {MatInputModule} from '@angular/material/input';
import {of} from 'rxjs';
import {RolesMock} from '../../../mocks/rolesMock';

describe('SearchUsersDialogComponent', () => {
  let component: SearchUsersDialogComponent;
  let fixture: ComponentFixture<SearchUsersDialogComponent>;
  let dataServiceSpy;
  let userServiceSpy;
  let errorServiceSpy;
  beforeEach(async(() => {
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'presentToastCustom', 'getCallHttpService', 'setMessageGeneralCallHttp', 'setUrlActive',
      'setCallHttpService', 'setMessageGeneralCallHttp', 'getNormalizeString'
    ]);
    userServiceSpy = jasmine.createSpyObj<UsersService>('UsersService', [
      'getRoles', 'createUserService' , 'updateUser'
    ]);
    userServiceSpy.getRoles.and.callFake(() => {
      return of(RolesMock);
    });
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    TestBed.configureTestingModule({
      imports: [FormsModule, HttpClientTestingModule, MatCardModule, ReactiveFormsModule,
        MatDialogModule,
        BrowserAnimationsModule,
        MatFormFieldModule,
        MatSelectModule,
        MatInputModule],
      declarations: [ SearchUsersDialogComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        {
          provide: MatDialogRef,
          useValue: {close}
        },
        { provide: MAT_DIALOG_DATA, useValue: {
            activoSe: '0',
            asignableSe: '1',
            firstNameSe: 'name',
            lastNameSe: 'last',
            userNameSe: 'user',
            userTypeRSe: '2',
            classificationQFBSe: 'BE'
          } } ,
        { provide: UsersService, useValue: userServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: DataService, useValue: dataServiceSpy }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchUsersDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

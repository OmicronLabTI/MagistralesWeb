import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserListComponent } from './user-list.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserListRoutingModule } from './user-list-routing.module';
import {MatTableModule} from '@angular/material/table';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatDialogModule} from '@angular/material/dialog';
import {MatCardModule} from '@angular/material/card';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {UsersService} from '../../services/users.service';
import {of, Subject, throwError} from 'rxjs';
import {UserListMock} from '../../../mocks/userListMock';
import {DataService} from '../../services/data.service';
import { HttpServiceTOCall} from '../../constants/const';
import {ErrorService} from '../../services/error.service';
import {PageEvent} from '@angular/material/paginator';
import { RouterTestingModule } from '@angular/router/testing';

describe('UserListComponent', () => {
  let component: UserListComponent;
  let fixture: ComponentFixture<UserListComponent>;
  let userServiceSpy;
  let dataServiceSpy;
  let errorServiceSpy;
  beforeEach(async(() => {
    userServiceSpy = jasmine.createSpyObj<UsersService>('UsersService', [
      'getUsers'
    ]);
    userServiceSpy.getUsers.and.callFake(() => {
      return of(UserListMock);
    });
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'presentToastCustom', 'getCallHttpService', 'setMessageGeneralCallHttp', 'setUrlActive', 'getFormattedNumber'
    ]);
    dataServiceSpy.getCallHttpService.and.callFake(() => {
      const callHttpService = new Subject<HttpServiceTOCall>();
      return callHttpService.asObservable();
    });
    dataServiceSpy.presentToastCustom.and.callFake(() => {
      return new Promise(resolve => { resolve(''); });
    });
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    TestBed.configureTestingModule({
      imports: [        CommonModule,
        HttpClientTestingModule,
        UserListRoutingModule,
        MatTableModule,
        MatCheckboxModule,
        FormsModule,
        MatIconModule,
        MatButtonModule,
        MatDialogModule,
        MatCardModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        MatInputModule,
        MatSelectModule,
      RouterTestingModule],
      declarations: [ UserListComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [DatePipe,
        { provide: UsersService, useValue: userServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.offset).toEqual(0);
    expect(component.limit).toEqual(10);
    expect(component.pageSize).toEqual(10);
  });
  it('should call get Users Ok', () => {
    component.offset = 0;
    component.limit = 10;
    component.getUsers();
    expect(userServiceSpy.getUsers).toHaveBeenCalled();
    expect(component.lengthPaginator).toEqual(20);
    expect(component.dataSource.data).toEqual(UserListMock.response);
  });
  it('should call get Users Error', () => {
    userServiceSpy.getUsers.and.callFake(() => {
      return throwError({ error: true });
    });
    component.getUsers();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });
  it('should call updateAllComplete()', () => {
    component.dataSource.data = UserListMock.response;
    component.updateAllComplete();
    expect(component.isAllComplete).toBeFalsy();
    component.dataSource.data.forEach( user => user.isChecked = true);
    component.updateAllComplete();
    expect(component.isAllComplete).toBeTruthy();
  });
  it('should call someComplete()', () => {
    component.dataSource.data = UserListMock.response;
    component.isAllComplete = false;
    expect(component.someComplete()).toBeFalsy();
    component.dataSource.data.forEach( user => user.isChecked = true);
    expect(component.someComplete()).toBeTruthy();
  });
  it('should call setAll()', () => {
    component.dataSource.data = UserListMock.response;
    component.setAll(true);
    expect(component.dataSource.data.every(user => user.isChecked)).toBeTruthy();
    component.setAll(false);
    expect(component.dataSource.data.every(user => user.isChecked)).toBeFalsy();
  });
  it('should call deleteUsers()', () => {
    component.dataSource.data = UserListMock.response;
    component.deleteUsers('cda94a8a-366e-46c9-b120-68c6edce3c44');
    expect(component.dataSource.data.filter(user => user.id === 'cda94a8a-366e-46c9-b120-68c6edce3c44')[0].isChecked).toBeTruthy();
    expect(dataServiceSpy.presentToastCustom).toHaveBeenCalled();
  });
  it('should call createMessageHttpOk()', () => {
    component.createMessageHttpOk();
    expect(dataServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
  });
  it('should call changeDataEvent', () => {
    expect(component.changeDataEvent({pageSize: 10, pageIndex: 0} as PageEvent)).toEqual({pageSize: 10, pageIndex: 0} as PageEvent);
    expect(component.offset).toEqual(0);
    expect(component.limit).toEqual(10);
  });

});

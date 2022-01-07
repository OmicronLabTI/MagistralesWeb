import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginComponent } from './login.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { DataService } from 'src/app/services/data.service';
import { SecurityService } from 'src/app/services/security.service';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {of, throwError} from 'rxjs';
import {LoginMock} from '../../../mocks/loginMock';
import { Router } from '@angular/router';
import { HttpStatus, MODAL_FIND_ORDERS } from 'src/app/constants/const';
import { ErrorService } from 'src/app/services/error.service';
import { ErrorHttpInterface } from 'src/app/model/http/commons';
import { IUserRes, UserRes } from 'src/app/model/http/users';
describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let securityServiceSpy: jasmine.SpyObj<SecurityService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let errorServiceSpy;
  const routerSpy = {
    navigate: jasmine.createSpy('navigate')
  };
  const iUserRes = new IUserRes();
  const userRes: UserRes = new UserRes();

  iUserRes.response = userRes;

  beforeEach(async(() => {

    securityServiceSpy = jasmine.createSpyObj<SecurityService>('SecurityService', [
      'login', 'getUser'
    ]);
    securityServiceSpy.login.and.callFake(() => {
      return of(LoginMock);
    });
    securityServiceSpy.getUser.and.callFake(() => {
      return of(iUserRes);
    });
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'setToken',
      'setIsLogin',
      'setUserName',
      'userIsAuthenticated',
      'setGeneralNotificationMessage',
      'setRefreshToken',
      'setRememberSession',
      'setUserId',
      'setUserRole',
      'setMessageGeneralCallHttp',
      'getUserRole',
    ]);
    dataServiceSpy.setMessageGeneralCallHttp.and.callFake(() => {
      return;
    });
    dataServiceSpy.setUserId.and.callFake(() => {
      return;
    });
    dataServiceSpy.setUserName.and.callFake(() => {
      return;
    });
    dataServiceSpy.getUserRole.and.callFake(() => {
      return '';
    });
    TestBed.configureTestingModule({
      declarations: [ LoginComponent ],
      imports: [
        BrowserAnimationsModule,
        ReactiveFormsModule,
        RouterTestingModule,
        HttpClientTestingModule,
        MATERIAL_COMPONENTS
      ],
      providers: [
        DatePipe,
        { provide: SecurityService, useValue: securityServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: Router, useValue: routerSpy },
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should form defined', () => {
    expect(component.formLogin).toBeDefined();
  });
  it('should login call', () => {
    component.formLogin.get('username').setValue('user');
    component.formLogin.get('password').setValue('pass');
    component.login();
    expect(securityServiceSpy.login).toHaveBeenCalled();
  });
  it('should login getUser error', () => {
    // component.formLogin.get('username').setValue('user');
    // component.formLogin.get('password').setValue('pass');
    securityServiceSpy.getUser.and.callFake(() => {
      return throwError({ error: true });
    });
    component.login();
    // HttpStatus.serverError;
    expect(securityServiceSpy.login).toHaveBeenCalled();
    // expect(errorServiceSpy.httpError).toHaveBeenCalled();
    // expect(securityServiceSpy.login).toHaveBeenCalled();
  });
  it('should gotoPedidos', () => {
    component.goToPedidos();
    expect(routerSpy.navigate).toHaveBeenCalled();
  });

  it('should KeyDownFunction', () => {
    const fb: FormBuilder = new FormBuilder();
    component.formLogin = fb.group({
      username: [''],
      password: [''],
        rememberSession: [false, []]
    });
    // component.findOrdersForm.get('docNumDxp').setValue('XXXPOK');
    const keyEvent = new KeyboardEvent('keyEnter', { code: 'Digit0', key: MODAL_FIND_ORDERS.keyEnter});
    component.keyDownFunction(keyEvent);
    // expect(MockDialogRef.close).toHaveBeenCalled();
  });

  it('Should evaluatedGoTo if dataService.getUserRole = 3 || 4 || 5', () => {
    dataServiceSpy.getUserRole.and.callFake(() => {
      return '3';
    });
    component.evaluatedGoTo();
  });

  it('Should evaluatedGoTo if dataService.getUserRole = 7', () => {
    dataServiceSpy.getUserRole.and.callFake(() => {
      return '7';
    });
    component.evaluatedGoTo();
  });
});

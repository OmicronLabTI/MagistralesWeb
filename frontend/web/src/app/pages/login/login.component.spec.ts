import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginComponent } from './login.component';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { DataService } from 'src/app/services/data.service';
import { SecurityService } from 'src/app/services/security.service';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {of} from 'rxjs';
import {LoginMock} from '../../../mocks/loginMock';
describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let securityServiceSpy;
  let dataServiceSpy;


  beforeEach(async(() => {
    securityServiceSpy = jasmine.createSpyObj<SecurityService>('SecurityService', [
      'login', 'getUser'
    ]);
    securityServiceSpy.login.and.callFake(() => {
      return of(LoginMock);
    });
    securityServiceSpy.getUser.and.callFake(() => {
      return of({});
    });
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'setToken', 'setIsLogin', 'setUserName', 'userIsAuthenticated', 'setGeneralNotificationMessage'
    ]);
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
        { provide: DataService, useValue: dataServiceSpy }
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
  /*  //spyOn(dataServiceSpy, 'setToken');
    expect(dataServiceSpy.setToken).
    expect(dataServiceSpy.setIsLogin).toHaveBeenCalled();
    expect(dataServiceSpy.setUserName).toHaveBeenCalled();
    //expect(securityServiceSpy.getUser).toHaveBeenCalled();*/

  });
});

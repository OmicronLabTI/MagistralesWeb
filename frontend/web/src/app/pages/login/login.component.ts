import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SecurityService } from 'src/app/services/security.service';
import { ILoginReq } from 'src/app/model/http/security.model';
import { DataService } from 'src/app/services/data.service';
import { Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { ConstLogin, ConstToken, defaultClasificationColor, HttpStatus, MODAL_FIND_ORDERS,
  RolesType, RouterPaths } from '../../constants/const';
import { ErrorService } from '../../services/error.service';
import { ErrorHttpInterface } from '../../model/http/commons';
import { Messages } from '../../constants/messages';
import { ObservableService } from 'src/app/services/observable.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { UsersService } from 'src/app/services/users.service';
import { userClasificationMock } from 'src/mocks/userListMock';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  formLogin: FormGroup;
  hide = true;
  constructor(
    private fb: FormBuilder,
    private securityService: SecurityService,
    private dataService: DataService,
    private router: Router,
    private titleService: Title,
    private errorService: ErrorService,
    private observableService: ObservableService,
    private localStorageService: LocalStorageService,
    private userService: UsersService
  ) {
    if (this.localStorageService.userIsAuthenticated()) {
      this.evaluatedGoTo();
    }
    this.formLogin = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      rememberSession: [false, []]
    });
  }

  ngOnInit() {
    this.titleService.setTitle('OmicronLab - Login');
  }

  login() {
    const userLoginReq = {
      user: this.formLogin.get('username').value,
      password: btoa(this.formLogin.get('password').value),
      redirectUri: ConstLogin.defaultRedirectUri,
      clientId2: ConstLogin.defaultClientId2,
      origin: ConstLogin.defaultOrigin
    } as ILoginReq;
    this.securityService.login(userLoginReq).toPromise().then(async res => {
      this.localStorageService.setToken(res.access_token);
      this.localStorageService.setRefreshToken(res.refresh_token);
      if (this.formLogin.get('rememberSession').value) {
        this.localStorageService.setRememberSession(ConstToken.rememberSession);
      }
      await this.securityService.getUser(userLoginReq.user).toPromise().then(
        userRes => {
          this.localStorageService.setUserId(userRes.response.id);
          this.localStorageService.setUserName(`${userRes.response.firstName} ${userRes.response.lastName}`);
          this.localStorageService.setUserRole(userRes.response.role);
          this.localStorageService.setUserClasification(userRes.response.classification);
        }
      ).catch((error) => {
        this.errorService.httpError(error);
        this.observableService.setGeneralNotificationMessage('Error al obtener usuario');
      });
      await this.userService.getClasifications().toPromise().then(clasificationsResponse => {
        clasificationsResponse.response.forEach(clasification => {
          clasification.color = this.dataService.calculateTernary(
            this.dataService.validHexadecimalColor(clasification.color),
            clasification.color,
            defaultClasificationColor
          );
        });
        this.localStorageService.setClasificationList(clasificationsResponse.response);
      }).catch(error => {
        this.errorService.httpError(error);
        this.observableService.setGeneralNotificationMessage('Error al obtener las clasificaciones');
      });
      this.observableService.setIsLogin(true);
      this.evaluatedGoTo();
    }).catch((error: ErrorHttpInterface) => {
      switch (error.status) {
        case HttpStatus.serverError:
          this.observableService.setMessageGeneralCallHttp({ title: Messages.credentialsInvalid, icon: 'warning', isButtonAccept: true });
          break;
        case HttpStatus.unauthorized:
          this.observableService.setMessageGeneralCallHttp({ title: error.error.userError, icon: 'warning', isButtonAccept: true });
          break;
        default:
          this.errorService.httpError(error);
      }
    });
  }

  goToPedidos() {
    this.router.navigate(['pedidos']);
  }

  goToUsers() {
    this.router.navigate(['userList']);
  }
  keyDownFunction(event: KeyboardEvent) {
    if (event.key === MODAL_FIND_ORDERS.keyEnter && this.formLogin.valid) {
      this.login();
    }
  }

  evaluatedGoTo() {
    if (this.localStorageService.getUserRole() === RolesType.logistic || this.localStorageService.getUserRole() === RolesType.design
      || this.localStorageService.getUserRole() === RolesType.warehouse) {
      this.goToPedidos();
    } else if (this.localStorageService.getUserRole() === RolesType.incidents) {
      this.router.navigate([RouterPaths.incidentsList]);
    } else {
      this.goToUsers();
    }
  }
}

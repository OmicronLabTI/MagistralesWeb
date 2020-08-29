import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SecurityService } from 'src/app/services/security.service';
import { ILoginReq } from 'src/app/model/http/security.model';
import { DataService } from 'src/app/services/data.service';
import { Router } from '@angular/router';
import { Title } from '@angular/platform-browser';
import {ConstLogin, ConstToken, HttpStatus, MODAL_FIND_ORDERS} from '../../constants/const';
import {ErrorService} from '../../services/error.service';
import {ErrorHttpInterface} from '../../model/http/commons';
import {Messages} from '../../constants/messages';

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
    private errorService: ErrorService
  ) {
    if (this.dataService.userIsAuthenticated()) {
      this.goToPedidos();
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
        password: this.formLogin.get('password').value,
        redirectUri: ConstLogin.defaultRedirectUri,
        clientId2: ConstLogin.defaultClientId2,
        origin: ConstLogin.defaultOrigin
    } as ILoginReq;
    this.securityService.login(userLoginReq).toPromise().then(async res => {
      this.dataService.setToken(res.access_token);
      this.dataService.setRefreshToken(res.refresh_token);
      if (this.formLogin.get('rememberSession').value) {
          this.dataService.setRememberSession(ConstToken.rememberSession);
      }
      await this.securityService.getUser(userLoginReq.user).toPromise().then(
          userRes => {
              this.dataService.setUserId(userRes.response.id);
              this.dataService.setUserName(`${userRes.response.firstName} ${userRes.response.lastName}`);
          }
      ).catch((error) => {
          this.errorService.httpError(error);
          this.dataService.setGeneralNotificationMessage('Error al obtener usuario');
      });
      this.dataService.setIsLogin(true);
      this.goToPedidos();
    }).catch( (error: ErrorHttpInterface) => {
        switch (error.status) {
            case HttpStatus.serverError:
                this.dataService.setMessageGeneralCallHttp({title: Messages.credentialsInvalid, icon: 'warning', isButtonAccept: true});
                break;
            case HttpStatus.unauthorized:
                this.dataService.setMessageGeneralCallHttp({title: error.error.userError, icon: 'warning', isButtonAccept: true});
                break;
            default:
                this.errorService.httpError(error);
        }
    });
  }
  goToPedidos() {
    this.router.navigate(['pedidos']);
  }
  keyDownFunction(event: KeyboardEvent) {
        if (event.key === MODAL_FIND_ORDERS.keyEnter && this.formLogin.valid) {
            this.login();
        }
  }

}

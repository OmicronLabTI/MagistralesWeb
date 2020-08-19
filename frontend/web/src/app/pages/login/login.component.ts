import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SecurityService } from 'src/app/services/security.service';
import { ILoginReq } from 'src/app/model/http/security.model';
import { DataService } from 'src/app/services/data.service';
import { Router } from '@angular/router';
import { Title } from '@angular/platform-browser';

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
    private titleService: Title
  ) {
    if (this.dataService.userIsAuthenticated()) {
      this.goToPedidos();
    }
    this.formLogin = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.titleService.setTitle('OmicronLab - Login');
  }

   login() {
    const userLoginReq = {
      user: this.formLogin.get('username').value,
      password: this.formLogin.get('password').value,
      redirectUri: 'asdad',
      clientId2: ''
    } as ILoginReq;
    this.securityService.login(userLoginReq).toPromise().then(async res => {
      this.dataService.setToken(res.access_token);
      await this.securityService.getUser(userLoginReq.user).toPromise().then(
          userRes => {
              this.dataService.setUserId(userRes.response.id);
              this.dataService.setUserName(`${userRes.response.firstName} ${userRes.response.lastName}`);
          }
      ).catch(() => {
        this.dataService.setGeneralNotificationMessage('Error al obtener usuario');
      });
      this.dataService.setIsLogin(true);
      this.goToPedidos();
    }).catch(() => {
      this.dataService.setGeneralNotificationMessage('Credenciales inválidas.');
    });
  }
  goToPedidos() {
    this.router.navigate(['pedidos']);
  }
}

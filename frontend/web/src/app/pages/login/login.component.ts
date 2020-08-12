import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SecurityService } from 'src/app/services/security.service';
import { ILoginReq } from 'src/app/model/http/security.model';
import { DataService } from 'src/app/services/data.service';
import { Router } from '@angular/router';
import { IUserRes } from 'src/app/model/http/users';

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
    private router: Router
  ) {
    if (this.dataService.userIsAuthenticated()) {
      this.goToPedidos();
    }
    this.formLogin = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  ngOnInit() { }

  async login() {
    const userLoginReq = {
      user: this.formLogin.get('username').value,
      password: this.formLogin.get('password').value,
      redirectUri: 'asdad',
      clientId2: ''
    } as ILoginReq;
    await this.securityService.login(userLoginReq).toPromise().then(res => {
      this.dataService.setToken(res.access_token);
      this.securityService.getUser(userLoginReq.user).subscribe(
          userRes => {
            console.log('userRes: ', userRes)
            this.dataService.setUserId(userRes.response.id);
            this.dataService.setUserName(`${userRes.response.firstName} ${userRes.response.lastName}`);
          }
      );
      this.dataService.setIsLogin(true);
      this.goToPedidos();
    }).catch(err => {
      console.log('error  login: ', err);
      this.dataService.setGeneralNotificationMessage('Credenciales inválidas.');
    })
  }
  goToPedidos() {
    this.router.navigate(['pedidos']);
  }
}

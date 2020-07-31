import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SecurityService } from 'src/app/services/security.service';
import { ILoginReq } from 'src/app/model/http/security.model';
import { DataService } from 'src/app/services/data.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  formLogin: FormGroup;
  gridColumns = 3; //delete
  hide = true;
  constructor(
    private fb: FormBuilder,
    private securityService: SecurityService,
    private dataService: DataService,
    private router: Router
  ) {
    this.formLogin = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  ngOnInit() { }

  login() {
    const userLoginReq = {
      user: this.formLogin.get('username').value,
      password: this.formLogin.get('password').value,
      redirectUri: 'asdad',
      clientId2:''
    } as ILoginReq;
    console.log('to req user: ', userLoginReq)
    this.securityService.login(userLoginReq).subscribe(res => {
      console.log('acces token: ', res)
      this.dataService.setToken(res.access_token);
      this.dataService.setIsLogin(true);
      this.router.navigate(['home']);
    }, err => {
      console.log('error  login: ', err)
      this.dataService.setGeneralNotificationMessage('Credenciales inválidas.');
    }
    );
  }


  toggleGridColumns() {///delete
    this.gridColumns = this.gridColumns === 3 ? 4 : 3;
  }
}

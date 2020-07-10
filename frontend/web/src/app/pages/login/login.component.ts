import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SecurityService } from 'src/app/services/security.service';
import { ILoginReq } from 'src/app/model/http/security.model';
import { DataService } from 'src/app/services/data.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  formLogin: FormGroup;

  constructor(private _fb: FormBuilder, private _securityService: SecurityService, private _dataService: DataService, private _router: Router) {
    this.formLogin = this._fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    // TEST ONLY
    this.formLogin.get('username').setValue('eve.holt@reqres.in')
    this.formLogin.get('password').setValue('cityslicka')
  }

  ngOnInit() {
  }

  login() {
    let data = <ILoginReq>{
      email: this.formLogin.get('username').value,
      password: this.formLogin.get('password').value
    }

    this._securityService.login(data).subscribe(res => {
      this._dataService.setToken(res.token);
      this._router.navigate(['home']);
    }, err => {
      this._dataService.setGeneralNotificationMessage(err);
    });
  }
}

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

    // TEST ONLY
    this.formLogin.get('username').setValue('eve.holt@reqres.in');
    this.formLogin.get('password').setValue('cityslicka');
  }

  ngOnInit() { }

  login() {
    const data = {
      email: this.formLogin.get('username').value,
      password: this.formLogin.get('password').value,
    } as ILoginReq;

    this.securityService.login(data).subscribe(res => {
      this.dataService.setToken(res.token);
      this.dataService.setIsLogin(true);
      this.router.navigate(['home']);
    }, err => {
      this.dataService.setGeneralNotificationMessage(err);
    }
    );
  }


  toggleGridColumns() {///delete
    this.gridColumns = this.gridColumns === 3 ? 4 : 3;
  }
}
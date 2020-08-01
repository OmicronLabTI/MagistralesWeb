import { Component } from '@angular/core';
import { DataService } from './services/data.service';
import { Observable } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { AppConfig } from './constants/app-config';
import { Router} from "@angular/router";
import {CONST_NUMBER} from "../environments/environment";


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  iconMenuActive: number = CONST_NUMBER.one;
  title = 'omicron';
  now = new Date();
  isLoading: Observable<boolean>;
  isLogin = false;
  constructor(private _dataService: DataService, private _snackBar: MatSnackBar,
             private router: Router) {
    this.isLoading = this._dataService.getIsLoading();
    this.isLogin = this._dataService.userIsAuthenticated();
    this._dataService.getIsLogin().subscribe( isLoginS => this.isLogin = isLoginS);

    this._dataService
      .getGeneralNotificationMessage()
      .subscribe(msg => {
        this._snackBar.open(msg, 'OK', {
          duration: AppConfig.generalMessageTimeout
        });
      });
  }
  ngOnInit() { console.log('on init')}
  logoutSession(){
    this._dataService.setIsLogin(false);
    this._dataService.clearToken();
    this.router.navigate(['/login'])
  }

  changeIconActive(newMeuActive: number) {
    this.iconMenuActive = newMeuActive;
  }
}

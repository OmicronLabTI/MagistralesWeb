import { Component } from '@angular/core';
import { DataService } from './services/data.service';
import { Observable } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { AppConfig } from './constants/app-config';
import { Router} from '@angular/router';
import {CONST_NUMBER} from './constants/const';



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
  constructor(private dataService: DataService, private snackBar: MatSnackBar,
              private router: Router) {
    this.isLoading = this.dataService.getIsLoading();
    this.isLogin = this.dataService.userIsAuthenticated();
    this.dataService.getIsLogin().subscribe( isLoginS => this.isLogin = isLoginS);

    this.dataService
      .getGeneralNotificationMessage()
      .subscribe(msg => {
        this.snackBar.open(msg, 'OK', {
          duration: AppConfig.generalMessageTimeout
        });
      });
  }
  logoutSession() {
    this.dataService.setIsLogin(false);
    this.dataService.clearToken();
    this.router.navigate(['/login']);
  }

  changeIconActive(newMeuActive: number) {
    this.iconMenuActive = newMeuActive;
  }
}

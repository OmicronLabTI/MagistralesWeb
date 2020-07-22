import { Component } from '@angular/core';
import { DataService } from './services/data.service';
import { Observable } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { AppConfig } from './constants/app-config';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'omicron';
  now = new Date();
  isLoading: Observable<boolean>;
  isLogin: boolean = false;

  constructor(private _dataService: DataService, private _snackBar: MatSnackBar) {
    this._dataService.getIsLogin().subscribe(isLoginService => this.isLogin = isLoginService)
    this.isLoading = this._dataService.getIsLoading();
    console.log('tokenServiceLogin: ', this._dataService.userIsAuthenticated())

    this._dataService
      .getGeneralNotificationMessage()
      .subscribe(msg => {
        this._snackBar.open(msg, 'OK', {
          duration: AppConfig.generalMessageTimeout
        });
      });
  }

}

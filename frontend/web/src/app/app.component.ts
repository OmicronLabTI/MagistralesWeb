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

  constructor(private _dataService: DataService, private _snackBar: MatSnackBar) {
    this.isLoading = this._dataService.getIsLoading();

    this._dataService
      .getGeneralNotificationMessage()
      .subscribe(msg => {
        this._snackBar.open(msg, 'OK', {
          duration: AppConfig.generalMessageTimeout
        });
      });
  }
}

import { Injectable } from '@angular/core';
import {DataService} from './data.service';
import {Messages} from '../constants/messages';
import {ErrorHttpInterface} from '../model/http/commons';
import {HttpStatus} from '../constants/const';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  constructor(private dataService: DataService) { }
  httpError(error: ErrorHttpInterface, isFromLogin = false) {
    console.log('error httpService: ', error);
    switch (error.status) { // status: 0 = server refused
      case HttpStatus.unauthorized:
        if (isFromLogin) {
          this.dataService.setGeneralNotificationMessage('Credenciales inválidas.');
        } else {
          this.dataService.setIsLogout(true);
        }
        break;
      default:
        this.dataService.setMessageGeneralCallHttp({title: Messages.generic, icon: 'error', isButtonAccept: true});
        break;
    }

  }
}

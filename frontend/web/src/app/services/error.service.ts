import { Injectable } from '@angular/core';
import {DataService} from './data.service';
import {Messages} from '../constants/messages';
import {ErrorHttpInterface} from '../model/http/commons';
import {HttpStatus} from '../constants/const';
import { ObservableService } from './observable.service';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  constructor(
    private dataService: DataService,
    private observableService: ObservableService) { }
  httpError(error: ErrorHttpInterface) {
    switch (error.status) { // status: 0 = server refused
      case HttpStatus.unauthorized:
        this.observableService.setIsLogout(true);
        break;
      case HttpStatus.timeOut:
        this.observableService.setMessageGeneralCallHttp({title: Messages.timeout, icon: 'error', isButtonAccept: true});
        break;
      case HttpStatus.redirection:
        this.observableService.setMessageGeneralCallHttp({title: Messages.redirectionError, icon: 'error', isButtonAccept: true});
        break;
      case HttpStatus.notFound:
      case HttpStatus.connectionRefused:
        this.observableService.setMessageGeneralCallHttp({title: Messages.connectionRefused, icon: 'error', isButtonAccept: true});
        break;
      default:
        this.observableService.setMessageGeneralCallHttp({title: Messages.generic, icon: 'error', isButtonAccept: true});
        break;
    }

  }
}

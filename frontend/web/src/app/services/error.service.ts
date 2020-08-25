import { Injectable } from '@angular/core';
import {DataService} from './data.service';
import {Messages} from '../constants/messages';
import {ErrorHttp} from '../model/http/commons';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  constructor(private dataService: DataService) { }
  httpError(error: ErrorHttp) {
    this.dataService.setMessageGeneralCallHttp({title: Messages.generic, icon: 'error', isButtonAccept: true});
    console.log('error httpService: ', error);
  }
}

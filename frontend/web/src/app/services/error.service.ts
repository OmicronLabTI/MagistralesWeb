import { Injectable } from '@angular/core';
import {isString} from "util";

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  constructor() { }
  httpError(error: any) {
    console.log('error httpService: ', error, 'isString: ', isString(error));
  }
}

import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  constructor() { }
  httpError(error: any){
    console.log('error httpService: ', error)
  }
}

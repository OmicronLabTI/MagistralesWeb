import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private isLoading = new Subject<boolean>();
  private generalNotificationMessage = new Subject<string>();

  constructor() { }

  setIsLoading(loading: boolean) {
    this.isLoading.next(loading);
  }

  getIsLoading() {
    return this.isLoading.asObservable();
  }

  getGeneralNotificationMessage() {
    return this.generalNotificationMessage.asObservable();
  }

  setGeneralNotificationMessage(msg: string) {
    this.generalNotificationMessage.next(msg);
  }

  getToken(): string {
    return sessionStorage.getItem('token')
  }

  setToken(token: string) {
    sessionStorage.setItem('token', token);
  }

  userIsAuthenticated(): boolean {
    return sessionStorage.getItem('token') ? true : false;
  }
}

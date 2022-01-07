import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import {
  ConstToken,
  HttpServiceTOCall,
} from '../constants/const';
import { Catalogs } from '../model/http/pedidos';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  constructor() {}

  setRememberSession(rememberSession: string): void {
    localStorage.setItem(ConstToken.rememberSession, rememberSession);
  }
  getRememberSession(): string {
    return localStorage.getItem(ConstToken.rememberSession);
  }
  setProductNoLabel(productNoLabel: Catalogs): void {
    localStorage.setItem(ConstToken.productNoLabel, JSON.stringify(productNoLabel));
  }
  getProductNoLabel(): Catalogs {
    const productNoLabelSTR = localStorage.getItem(ConstToken.productNoLabel);
    const productNoLabel = JSON.parse(productNoLabelSTR) as Catalogs;
    return productNoLabel;

  }
  setRefreshToken(refreshToken: string) {
    localStorage.setItem(ConstToken.refreshToken, refreshToken);
  }
  getRefreshToken() {
    return localStorage.getItem(ConstToken.refreshToken);
  }
  getToken(): string {
    return localStorage.getItem(ConstToken.accessToken);
  }
  setToken(token: string) {
    localStorage.setItem(ConstToken.accessToken, token);
  }
  clearSession() {
    localStorage.removeItem(ConstToken.accessToken);
    localStorage.removeItem(ConstToken.rememberSession);
    localStorage.removeItem(ConstToken.refreshToken);
    localStorage.removeItem(ConstToken.userId);
    localStorage.removeItem(ConstToken.userName);
    localStorage.removeItem(ConstToken.isolatedOrder);
  }
  setUserId(userId: string) {
    localStorage.setItem(ConstToken.userId, userId);
  }

  getUserId() {
    return localStorage.getItem(ConstToken.userId);
  }
  userIsAuthenticated(): boolean {
    return !!localStorage.getItem(ConstToken.accessToken);
  }

}


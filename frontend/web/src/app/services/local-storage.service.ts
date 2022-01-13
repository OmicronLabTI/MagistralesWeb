import { Injectable } from '@angular/core';
import {
  ConstToken,
} from '../constants/const';
import { Catalogs, ParamsPedidos } from '../model/http/pedidos';

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

  setUserName(userName: string) {
    localStorage.setItem(ConstToken.userName, userName);
  }

  getUserName() {
    return localStorage.getItem(ConstToken.userName);
  }

  setUserRole(role: number) {
    localStorage.setItem(ConstToken.userRole, String(role));
  }

  getUserRole() {
    return localStorage.getItem(ConstToken.userRole);
  }
  getOrderIsolated() {
    return localStorage.getItem(ConstToken.isolatedOrder);
  }

  removeOrderIsolated() {
    localStorage.removeItem(ConstToken.isolatedOrder);
  }
  setFiltersActives(filters: string) {
    localStorage.setItem(ConstToken.filtersActive, filters);
  }
  getFiltersActives() {
    return localStorage.getItem(ConstToken.filtersActive);
  }
  removeFiltersActive() {
    localStorage.removeItem(ConstToken.filtersActive);
  }
  getFiltersActivesAsModel(): ParamsPedidos {
    return JSON.parse(this.getFiltersActives());
  }

  setFiltersActivesOrders(filters: string) {
    localStorage.setItem(ConstToken.filtersActiveOrders, filters);
  }
  getFiltersActivesOrders() {
    return localStorage.getItem(ConstToken.filtersActiveOrders);
  }
  removeFiltersActiveOrders() {
    localStorage.removeItem(ConstToken.filtersActiveOrders);
  }
  getFiltersActivesAsModelOrders(): ParamsPedidos {
    return JSON.parse(this.getFiltersActivesOrders());
  }
  setCurrentDetailOrder(detailOrder: string) {
    localStorage.setItem(ConstToken.detailOrderCurrent, detailOrder);
  }
  getCurrentDetailOrder() {
    return localStorage.getItem(ConstToken.detailOrderCurrent);
  }
  removeCurrentDetailOrder() {
    localStorage.removeItem(ConstToken.detailOrderCurrent);
  }
}


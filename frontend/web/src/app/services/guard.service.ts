import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CONST_NUMBER, pathRoles, RolesType } from '../constants/const';
import { DataService } from './data.service';
import { LocalStorageService } from './local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class GuardService implements CanActivate {

  constructor(
    private dataService: DataService,
    private router: Router,
    private localStorageService: LocalStorageService
  ) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const pass = false;
    const path = route.url[CONST_NUMBER.zero].path;
    if (this.localStorageService.userIsAuthenticated()) {
      switch (this.localStorageService.getUserRole()) {
        case RolesType.admin:
          return this.validateAndReturn(path, pathRoles.admin, [pathRoles.admin[CONST_NUMBER.zero]]);
        case RolesType.logistic:
          return this.validateAndReturn(path, pathRoles.logistica, [pathRoles.logistica[CONST_NUMBER.zero]]);
        case RolesType.warehouse:
        case RolesType.design: // role design === role warehouse
          return this.validateAndReturn(path, pathRoles.design, [pathRoles.design[CONST_NUMBER.zero]]);
        case RolesType.incidents:
          return this.validateAndReturn(path, pathRoles.incidents, [pathRoles.incidents[CONST_NUMBER.zero]]);
        default:
          return pass;
      }
    } else {
      this.router.navigate(['login']);
      return pass;
    }
  }

  validateAndReturn(path: string, rolesList: string[], route: string[]): boolean {
    const pass = rolesList.includes(path);
    if (!pass) {
      this.router.navigate(route);
      return pass;
    }
    return true;
  }
}

import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import {CONST_NUMBER, pathRoles, RolesType} from '../constants/const';
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

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    let pass = false;
    if (this.localStorageService.userIsAuthenticated()) {
      switch (this.dataService.getUserRole()) {
        case RolesType.admin:
          pass = pathRoles.admin.includes(route.url[CONST_NUMBER.zero].path);
          if (!pass) {
            this.router.navigate([pathRoles.admin[CONST_NUMBER.zero]]);
            return pass;
          }
          break;
        case RolesType.logistic:
          pass = pathRoles.logistica.includes(route.url[CONST_NUMBER.zero].path);
          if (!pass) {
            this.router.navigate([pathRoles.logistica[CONST_NUMBER.zero]]);
            return pass;
          }
          break;
        case RolesType.warehouse:
        case RolesType.design: // role design === role warehouse
          pass = pathRoles.design.includes(route.url[CONST_NUMBER.zero].path);
          if (!pass) {
            this.router.navigate([pathRoles.design[CONST_NUMBER.zero]]);
            return pass;
          }
          break;
        case RolesType.incidents:
          pass = pathRoles.incidents.includes(route.url[CONST_NUMBER.zero].path);
          if (!pass) {
            this.router.navigate([pathRoles.incidents[CONST_NUMBER.zero]]);
            return pass;
          }
          break;
      }
      return pass;
    } else {
      this.router.navigate(['login']);
      return pass;
    }
  }
}

import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { PassThrough } from 'stream';
import { CONST_NUMBER, pathRoles } from '../constants/const';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class GuardService implements CanActivate {

  constructor(private dataService: DataService, private router: Router) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    let pass = false;
    if (this.dataService.userIsAuthenticated()) {
      switch (this.dataService.getUserRole()) {
        case '1':
          pass = pathRoles.admin.includes(route.url[CONST_NUMBER.zero].path);
          if (!pass) {
            this.router.navigate([pathRoles.admin[CONST_NUMBER.zero]]);
            return pass;
          }
          break;
        case '3':
          pass = pathRoles.logistica.includes(route.url[CONST_NUMBER.zero].path);
          if (!pass) {
            this.router.navigate([pathRoles.logistica[CONST_NUMBER.zero]]);
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

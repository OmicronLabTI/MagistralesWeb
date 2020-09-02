import { Injectable } from '@angular/core';
import { Endpoints } from 'src/environments/endpoints';
import { ILoginRes, ILoginReq, IRefreshTokenReq} from '../model/http/security.model';
import { Observable } from 'rxjs';
import { ConsumeService } from './consume.service';
import { IUserRes} from '../model/http/users';
import {DataService} from './data.service';
import {CONST_STRING} from '../constants/const';

@Injectable({
  providedIn: 'root'
})
export class SecurityService {
  constructor(private consumeService: ConsumeService, private dataService: DataService) { }

  login(req: ILoginReq): Observable<ILoginRes> {
    return this.consumeService.httpPost(Endpoints.security.login, req);
  }
  refreshToken() {
    const refreshTokenReq = {
      scope: CONST_STRING.empty,
      refresh_token: this.dataService.getRefreshToken(),
      grant_type: CONST_STRING.empty
    } as IRefreshTokenReq;
    return this.consumeService.httpPost<ILoginRes>(Endpoints.security.refresh, refreshTokenReq);
  }

  getUser(userName: string) {
    return this.consumeService.httpGet<IUserRes>(Endpoints.users.getUser + userName);
  }
}

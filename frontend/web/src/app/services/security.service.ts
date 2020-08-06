import { Injectable } from '@angular/core';
import { Endpoints } from 'src/environments/endpoints';
import { ILoginRes, ILoginReq } from '../model/http/security.model';
import { Observable } from 'rxjs';
import { ConsumeService } from './consume.service';
import { IUserReq } from '../model/http/users';

@Injectable({
  providedIn: 'root'
})
export class SecurityService {

  constructor(private consumeService: ConsumeService) { }

  login(req: ILoginReq): Observable<ILoginRes> {
    return this.consumeService.httpPost(Endpoints.security.login, req);
  }

  getUser(userName: string){
    return this.consumeService.httpGet(Endpoints.users.getUser + userName);
  }
}

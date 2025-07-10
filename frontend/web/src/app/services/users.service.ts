import { Injectable } from '@angular/core';
import { ConsumeService } from './consume.service';
import { ClasificationsResponse, IRolesRes, IUserListRes, IUserReq, TechnicalListResponse } from '../model/http/users';
import { Endpoints } from '../../environments/endpoints';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private consumeService: ConsumeService) {
  }

  createUserService(user: IUserReq) {
    return this.consumeService.httpPost(Endpoints.users.createUser, user);
  }
  getRoles(): Observable<IRolesRes> {
    return this.consumeService.httpGet<IRolesRes>(Endpoints.users.roles);
  }
  getUsers(fullQueryString: string): Observable<IUserListRes> {
    return this.consumeService.httpGet<IUserListRes>(`${Endpoints.users.getUsers}${fullQueryString}`);
  }
  deleteUsers(idsToDelete: string[]) {
    return this.consumeService.httpPatch(Endpoints.users.delete, idsToDelete);
  }
  updateUser(user: IUserReq) {
    return this.consumeService.httpPut(Endpoints.users.update, user);
  }
  getClasifications(): Observable<ClasificationsResponse> {
    return this.consumeService.httpGet(Endpoints.users.getClasifications);
  }

  getColors(): Observable<ClasificationsResponse> {
    return this.consumeService.httpGet(Endpoints.users.getColors);
  }
  getTehcnicalUsers(): Observable<TechnicalListResponse> {
    return this.consumeService.httpGet(Endpoints.users.getTechnical);
  }
  getWorkTeam(id: string): Observable<TechnicalListResponse> {
    return this.consumeService.httpGet(`${Endpoints.users.workTeam}/${id}`);
  }
}

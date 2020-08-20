import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {IRolesRes, IUserListRes, IUserReq} from '../model/http/users';
import {Endpoints} from '../../environments/endpoints';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private consumeService: ConsumeService, private httpClient: HttpClient) {
  }

  createUser(user: IUserReq) {
    console.log('user req inter: ', user);
    return this.httpClient.post(Endpoints.users.createUser, user);
  }
  getRoles() {
    return this.consumeService.httpGet<IRolesRes>(Endpoints.users.roles);
  }
  getUsers(offset: number, limit: number) {
    return this.consumeService.httpGet<IUserListRes>(`${Endpoints.users.getUsers}?offset=${offset}&limit=${limit}`);
  }
  deleteUsers(idsToDelete: string[]) {
    return this.consumeService.httpPatch(Endpoints.users.delete, idsToDelete);
  }
  updateUser(user: IUserReq) {
    return this.consumeService.httpPut(Endpoints.users.update, user);

}
}

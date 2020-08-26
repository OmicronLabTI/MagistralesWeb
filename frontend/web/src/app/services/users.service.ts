import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {IRolesRes, IUserListRes, IUserReq} from '../model/http/users';
import {Endpoints} from '../../environments/endpoints';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private consumeService: ConsumeService) {
  }

  createUser(user: IUserReq) {
    return this.consumeService.httpPost(Endpoints.users.createUser, user);
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

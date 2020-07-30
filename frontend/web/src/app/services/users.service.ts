import { Injectable } from '@angular/core';
import {ConsumeService} from "./consume.service";
import {IUserReq} from "../model/http/users";
import {Endpoints} from "../../environments/endpoints";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private consumeService: ConsumeService) {
  }

  createUser(user: IUserReq){
    return this.consumeService.httpPost(Endpoints.users.createUser,user);
  }
  getRoles(){
    return this.consumeService.httpGet(Endpoints.users.roles);
  }
  getUsers(){
    return this.consumeService.httpGet(Endpoints.users.getUsers);
  }
  deleteUsers(idsToDelete: string[]){//check
    return this.consumeService.httpDelete(Endpoints.users.delete,'');
  }
}

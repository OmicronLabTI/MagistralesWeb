import { Injectable } from '@angular/core';
import {ConsumeService} from "./consume.service";
import {IUserReq} from "../model/http/users";
import {Endpoints} from "../../environments/endpoints";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private consumeService: ConsumeService, private httpClient: HttpClient) {
  }

  createUser(user: IUserReq){
    console.log('user req inter: ', user)
    return this.httpClient.post(Endpoints.users.createUser,user);
    //return this.consumeService.httpPost(Endpoints.users.createUser,user);
  }
  getRoles(){
    return this.consumeService.httpGet(Endpoints.users.roles);
  }
  getUsers(){
    return this.consumeService.httpGet(Endpoints.users.getUsers);
  }
  deleteUsers(idsToDelete: string[]){//check
    console.log('idsde: ',idsToDelete)
    return this.consumeService.httpPatch(Endpoints.users.delete,idsToDelete);
  }
  updateUser(user: IUserReq){
  console.log('user req update: ', user)
    return this.consumeService.httpPut(Endpoints.users.update,user);

}
}

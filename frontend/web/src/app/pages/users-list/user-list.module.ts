import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserListRoutingModule } from './user-list-routing.module';
import {UsersListComponent} from "./users-list.component";
import {FormsModule} from "@angular/forms";
import {ModalModule} from "../../_modal";
import {InfiniteScrollModule} from "ngx-infinite-scroll";


@NgModule({
  declarations: [UsersListComponent],
    imports: [
        CommonModule,
        UserListRoutingModule,
        FormsModule,
        ModalModule,
        InfiniteScrollModule
    ]
})
export class UserListModule { }

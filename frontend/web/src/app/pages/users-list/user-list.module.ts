import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserListRoutingModule } from './user-list-routing.module';
import {UserListComponent} from "./users-list.component";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ModalModule} from "../../_modal";
import {InfiniteScrollModule} from "ngx-infinite-scroll";


@NgModule({
  declarations: [UserListComponent],
    imports: [
        CommonModule,
        UserListRoutingModule,
        FormsModule,
        ModalModule,
        InfiniteScrollModule,
        ReactiveFormsModule
    ]
})
export class UserListModule { }

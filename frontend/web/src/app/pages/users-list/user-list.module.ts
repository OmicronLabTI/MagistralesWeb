import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserListRoutingModule } from './user-list-routing.module';
import {UserListComponent} from "./users-list.component";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {InfiniteScrollModule} from "ngx-infinite-scroll";


@NgModule({
  declarations: [UserListComponent],
    imports: [
        CommonModule,
        UserListRoutingModule,
        FormsModule,
        InfiniteScrollModule,
        ReactiveFormsModule
    ]
})
export class UserListModule { }

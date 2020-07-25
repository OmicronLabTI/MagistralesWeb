import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserListRoutingModule } from './user-list-routing.module';
import {UsersListComponent} from "./users-list.component";
import {FormsModule} from "@angular/forms";


@NgModule({
  declarations: [UsersListComponent],
    imports: [
        CommonModule,
        UserListRoutingModule,
        FormsModule
    ]
})
export class UserListModule { }

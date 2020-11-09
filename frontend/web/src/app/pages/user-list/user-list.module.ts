import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserListRoutingModule } from './user-list-routing.module';
import {UserListComponent} from './user-list.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {AddUserDialogComponent} from '../../dialogs/add-user-dialog/add-user-dialog.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {FaborderListModule} from '../faborders-list/faborder-list.module';
import {SearchUsersDialogComponent} from '../../dialogs/search-users-dialog/search-users-dialog.component';


@NgModule({
  declarations: [UserListComponent, AddUserDialogComponent, SearchUsersDialogComponent],
    imports: [
        CommonModule,
        UserListRoutingModule,
        ReactiveFormsModule,
        FormsModule,
        MATERIAL_COMPONENTS,
        FaborderListModule
    ],
    entryComponents: [AddUserDialogComponent, SearchUsersDialogComponent],
})
export class UserListModule { }

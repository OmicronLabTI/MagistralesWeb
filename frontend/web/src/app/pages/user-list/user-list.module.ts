import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserListRoutingModule } from './user-list-routing.module';
import {UserListComponent} from './user-list.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {AddUserDialogComponent} from '../../dialogs/add-user-dialog/add-user-dialog.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {FaborderListModule} from '../faborders-list/faborder-list.module';
import {SearchUsersDialogComponent} from '../../dialogs/search-users-dialog/search-users-dialog.component';
import { WorkTeamComponent } from 'src/app/dialogs/work-team/work-team.component';


@NgModule({
  declarations: [UserListComponent, AddUserDialogComponent, SearchUsersDialogComponent, WorkTeamComponent],
    imports: [
        CommonModule,
        UserListRoutingModule,
        ReactiveFormsModule,
        FormsModule,
        MATERIAL_COMPONENTS,
        FaborderListModule,
    ],
    entryComponents: [AddUserDialogComponent, SearchUsersDialogComponent, WorkTeamComponent],
})
export class UserListModule { }

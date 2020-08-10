import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserListRoutingModule } from './user-list-routing.module';
import {UserListComponent} from './user-list.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {AddUserDialogComponent} from '../../dialogs/add-user-dialog/add-user-dialog.component';
import {MATERIAL_COMPONENTS} from '../../app.material';


@NgModule({
  declarations: [UserListComponent, AddUserDialogComponent],
    imports: [
        CommonModule,
        UserListRoutingModule,
        ReactiveFormsModule,
        FormsModule,
        MATERIAL_COMPONENTS
    ],
    entryComponents: [AddUserDialogComponent],
})
export class UserListModule { }

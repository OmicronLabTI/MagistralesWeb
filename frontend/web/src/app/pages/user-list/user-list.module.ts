import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserListRoutingModule } from './user-list-routing.module';
import {UserListComponent} from "./user-list.component";
import {MatTableModule} from "@angular/material/table";
import {MatCheckboxModule} from "@angular/material/checkbox";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {MatIconModule} from "@angular/material/icon";
import {MatButtonModule} from "@angular/material/button";
import {MatDialogModule} from '@angular/material/dialog';
import {AddUserDialogComponent} from "../../dialogs/add-user-dialog/add-user-dialog.component";
import {MatCardModule} from "@angular/material/card";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {MatSelectModule} from "@angular/material/select";


@NgModule({
  declarations: [UserListComponent,AddUserDialogComponent],
    imports: [
        CommonModule,
        UserListRoutingModule,
        MatTableModule,
        MatCheckboxModule,
        FormsModule,
        MatIconModule,
        MatButtonModule,
        MatDialogModule,
        MatCardModule,
        MatFormFieldModule,
        ReactiveFormsModule,
        MatInputModule,
        MatSelectModule
    ],
    entryComponents:[AddUserDialogComponent],
})
export class UserListModule { }

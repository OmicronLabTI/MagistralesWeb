import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IncidentsListRoutingModule } from './incidents-list-routing.module';
import {IncidentsListComponent} from './incidents-list.component';
import {MatIconModule} from "@angular/material/icon";
import {MatButtonModule} from "@angular/material/button";
import {PipesModule} from "../../pipes/pipes.module";


@NgModule({
  declarations: [
      IncidentsListComponent
  ],
    imports: [
        CommonModule,
        IncidentsListRoutingModule,
        MatIconModule,
        MatButtonModule,
        PipesModule
    ]
})
export class IncidentsListModule { }

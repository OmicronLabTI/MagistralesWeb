import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IncidentsListRoutingModule } from './incidents-list-routing.module';
import {IncidentsListComponent} from './incidents-list.component';


@NgModule({
  declarations: [
      IncidentsListComponent
  ],
  imports: [
    CommonModule,
    IncidentsListRoutingModule
  ]
})
export class IncidentsListModule { }

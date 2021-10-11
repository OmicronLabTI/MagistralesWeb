import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IncidentsListRoutingModule } from './incidents-list-routing.module';
import {IncidentsListComponent} from './incidents-list.component';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {PipesModule} from '../../pipes/pipes.module';
import {MatTableModule} from '@angular/material/table';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatTooltipModule} from '@angular/material/tooltip';


@NgModule({
  declarations: [
      IncidentsListComponent
  ],
    imports: [
        CommonModule,
        IncidentsListRoutingModule,
        MatIconModule,
        MatButtonModule,
        PipesModule,
        MatTableModule,
        MatPaginatorModule,
        MatTooltipModule
    ]
})
export class IncidentsListModule { }

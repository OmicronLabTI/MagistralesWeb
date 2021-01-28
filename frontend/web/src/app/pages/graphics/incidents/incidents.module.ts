import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { IncidentsRoutingModule } from './incidents-routing.module';
import {IncidentsComponent} from './incidents.component';
import {ComponentsModule} from '../../../components/components.module';


@NgModule({
  declarations: [IncidentsComponent],
  imports: [
    CommonModule,
    IncidentsRoutingModule,
    ComponentsModule,
  ]
})
export class IncidentsModule { }

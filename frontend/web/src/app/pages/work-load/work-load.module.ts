import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WorkLoadRoutingModule } from './work-load-routing.module';
import {WorkLoadComponent} from './work-load.component';


@NgModule({
  declarations: [WorkLoadComponent],
  imports: [
    CommonModule,
    WorkLoadRoutingModule
  ]
})
export class WorkLoadModule { }

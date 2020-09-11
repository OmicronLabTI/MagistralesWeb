import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WorkLoadRoutingModule } from './work-load-routing.module';
import {WorkLoadComponent} from './work-load.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {FormsModule} from '@angular/forms';


@NgModule({
  declarations: [WorkLoadComponent],
  imports: [
    CommonModule,
    WorkLoadRoutingModule,
      MATERIAL_COMPONENTS,
      FormsModule
  ]
})
export class WorkLoadModule { }

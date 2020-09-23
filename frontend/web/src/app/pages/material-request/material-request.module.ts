import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialRequestRoutingModule } from './material-request-routing.module';
import {MaterialRequestComponent} from './material-request.component';
import {MatButtonModule} from '@angular/material/button';
import {MatTabsModule} from '@angular/material/tabs';

@NgModule({
  declarations: [ MaterialRequestComponent],
    imports: [
        CommonModule,
        MaterialRequestRoutingModule,
        MatButtonModule,
        MatTabsModule
    ]
})
export class MaterialRequestModule { }

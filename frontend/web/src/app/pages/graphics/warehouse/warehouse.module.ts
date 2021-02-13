import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { WarehouseRoutingModule } from './warehouse-routing.module';
import {WarehouseComponent} from './warehouse.component';
import {ComponentsModule} from '../../../components/components.module';
import {MatCardModule} from '@angular/material/card';
import {MatListModule} from '@angular/material/list';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import {ProgressBarModule} from 'angular-progress-bar';


@NgModule({
  declarations: [
      WarehouseComponent
  ],
    imports: [
        CommonModule,
        WarehouseRoutingModule,
        ComponentsModule,
        MatCardModule,
        MatListModule,
        MatProgressBarModule,
        ProgressBarModule
    ]
})
export class WarehouseModule { }

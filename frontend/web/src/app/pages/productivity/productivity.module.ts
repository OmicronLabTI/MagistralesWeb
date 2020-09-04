import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import {MATERIAL_COMPONENTS} from '../../app.material';
import { ProductivityComponent } from './productivity.component';
import { ProductivityRoutingModule } from './productivity-routing.module';
import { Chart } from 'chart.js';


@NgModule({
  declarations: [ProductivityComponent],
  imports: [
    CommonModule,
    ProductivityRoutingModule,
    MATERIAL_COMPONENTS
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ProductivityModule { }

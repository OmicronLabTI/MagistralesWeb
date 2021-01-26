import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import {MATERIAL_COMPONENTS} from '../../../app.material';
import { ProductivityComponent } from './productivity.component';
import { ProductivityRoutingModule } from './productivity-routing.module';
import {  ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [ProductivityComponent],
  imports: [
    CommonModule,
    ProductivityRoutingModule,
    MATERIAL_COMPONENTS,
    ReactiveFormsModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ProductivityModule { }

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RangeDateComponent} from './range-date/range-date.component';

@NgModule({
  declarations: [RangeDateComponent],
  exports: [
    RangeDateComponent
  ],
  imports: [
    CommonModule
  ]
})
export class ComponentsModule { }

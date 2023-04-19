import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LimitEntiresDirective } from './limit-entires.directive';

@NgModule({
  declarations: [LimitEntiresDirective],
  imports: [
    CommonModule
  ],
  exports: [
    LimitEntiresDirective
  ]
})
export class DirectivesModule { }

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {DateformatPipe} from './dateformat.pipe';



@NgModule({
  declarations: [DateformatPipe],
  exports: [
    DateformatPipe
  ],
  imports: [
    CommonModule
  ]
})
export class PipesModule { }

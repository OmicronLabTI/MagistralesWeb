import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {DateformatPipe} from './dateformat.pipe';
import { SanitizeHtmlPipe } from './sanitizeHtml/sanitize-html.pipe';



@NgModule({
  declarations: [DateformatPipe, SanitizeHtmlPipe],
  exports: [
    DateformatPipe,
    SanitizeHtmlPipe
  ],
  imports: [
    CommonModule,
  ]
})
export class PipesModule { }

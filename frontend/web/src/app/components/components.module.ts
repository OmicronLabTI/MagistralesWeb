import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RangeDateComponent} from './range-date/range-date.component';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {FormsModule} from '@angular/forms';
import {MatInputModule} from '@angular/material/input';

@NgModule({
  declarations: [RangeDateComponent],
  exports: [
    RangeDateComponent
  ],
    imports: [
        CommonModule,
        MatFormFieldModule,
        MatDatepickerModule,
        FormsModule,
        MatInputModule
    ]
})
export class ComponentsModule { }

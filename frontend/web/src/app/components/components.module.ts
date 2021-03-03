import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {FormsModule} from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import {ButtonRefreshComponent} from './button-refresh/button-refresh.component';
import { ButtonBackComponent } from './button-back/button-back.component';
import { ButtonNextComponent } from './button-next/button-next.component';


@NgModule({
  declarations: [ButtonRefreshComponent, ButtonBackComponent, ButtonNextComponent],
  exports: [
    ButtonRefreshComponent,
    ButtonBackComponent,
    ButtonNextComponent
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

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {RangeDateComponent} from './range-date/range-date.component';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {FormsModule} from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import { GraphShowComponent } from './graph-show/graph-show.component';
import {ButtonRefreshComponent} from './button-refresh/button-refresh.component';
import { ButtonBackComponent } from './button-back/button-back.component';
import { ButtonNextComponent } from './button-next/button-next.component';
import {MatTooltipModule} from '@angular/material/tooltip';

@NgModule({
  declarations: [RangeDateComponent, GraphShowComponent, ButtonRefreshComponent, ButtonBackComponent, ButtonNextComponent],
    exports: [
        RangeDateComponent,
        GraphShowComponent,
        ButtonRefreshComponent,
        ButtonBackComponent,
        ButtonNextComponent
    ],
    imports: [
        CommonModule,
        MatFormFieldModule,
        MatDatepickerModule,
        FormsModule,
        MatInputModule,
        MatTooltipModule
    ]
})
export class ComponentsModule { }

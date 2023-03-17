import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialRequestRoutingModule } from './material-request-routing.module';
import { MaterialRequestComponent } from './material-request.component';
import { MatButtonModule } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MATERIAL_COMPONENTS } from '../../app.material';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FaborderListModule } from '../faborders-list/faborder-list.module';
import { NgxMaskModule } from 'ngx-mask';
import { HistoryMaterialRequestComponent } from './history-material-request/history-material-request.component';
import { ComponentsModule } from 'src/app/components/components.module';
import { SatDatepickerModule, SatNativeDateModule } from 'saturn-datepicker';
import { APP_PROVIDERS } from 'src/app/app.providers';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [MaterialRequestComponent, HistoryMaterialRequestComponent],
  imports: [
    CommonModule,
    MaterialRequestRoutingModule,
    MatButtonModule,
    MatTabsModule,
    MatTableModule,
    MatCheckboxModule,
    MatFormFieldModule,
    FormsModule,
    MatInputModule,
    MATERIAL_COMPONENTS,
    FaborderListModule,
    NgxMaskModule.forRoot(),
    ComponentsModule,
    FormsModule,
    ReactiveFormsModule,
    SatDatepickerModule,
    SatNativeDateModule,
    RouterModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    APP_PROVIDERS
  ]
})
export class MaterialRequestModule { }

import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AutomaticBillingRoutingModule } from './automatic-billing-routing.module';
import { AutomaticBillingComponent } from './automatic-billing.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { MatSlideToggleModule, MatTooltipModule } from '@angular/material';
import {
  ManualAdjustmentConfirmedDialogComponent
} from 'src/app/dialogs/manual-adjustment-confirmed-dialog/manual-adjustment-confirmed-dialog.component';


@NgModule({
  declarations: [AutomaticBillingComponent, ManualAdjustmentConfirmedDialogComponent],
  imports: [
    CommonModule,
    AutomaticBillingRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    MATERIAL_COMPONENTS,
    MatSlideToggleModule,
    MatTooltipModule
  ],
  entryComponents: [ManualAdjustmentConfirmedDialogComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AutomaticBillingModule { }

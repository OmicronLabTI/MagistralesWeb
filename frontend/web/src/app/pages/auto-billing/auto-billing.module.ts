import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MATERIAL_COMPONENTS } from '../../app.material';
import { AutoBillingRoutingModule } from './auto-billing-routing.module';
import { AutoBillingComponent } from './auto-billing.component';

@NgModule({
  declarations: [AutoBillingComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    ...MATERIAL_COMPONENTS,
    AutoBillingRoutingModule
  ]
})
export class AutoBillingModule {}

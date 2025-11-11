import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AutoBillingComponent } from './auto-billing.component';

const routes: Routes = [
  { path: '', component: AutoBillingComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AutoBillingRoutingModule {}

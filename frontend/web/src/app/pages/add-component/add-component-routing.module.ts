import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AddComponentComponent } from './add-component.component';

const routes: Routes = [{ path: '', component: AddComponentComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AddComponentRoutingModule { }

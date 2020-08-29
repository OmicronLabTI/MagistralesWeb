import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {InventorybatchesComponent} from './inventorybatches.component';


const routes: Routes = [
  {
    path: '',
    component: InventorybatchesComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InventorybatchesRoutingModule { }

import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FabordersListComponent } from './faborders-list.component';


const routes: Routes = [
  {
    path: '',
    component: FabordersListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FaborderListRoutingModule { }

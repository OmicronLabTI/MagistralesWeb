import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {IncidentsListComponent} from './incidents-list.component';


const routes: Routes = [
  {
    path: '',
    component: IncidentsListComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class IncidentsListRoutingModule { }

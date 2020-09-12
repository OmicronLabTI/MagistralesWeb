import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {WorkLoadComponent} from './work-load.component';


const routes: Routes = [
  {
    path: '',
    component: WorkLoadComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WorkLoadRoutingModule { }

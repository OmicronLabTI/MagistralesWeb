import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {MaterialRequestComponent} from './material-request.component';


const routes: Routes = [
  {
    path: '',
    component: MaterialRequestComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MaterialRequestRoutingModule { }

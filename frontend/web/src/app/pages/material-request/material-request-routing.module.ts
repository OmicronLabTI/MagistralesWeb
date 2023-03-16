import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HistoryMaterialRequestComponent } from './history-material-request/history-material-request.component';
import {MaterialRequestComponent} from './material-request.component';


const routes: Routes = [
  {
    path: ':requests/:isOrder',
    component: MaterialRequestComponent
  },
  {
    path: 'history',
    component: HistoryMaterialRequestComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MaterialRequestRoutingModule { }

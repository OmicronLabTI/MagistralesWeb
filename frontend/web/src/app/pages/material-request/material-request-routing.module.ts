import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HistoryMaterialRequestComponent } from './history-material-request/history-material-request.component';
import { MaterialRequestComponent } from './material-request.component';
import { RequestSuppliesComponent } from './request-supplies/request-supplies.component';

const routes: Routes = [
  {
    path: '',
    component: MaterialRequestComponent,
    children: [
      {
        path: ':requests/:isOrder',
        component: RequestSuppliesComponent
      },
      {
        path: 'history',
        component: HistoryMaterialRequestComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MaterialRequestRoutingModule { }

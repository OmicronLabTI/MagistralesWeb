import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {DetalleFormulaComponent} from './detalle-formula.component';


const routes: Routes = [
  {
    path: '',
    component: DetalleFormulaComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DetalleFormulaRoutingModule { }

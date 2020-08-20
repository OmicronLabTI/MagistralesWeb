import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {PedidoDetalleComponent} from './pedido-detalle.component';


const routes: Routes = [
  {
    path: '',
    component: PedidoDetalleComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PedidoDetalleRoutingModule { }

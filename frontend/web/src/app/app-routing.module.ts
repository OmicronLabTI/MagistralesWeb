import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GuardService } from './services/guard.service';

const routes: Routes = [
  {
    path: 'login',
    loadChildren: () => import('./pages/login/login.module').then(m => m.LoginModule)
  },
  {
    path: 'userList',
    loadChildren: () => import('./pages/user-list/user-list.module').then(m => m.UserListModule),
    canActivate: [GuardService]
  },
  {
    path: 'pedidos',
    loadChildren: () => import('./pages/pedidos/pedidos.module').then(m => m.PedidosModule),
    canActivate: [GuardService]
  },
  {
    path: 'pdetalle/:id',
    loadChildren: () => import('./pages/pedido-detalle/pedido-detalle.module').then(m => m.PedidoDetalleModule),
    canActivate: [GuardService]
  },
  {
    path: 'ordenfabricacion/:ordenid',
    loadChildren: () => import('./pages/detalle-formula/detalle-formula.module').then(m => m.DetalleFormulaModule),
    canActivate: [GuardService]
  },
  {
    path: 'lotes/:document/:ordenid',
    loadChildren: () => import('./pages/inventorybatches/inventorybatches.module').then(m => m.InventorybatchesModule),
    canActivate: [GuardService]
  },
  {
    path: 'ordenes',
    loadChildren: () => import('./pages/faborders-list/faborder-list.module').then(m => m.FaborderListModule),
    canActivate: [GuardService]
  },
  {
    path: 'productividad',
    loadChildren: () => import('./pages/productivity/productivity.module').then(m => m.ProductivityModule),
    canActivate: [GuardService]
  },
  {
    path: 'workLoad',
    loadChildren: () => import('./pages/work-load/work-load.module').then(m => m.WorkLoadModule),
    canActivate: [GuardService]
  },
  {
    path: '**',
    redirectTo: '/login'

  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

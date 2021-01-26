import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GuardService } from './services/guard.service';
import {RouterPaths} from './constants/const';

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
    path: 'lotes/:document/:ordenid/:code/:description/:hasMissingStock',
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
    loadChildren: () => import('./pages/graphics/productivity/productivity.module').then(m => m.ProductivityModule),
    canActivate: [GuardService]
  },
  {
    path: 'workLoad',
    loadChildren: () => import('./pages/graphics/work-load/work-load.module').then(m => m.WorkLoadModule),
    canActivate: [GuardService]
  },
  {
    path: `${RouterPaths.materialRequest}/:requests/:isOrder`,
    loadChildren: () => import('./pages/material-request/material-request.module').then(m => m.MaterialRequestModule),
    canActivate: [GuardService]
  },
  {
    path: `${RouterPaths.incidents}`,
    loadChildren: () => import('./pages/graphics/incidents/incidents.module').then(m => m.IncidentsModule),
    // canActivate: [GuardService]
  },
  {
    path: `${RouterPaths.incidentsList}`,
    loadChildren: () => import('./pages/incidents-list/incidents-list.module').then(m => m.IncidentsListModule),
    // canActivate: [GuardService]
  },
  {
    path: `${RouterPaths.warehousePage}`,
    loadChildren: () => import('./pages/graphics/warehouse/warehouse.module').then(m => m.WarehouseModule),
    // canActivate: [GuardService]
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

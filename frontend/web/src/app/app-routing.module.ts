import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GuardService } from './services/guard.service';
import { PedidosComponent } from './components/pedidos/pedidos.component';

const routes: Routes = [
  {
    path: 'login',
    loadChildren:() => import('./pages/login/login.module').then(m => m.LoginModule)
  },
  {
    path: 'home',
    loadChildren:() => import('./pages/home/home.module').then(m => m.HomeModule),
    canActivate: [GuardService]
  },
  {
    path: 'userList',
    loadChildren:() => import('./pages/user-list/user-list.module').then(m => m.UserListModule),
    canActivate: [GuardService]
  },
  {
    path: 'pedidos',
    component: PedidosComponent
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

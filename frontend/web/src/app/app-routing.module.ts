import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { GuardService } from './services/guard.service';


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
    loadChildren: () => import('./pages/users-list/user-list.module').then(m => m.UserListModule),
    canActivate: [GuardService]
  },
  {
    path: 'userList',
    loadChildren: () => import('./pages/users-list/user-list.module').then(m => m.UserListModule),
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

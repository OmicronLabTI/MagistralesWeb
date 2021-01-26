import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProductivityComponent } from './productivity.component';


const routes: Routes = [
  {
    path: '',
    component: ProductivityComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductivityRoutingModule { }

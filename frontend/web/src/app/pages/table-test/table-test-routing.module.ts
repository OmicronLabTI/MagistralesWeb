import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {TableTestComponent} from './table-test.component';


const routes: Routes = [{
  path: '',
  component: TableTestComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TableTestRoutingModule { }

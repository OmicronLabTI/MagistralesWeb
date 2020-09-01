import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FabordersListComponent } from './faborders-list.component';
import {MATERIAL_COMPONENTS} from '../../app.material';

import { FaborderListRoutingModule } from './faborder-list-routing.module';


@NgModule({
  declarations: [FabordersListComponent],
  imports: [
    CommonModule,
    FaborderListRoutingModule,
    MATERIAL_COMPONENTS
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class FaborderListModule { }

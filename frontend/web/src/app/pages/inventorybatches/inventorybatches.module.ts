import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InventorybatchesRoutingModule } from './inventorybatches-routing.module';
import {MATERIAL_COMPONENTS} from '../../app.material';
import { InventorybatchesComponent } from './inventorybatches.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';


@NgModule({
  declarations: [InventorybatchesComponent],
  imports: [
    CommonModule,
    InventorybatchesRoutingModule,
    MATERIAL_COMPONENTS,
    FormsModule,
    ReactiveFormsModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class InventorybatchesModule { }

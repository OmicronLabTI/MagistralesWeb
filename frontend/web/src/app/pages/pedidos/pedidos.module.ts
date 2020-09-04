import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import { PedidosRoutingModule } from './pedidos-routing.module';
import {PedidosComponent} from './pedidos.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import { ReactiveFormsModule , FormsModule} from '@angular/forms';

@NgModule({
  declarations: [PedidosComponent],
  imports: [
    CommonModule,
    PedidosRoutingModule,
      MATERIAL_COMPONENTS,
    ReactiveFormsModule,
      FormsModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class PedidosModule { }

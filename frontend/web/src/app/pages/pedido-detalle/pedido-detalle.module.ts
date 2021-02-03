import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';

import { PedidoDetalleRoutingModule } from './pedido-detalle-routing.module';
import {PedidoDetalleComponent} from './pedido-detalle.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {FormsModule} from '@angular/forms';


@NgModule({
  declarations: [PedidoDetalleComponent],
    imports: [
        CommonModule,
        PedidoDetalleRoutingModule,
        MATERIAL_COMPONENTS,
        FormsModule
    ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class PedidoDetalleModule { }

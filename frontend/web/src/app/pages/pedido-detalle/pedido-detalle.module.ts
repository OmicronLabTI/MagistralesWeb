import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';

import { PedidoDetalleRoutingModule } from './pedido-detalle-routing.module';
import {PedidoDetalleComponent} from './pedido-detalle.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {FormsModule} from '@angular/forms';
import {AddCommentsDialogComponent} from '../../dialogs/add-comments-dialog/add-comments-dialog.component';
import {ComponentsModule} from '../../components/components.module';


@NgModule({
  declarations: [PedidoDetalleComponent, AddCommentsDialogComponent],
    imports: [
        CommonModule,
        PedidoDetalleRoutingModule,
        MATERIAL_COMPONENTS,
        FormsModule,
        ComponentsModule
    ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
    entryComponents: [AddCommentsDialogComponent]
})
export class PedidoDetalleModule { }

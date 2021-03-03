import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import { PedidosRoutingModule } from './pedidos-routing.module';
import {PedidosComponent} from './pedidos.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import { ReactiveFormsModule , FormsModule} from '@angular/forms';
import {PipesModule} from '../../pipes/pipes.module';
import {OrdersRefuseComponent} from '../../dialogs/orders-refuse/orders-refuse.component';
import {ComponentsModule} from '../../components/components.module';

@NgModule({
  declarations: [PedidosComponent, OrdersRefuseComponent],
    imports: [
        CommonModule,
        PedidosRoutingModule,
        MATERIAL_COMPONENTS,
        ReactiveFormsModule,
        FormsModule,
        PipesModule,
        ComponentsModule
    ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [OrdersRefuseComponent]
})
export class PedidosModule { }

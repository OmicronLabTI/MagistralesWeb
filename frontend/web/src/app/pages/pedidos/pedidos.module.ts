import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import { PedidosRoutingModule } from './pedidos-routing.module';
import {PedidosComponent} from './pedidos.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {FindOrdersDialogComponent} from '../../dialogs/find-orders-dialog/find-orders-dialog.component';
import { ReactiveFormsModule , FormsModule} from '@angular/forms';
import {PlaceOrderDialogComponent} from '../../dialogs/place-order-dialog/place-order-dialog.component';

@NgModule({
  declarations: [PedidosComponent, FindOrdersDialogComponent, PlaceOrderDialogComponent],
  imports: [
    CommonModule,
    PedidosRoutingModule,
      MATERIAL_COMPONENTS,
    ReactiveFormsModule,
      FormsModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [FindOrdersDialogComponent, PlaceOrderDialogComponent]
})
export class PedidosModule { }

import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';
import { DetalleFormulaRoutingModule } from './detalle-formula-routing.module';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {DetalleFormulaComponent} from './detalle-formula.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { FaborderListModule } from '../faborders-list/faborder-list.module';
import {ComponentsModule} from '../../components/components.module';

@NgModule({
  declarations: [DetalleFormulaComponent],
    imports: [
        CommonModule,
        DetalleFormulaRoutingModule,
        MATERIAL_COMPONENTS,
        FormsModule,
        ReactiveFormsModule,
        FaborderListModule,
        ComponentsModule
    ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DetalleFormulaModule { }

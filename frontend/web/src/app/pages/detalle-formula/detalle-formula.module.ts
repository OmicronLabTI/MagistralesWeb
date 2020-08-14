import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import { CommonModule } from '@angular/common';

import { DetalleFormulaRoutingModule } from './detalle-formula-routing.module';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {DetalleFormulaComponent} from './detalle-formula.component';
import {FormsModule} from '@angular/forms';
import {ComponentSearchComponent} from '../../dialogs/components-search-dialog/component-search.component';


@NgModule({
  declarations: [DetalleFormulaComponent, ComponentSearchComponent],
    imports: [
        CommonModule,
        DetalleFormulaRoutingModule,
        MATERIAL_COMPONENTS,
        FormsModule
    ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [ComponentSearchComponent]
})
export class DetalleFormulaModule { }

import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FabordersListComponent } from './faborders-list.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import { ReactiveFormsModule , FormsModule} from '@angular/forms';
import { FaborderListRoutingModule } from './faborder-list-routing.module';
import {FinalizeOrdersComponent} from '../../dialogs/finalize-orders/finalize-orders.component';
import { DigitDecimalNumberDirective } from 'src/app/directives/digit-decima-number.directive';
import {PipesModule} from '../../pipes/pipes.module';

@NgModule({
  declarations: [FabordersListComponent, FinalizeOrdersComponent, DigitDecimalNumberDirective],
    imports: [
        CommonModule,
        FaborderListRoutingModule,
        MATERIAL_COMPONENTS,
        ReactiveFormsModule,
        FormsModule,
        PipesModule
    ],
  exports: [DigitDecimalNumberDirective],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [FinalizeOrdersComponent]
})
export class FaborderListModule { }

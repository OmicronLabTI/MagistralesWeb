import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FabordersListComponent } from './faborders-list.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import { ReactiveFormsModule , FormsModule} from '@angular/forms';
import { FaborderListRoutingModule } from './faborder-list-routing.module';
import {FinalizeOrdersComponent} from '../../dialogs/finalize-orders/finalize-orders.component';
import { DigitDecimaNumberDirective } from 'src/app/directives/digit-decima-number.directive';


@NgModule({
  declarations: [FabordersListComponent, FinalizeOrdersComponent, DigitDecimaNumberDirective],
  imports: [
    CommonModule,
    FaborderListRoutingModule,
    MATERIAL_COMPONENTS,
    ReactiveFormsModule,
    FormsModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [FinalizeOrdersComponent]
})
export class FaborderListModule { }

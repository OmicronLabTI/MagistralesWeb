import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AddComponentRoutingModule } from './add-component-routing.module';
import { AddComponentComponent } from './add-component.component';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [AddComponentComponent],
  imports: [CommonModule,
    AddComponentRoutingModule,
    MATERIAL_COMPONENTS,
    FormsModule,
    ReactiveFormsModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AddComponentModule { }

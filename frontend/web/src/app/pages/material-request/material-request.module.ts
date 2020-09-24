import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialRequestRoutingModule } from './material-request-routing.module';
import {MaterialRequestComponent} from './material-request.component';
import {MatButtonModule} from '@angular/material/button';
import {MatTabsModule} from '@angular/material/tabs';
import {MatTableModule} from '@angular/material/table';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatFormFieldModule} from '@angular/material/form-field';
import {FormsModule} from '@angular/forms';
import {MatInputModule} from '@angular/material/input';
import { RequestSignatureDialogComponent } from '../../dialogs/request-signature-dialog/request-signature-dialog.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import { SignaturePadModule } from 'angular2-signaturepad';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';

@NgModule({
  declarations: [ MaterialRequestComponent, RequestSignatureDialogComponent ],
  imports: [
      CommonModule,
      MaterialRequestRoutingModule,
      MatButtonModule,
      MatTabsModule,
      MatTableModule,
      MatCheckboxModule,
      MatFormFieldModule,
      FormsModule,
      MatInputModule,
      MATERIAL_COMPONENTS,
      SignaturePadModule
  ],
  entryComponents: [RequestSignatureDialogComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class MaterialRequestModule { }

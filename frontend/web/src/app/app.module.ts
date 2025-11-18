import { BrowserModule, Title } from '@angular/platform-browser';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { MATERIAL_COMPONENTS } from './app.material';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { APP_PROVIDERS } from './app.providers';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { FlexLayoutModule } from '@angular/flex-layout';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { NgxMaskModule } from 'ngx-mask';
import { SignaturePadModule } from 'angular2-signaturepad';

// 🔹 Angular Material Dialog tokens
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

// 🔹 Componentes de diálogos existentes
import { PlaceOrderDialogComponent } from './dialogs/place-order-dialog/place-order-dialog.component';
import { ComponentSearchComponent } from './dialogs/components-search-dialog/component-search.component';
import { FindOrdersDialogComponent } from './dialogs/find-orders-dialog/find-orders-dialog.component';
import { MiListaComponent } from './dialogs/mi-lista/mi-lista.component';
import { ComponentslistComponent } from './dialogs/componentslist/componentslist.component';
import { RequestSignatureDialogComponent } from './dialogs/request-signature-dialog/request-signature-dialog.component';
import { AddCommentsDialogComponent } from './dialogs/add-comments-dialog/add-comments-dialog.component';

// 🔹 Nuevos diálogos (SAP Orders / Shipments)
import { ViewSapOrdersDialogComponent } from './dialogs/view-sap-orders-dialog/view-sap-orders-dialog.component';
import { ViewShipmentsDialogComponent } from './dialogs/view-shipments-dialog/view-shipments-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    // 🔹 Dialog components
    PlaceOrderDialogComponent,
    ComponentSearchComponent,
    FindOrdersDialogComponent,
    MiListaComponent,
    ComponentslistComponent,
    RequestSignatureDialogComponent,
    AddCommentsDialogComponent,
    ViewSapOrdersDialogComponent,
    ViewShipmentsDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    ReactiveFormsModule,
    MATERIAL_COMPONENTS,
    FlexLayoutModule,
    FormsModule,
    InfiniteScrollModule,
    SignaturePadModule,
    NgxMaskModule.forRoot(),
    MatDialogModule // ✅ Import obligatorio para MatDialog
  ],
  providers: [
    APP_PROVIDERS,
    Title,

    // ✅ Providers de respaldo para evitar NullInjectorError
    { provide: MAT_DIALOG_DATA, useValue: {} },
    { provide: MatDialogRef, useValue: {} }
  ],
  bootstrap: [AppComponent],
  entryComponents: [
    // 🔹 Legacy dialogs para compatibilidad (si usas versiones < Angular 9)
    PlaceOrderDialogComponent,
    ComponentSearchComponent,
    FindOrdersDialogComponent,
    MiListaComponent,
    ComponentslistComponent,
    RequestSignatureDialogComponent,
    AddCommentsDialogComponent,
    ViewSapOrdersDialogComponent,
    ViewShipmentsDialogComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule {}

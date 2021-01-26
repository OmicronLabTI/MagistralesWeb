import { BrowserModule, Title } from '@angular/platform-browser';
import {CUSTOM_ELEMENTS_SCHEMA, NgModule} from '@angular/core';
import { MATERIAL_COMPONENTS } from './app.material';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { APP_PROVIDERS } from './app.providers';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule} from '@angular/common/http';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule } from '@angular/forms';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import {PlaceOrderDialogComponent} from './dialogs/place-order-dialog/place-order-dialog.component';
import {ComponentSearchComponent} from './dialogs/components-search-dialog/component-search.component';
import {FindOrdersDialogComponent} from './dialogs/find-orders-dialog/find-orders-dialog.component';
import { MiListaComponent } from './dialogs/mi-lista/mi-lista.component';
import { ComponentslistComponent } from './dialogs/componentslist/componentslist.component';
import {RequestSignatureDialogComponent} from './dialogs/request-signature-dialog/request-signature-dialog.component';
import {SignaturePadModule} from 'angular2-signaturepad';

@NgModule({
    declarations: [
        AppComponent, PlaceOrderDialogComponent, ComponentSearchComponent, FindOrdersDialogComponent, MiListaComponent,
        ComponentslistComponent, RequestSignatureDialogComponent,
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

    ],
    providers: [
        APP_PROVIDERS,
        Title
    ],
    bootstrap: [AppComponent],
    entryComponents: [
        PlaceOrderDialogComponent,
        ComponentSearchComponent,
        FindOrdersDialogComponent,
        MiListaComponent,
        ComponentslistComponent,
        RequestSignatureDialogComponent
    ],
    exports: [
    ],
    schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppModule { }

import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
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
import { WorkLoadComponent } from './pages/work-load/work-load.component';

@NgModule({
  declarations: [
    AppComponent, PlaceOrderDialogComponent, ComponentSearchComponent, FindOrdersDialogComponent, WorkLoadComponent
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

    ],
  providers: [
    APP_PROVIDERS,
    Title
  ],
  bootstrap: [AppComponent],
    entryComponents: [PlaceOrderDialogComponent, ComponentSearchComponent, FindOrdersDialogComponent]
})
export class AppModule { }

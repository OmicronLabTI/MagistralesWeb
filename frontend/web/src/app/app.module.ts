import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { MATERIAL_COMPONENTS } from './app.material';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { APP_PROVIDERS } from './app.providers';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HttpParams } from '@angular/common/http';
import { PedidosComponent } from './components/pedidos/pedidos.component';
import { FlexLayoutModule } from "@angular/flex-layout";
import { FormsModule } from '@angular/forms';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';

@NgModule({
  declarations: [
    AppComponent,
    PedidosComponent,
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
        InfiniteScrollModule
    ],
  providers: [
    APP_PROVIDERS
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

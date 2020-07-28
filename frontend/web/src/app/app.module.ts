import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { 
  MatToolbarModule,
  MatSidenavModule,
  MatButtonModule,
  MatListModule,
  MatIconModule,
  MatInputModule,
  MatProgressBarModule,
  MatSnackBarModule,
  MatSelectModule, 
  MatCardModule,
  MatExpansionModule,
  MatPaginatorModule,
  MatMenuModule,
  MatTableModule,
  MatSlideToggleModule,
  MatDialogModule,
  MatTabsModule,
  MatGridListModule,
  MatProgressSpinnerModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatRadioModule,
  MatAutocompleteModule
} from '@angular/material';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { APP_PROVIDERS } from './app.providers';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { PedidosComponent } from './components/pedidos/pedidos.component';

export const MATERIAL_COMPONENTS = [
  MatToolbarModule,
  MatSidenavModule,
  MatButtonModule,
  MatListModule,
  MatIconModule,
  MatInputModule,
  MatProgressBarModule,
  MatSnackBarModule,
  MatSelectModule,
  MatCardModule,
  MatExpansionModule,
  MatPaginatorModule,
  MatMenuModule,
  MatTableModule,
  MatSlideToggleModule,
  MatDialogModule,
  MatTabsModule,
  MatProgressSpinnerModule, 
  MatDatepickerModule,
  MatNativeDateModule,
  MatRadioModule, 
  MatAutocompleteModule
]

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
    MATERIAL_COMPONENTS],
  providers: [
    APP_PROVIDERS
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

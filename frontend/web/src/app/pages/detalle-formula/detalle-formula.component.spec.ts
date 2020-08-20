import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { DetalleFormulaComponent } from './detalle-formula.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientModule } from '@angular/common/http';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('DetalleFormulaComponent', () => {
  let component: DetalleFormulaComponent;
  let fixture: ComponentFixture<DetalleFormulaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, MATERIAL_COMPONENTS, HttpClientTestingModule, ReactiveFormsModule, FormsModule, BrowserAnimationsModule],
      declarations: [ DetalleFormulaComponent ],
      providers: [DatePipe]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetalleFormulaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

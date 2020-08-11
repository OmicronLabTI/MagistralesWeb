import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { PedidosComponent } from './pedidos.component';
import {CUSTOM_ELEMENTS_SCHEMA} from "@angular/core";
import {MatTableModule, MatMenuModule} from '@angular/material';
import {HttpClientModule} from "@angular/common/http";
import {DatePipe} from '@angular/common';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';

describe('PedidosComponent', () => {
  let component: PedidosComponent;
  let fixture: ComponentFixture<PedidosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PedidosComponent ],
      imports: [RouterTestingModule, MATERIAL_COMPONENTS, HttpClientModule],
      providers: [
        DatePipe
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PedidosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

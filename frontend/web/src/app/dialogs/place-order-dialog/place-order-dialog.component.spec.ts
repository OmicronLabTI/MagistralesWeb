import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';

import { PlaceOrderDialogComponent } from './place-order-dialog.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {HttpClientModule} from '@angular/common/http';
import {DatePipe} from '@angular/common';
import {PlaceOrders} from '../../model/device/orders';

describe('PlaceOrderDialogComponent', () => {
  let component: PlaceOrderDialogComponent;
  let fixture: ComponentFixture<PlaceOrderDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlaceOrderDialogComponent ],
      imports: [
          HttpClientModule,
        MatDialogModule,
        MATERIAL_COMPONENTS,
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        {
          provide: MatDialogRef,
          useValue: {}
        },
        { provide: MAT_DIALOG_DATA, useValue: {PlaceOrders}},
          DatePipe
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlaceOrderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

/*  it('should create', () => {
    component.placeData.placeOrdersData = {list: [], modalType: '', userId: ''};
    // expect(component).toBeTruthy();
  });*/
});

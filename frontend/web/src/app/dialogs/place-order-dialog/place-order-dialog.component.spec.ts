import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlaceOrderDialogComponent } from './place-order-dialog.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';

describe('PlaceOrderDialogComponent', () => {
  let component: PlaceOrderDialogComponent;
  let fixture: ComponentFixture<PlaceOrderDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlaceOrderDialogComponent ],
      imports: [MATERIAL_COMPONENTS],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlaceOrderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

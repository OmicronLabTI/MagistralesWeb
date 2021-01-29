import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WarehouseComponent } from './warehouse.component';
import {DatePipe} from '@angular/common';

describe('WarehouseComponent', () => {
  let component: WarehouseComponent;
  let fixture: ComponentFixture<WarehouseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WarehouseComponent ],
      providers: [DatePipe]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WarehouseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

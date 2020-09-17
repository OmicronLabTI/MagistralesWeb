import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FinalizeOrdersComponent } from './finalize-orders.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {FormsModule} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {DatePipe} from '@angular/common';

describe('FinalizeOrdersComponent', () => {
  let component: FinalizeOrdersComponent;
  let fixture: ComponentFixture<FinalizeOrdersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FinalizeOrdersComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      imports: [MATERIAL_COMPONENTS, FormsModule, HttpClientTestingModule],
      providers: [{ provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {filterOrdersData: []} },
        DatePipe]

    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinalizeOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

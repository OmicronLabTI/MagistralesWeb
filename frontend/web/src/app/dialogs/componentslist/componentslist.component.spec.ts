import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
  MatTableModule,
  MatCheckboxModule,
  MatFormFieldModule,
  MatInputModule,
} from '@angular/material';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { ComponentslistComponent } from './componentslist.component';
import {RouterTestingModule} from '@angular/router/testing';

describe('ComponentslistComponent', () => {
  let component: ComponentslistComponent;
  let fixture: ComponentFixture<ComponentslistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        MatTableModule,
        MatDialogModule,
        MatCheckboxModule,
        MatFormFieldModule, MatInputModule,
        BrowserAnimationsModule, RouterTestingModule],
      declarations: [ ComponentslistComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        DatePipe, {
          provide: MatDialogRef,
          useValue: {}
        },
        {
          provide: MAT_DIALOG_DATA, useValue: {}
        }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ComponentslistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

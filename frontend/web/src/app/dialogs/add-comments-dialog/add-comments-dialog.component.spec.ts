import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCommentsDialogComponent } from './add-comments-dialog.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

describe('AddCommentsDialogComponent', () => {
  let component: AddCommentsDialogComponent;
  let fixture: ComponentFixture<AddCommentsDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddCommentsDialogComponent ],
      imports: [FormsModule, ReactiveFormsModule],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        {
          provide: MatDialogRef,
          useValue: () => {}
        },
        { provide: MAT_DIALOG_DATA, useValue: 'comments' }
        ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddCommentsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
});

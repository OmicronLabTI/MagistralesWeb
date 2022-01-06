import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCommentsDialogComponent } from './add-comments-dialog.component';
import {CUSTOM_ELEMENTS_SCHEMA, ElementRef} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import { MockElementRef } from 'src/mocks/ElementRefMock';

describe('AddCommentsDialogComponent', () => {
  let component: AddCommentsDialogComponent;
  let fixture: ComponentFixture<AddCommentsDialogComponent>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<AddCommentsDialogComponent>>;

  beforeEach(async(() => {
    dialogRefSpy = jasmine.createSpyObj<MatDialogRef<AddCommentsDialogComponent>>('MatDialogRef', [
      'close',
    ]);
    // dialogRefSpy.close.and.returnValue();
    TestBed.configureTestingModule({
      declarations: [ AddCommentsDialogComponent ],
      imports: [FormsModule, ReactiveFormsModule],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        {
          provide: MatDialogRef,
          useValue: dialogRefSpy
        },
        { provide: MAT_DIALOG_DATA, useValue: 'comments' },
        { provide: ElementRef, useValue: MockElementRef}
        ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddCommentsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  // // @ViewChild('finishComments', {static: true}) finishComments: ElementRef;
  //   // expect(component).toBeTruthy();
  // });
  // // it('should saveComments', () => {
  // //   component.saveComments();
  // //   expect(dialogRefSpy.close).toBeTruthy();
  // // });
  // it('should scroll', () => {
  //   // expect(component.scroll).toBeTruthy();
  //   // expect(component.finishComments).toHaveBeenCalled();
  // });
  // it('should checkData', () => {
  //   component.checkData();
  //   expect(component.isCorrectData).toBeTruthy();
  // });
});

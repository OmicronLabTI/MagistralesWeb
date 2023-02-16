import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCommentsDialogComponent } from './add-comments-dialog.component';
import { CUSTOM_ELEMENTS_SCHEMA, ElementRef } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MockElementRef } from 'src/mocks/ElementRefMock';

const finishCommentsMock = {
  nativeElement: {
    scrollIntoView: () => { }
  }
};
const mockCommentsConfig1 = {
  comments: '',
  isForClose: false,
  isReadyOnly: false,
  isForRefuseOrders: false
};

const mockCommentsConfig2 = {
  comments: '',
  isForClose: true,
  isReadyOnly: true,
  isForRefuseOrders: true
};
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
      declarations: [AddCommentsDialogComponent],
      imports: [FormsModule, ReactiveFormsModule],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        {
          provide: MatDialogRef,
          useValue: dialogRefSpy
        },
        { provide: MAT_DIALOG_DATA, useValue: 'comments' },
        { provide: ElementRef, useValue: MockElementRef }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddCommentsDialogComponent);
    component = fixture.componentInstance;
    component.finishComments = finishCommentsMock;
    component.commentsConfig = mockCommentsConfig1;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should save comments', () => {
    component.saveComments();
    expect(component.commentsConfig.isReadyOnly).toBeFalsy();
  });
  it('should save comments config 2', () => {
    component.commentsConfig = mockCommentsConfig2;
    component.saveComments();
    expect(component.commentsConfig.isReadyOnly).toBeTruthy();
  });

  it('should component create config 2', () => {
    component.commentsConfig = mockCommentsConfig2;
    component.ngOnInit();
    expect(component.commentsConfig.isReadyOnly).toBeTruthy();
  });

  it('should checkData', () => {
    component.checkData();
    expect(component.isCorrectData).toBeTruthy();
  });
});

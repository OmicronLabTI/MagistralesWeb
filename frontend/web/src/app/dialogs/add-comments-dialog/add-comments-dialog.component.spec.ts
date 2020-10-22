import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCommentsDialogComponent } from './add-comments-dialog.component';

describe('AddCommentsDialogComponent', () => {
  let component: AddCommentsDialogComponent;
  let fixture: ComponentFixture<AddCommentsDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddCommentsDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddCommentsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

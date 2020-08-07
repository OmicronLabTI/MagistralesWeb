import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FindOrdersDialogComponent } from './find-orders-dialog.component';

describe('FindOrdersDialogComponent', () => {
  let component: FindOrdersDialogComponent;
  let fixture: ComponentFixture<FindOrdersDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FindOrdersDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FindOrdersDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

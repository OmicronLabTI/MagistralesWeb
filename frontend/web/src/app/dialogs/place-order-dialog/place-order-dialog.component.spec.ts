import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlaceOrderDialogComponent } from './place-order-dialog.component';

describe('PlaceOrderDialogComponent', () => {
  let component: PlaceOrderDialogComponent;
  let fixture: ComponentFixture<PlaceOrderDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlaceOrderDialogComponent ]
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

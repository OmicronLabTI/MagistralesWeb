import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersRefuseComponent } from './orders-refuse.component';

describe('OrdersRefuseComponent', () => {
  let component: OrdersRefuseComponent;
  let fixture: ComponentFixture<OrdersRefuseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrdersRefuseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdersRefuseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

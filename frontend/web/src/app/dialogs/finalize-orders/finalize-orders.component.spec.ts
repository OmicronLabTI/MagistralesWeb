import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FinalizeOrdersComponent } from './finalize-orders.component';

describe('FinalizeOrdersComponent', () => {
  let component: FinalizeOrdersComponent;
  let fixture: ComponentFixture<FinalizeOrdersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FinalizeOrdersComponent ]
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

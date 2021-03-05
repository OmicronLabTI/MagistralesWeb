import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ButtonRefreshComponent } from './button-refresh.component';

describe('ButtonRefreshComponent', () => {
  let component: ButtonRefreshComponent;
  let fixture: ComponentFixture<ButtonRefreshComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ButtonRefreshComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ButtonRefreshComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

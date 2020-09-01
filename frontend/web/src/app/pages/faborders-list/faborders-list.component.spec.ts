import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FabordersListComponent } from './faborders-list.component';

describe('FabordersListComponent', () => {
  let component: FabordersListComponent;
  let fixture: ComponentFixture<FabordersListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FabordersListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FabordersListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

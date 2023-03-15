import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HistoryMaterialRequestComponent } from './history-material-request.component';

describe('HistoryMaterialRequestComponent', () => {
  let component: HistoryMaterialRequestComponent;
  let fixture: ComponentFixture<HistoryMaterialRequestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HistoryMaterialRequestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HistoryMaterialRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

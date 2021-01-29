import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IncidentsListComponent } from './incidents-list.component';
import {DatePipe} from '@angular/common';

describe('IncidentsListComponent', () => {
  let component: IncidentsListComponent;
  let fixture: ComponentFixture<IncidentsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IncidentsListComponent ],
      providers: [DatePipe]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IncidentsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

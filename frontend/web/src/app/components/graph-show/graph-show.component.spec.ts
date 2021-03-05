import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GraphShowComponent } from './graph-show.component';
import {DatePipe} from '@angular/common';
import {RouterTestingModule} from '@angular/router/testing';

describe('GraphShowComponent', () => {
  let component: GraphShowComponent;
  let fixture: ComponentFixture<GraphShowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GraphShowComponent ],
      providers: [DatePipe],
      imports: [RouterTestingModule]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GraphShowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

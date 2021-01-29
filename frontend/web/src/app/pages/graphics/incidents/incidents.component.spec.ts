import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IncidentsComponent } from './incidents.component';
import {DatePipe} from '@angular/common';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('IncidentsComponent', () => {
  let component: IncidentsComponent;
  let fixture: ComponentFixture<IncidentsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IncidentsComponent ],
      imports: [HttpClientTestingModule],
      providers: [DatePipe],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IncidentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

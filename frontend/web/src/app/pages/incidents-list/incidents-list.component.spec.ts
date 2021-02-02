import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IncidentsListComponent } from './incidents-list.component';
import {DatePipe} from '@angular/common';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {PipesModule} from '../../pipes/pipes.module';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {HttpClientTestingModule} from '@angular/common/http/testing';

describe('IncidentsListComponent', () => {
  let component: IncidentsListComponent;
  let fixture: ComponentFixture<IncidentsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IncidentsListComponent ],
      providers: [DatePipe],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      imports: [ PipesModule, MATERIAL_COMPONENTS, HttpClientTestingModule]
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

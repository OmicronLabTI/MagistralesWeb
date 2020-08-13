import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ComponentSearchComponent } from './component-search.component';
import { HttpClientModule } from '@angular/common/http';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatChipsModule} from '@angular/material/chips';
import {
  MatTableModule,
  MatCheckboxModule,
  MatFormFieldModule,
  MatInputModule,
} from '@angular/material';
import {DatePipe} from '@angular/common';

describe('ComponentSearchComponent', () => {
  let component: ComponentSearchComponent;
  let fixture: ComponentFixture<ComponentSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule, MatTableModule,
        MatCheckboxModule, MatFormFieldModule, MatInputModule, BrowserAnimationsModule, MatChipsModule],
      declarations: [ ComponentSearchComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [DatePipe]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ComponentSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

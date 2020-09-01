import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {DatePipe} from '@angular/common';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import { FabordersListComponent } from './faborders-list.component';

describe('FabordersListComponent', () => {
  let component: FabordersListComponent;
  let fixture: ComponentFixture<FabordersListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FabordersListComponent ],
      imports: [ RouterTestingModule, MATERIAL_COMPONENTS, HttpClientTestingModule, BrowserAnimationsModule ],
      providers: [
        DatePipe,
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
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

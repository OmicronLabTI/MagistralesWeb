import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RangeDateComponent } from './range-date.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {FormsModule} from '@angular/forms';
import {DatePipe} from '@angular/common';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {RouterTestingModule} from '@angular/router/testing';

import { DateService } from 'src/app/services/date.service';

describe('RangeDateComponent', () => {
  let component: RangeDateComponent;
  let fixture: ComponentFixture<RangeDateComponent>;
  let dateServiceSpy: jasmine.SpyObj<DateService>;

  beforeEach(async(() => {
    dateServiceSpy = jasmine.createSpyObj<DateService>('DataService', [
      'getDateArray',
      'getMaxMinDate',
      'getDateFormatted'
    ]);
    dateServiceSpy.getDateArray.and.returnValue([]);
    dateServiceSpy.getMaxMinDate.and.returnValue(new Date());
    dateServiceSpy.getDateFormatted.and.returnValue('');
    TestBed.configureTestingModule({
      declarations: [ RangeDateComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      imports: [MATERIAL_COMPONENTS, FormsModule, BrowserAnimationsModule, RouterTestingModule],
      providers:
      [
        DatePipe,
        { provide: DateService, useValue: dateServiceSpy },
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RangeDateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    // component.typeInitialRange = 1;
    expect(component).toBeTruthy();
  });

  it('should onDataChange', () => {
    component.onDataChange();
    expect(component.onDataChange).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GraphShowComponent } from './graph-show.component';
import {DatePipe} from '@angular/common';
import {RouterTestingModule} from '@angular/router/testing';
import { DataService } from 'src/app/services/data.service';
import { IncidentsGraphicsMatrix } from 'src/app/model/http/incidents.model';
import { ConfigurationGraphic } from 'src/app/model/device/incidents.model';
import { ChangeDetectorRef } from '@angular/core';

describe('GraphShowComponent', () => {
  let component: GraphShowComponent;
  let fixture: ComponentFixture<GraphShowComponent>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  // const

  beforeEach(async(() => {
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'getOptionsGraphToShow',
      'getDataForGraphic',
      'getPercentageByItem']);
    // dataServiceSpy.getOptionsGraphToShow.and.returnValue(());
    // dataServiceSpy.getDataForGraphic.and.returnValue();
    dataServiceSpy.getPercentageByItem.and.returnValue('');
    TestBed.configureTestingModule({
      declarations: [ GraphShowComponent ],
      providers: [DatePipe,
      { provide: DataService, useValue: dataServiceSpy},
    ],
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

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IncidentsComponent } from './incidents.component';
import {DatePipe} from '@angular/common';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {RouterTestingModule} from '@angular/router/testing';
import { DataService } from 'src/app/services/data.service';
import {IncidentsService} from '../../../services/incidents.service';
import { of } from 'rxjs';
import { IIncidentsGraphicRes } from 'src/app/model/http/incidents.model';
import { ItemIndicator } from 'src/app/model/device/incidents.model';

describe('IncidentsComponent', () => {
  let component: IncidentsComponent;
  let fixture: ComponentFixture<IncidentsComponent>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let incidentsServiceSpy;
  let errorServiceSpy;

  beforeEach(async(() => {
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'setUrlActive'
    ]);
    dataServiceSpy.setUrlActive.and.returnValue();
    incidentsServiceSpy = jasmine.createSpyObj<IncidentsService>('IncidentsService', [
      'getIncidentsGraph'
    ]);
    incidentsServiceSpy.getIncidentsGraph.and.callFake(() => {
      return of(IIncidentsGraphicRes);
    });
    TestBed.configureTestingModule({
      declarations: [ IncidentsComponent ],
      imports: [HttpClientTestingModule, RouterTestingModule],
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

  it('should newIteratorsEvent', () => {
    const newIndicators: ItemIndicator[] = [{
      nameItem: '',
      background: '',
      percentage: '',
      count: 1
    }];
    component.newIteratorsEvent(newIndicators);
    expect(component.newIteratorsEvent).toBeTruthy();
  });
});

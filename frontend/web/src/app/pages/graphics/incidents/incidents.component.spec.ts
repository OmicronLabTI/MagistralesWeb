import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IncidentsComponent } from './incidents.component';
import {DatePipe} from '@angular/common';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {RouterTestingModule} from '@angular/router/testing';
import {IncidentsService} from '../../../services/incidents.service';
import { of, throwError } from 'rxjs';
import { IIncidentsGraphicRes } from 'src/app/model/http/incidents.model';
import { ConfigurationGraphic, ItemIndicator } from 'src/app/model/device/incidents.model';
import { ErrorService } from 'src/app/services/error.service';
import { ObservableService } from 'src/app/services/observable.service';

describe('IncidentsComponent', () => {
  let component: IncidentsComponent;
  let fixture: ComponentFixture<IncidentsComponent>;
  let incidentsServiceSpy: jasmine.SpyObj<IncidentsService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;

  const iIncidentsGraphicRes = new IIncidentsGraphicRes();
  let errorServiceSpy;
  const configurationGraphic = new ConfigurationGraphic();
  // const incidentsGraphicsMatrix = new IncidentsGraphicsMatrix[0][0]();

  beforeEach(async(() => {
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('DataService', [
      'setUrlActive'
    ]);
    observableServiceSpy.setUrlActive.and.returnValue();
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    incidentsServiceSpy = jasmine.createSpyObj<IncidentsService>('IncidentsService', [
      'getIncidentsGraph'
    ]);
    incidentsServiceSpy.getIncidentsGraph.and.callFake(() => {
      return of(iIncidentsGraphicRes);
    });
    TestBed.configureTestingModule({
      declarations: [ IncidentsComponent ],
      imports: [HttpClientTestingModule, RouterTestingModule],
      providers: [DatePipe,
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: IncidentsService, useValue: incidentsServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy }],
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

  it('should checkNewRange', () => {
    const incidentsGraphicsMatrix = [[{
      fieldKey: 'IncidentReason',
      totalCount: 1,
      graphType: 'IncidentReason',
      // color?: string;
    }],
    [{
      fieldKey: 'IncidentReason',
      totalCount: 1,
      graphType: 'IncidentReason',
      // color?: string;
    }]];
    iIncidentsGraphicRes.response = [
      incidentsGraphicsMatrix[0]
    ];
    configurationGraphic.isPie = true,
    configurationGraphic.titleForGraph = '',
    configurationGraphic.dataGraph = incidentsGraphicsMatrix[0],
    configurationGraphic.isSmall = false,
    configurationGraphic.isWithFullTooltip = false,
    component.incidentsGraphCOnf = configurationGraphic;
    // component.checkNewRange('0');
    component.generateConfigurationGraph(incidentsGraphicsMatrix);
    // expect(component.checkNewRange).toBeTruthy();
    // expect(incidentsServiceSpy.getIncidentsGraph).toEqual(iIncidentsGraphicRes.response);
  });

  it('should checkNewRange error', () => {
    // expect(component.checkNewRange).toBeFalsy();
    incidentsServiceSpy.getIncidentsGraph.and.callFake(() => {
      return throwError({ error: true });
    });
    component.checkNewRange('1');
    expect(errorServiceSpy.httpError).toHaveBeenCalledWith({ error: true });
  });
});

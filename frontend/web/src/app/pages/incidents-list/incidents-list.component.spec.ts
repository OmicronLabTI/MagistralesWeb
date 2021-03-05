import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IncidentsListComponent } from './incidents-list.component';
import {DatePipe} from '@angular/common';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {PipesModule} from '../../pipes/pipes.module';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {IncidentsService} from '../../services/incidents.service';
import {of} from 'rxjs';
import {IncidentListMock} from '../../../mocks/incidentsListMock';
import {ConstStatus} from '../../constants/const';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {RouterTestingModule} from '@angular/router/testing';

describe('IncidentsListComponent', () => {
  let component: IncidentsListComponent;
  let fixture: ComponentFixture<IncidentsListComponent>;
  let incidentsServiceSpy;
  beforeEach(async(() => {
    incidentsServiceSpy = jasmine.createSpyObj<IncidentsService>('IncidentsService', [
      'getIncidentsList'
    ]);
    incidentsServiceSpy.getIncidentsList.and.callFake(() => {
      return of(IncidentListMock);
    });
    TestBed.configureTestingModule({
      declarations: [ IncidentsListComponent ],
      providers: [DatePipe,
        { provide: IncidentsService, useValue: incidentsServiceSpy }],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      imports: [ PipesModule, MATERIAL_COMPONENTS, HttpClientTestingModule,
        BrowserAnimationsModule, RouterTestingModule]
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
  it('should call onInit()', () => {
     component.ngOnInit();
     expect(component.filterDataIncidents.isFromIncidents).toBeTruthy();
  });
  it('should call onSuccessSearchOrderModal()', () => {
    component.onSuccessSearchOrderModal({
      dateType: '0',
      docNum: 12345,
      fini: new Date(),
      ffin: new Date(),
      status: ConstStatus.enProceso,
      dateFull: '20/12/2020-20/01/2021',
      isFromIncidents: true,
      finlabel: ''
    });
    expect(component.pageIndex).toEqual(0);
    expect(component.offset).toEqual(0);
    expect(component.limit).toEqual(10);
    expect(component.lengthPaginator).toEqual(IncidentListMock.comments);
    expect(component.isOnInit).toBeFalsy();
  });

});

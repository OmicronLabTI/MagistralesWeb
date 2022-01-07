import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IncidentsListComponent } from './incidents-list.component';
import { DatePipe } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { PipesModule } from '../../pipes/pipes.module';
import { MATERIAL_COMPONENTS } from '../../app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { IncidentsService } from '../../services/incidents.service';
import { of, throwError } from 'rxjs';
import { IncidentListMock } from '../../../mocks/incidentsListMock';
import { ConstStatus } from '../../constants/const';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterTestingModule } from '@angular/router/testing';
import { ErrorService } from 'src/app/services/error.service';
import { PageEvent } from '@angular/material';
import { ObservableService } from 'src/app/services/observable.service';
import { ParamsPedidos } from '../../model/http/pedidos';
import { CommentsConfig } from '../../model/device/incidents.model';
import { DateService } from 'src/app/services/date.service';

describe('IncidentsListComponent', () => {
  let component: IncidentsListComponent;
  let fixture: ComponentFixture<IncidentsListComponent>;
  let incidentsServiceSpy;
  let errorServiceSpy;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let dateServiceSpy: jasmine.SpyObj<DateService>;

  const paramPedidos = new ParamsPedidos();
  const commentsConfig = new CommentsConfig();

  const pageEvent = new PageEvent();
  beforeEach(async(() => {
    incidentsServiceSpy = jasmine.createSpyObj<IncidentsService>('IncidentsService', [
      'getIncidentsList'
    ]);
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    incidentsServiceSpy.getIncidentsList.and.callFake(() => {
      return of(IncidentListMock);
    });
    //  --- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'setUrlActive',
        'getNewSearchOrdersModal',
        'getNewCommentsResult',
        'setSearchOrdersModal',
        'setMessageGeneralCallHttp',
        'setOpenCommentsDialog',
      ]);
    observableServiceSpy.getNewSearchOrdersModal.and.returnValue(of(paramPedidos));
    observableServiceSpy.getNewCommentsResult.and.returnValue(of(commentsConfig));
    // --- Date Service
    dateServiceSpy = jasmine.createSpyObj<DateService>('DateService', [
      'getDateFormatted'
    ]);
    dateServiceSpy.getDateFormatted.and.returnValue('');
    TestBed.configureTestingModule({
      declarations: [IncidentsListComponent],
      providers: [DatePipe,
        { provide: IncidentsService, useValue: incidentsServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: DateService, useValue: dateServiceSpy },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      imports: [PipesModule, MATERIAL_COMPONENTS, HttpClientTestingModule,
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

  it('should openFindOrdersDialog', () => {
    component.openFindOrdersDialog();
  });

  it('should updateIncidentList error', () => {
    incidentsServiceSpy.getIncidentsList.and.callFake(() => {
      return throwError({ error: true });
    });
    component.updateIncidentList();
    expect(errorServiceSpy.httpError).toHaveBeenCalledWith({ error: true });
  });

  it('should reloadIncidentsList', () => {
    component.reloadIncidentsList();
  });

  it('should openCommentsDialog', () => {
    component.openCommentsDialog(0);
  });

  it('should changeDataEvent', () => {
    component.changeDataEvent(pageEvent);
  });
});

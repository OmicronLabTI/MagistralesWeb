import { TestBed } from '@angular/core/testing';

import { IncidentsService } from './incidents.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {DatePipe} from '@angular/common';
import {Observable} from 'rxjs';
import {ChangeStatusIncidentReq} from '../model/http/incidents.model';
import {RouterTestingModule} from '@angular/router/testing';

describe('IncidentsService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule, RouterTestingModule],
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: IncidentsService = TestBed.get(IncidentsService);
    expect(service).toBeTruthy();
  });
  it('should getPreMaterialRequest', () => {
    const service: IncidentsService = TestBed.get(IncidentsService);
    expect(service.getIncidentsGraph('12/12/2020-12/12/2021') instanceof Observable).toBeTruthy();
  });
  it('should getWarehouseGraph', () => {
    const service: IncidentsService = TestBed.get(IncidentsService);
    expect(service.getWarehouseGraph('12/12/2020-12/12/2021') instanceof Observable).toBeTruthy();
  });
  it('should getIncidentsList', () => {
    const service: IncidentsService = TestBed.get(IncidentsService);
    expect(service.getIncidentsList('queryString') instanceof Observable).toBeTruthy();
  });
  it('should patchStatusIncidents', () => {
    const service: IncidentsService = TestBed.get(IncidentsService);
    expect(service.patchStatusIncidents({} as ChangeStatusIncidentReq) instanceof Observable).toBeTruthy();
  });
});

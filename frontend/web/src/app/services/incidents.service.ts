import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {IIncidentsGraphicRes} from '../model/http/incidents.model';
import {Endpoints} from '../../environments/endpoints';

@Injectable({
  providedIn: 'root'
})
export class IncidentsService {

  constructor(private consumeService: ConsumeService) {
  }
  getIncidentsGraph(rangeDate: string) {
    return this.consumeService.httpGet<IIncidentsGraphicRes>(`${Endpoints.incidents.graphIncidents}?fini=${rangeDate}`);
  }
}

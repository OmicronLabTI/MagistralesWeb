import {Component, OnInit} from '@angular/core';
import {CONST_NUMBER, CONST_STRING, GraphType, HttpServiceTOCall} from '../../../constants/const';

import {IncidentsGraphicsMatrix} from '../../../model/http/incidents.model';
import {ConfigurationGraphic, ItemIndicator} from '../../../model/device/incidents.model';
import {IncidentsService} from '../../../services/incidents.service';
import {ErrorService} from '../../../services/error.service';
import { ObservableService } from '../../../services/observable.service';
@Component({
  selector: 'app-incidents',
  templateUrl: './incidents.component.html',
  styleUrls: ['./incidents.component.scss']
})
export class IncidentsComponent implements OnInit {
  incidentsGraphCOnf = new ConfigurationGraphic();
  statusGraph = new ConfigurationGraphic();
  newIndicators: ItemIndicator[] = [];
  isNoDataIncidentsGraph = false;
  isNoDataStatusGraph = false;
  constructor(
    private incidentsService: IncidentsService,
    private errorService: ErrorService,
    private observableService: ObservableService) {
    this.observableService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
  }

  ngOnInit() {
  }

  checkNewRange(newRange: string) {
    this.incidentsService.getIncidentsGraph(newRange).subscribe( ({response}) => this.generateConfigurationGraph(response)
    , error => this.errorService.httpError(error));
  }

  newIteratorsEvent(newIterators: ItemIndicator[]) {
      this.newIndicators = newIterators;
  }
  generateConfigurationGraph(response: IncidentsGraphicsMatrix[][]) {
    this.incidentsGraphCOnf = new ConfigurationGraphic();
    this.incidentsGraphCOnf.isPie = true;
    this.incidentsGraphCOnf.titleForGraph = CONST_STRING.empty;
    this.incidentsGraphCOnf.dataGraph = response[0][0].graphType === GraphType.incidentGraph ? response[0] : response[1];
    this.isNoDataIncidentsGraph = this.incidentsGraphCOnf.dataGraph.every( incident =>
                                         incident.totalCount === CONST_NUMBER.zero);

    this.statusGraph = new ConfigurationGraphic();
    this.statusGraph.titleForGraph = CONST_STRING.empty;
    this.statusGraph.isPie = false;
    this.statusGraph.dataGraph = response[1][0].graphType === GraphType.statusGraph ? response[1] : response[0];
    this.isNoDataStatusGraph = this.statusGraph.dataGraph.every( status => status.totalCount === CONST_NUMBER.zero);

  }
}

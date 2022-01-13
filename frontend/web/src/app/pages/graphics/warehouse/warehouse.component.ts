import {Component, OnInit} from '@angular/core';
import {DataService} from '../../../services/data.service';
import {
  ColorsReception,
  CONST_NUMBER,
  GraphType,
  HttpServiceTOCall,
  TypeReception
} from '../../../constants/const';
import {IncidentsGraphicsMatrix} from '../../../model/http/incidents.model';
import {ConfigurationGraphic} from '../../../model/device/incidents.model';
import {IncidentsService} from '../../../services/incidents.service';
import {ErrorService} from '../../../services/error.service';
import { ObservableService } from '../../../services/observable.service';


@Component({
  selector: 'app-warehouse',
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.scss']
})
export class WarehouseComponent implements OnInit {
  itemsGraphReceive: IncidentsGraphicsMatrix[] = [];
  itemsGraph: IncidentsGraphicsMatrix[] = [];
  localPackages: IncidentsGraphicsMatrix[] = [];
  foreignPackages: IncidentsGraphicsMatrix[] = [];
  incidentsConfigurationGraph = new ConfigurationGraphic();
  typesGraph = TypeReception;
  percentageLocal = CONST_NUMBER.zero;
  percentageForeign = CONST_NUMBER.zero;
  isNoDataNoDeliveredGraph = false;
  constructor(
    private dataService: DataService,
    private incidentsService: IncidentsService,
    private errorService: ErrorService,
    private observableService: ObservableService) {
    this.observableService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
  }

  ngOnInit() {
  }

  checkNewRange(rangeDate: string) {
    this.incidentsService.getWarehouseGraph(rangeDate).subscribe(({response}) => this.generateDataWarehouse(response)
            , error => this.errorService.httpError(error));
  }
  generateDataWarehouse(response: IncidentsGraphicsMatrix[]) {
    // generate data init
    // items top
    this.itemsGraphReceive = this.getNewReceptionData(response.filter( itemGraph => itemGraph.graphType === GraphType.reception));
    this.itemsGraph = [this.itemsGraphReceive[0], this.itemsGraphReceive[3], this.itemsGraphReceive[2], this.itemsGraphReceive[1]];

    // local package data
    this.localPackages = response.filter(
        itemGraph => itemGraph.graphType.toLowerCase() === GraphType.packageLocal.toLowerCase());

    // foreign package data
    this.foreignPackages = response.filter(
        itemGraph => itemGraph.graphType.toLowerCase() === GraphType.foreignPackage.toLowerCase());

    this.incidentsConfigurationGraph = new ConfigurationGraphic();
    this.incidentsConfigurationGraph.isPie = false;
    this.incidentsConfigurationGraph.titleForGraph = 'MOTIVOS - No entregado';
    this.incidentsConfigurationGraph.dataGraph = response.filter(
        itemGraph => itemGraph.graphType.toLowerCase() === GraphType.incidentGraph.toLowerCase());
    this.incidentsConfigurationGraph.isSmall = true;
    this.incidentsConfigurationGraph.isWithFullTooltip = true;

    // percentage items data
    this.percentageLocal = Number(
        this.dataService.getPercentageByItem(this.localPackages.filter(itemLocal =>
            itemLocal.fieldKey.toLowerCase() === GraphType.deliveredItemLocal.toLowerCase())[0].totalCount,
            this.localPackages.map(itemLocal => (itemLocal.totalCount)), true)) || 0;

    this.percentageForeign = Number(
        this.dataService.getPercentageByItem(this.foreignPackages.filter(itemLocal =>
            itemLocal.fieldKey.toLowerCase() === GraphType.sentItemForeign.toLowerCase())[0].totalCount,
            this.foreignPackages.map(itemLocal => (itemLocal.totalCount)), true)) || 0;

    this.isNoDataNoDeliveredGraph = this.incidentsConfigurationGraph.dataGraph.every( incident =>
        incident.totalCount === CONST_NUMBER.zero);

    // generate data finish
  }

  getNewReceptionData(incidentsGraphicsMatrices: IncidentsGraphicsMatrix[]) {
    incidentsGraphicsMatrices.forEach(itemGraph => {
      itemGraph.color = this.getColorItem(itemGraph.fieldKey.toLowerCase());
    });
    return incidentsGraphicsMatrices;
  }

  getColorItem(fieldKey: string) {
    switch (fieldKey) {
      case TypeReception.backOrder:
        return ColorsReception.backOrder;
      case TypeReception.byReceive:
        return  ColorsReception.byReceive;
      case TypeReception.pending:
        return  ColorsReception.pending;
      case TypeReception.warehoused:
        return ColorsReception.warehoused;
    }
  }
}

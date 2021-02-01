import {Component, OnInit} from '@angular/core';
import {DataService} from '../../../services/data.service';
import {
  ColorsReception,
  CONST_NUMBER,
  CONST_STRING,
  GraphType,
  HttpServiceTOCall,
  TypeReception
} from '../../../constants/const';
import {IncidentsGraphicsMatrix} from '../../../model/http/incidents.model';
import {ConfigurationGraphic} from '../../../model/device/incidents.model';
import {IncidentsService} from '../../../services/incidents.service';
import {ErrorService} from '../../../services/error.service';


@Component({
  selector: 'app-warehouse',
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.scss']
})
export class WarehouseComponent implements OnInit {
  responseTest: IncidentsGraphicsMatrix[]  = [
    {
      fieldKey: 'Por Recibir',
      totalCount: 1,
      graphType: 'Almacen'
    },
    {
      fieldKey: 'Almacenado',
      totalCount: 2,
      graphType: 'Almacen'
    },
    {
      fieldKey: 'Back order',
      totalCount: 3,
      graphType: 'Almacen'
    },
    {
      fieldKey: 'Pendiente',
      totalCount: 4,
      graphType: 'Almacen'
    },
    {
      fieldKey: 'Empaquetado',
      totalCount: 5,
      graphType: 'PackageLocal'
    },
    {
      fieldKey: 'Asignado',
      totalCount: 6,
      graphType: 'PackageLocal'
    },
    {
      fieldKey: 'En Camino',
      totalCount: 7,
      graphType: 'PackageLocal'
    },
    {
      fieldKey: 'Entregado',
      totalCount: 8,
      graphType: 'PackageLocal'
    },
    {
      fieldKey: 'No Entregado',
      totalCount: 9,
      graphType: 'PackageLocal'
    },
    {
      fieldKey: 'Empaquetado',
      totalCount: 10,
      graphType: 'PackageForeign'
    },
    {
      fieldKey: 'Enviado',
      totalCount: 6,
      graphType: 'PackageForeign'
    },
    {
      fieldKey: 'Cerrado',
      totalCount: 12,
      graphType: 'IncidentReason'
    },
    {
      fieldKey: 'Dirección incorrecta',
      totalCount: 13,
      graphType: 'IncidentReason'
    },
    {
      fieldKey: 'Pedido no confirmado',
      totalCount: 14,
      graphType: 'IncidentReason'
    },
    {
      fieldKey: 'Teléfono incorrecto',
      totalCount: 15,
      graphType: 'IncidentReason'
    }
  ];
  itemsGraph: IncidentsGraphicsMatrix[] = [];
  localPackages: IncidentsGraphicsMatrix[] = [];
  foreignPackages: IncidentsGraphicsMatrix[] = [];
  incidentsConfigurationGraph = new ConfigurationGraphic();
  typesGraph = TypeReception;
  percentageLocal = CONST_NUMBER.zero;
  percentageForeign = CONST_NUMBER.zero;
  constructor(private dataService: DataService, private incidentsService: IncidentsService,
              private errorService: ErrorService) {
    this.dataService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
  }

  ngOnInit() {
  }

  checkNewRange(rangeDate: string) {
      /*this.incidentsService.getWarehouseGraph(rangeDate).subscribe(warehouseResult => console.log('warehouseResult: ', warehouseResult)
            , error => this.errorService.httpError(error));*/
      // generate data init
      // items top
      this.itemsGraph = this.getNewReceptionData(this.responseTest.filter( itemGraph => itemGraph.graphType === GraphType.reception));

      // local package data
      this.localPackages = this.responseTest.filter(
          itemGraph => itemGraph.graphType.toLowerCase() === GraphType.packageLocal.toLowerCase());

      // foreign package data
      this.foreignPackages = this.responseTest.filter(
          itemGraph => itemGraph.graphType.toLowerCase() === GraphType.foreignPackage.toLowerCase());

      this.incidentsConfigurationGraph = new ConfigurationGraphic();
      this.incidentsConfigurationGraph.isPie = true;
      this.incidentsConfigurationGraph.titleForGraph = CONST_STRING.empty;
      this.incidentsConfigurationGraph.dataGraph = this.responseTest.filter(
          itemGraph => itemGraph.graphType.toLowerCase() === GraphType.incidentGraph.toLowerCase());
      this.incidentsConfigurationGraph.isSmall = true;
      this.incidentsConfigurationGraph.isWithFullTooltip = true;

      // percentage items data
      this.percentageLocal = Number(
          this.dataService.getPercentageByItem(this.localPackages.filter(itemLocal =>
              itemLocal.fieldKey.toLowerCase() === GraphType.deliveredItemLocal.toLowerCase())[0].totalCount,
              this.localPackages.map(itemLocal => (itemLocal.totalCount)), true));

      this.percentageForeign = Number(
          this.dataService.getPercentageByItem(this.foreignPackages.filter(itemLocal =>
              itemLocal.fieldKey.toLowerCase() === GraphType.sentItemForeign.toLowerCase())[0].totalCount,
              this.foreignPackages.map(itemLocal => (itemLocal.totalCount)), true));

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

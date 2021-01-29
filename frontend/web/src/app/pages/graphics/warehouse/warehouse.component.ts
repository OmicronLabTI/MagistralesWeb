import {Component, OnInit} from '@angular/core';
import {DataService} from '../../../services/data.service';
import {ColorsReception, GraphType, HttpServiceTOCall, TypeReception} from '../../../constants/const';
import {IncidentsGraphicsMatrix} from ../../../model/http/incidents.model;


@Component({
  selector: 'app-warehouse',
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.scss']
})
export class WarehouseComponent implements OnInit {
  responseTest: IncidentsGraphicsMatrix[]  = [
    {
      "fieldKey": "Por Recibir",
      "totalCount": 1,
      "graphType": "Almacen"
    },
    {
      "fieldKey": "Almacenado",
      "totalCount": 2,
      "graphType": "Almacen"
    },
    {
      "fieldKey": "Back order",
      "totalCount": 3,
      "graphType": "Almacen"
    },
    {
      "fieldKey": "Pendiente",
      "totalCount": 4,
      "graphType": "Almacen"
    },
    {
      "fieldKey": "Empaquetado",
      "totalCount": 5,
      "graphType": "PackageLocal"
    },
    {
      "fieldKey": "Asignado",
      "totalCount": 6,
      "graphType": "PackageLocal"
    },
    {
      "fieldKey": "En Camino",
      "totalCount": 7,
      "graphType": "PackageLocal"
    },
    {
      "fieldKey": "Entregado",
      "totalCount": 8,
      "graphType": "PackageLocal"
    },
    {
      "fieldKey": "No Entregado",
      "totalCount": 9,
      "graphType": "PackageLocal"
    },
    {
      "fieldKey": "Empaquetado",
      "totalCount": 10,
      "graphType": "PackageForeign"
    },
    {
      "fieldKey": "Enviado",
      "totalCount": 11,
      "graphType": "PackageForeign"
    },
    {
      "fieldKey": "Cerrado",
      "totalCount": 12,
      "graphType": "IncidentReason"
    },
    {
      "fieldKey": "Dirección incorrecta",
      "totalCount": 13,
      "graphType": "IncidentReason"
    },
    {
      "fieldKey": "Pedido no confirmado",
      "totalCount": 14,
      "graphType": "IncidentReason"
    },
    {
      "fieldKey": "Teléfono incorrecto",
      "totalCount": 15,
      "graphType": "IncidentReason"
    }
  ];
  typesReception = TypeReception;
  itemsGraph: IncidentsGraphicsMatrix[] = [];
  constructor(private dataService: DataService) {
    this.dataService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
  }

  ngOnInit() {
    this.itemsGraph = this.getNewReceptionData(this.responseTest.filter( itemGraph => itemGraph.graphType === GraphType.reception));

  }

    checkNewRange(rangeDate: string) {
      console.log('newRangeDate: ', rangeDate)
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

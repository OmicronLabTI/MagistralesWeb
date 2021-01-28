import {Component, OnInit} from '@angular/core';
import {CONST_STRING, GraphType, HttpServiceTOCall} from '../../../constants/const';
import {DataService} from '../../../services/data.service';

import {IncidentsGraphicsMatrix} from '../../../model/http/incidents.model';
import {ConfigurationGraphic} from '../../../model/device/incidents.model';
@Component({
  selector: 'app-incidents',
  templateUrl: './incidents.component.html',
  styleUrls: ['./incidents.component.scss']
})
export class IncidentsComponent implements OnInit {
  responseTest: IncidentsGraphicsMatrix [][] = [
    [
      {
        fieldKey: 'Producto derramado',
        totalCount: 0,
        graphType: "IncidentReason"
      },
      {
        fieldKey: "Nombre incorrecto",
        totalCount: 2,
        graphType: "IncidentReason"
      },
      {
        fieldKey: "Pedido incompleto",
        totalCount: 3,
        graphType: "IncidentReason"
      },
      {
        fieldKey: "Ingredientes incorrectos",
        totalCount: 4,
        graphType: "IncidentReason"
      },
      {
        fieldKey: "Envase incorrecto",
        totalCount: 5,
        graphType: "IncidentReason"
      },
    /*  {
        fieldKey: "Producto dañado",
        totalCount: 0,
        graphType: "IncidentReason"
      }*/
    ],
    [
      {
        fieldKey: "Abierta",
        totalCount: 3,
        graphType: "status"
      },
      {
        fieldKey: "Atendiendo",
        totalCount: 1,
        graphType: "status"
      },
      {
        fieldKey: "Cerrada",
        totalCount: 0,
        graphType: "status"
      }
    ]
  ];
  incidentsGraphCOnf = new ConfigurationGraphic();
  countTest = 0;
  constructor(private dataService: DataService) {
    this.dataService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
  }

  ngOnInit() {
  }

  checkNewRange(newRange: string) {
    if (this.countTest !== 0 ) {
      this.responseTest[0][0].totalCount = 20 + this.countTest;
    }
    this.incidentsGraphCOnf = new ConfigurationGraphic();
    this.incidentsGraphCOnf.isPie = false;
    this.incidentsGraphCOnf.titleForGraph = CONST_STRING.empty;
    this.incidentsGraphCOnf.dataGraph = this.responseTest[0][0].graphType === GraphType.incidentGraph ?
        this.responseTest[0] :
        this.responseTest[1];
    this.countTest ++;
    // this.statusGraphics = this.responseTest[1][0].graphType === GraphType.statusGraph ? this.responseTest[1] : this.responseTest[0];
  }
}

import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {Chart} from 'chart.js';
import {CONST_STRING, GraphType, HttpServiceTOCall} from '../../../constants/const';
import {DataService} from '../../../services/data.service';
import 'chartjs-plugin-labels';
import {IncidentsGraphicsMatrix} from '../../../model/http/incidents.model';
@Component({
  selector: 'app-incidents',
  templateUrl: './incidents.component.html',
  styleUrls: ['./incidents.component.scss']
})
export class IncidentsComponent implements OnInit {
  @ViewChild('incidentsChart', {static: true}) incidentsChart: ElementRef;
  myChart = CONST_STRING.empty;
  responseTest: IncidentsGraphicsMatrix [][] = [
    [
      {
        fieldKey: 'Producto derramado',
        totalCount: 1,
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
  incidentsGraphics: IncidentsGraphicsMatrix[] = [];
  statusGraphics: IncidentsGraphicsMatrix[] = [];
  constructor(private dataService: DataService) {
    this.dataService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
  }

  ngOnInit() {
  }

  checkNewRange(newRange: string) {

    this.incidentsGraphics = this.responseTest[0][0].graphType === GraphType.incidentGraph ? this.responseTest[0] : this.responseTest[1];
    this.statusGraphics = this.responseTest[1][0].graphType === GraphType.statusGraph ? this.responseTest[1] : this.responseTest[0];

    this.myChart = new Chart(this.incidentsChart.nativeElement.getContext('2d'), {
      type: 'pie',
      data: this.dataService.getDataForGraphic(this.incidentsGraphics),
      options: this.dataService.getOptionsGraphToShow({isPie: false, titleForGraph: 'Incidencias'})
    });

  }


}

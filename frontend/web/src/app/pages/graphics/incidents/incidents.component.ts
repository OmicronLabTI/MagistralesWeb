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
  optionsChart = {};
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
    const isPie = false;
    this.optionsChart = {
      tooltips: {
        enabled: isPie,
        callbacks: {
          label: (tooltipItem, data) => {
            return `${data.labels[tooltipItem.index]}: ${data.datasets[0].data[tooltipItem.index]} ( ${
              this.dataService.getPercentageByItem(data.datasets[0].data[tooltipItem.index], data.datasets[0].data)} )`;
          }
        }
      },
      legend: { display: false },
       title: {
        display: true,
        text: 'Predicted world population (millions) in 2050'
       },
      plugins: {
        labels: [
          ( isPie ? {
            render: 'label',
            fontColor: '#000',
            precision: 2,
            fontStyle: 'bold',
            position: 'outside'
          } : {} )
        ]
      }
    };
  }

  checkNewRange(newRange: string) {

    this.incidentsGraphics = this.responseTest[0][0].graphType === GraphType.incidentGraph ? this.responseTest[0] : this.responseTest[1];
    this.statusGraphics = this.responseTest[1][0].graphType === GraphType.statusGraph ? this.responseTest[1] : this.responseTest[0];
    // console.log('getData: ', this.getDataForGraphic(this.incidentsGraphics))
    this.myChart = new Chart(this.incidentsChart.nativeElement.getContext('2d'), {
      type: 'bar',
      data: this.getDataForGraphic(this.incidentsGraphics),
      options: this.optionsChart
    });

  }

  getDataForGraphic(itemsArray: IncidentsGraphicsMatrix[]) {
    console.log('labels: ', itemsArray.map(item => item.fieldKey))
    console.log('counts: ', itemsArray.map(item => item.totalCount))
    return {
      labels: itemsArray.map(item => item.fieldKey),
      datasets: [{
        label: 'Population (millions)',
        backgroundColor: ['#3e95cd', '#8e5ea2', '#3cba9f', '#e8c3b9', '#c45850'],
        data: itemsArray.map(item => item.totalCount),
        borderColor: '#fff',
        borderWidth: 3,
        hoverBorderWidth: 10,
        hoverBorderColor: '#c0c8ce'
      }]
    };
  }
}

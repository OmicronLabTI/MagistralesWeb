import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {Chart} from 'chart.js';
import {CONST_STRING, HttpServiceTOCall} from '../../../constants/const';
import {DataService} from '../../../services/data.service';
import 'chartjs-plugin-labels';
@Component({
  selector: 'app-incidents',
  templateUrl: './incidents.component.html',
  styleUrls: ['./incidents.component.scss']
})
export class IncidentsComponent implements OnInit {
  @ViewChild('incidentsChart', {static: true}) incidentsChart: ElementRef;
  myChart = CONST_STRING.empty;
  constructor(private dataService: DataService) {
    this.dataService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY);
  }

  ngOnInit() {

    const data = [{
      data: [20, 10, 30, 40 ],
      labels: ['India', 'China', 'US', 'Canada'],
      backgroundColor: [
        '#4b77a9',
        '#5f255f',
        '#d21243',
        '#B27200'
      ],
      borderColor: '#fff',
      borderWidth: 5,
      hoverBorderWidth: 15,
      hoverBorderColor: '#6C90B3'
    }];

    const optionsChart = {
      tooltips: {
        enabled: false
      },
      plugins: {
        labels: {
          render: 'percentage',
          fontColor: '#fff',
          precision: 2,
          fontStyle: 'bold',
        }
      }
    };
    this.myChart = new Chart(this.incidentsChart.nativeElement.getContext('2d'), {
      type: 'pie',
      data: {
        datasets: data
      },
      options: optionsChart
    });
  }

}

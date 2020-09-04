import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';

import { Title } from '@angular/platform-browser';
import { MatTableDataSource} from '@angular/material';
import {DataService} from '../../services/data.service';
import { ErrorService } from 'src/app/services/error.service';
import { IProductivityReq } from 'src/app/model/http/productivity';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Chart } from 'chart.js';
import { Colors } from 'src/app/constants/const';

const ELEMENT_DATA: string[][] = [
  [
    '-',
    'julio',
    'agosto',
    'septiembre'
  ],
  [
    'Armando Hoyos',
    '12',
    '13',
    '14'
  ],
  [
    'Carlos Espejel',
    '15',
    '16',
    '17'
  ],
  [
    'Fulano Perez',
    '18',
    '19',
    '20'
  ],
  [
    'Jaimito Luna',
    '21',
    '22',
    '23'
  ],
  [
    'Luis Mejia',
    '24',
    '25',
    '26'
  ]
];

@Component({
  selector: 'app-productivity',
  templateUrl: './productivity.component.html',
  styleUrls: ['./productivity.component.scss']
})
export class ProductivityComponent implements OnInit, AfterViewInit {
  monthColumns: string[];
  dataSourceDetails: string[][];
  dataSource = new MatTableDataSource<string[]>();
  colors = Colors;
  @ViewChild('productivityChart', {static: false}) productivityChart: ElementRef;
  constructor(
    private titleService: Title,
    private dataService: DataService,
    private errorService: ErrorService
  ) { }

  ngOnInit() {
    //let productivityChart = document.getElementById('productivityChart');
  }

  ngAfterViewInit() {
    console.log("hola: ", this.productivityChart.nativeElement);
    this.getProductivityData();
  }

  getProductivityData() {
    this.dataSourceDetails = ELEMENT_DATA;
    this.dataSource.data = ELEMENT_DATA.filter(element => ELEMENT_DATA.indexOf(element) > 0);
    this.monthColumns = this.dataSourceDetails[0];
    this.chartObject(ELEMENT_DATA);
  }

  chartObject(datos) {
    let barChartData = {
      labels: this.monthColumns.filter(elem => this.monthColumns.indexOf(elem) > 0),
      datasets: this.dataSets(this.dataSource.data)
    };
    let myChart = new Chart(this.productivityChart.nativeElement, {
      type: 'bar',
      data: barChartData,
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            }
          }]
        }
      }
    });
  }

  dataSets(datos) {
    const colors = this.colors;
    const dataSet = []
    datos.forEach((element, index) => {
      const objeto = {
        label: element[0],
        backgroundColor: colors[index],
        borderColor: colors[index],
        borderWith: 1,
        data: element.filter(elem => element.indexOf(elem) > 0)
      };
      dataSet.push(objeto);
    });
    return dataSet;
  }

}

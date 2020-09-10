import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';

import { Title } from '@angular/platform-browser';
import { MatTableDataSource} from '@angular/material';
import {DataService} from '../../services/data.service';
import { ErrorService } from 'src/app/services/error.service';
import { Chart } from 'chart.js';
import { Colors, CONST_STRING, HttpStatus, MODAL_FIND_ORDERS, CONST_NUMBER, HttpServiceTOCall } from 'src/app/constants/const';
import { ProductivityService } from 'src/app/services/productivity.service';
import { ErrorHttpInterface } from 'src/app/model/http/commons';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { ProductivityListMock } from 'src/mocks/productivityMock';

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
  queryString = CONST_STRING.empty;
  today: Date;
  minDate: Date;
  productivityForm: FormGroup;
  fullDate = this.dataService.getDateFormatted(new Date(), new Date(), true, true).split('-');
  @ViewChild('productivityChart', {static: false}) productivityChart: ElementRef;
  constructor(
    private titleService: Title,
    private dataService: DataService,
    private errorService: ErrorService,
    private productivityService: ProductivityService,
    private formBuilder: FormBuilder,
    private cdRef: ChangeDetectorRef,
  ) {
    this.productivityForm = this.formBuilder.group({
      fini: ['', []],
      ffin: ['', []],
    });
    this.dataService.setUrlActive(HttpServiceTOCall.PRODUCTIVITY)
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.today = new Date();
    this.minDate = new Date();
    const initDateTrans = this.fullDate[0].split('/');
    const finishDateTrans = this.fullDate[1].split('/');
    this.productivityForm.get('fini').setValue(new Date(`${initDateTrans[1]}/${initDateTrans[0]}/${initDateTrans[2]}`));
    this.productivityForm.get('ffin').setValue(new Date(`${finishDateTrans[1]}/${finishDateTrans[0]}/${finishDateTrans[2]}`));
    this.minDateIni(this.today, false);
    this.cdRef.detectChanges();
    this.getProductivityData();
  }

  getProductivityData() {
    this.queryString = `?ffin=${this.dataService.getDateFormatted(
      this.productivityForm.get('fini').value,
      this.productivityForm.get('ffin').value,
      false,
      false
    )}`;
    this.productivityService.getProductivity(this.queryString).subscribe(
      productivityRes => {
        this.dataSourceDetails = productivityRes.response.matrix;
        this.dataSource.data = productivityRes.response.matrix.filter(element => productivityRes.response.matrix.indexOf(element) > 0);
        this.monthColumns = this.dataSourceDetails[0];
        this.chartObject(productivityRes.response.matrix);
      }, (error: ErrorHttpInterface) => {
      if (error.status !== HttpStatus.notFound) {
        this.errorService.httpError(error);
      }
      this.dataSource.data = [];
    }
    );
  }

  chartObject(datos) {
    const barChartData = {
      labels: this.monthColumns.filter(elem => this.monthColumns.indexOf(elem) > 0),
      datasets: this.dataSets(this.dataSource.data)
    };
    Chart.defaults.global.defaultFontFamily = 'Quicksand';
    const myChart = new Chart(this.productivityChart.nativeElement, {
      type: 'bar',
      data: barChartData,
      options: {
        scales: {
          yAxes: [{
            ticks: {
              beginAtZero: true
            },
            gridLines: {
              color: 'rgba(0, 0, 0, 0)',
            }
          }],
          xAxes: [{
            gridLines: {
              color: 'rgba(0, 0, 0, 0)',
            },
            barPercentage: 1
          }]
        }
      }
    });
  }

  dataSets(datos) {
    const colors = this.colors;
    const dataSet = [];
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

  minDateIni(fecha: Date, getData = true) {
    this.minDate = new Date(
      fecha.getFullYear(),
      fecha.getMonth() - CONST_NUMBER.six,
      fecha.getDate()
    );
    if (getData){
      this.getProductivityData();
    }
  }
}

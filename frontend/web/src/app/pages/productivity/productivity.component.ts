import { Component, OnInit } from '@angular/core';

import { Title } from '@angular/platform-browser';
import { MatTableDataSource} from '@angular/material';
import {DataService} from '../../services/data.service';
import { ErrorService } from 'src/app/services/error.service';
import { IProductivityReq } from 'src/app/model/http/productivity';
import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';

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
export class ProductivityComponent implements OnInit {
  monthColumns: string[];
  dataSourceDetails: string[][];
  dataSource = new MatTableDataSource<string[]>();
  constructor(
    private titleService: Title,
    private dataService: DataService,
    private errorService: ErrorService
  ) { }

  ngOnInit() {
    this.getProductivityData();
  }

  getProductivityData() {
    this.dataSourceDetails = ELEMENT_DATA;
    this.dataSource.data = ELEMENT_DATA.filter(element => ELEMENT_DATA.indexOf(element) > 0);
    this.monthColumns = this.dataSourceDetails[0];
  }

}

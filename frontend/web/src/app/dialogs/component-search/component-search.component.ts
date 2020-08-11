import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {IFormulaDetalleReq} from '../../model/http/detalleformula';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {CONST_NUMBER} from '../../constants/const';

const ELEMENT_DATA: IFormulaDetalleReq[] = [
  {isChecked: false, productId: "CL   0922", description: "Cloruro de sodio 2%", baseQuantity: 0, requiredQuantity: 0, consumed: 0, available: 10, unit: "PZA", warehouse: "MN", pendingQuantity: 10, stock: 20, warehouseQuantity: 35},
  {isChecked: false, productId: "CL   0921", description: "Cloruro de sodio 1%", baseQuantity: 0, requiredQuantity: 0, consumed: 0, available: 10, unit: "PZA", warehouse: "MN", pendingQuantity: 10, stock: 20, warehouseQuantity: 35},
  {isChecked: false, productId: "CL   0923", description: "Cloruro de sodio 3%", baseQuantity: 0, requiredQuantity: 0, consumed: 0, available: 10, unit: "PZA", warehouse: "MN", pendingQuantity: 10, stock: 20, warehouseQuantity: 35},
  {isChecked: false, productId: "CL   0925", description: "Cloruro de sodio 5%", baseQuantity: 0, requiredQuantity: 0, consumed: 0, available: 10, unit: "PZA", warehouse: "MN", pendingQuantity: 10, stock: 20, warehouseQuantity: 35},
  {isChecked: false, productId: "CL   0929", description: "Cloruro de sodio 9%", baseQuantity: 0, requiredQuantity: 0, consumed: 0, available: 10, unit: "PZA", warehouse: "MN", pendingQuantity: 10, stock: 20, warehouseQuantity: 35}
];

@Component({
  selector: 'app-component-search',
  templateUrl: './component-search.component.html',
  styleUrls: ['./component-search.component.scss']
})
export class ComponentSearchComponent implements OnInit {
  allComplete = false;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  pageSize = CONST_NUMBER.five;
  dataSource = new MatTableDataSource<IFormulaDetalleReq>();
  displayedColumns: string[] = [
    'numero',
    'descripcion'
  ];
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;

  constructor() { }

  ngOnInit() {
    this.getResults();
    this.dataSource.paginator = this.paginator;
  }

  getResults() {
    this.dataSource.data = ELEMENT_DATA;
    this.lengthPaginator = 54;
    //this.dataSource._updateChangeSubscription();
  }

  changeDataEvent(event: PageEvent) {
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getResults();
    return event;
}

}

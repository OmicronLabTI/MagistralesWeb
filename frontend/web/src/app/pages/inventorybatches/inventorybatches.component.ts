import { Component, OnInit } from '@angular/core';

import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { ILotesFormulaReq, ILotesReq, ILotesSelectedReq } from 'src/app/model/http/lotesformula';
import { MatTableDataSource} from '@angular/material';

const ELEMENT_DATA: ILotesFormulaReq[] = [
  {codigoProducto: "MP   001", descripcionProducto: 'Hidrogeno', almacen: 'MG', totalNecesario: 21, totalSeleccionado: 0, lotesSeleccionados:[]},
  {codigoProducto: "MP   321", descripcionProducto: 'Helio', almacen: 'MG', totalNecesario: 22, totalSeleccionado: 0, lotesSeleccionados: []},
  {codigoProducto: "MP   421", descripcionProducto: 'Azucar', almacen: 'MG', totalNecesario: 23, totalSeleccionado: 0, lotesSeleccionados: []},
  {codigoProducto: "MP   999", descripcionProducto: 'Hidrogeno', almacen: 'MG', totalNecesario: 21, totalSeleccionado: 0, lotesSeleccionados:[]},
];

const LOTES_DATA: ILotesReq[] = [
  {numeroLote: 1, cantidadDisponible: 30},
  {numeroLote: 2, cantidadDisponible: 35}
];

const SELECTED_DATA: ILotesSelectedReq[] = [
  {numeroLote: 1, cantidadSeleccionada: 21}
];

@Component({
  selector: 'app-inventorybatches',
  templateUrl: './inventorybatches.component.html',
  styleUrls: ['./inventorybatches.component.scss']
})

export class InventorybatchesComponent implements OnInit {
  ordenFabricacionId: string;
  dataSourceDetails = new MatTableDataSource<ILotesFormulaReq>();
  dataSourceLotes = new MatTableDataSource<ILotesReq>();
  dataSourceLotesSelected = new MatTableDataSource<ILotesSelectedReq>();
  isReadyToSave = false;
  detailsColumns: string[] = [
    'cons',
    'codigoProducto',
    'descripcionProducto',
    'almacen',
    'totalNecesario',
    'totalSeleccionado'
  ];
  lotesColumns: string[] = [
    'cons',
    'disponible',
    'seleccionada',
    'asignada'
  ];
  lotesSelectedColumns: string[] = [
    'lote',
    'seleccionada'
  ]
  constructor(
    private titleService: Title,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.ordenFabricacionId = params.get('ordenid');
      this.titleService.setTitle('OmicronLab - Lotes ' + this.ordenFabricacionId);
    });
    this.dataSourceDetails.data = ELEMENT_DATA;
    this.dataSourceLotes.data = LOTES_DATA;
    this.dataSourceLotesSelected.data = SELECTED_DATA;
  }

  setSelectedTr(event){
    console.log(event);
  }

}

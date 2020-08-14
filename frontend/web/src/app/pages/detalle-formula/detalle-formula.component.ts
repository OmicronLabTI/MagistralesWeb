import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import {IComponentsSaveReq, IFormulaDetalleReq, IFormulaReq} from '../../model/http/detalleformula';
import { ActivatedRoute } from '@angular/router';
import {ErrorService} from '../../services/error.service';
import {MatDialog} from '@angular/material/dialog';
import {ComponentSearchComponent} from '../../dialogs/components-search-dialog/component-search.component';
import {DataService} from '../../services/data.service';

@Component({
  selector: 'app-detalle-formula',
  templateUrl: './detalle-formula.component.html',
  styleUrls: ['./detalle-formula.component.scss']
})

export class DetalleFormulaComponent implements OnInit {
  allComplete = false;
  oldDataFormulaDetail = new IFormulaReq();
  ordenFabricacionId: string;
  displayedColumns: string[] = [
    'seleccion',
    'cons',
    'numero',
    'descripcion',
    'cantbase',
    'cantreq',
    'consumido',
    'disponible',
    'unidad',
    'almacen',
    'cantpend',
    'enstock',
    'cantalmacen'
  ];
  dataSource = new MatTableDataSource<IFormulaDetalleReq>();
  comments = '';
  baseQuantity = 0.0;
  endDateGeneral = new Date();
  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              private errorService: ErrorService, private dialog: MatDialog,
              private dataService: DataService) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.ordenFabricacionId = params.get('ordenid');
    });
    this.getDetalleFormula();
  }

  getDetalleFormula() {
    this.pedidosService.getFormulaDetail(this.ordenFabricacionId).subscribe(
      (formulaRes) => {
        console.log('formula res: ', formulaRes);
        this.oldDataFormulaDetail = formulaRes.response;
        this.comments = this.oldDataFormulaDetail.comments || '';
        const endDate = this.oldDataFormulaDetail.endDate.split('/');
        this.endDateGeneral = new Date(`${endDate[1]}/${endDate[0]}/${endDate[2]}`);
        this.dataSource.data = this.oldDataFormulaDetail.details;
        this.dataSource.data.forEach(detail => {detail.isChecked = false; });
      }, error => this.errorService.httpError(error));
  }

  updateAllComplete() {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked);
  }

  someComplete(): boolean {
    if (this.dataSource.data == null) {
      return false;
    }
    return this.dataSource.data.filter(t => t.isChecked).length > 0 && !this.allComplete;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (this.dataSource.data == null) {
      return;
    }
    this.dataSource.data.forEach(t => t.isChecked = completed);
  }

  openDialog(modalTypeOpen: string) {
    const dialogRef = this.dialog.open(ComponentSearchComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        modalType: modalTypeOpen
      }
    });

    dialogRef.afterClosed().subscribe((resultComponents: IFormulaDetalleReq) => {
      if (resultComponents) {
        this.oldDataFormulaDetail.details.push(resultComponents);
        this.dataSource.data = this.oldDataFormulaDetail.details;
      }
    });
  }


  onBaseQuantityChange(baseQuantity: any, index: number) {
    console.log('on baseQuantity', baseQuantity, 'id: ', index);
    console.log('indexArray: ', this.oldDataFormulaDetail.details[index]);
    this.oldDataFormulaDetail.details[index].requiredQuantity = (this.oldDataFormulaDetail.plannedQuantity * baseQuantity);
    console.log('indexArray 2: ', this.oldDataFormulaDetail.details[index]);
  }

  onRequiredQuantityChange(requiredQuantity: any, index: number) {
    console.log('on requiredQuantity', requiredQuantity, 'id: ', index);
  }
  saveFormulaDetail() {
    const detailComponentsTOSave = new IComponentsSaveReq();
    detailComponentsTOSave.fabOrderId = Number(this.ordenFabricacionId);
    detailComponentsTOSave.plannedQuantity = this.oldDataFormulaDetail.plannedQuantity;

    const endDateToString = this.dataService.transformDate(this.endDateGeneral).split('/');
    detailComponentsTOSave.fechaFin = `${endDateToString[2]}/${endDateToString[1]}/${endDateToString[0]}`;
    detailComponentsTOSave.comments = this.comments;
    console.log('endDateToSave: ', endDateToString, this.oldDataFormulaDetail.endDate)
    console.log('componentstosave: ', detailComponentsTOSave)
  }

}


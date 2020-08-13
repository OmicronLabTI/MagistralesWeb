import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import {IFormulaRes, IFormulaDetalleReq, IFormulaReq} from '../../model/http/detalleformula';
import { ActivatedRoute } from '@angular/router';
import {ErrorService} from '../../services/error.service';
import {MatDialog} from '@angular/material/dialog';
import {ComponentSearchComponent} from '../../dialogs/components-search-dialog/component-search.component';

@Component({
  selector: 'app-detalle-formula',
  templateUrl: './detalle-formula.component.html',
  styleUrls: ['./detalle-formula.component.scss']
})

export class DetalleFormulaComponent implements OnInit {
  allComplete = false;
  dataFormulaDetail = new IFormulaReq();
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

  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              private errorService: ErrorService, private dialog: MatDialog) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.ordenFabricacionId = params.get('ordenid');
    });
    this.getDetalleFormula();
  }

  getDetalleFormula() {
    this.pedidosService.getFormulaDetail(this.ordenFabricacionId).subscribe(
      (formulaRes: IFormulaRes) => {
        this.dataFormulaDetail = formulaRes.response;
        this.dataSource.data = this.dataFormulaDetail.details;
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
        const oldDataSource = this.dataSource.data;
        oldDataSource.push(resultComponents);
        this.dataSource.data = oldDataSource;
      }
    });
  }

}


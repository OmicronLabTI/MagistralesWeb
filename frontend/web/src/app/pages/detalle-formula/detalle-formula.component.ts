import { Component, OnInit } from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import {IComponentsSaveReq, IFormulaDetalleReq, IFormulaReq} from '../../model/http/detalleformula';
import { ActivatedRoute } from '@angular/router';
import {ErrorService} from '../../services/error.service';
import {MatDialog} from '@angular/material/dialog';
import {ComponentSearchComponent} from '../../dialogs/components-search-dialog/component-search.component';
import {DataService} from '../../services/data.service';
import {CONST_DETAIL_FORMULA, CONST_NUMBER} from '../../constants/const';
import {Messages} from '../../constants/messages';
import { Title } from '@angular/platform-browser';

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
  endDateGeneral = new Date();
  isComponentsToDelete = false;
  isReadyToSave = false;
  componentsToDelete: IFormulaDetalleReq [] = [];
  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              private errorService: ErrorService, private dialog: MatDialog,
              private dataService: DataService,
              private titleService: Title) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.ordenFabricacionId = params.get('ordenid');
      this.titleService.setTitle('Orden de fabricación ' + this.ordenFabricacionId);
    });
    this.getDetalleFormula();
  }

  getDetalleFormula() {
    this.pedidosService.getFormulaDetail(this.ordenFabricacionId).subscribe(
      (formulaRes) => {
        console.log('formula res: ', formulaRes);
        this.oldDataFormulaDetail = formulaRes.response;
        this.comments = this.oldDataFormulaDetail.comments || '';
        const endDate = this.oldDataFormulaDetail.dueDate.split('/');
        this.endDateGeneral = new Date(`${endDate[1]}/${endDate[0]}/${endDate[2]}`);
        this.dataSource.data = this.oldDataFormulaDetail.details;
        this.dataSource.data.forEach(detail => {detail.isChecked = false; });
        this.isReadyToSave = false;
        this.componentsToDelete = [];
      }, error => this.errorService.httpError(error));
  }

  updateAllComplete() {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked);
    this.checkISComponentsToDelete();
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
    this.checkISComponentsToDelete();
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
        resultComponents.action = CONST_DETAIL_FORMULA.insert;
        this.oldDataFormulaDetail.details.push(resultComponents);
        this.dataSource.data = this.oldDataFormulaDetail.details;
        this.getIsReadyTOSave();
      }
    });
  }


  onBaseQuantityChange(baseQuantity: any, index: number) {
    this.dataSource.data[index].requiredQuantity =
        Number(( baseQuantity * this.oldDataFormulaDetail.plannedQuantity).toFixed(CONST_NUMBER.ten));
    this.getIsReadyTOSave();
    this.getAction(index);
  }

  onRequiredQuantityChange(requiredQuantity: any, index: number) {
    this.dataSource.data[index].baseQuantity =
        Number(( requiredQuantity / this.oldDataFormulaDetail.plannedQuantity).toFixed(CONST_NUMBER.ten));
    this.getIsReadyTOSave();
    this.getAction(index);

  }
  saveFormulaDetail() {
    this.dataService.presentToastCustom(Messages.saveFormulaDetail, 'question', '', true, true)
        .then( (resultSaveMessage: any) => {
          if (resultSaveMessage.isConfirmed) {
            const detailComponentsTOSave = this.createDeteailTOSave();
            detailComponentsTOSave.comments = this.comments;
            const componentsToDeleteFull = this.dataSource.data
                .filter(component => component.action === CONST_DETAIL_FORMULA.update || component.action === CONST_DETAIL_FORMULA.insert);
            componentsToDeleteFull.push(...this.componentsToDelete);
            detailComponentsTOSave.components =  componentsToDeleteFull;
            this.pedidosService.updateFormula(detailComponentsTOSave).subscribe( () => {
              this.getDetalleFormula();
              this.createMessageOkHttp();
            }, error => console.log('errorFormula: ', error ));
          }
        });


  }
  checkISComponentsToDelete() {
    this. isComponentsToDelete = this.dataSource.data.filter(t => t.isChecked).length > 0;
  }
  deleteComponents() {
    this.dataService.presentToastCustom(Messages.deleteComponents, 'warning', '', true, true)
        .then( (resultDeleteMessage: any) => {
          if (resultDeleteMessage.isConfirmed) {
            this.componentsToDelete.push(...this.dataSource.data.filter( component => component.isChecked &&
                (component.action === CONST_DETAIL_FORMULA.update || !component.action)));
            this.dataSource.data = this.dataSource.data.filter(component => !component.isChecked);
            this.oldDataFormulaDetail.details = this.dataSource.data;
            this.componentsToDelete.forEach( component => component.action = CONST_DETAIL_FORMULA.delete);
            this.getIsReadyTOSave();
            this.createMessageOkHttp();
          }
        });

  }
  createDeteailTOSave() {
    const detailComponentsTOSave = new IComponentsSaveReq();
    detailComponentsTOSave.fabOrderId = Number(this.ordenFabricacionId);
    detailComponentsTOSave.plannedQuantity = this.oldDataFormulaDetail.plannedQuantity;

    const endDateToString = this.dataService.transformDate(this.endDateGeneral).split('/');
    detailComponentsTOSave.fechaFin = `${endDateToString[2]}-${endDateToString[1]}-${endDateToString[0]}`;
    return detailComponentsTOSave;
  }
  getAction(index: number) {
    this.dataSource.data[index].action =
        !this.dataSource.data[index].action ||
        (this.dataSource.data[index].action && this.dataSource.data[index].action  !== CONST_DETAIL_FORMULA.insert) ?
        CONST_DETAIL_FORMULA.update : this.dataSource.data[index].action;
  }


  changeData() {
    this.getIsReadyTOSave();
  }
  getIsReadyTOSave() {
    this.isReadyToSave = true;
  }
  createMessageOkHttp() {
    this.dataService.setMessageGeneralCallHttp({title: Messages.success, icon: 'success', isButtonAccept: false});
  }
}


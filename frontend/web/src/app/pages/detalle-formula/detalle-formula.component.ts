import {Component, OnDestroy, OnInit} from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import {IComponentsSaveReq, IFormulaDetalleReq, IFormulaReq} from '../../model/http/detalleformula';
import { ActivatedRoute } from '@angular/router';
import {ErrorService} from '../../services/error.service';
import {MatDialog} from '@angular/material/dialog';
import {DataService} from '../../services/data.service';
import {ComponentSearch, CONST_DETAIL_FORMULA, CONST_NUMBER, HttpServiceTOCall} from '../../constants/const';
import {Messages} from '../../constants/messages';
import { Title } from '@angular/platform-browser';
import {Subscription} from 'rxjs';
import { MiListaComponent } from 'src/app/dialogs/mi-lista/mi-lista.component';
import { ComponentslistComponent } from 'src/app/dialogs/componentslist/componentslist.component';
import { Components } from 'src/app/model/http/listacomponentes';

@Component({
  selector: 'app-detalle-formula',
  templateUrl: './detalle-formula.component.html',
  styleUrls: ['./detalle-formula.component.scss']
})

export class DetalleFormulaComponent implements OnInit, OnDestroy {
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
  minDate = new Date();
  subscription = new Subscription();
  isSaveToMyList = false;
  isPlannedQuantityError = false;
  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              private errorService: ErrorService, private dialog: MatDialog,
              private dataService: DataService,
              private titleService: Title) {
    this.dataService.setUrlActive(HttpServiceTOCall.DETAIL_FORMULA);
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.ordenFabricacionId = params.get('ordenid');
      this.titleService.setTitle('Orden de fabricación ' + this.ordenFabricacionId);
    });
    this.getDetalleFormula();
    this.subscription.add(this.dataService.getNewFormulaComponent().subscribe( resultNewFormulaComponent => {
      resultNewFormulaComponent.action = CONST_DETAIL_FORMULA.insert;
      this.oldDataFormulaDetail.details.push(resultNewFormulaComponent);
      this.dataSource.data = this.oldDataFormulaDetail.details;
      this.getIsReadyTOSave();
      this.elementsToSave();
    }));
  }

  getDetalleFormula() {
    this.pedidosService.getFormulaDetail(this.ordenFabricacionId).subscribe(
      (formulaRes) => {
        this.oldDataFormulaDetail = formulaRes.response;
        this.comments = this.oldDataFormulaDetail.comments || '';
        const endDate = this.oldDataFormulaDetail.dueDate.split('/');
        this.endDateGeneral = new Date(`${endDate[1]}/${endDate[0]}/${endDate[2]}`);
        this.dataSource.data = this.oldDataFormulaDetail.details;
        this.dataSource.data.forEach(detail => {
          detail.description = detail.description.toUpperCase();
          detail.isChecked = false;
          const warehouseSplit = detail.warehouseQuantity.toString().split('.');
          const stockSplit = detail.stock.toString().split('.');
          detail.warehouseQuantity = warehouseSplit.length === 1 ? warehouseSplit[0] :
              `${new Intl.NumberFormat().format(Number(warehouseSplit[0]))}.${warehouseSplit[1]}`;
          detail.stock = stockSplit.length === 1 ? stockSplit[0] :
              `${new Intl.NumberFormat().format(Number(stockSplit[0]))}.${stockSplit[1]}`;
        });
        this.isReadyToSave = false;
        this.componentsToDelete = [];
        this.dataService.setIsToSaveAnything(false);
        if (this.oldDataFormulaDetail.baseDocument === 0) {
          this.dataService.setOrderIsolated(this.ordenFabricacionId);
        }
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

  openDialog() {
    this.dataService.setSearchComponentModal({ modalType: ComponentSearch.searchComponent, data: this.dataSource.data});
  }


  onBaseQuantityChange(baseQuantity: any, index: number) {
    if (baseQuantity !== null && baseQuantity > 0) {
      this.dataSource.data[index].requiredQuantity =
          Number((baseQuantity * this.oldDataFormulaDetail.plannedQuantity).toFixed(CONST_NUMBER.ten));
      this.getIsReadyTOSave();
      this.getAction(index);
    }
  }

  onRequiredQuantityChange(requiredQuantity: any, index: number) {
    if (requiredQuantity !== null && requiredQuantity > 0) {
      this.dataSource.data[index].baseQuantity =
          Number((requiredQuantity / this.oldDataFormulaDetail.plannedQuantity).toFixed(CONST_NUMBER.ten));
      this.getIsReadyTOSave();
      this.getAction(index);
    }

  }
  saveFormulaDetail() {
    if (this.isPlannedQuantityError) {
      this.dataService.setMessageGeneralCallHttp({title: Messages.onlyIntegerNumbers, icon: 'info', isButtonAccept: true});
      return;
    }
    if (this.getIsThereNull()) {
      this.dataService.presentToastCustom(Messages.saveFormulaDetail, 'question', '', true, true)
          .then( (resultSaveMessage: any) => {
            if (resultSaveMessage.isConfirmed) {
              const detailComponentsTOSave = this.createDeteailTOSave();
              detailComponentsTOSave.comments = this.comments;
              const componentsToDeleteFull = this.dataSource.data
                  .filter(component => component.action === CONST_DETAIL_FORMULA.update ||
                      component.action === CONST_DETAIL_FORMULA.insert);
              componentsToDeleteFull.push(...this.componentsToDelete);
              componentsToDeleteFull.forEach( component => {
                component.stock = Number(component.stock.toString().replace(',', ''));
                component.warehouseQuantity = Number(component.warehouseQuantity.toString().replace(',', ''));
              });
              detailComponentsTOSave.components =  componentsToDeleteFull;
              this.pedidosService.updateFormula(detailComponentsTOSave).subscribe( (res) => {
                this.getDetalleFormula();
                this.createMessageOkHttp();
              }, error => {
                this.getDetalleFormula();
                this.errorService.httpError(error);
                this.componentsToDelete = [];
              });
            }
          });
    } else {
      this.createMessageOnlyNumber();
    }

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
              this.checkISComponentsToDelete();
              this.elementsToSave();
            }
          });
  }
  createDeteailTOSave() {
    const detailComponentsTOSave = new IComponentsSaveReq();
    detailComponentsTOSave.fabOrderId = Number(this.ordenFabricacionId);
    detailComponentsTOSave.plannedQuantity = Number(this.oldDataFormulaDetail.plannedQuantity);
    detailComponentsTOSave.warehouse = this.oldDataFormulaDetail.warehouse;

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
    this.isPlannedQuantityError = this.oldDataFormulaDetail.plannedQuantity === null || this.oldDataFormulaDetail.plannedQuantity % 1 !== 0
        || this.oldDataFormulaDetail.plannedQuantity === 0;
    this.dataSource.data.forEach(component => {
      component.requiredQuantity = component.baseQuantity * this.oldDataFormulaDetail.plannedQuantity;
    });
    this.getIsReadyTOSave();
  }
  getIsReadyTOSave() {
    this.isReadyToSave = true;
    this.dataService.setIsToSaveAnything(true);
  }
  createMessageOkHttp() {
    this.dataService.setMessageGeneralCallHttp({title: Messages.success, icon: 'success', isButtonAccept: false});
  }
  createMessageOnlyNumber() {
    this.dataService.setMessageGeneralCallHttp({title: Messages.onlyPositiveNumber, icon: 'info', isButtonAccept: true});
  }
  getIsThereNull(isFromDelete: boolean = false) {
    if (!isFromDelete) {
      return this.dataSource.data.filter(component => ((component.baseQuantity === null || component.baseQuantity <= 0)
          && (component.requiredQuantity === null || component.baseQuantity <= 0))).length === 0;
    } else {
      return this.dataSource.data.filter(component => component.isChecked &&
          ((component.baseQuantity === null || component.baseQuantity <= 0)
          && (component.requiredQuantity === null || component.baseQuantity <= 0))).length === 0;
    }
  }
  onSelectWareHouseChange(value: string, index: number) {
    this.dataSource.data[index].warehouse = value;
    this.getAction(index);
    this.getIsReadyTOSave();
  }

  ngOnDestroy() {
    this.dataService.setIsToSaveAnything(false);
  }

  goToOrders(urlPath: string[]) {
    this.setPathUrlService(urlPath);
  }

  goToDetailOrder(urlPath: (string | number)[]) {
    this.setPathUrlService(urlPath);
  }
  setPathUrlService(urlPath: any[]) {
    this.dataService.setPathUrl(urlPath);
  }

  openMiListaDialog() {
    const dialogRef = this.dialog.open(MiListaComponent, {
      panelClass: 'custom-dialog-container',
      data: {
          data: this.dataSource.data,
          code: this.oldDataFormulaDetail.code,
          description: this.oldDataFormulaDetail.productDescription
      }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.isSaveToMyList = false;
      } else {
        this.isSaveToMyList = true;
      }
    });
  }

  elementsToSave() {
    if (this.dataSource.data.filter(element => element.action === CONST_DETAIL_FORMULA.insert).length > 0 ) {
      this.isSaveToMyList = true;
    } else {
      this.isSaveToMyList = false;
    }
  }

  openCustomList() {
    const dialogRef = this.dialog.open(ComponentslistComponent, {
      panelClass: 'custom-dialog-container',
      data: {
          code: this.oldDataFormulaDetail.code,
          description: this.oldDataFormulaDetail.productDescription
      }
    }).afterClosed().subscribe((result) => {
      this.replaceComponentsWithCustomList(result.componentes);
    });
  }

  replaceComponentsWithCustomList(components: Components[]) {
    this.componentsToDelete.push(...this.dataSource.data.filter(
      component =>
        (component.isInDb === undefined)
    ));
    const newData: IFormulaDetalleReq[] = [];
    // tslint:disable-next-line: radix
    const orderFabricacionId = parseInt(this.ordenFabricacionId);
    components.forEach(element => {
      newData.push({
        isChecked: false,
        orderFabId: orderFabricacionId,
        productId: element.productId,
        description: element.description.toUpperCase(),
        baseQuantity: element.baseQuantity,
        requiredQuantity: parseFloat((element.baseQuantity * this.oldDataFormulaDetail.plannedQuantity).toFixed(CONST_NUMBER.ten)),
        consumed: 0,
        available: 0,
        unit: 'GR',
        warehouse: 'MG',
        pendingQuantity: 0,
        stock: 0,
        warehouseQuantity: 10,
        action: CONST_DETAIL_FORMULA.insert,
        isInDb: false
      });
    });
    this.dataSource.data = newData;
    this.oldDataFormulaDetail.details = this.dataSource.data;
    this.componentsToDelete.forEach( component => component.action = CONST_DETAIL_FORMULA.delete);
    this.getIsReadyTOSave();
    this.checkISComponentsToDelete();
    this.elementsToSave();
  }
}


import {Component, OnDestroy, OnInit} from '@angular/core';
import {MatTableDataSource} from '@angular/material';
import {PedidosService} from '../../services/pedidos.service';
import {IComponentsSaveReq, IFormulaDetalleReq, IFormulaReq} from '../../model/http/detalleformula';
import {ActivatedRoute} from '@angular/router';
import {ErrorService} from '../../services/error.service';
import {MatDialog} from '@angular/material/dialog';
import {DataService} from '../../services/data.service';
import {
  CarouselOption, CarouselOptionString,
  ComponentSearch,
  CONST_DETAIL_FORMULA,
  CONST_NUMBER,
  CONST_STRING,
  HttpServiceTOCall
} from '../../constants/const';
import {Messages} from '../../constants/messages';
import {Title} from '@angular/platform-browser';
import {Subscription} from 'rxjs';
import {MiListaComponent} from 'src/app/dialogs/mi-lista/mi-lista.component';
import {ComponentslistComponent} from 'src/app/dialogs/componentslist/componentslist.component';
import {Components} from 'src/app/model/http/listacomponentes';

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
  comments = CONST_STRING.empty;
  plannedQuantity = CONST_NUMBER.zero;
  warehouse = CONST_STRING.empty;
  endDateGeneral = new Date();
  isComponentsToDelete = false;
  isReadyToSave = false;
  componentsToDelete: IFormulaDetalleReq [] = [];
  minDate = new Date();
  subscription = new Subscription();
  isSaveToMyList = false;
  isPlannedQuantityError = false;
  detailOrders: string[] = [];
  isFromDetail = false;
  queryString = CONST_STRING.empty;
  currentOrdenFabricacionId = CONST_STRING.empty;
  constructor(private pedidosService: PedidosService, private route: ActivatedRoute,
              private errorService: ErrorService, private dialog: MatDialog,
              private dataService: DataService,
              private titleService: Title) {
    this.dataService.setUrlActive(HttpServiceTOCall.DETAIL_FORMULA);
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.ordenFabricacionId = params.get('ordenid');
      this.isFromDetail = Number(params.get('isFromDetail')) === CONST_NUMBER.one;
      this.detailOrders = params.get('detailsOrders').split(',');
      this.queryString = params.get('filters');
      this.titleService.setTitle('Orden de fabricación ' + this.ordenFabricacionId);
    });
    this.getDetalleFormula();
    this.subscription.add(this.dataService.getNewFormulaComponent().subscribe( resultNewFormulaComponent => {
      resultNewFormulaComponent.action = CONST_DETAIL_FORMULA.insert;
      this.oldDataFormulaDetail.details.push(resultNewFormulaComponent);
      this.dataSource.data = this.oldDataFormulaDetail.details;
      this.getIsReadyTOSave();
      this.getIsElementsToSave();
    }));
  }

  getDetalleFormula() {
    this.pedidosService.getFormulaDetail(this.ordenFabricacionId).subscribe(
      ({response}) => {
        this.onSuccessDetailFormula(response);
      }, error => this.errorService.httpError(error));
  }
  onSuccessDetailFormula(response: IFormulaReq) {
    this.currentOrdenFabricacionId = response.productionOrderId;
    this.oldDataFormulaDetail = response;
    this.plannedQuantity = response.plannedQuantity;
    this.warehouse = response.warehouse;
    this.comments = response.comments || '';
    const endDate = this.oldDataFormulaDetail.dueDate.split('/');
    this.endDateGeneral = new Date(`${endDate[1]}/${endDate[0]}/${endDate[2]}`);
    this.dataSource.data = this.oldDataFormulaDetail.details;
    this.dataSource.data.forEach(detail => {
      detail.description = detail.description.toUpperCase();
      detail.isChecked = false;
      const warehouseSplit = detail.warehouseQuantity.toString().split('.');
      detail.stock = detail.stock || CONST_NUMBER.zero;
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
          Number((baseQuantity * this.plannedQuantity).toFixed(CONST_NUMBER.ten));
      this.getIsReadyTOSave();
      this.getAction(index);
    }
  }

  onRequiredQuantityChange(requiredQuantity: any, index: number) {
    if (requiredQuantity !== null && requiredQuantity > 0) {
      this.dataSource.data[index].baseQuantity =
          Number((requiredQuantity / this.plannedQuantity).toFixed(CONST_NUMBER.ten));
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
              const detailComponentsTOSave = this.createDetailTOSave(true);
              const componentsToDeleteFull = this.dataSource.data
                  .filter(component => component.action === CONST_DETAIL_FORMULA.update ||
                      component.action === CONST_DETAIL_FORMULA.insert);
              componentsToDeleteFull.push(...this.componentsToDelete);
              componentsToDeleteFull.forEach( component => {
                component.stock = Number(component.stock.toString().replace(',', ''));
                component.warehouseQuantity = Number(component.warehouseQuantity.toString().replace(',', ''));
              });
              detailComponentsTOSave.components =  componentsToDeleteFull;

              this.pedidosService.updateFormula(detailComponentsTOSave).subscribe( () => {
                this.getDetalleFormula();
                this.createMessageOkHttp();
                this.componentsToDelete = [];
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
              const detailComponentsToDelete = this.createDetailTOSave();
              const componentsToDeleteOnSave = [...this.dataSource.data.filter(component => component.isChecked &&
                  (component.action === CONST_DETAIL_FORMULA.update || !component.action))];
              componentsToDeleteOnSave.forEach( component => {
                component.action = CONST_DETAIL_FORMULA.delete;
                component.stock = Number(component.stock.toString().replace(',', ''));
                component.warehouseQuantity = Number(component.warehouseQuantity.toString().replace(',', ''));
              });
              detailComponentsToDelete.components = componentsToDeleteOnSave;

              this.pedidosService.updateFormula(detailComponentsToDelete).subscribe( () => {
                this.dataSource.data = this.dataSource.data.filter(component => !component.isChecked);
                this.oldDataFormulaDetail.details = this.dataSource.data;
                this.createMessageOkHttp();
                this.checkISComponentsToDelete();
                this.getIsElementsToSave();
                this.allComplete = this.dataSource.data.filter(t => t.isChecked).length > 0 && !this.allComplete;
              }, error => {
                this.getDetalleFormula();
                this.errorService.httpError(error);
              });
            }
          });
  }
  createDetailTOSave(isFromSave: boolean = false) {
    const detailComponentsTOSave = new IComponentsSaveReq();
    detailComponentsTOSave.fabOrderId = Number(this.ordenFabricacionId);
    detailComponentsTOSave.plannedQuantity = isFromSave ?  Number(this.plannedQuantity) : this.oldDataFormulaDetail.plannedQuantity;
    detailComponentsTOSave.warehouse = isFromSave ? this.warehouse : this.oldDataFormulaDetail.warehouse;
    detailComponentsTOSave.comments = isFromSave ? this.comments : this.oldDataFormulaDetail.comments;

    const endDateToString = isFromSave ? this.dataService.transformDate(this.endDateGeneral).split('/') :
                                         this.oldDataFormulaDetail.dueDate.split('/');
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
    this.isPlannedQuantityError = this.plannedQuantity === null || this.plannedQuantity <= 0
        || this.plannedQuantity === 0;
    this.dataSource.data.forEach(component => {
      component.requiredQuantity = component.baseQuantity * this.plannedQuantity;
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
  getIsThereNull() {
      return this.dataSource.data.filter(component => ((component.baseQuantity === null || component.baseQuantity <= 0)
          && (component.requiredQuantity === null || component.baseQuantity <= 0))).length === 0;
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
    if (this.dataSource.data.length > CONST_NUMBER.zero) {
      this.dialog.open(MiListaComponent, {
        panelClass: 'custom-dialog-container',
        data: {
          data: this.dataSource.data,
          code: this.oldDataFormulaDetail.code,
          description: this.oldDataFormulaDetail.productDescription
        }
      }).afterClosed().subscribe((result) => {
        this.isSaveToMyList = !result;
      });
    } else {
      this.dataService.presentToastCustom(Messages.noComponentsToCreateList, 'info', CONST_STRING.empty, true, false );
    }
  }

  getIsElementsToSave() {
    this.isSaveToMyList = this.dataSource.data.filter(element => element.action === CONST_DETAIL_FORMULA.insert).length > 0;
  }

  openCustomList() {
    this.dialog.open(ComponentslistComponent, {
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
    const newDataToUpdate: IFormulaDetalleReq [] = [];
    components.forEach(component => {
      if (this.dataSource.data.filter( element => element.productId === component.productId).length > CONST_NUMBER.zero) {
         const elementValue = this.dataSource.data.filter( element => element.productId === component.productId)[0];
         if (component.baseQuantity !== elementValue.baseQuantity || component.description !== elementValue.description) {
           newDataToUpdate.push({...elementValue, action: CONST_DETAIL_FORMULA.update});
         } else {
           newDataToUpdate.push(elementValue);
         }
      }
    });

    newDataToUpdate.forEach( component => {
      components.splice(components.findIndex( componentI => componentI.productId === component.productId)
          , CONST_NUMBER.one);
      this.dataSource.data.splice(this.dataSource.data.findIndex( element => element.productId === component.productId)
          , CONST_NUMBER.one );
    });

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
        requiredQuantity: parseFloat((element.baseQuantity * this.plannedQuantity).toFixed(CONST_NUMBER.ten)),
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
    this.dataSource.data = [].concat(newData, newDataToUpdate);
    this.oldDataFormulaDetail.details = this.dataSource.data;
    this.componentsToDelete.forEach( component => component.action = CONST_DETAIL_FORMULA.delete);
    this.getIsReadyTOSave();
    this.checkISComponentsToDelete();
    this.getIsElementsToSave();
  }

  changeDetailFormula(optionChangeDetail: number) {
    switch (optionChangeDetail) {
      case CarouselOption.backDetail:
        if (this.isFromDetail) {
          this.changeFormulaByIndex(CarouselOption.backDetail);
        } else {
          this.changeFormulaByFIltersService(this.dataService.getFullStringForCarousel(
              this.queryString, this.currentOrdenFabricacionId, CarouselOptionString.backDetail
          ));
        }
        break;
      case CarouselOption.nextDetail:
        if (this.isFromDetail) {
          this.changeFormulaByIndex(CarouselOption.nextDetail);
        } else {
          this.changeFormulaByFIltersService(this.dataService.getFullStringForCarousel(
              this.queryString, this.currentOrdenFabricacionId, CarouselOptionString.nextDetail
          ));
        }
        break;
    }


  }

  changeFormulaByIndex(backDetail: CarouselOption) {
      let currentIndex = this.detailOrders.findIndex( order => order === this.ordenFabricacionId);
      if (backDetail === CarouselOption.backDetail) {
          if (currentIndex === CONST_NUMBER.zero) {
            currentIndex = this.detailOrders.length - CONST_NUMBER.one;
          } else {
            currentIndex = currentIndex - CONST_NUMBER.one;
          }
      } else {
        if (currentIndex  === this.detailOrders.length - CONST_NUMBER.one) {
          currentIndex = CONST_NUMBER.zero;
        } else {
          currentIndex = currentIndex + CONST_NUMBER.one;
        }
      }
      this.ordenFabricacionId = this.detailOrders[currentIndex];
      this.getDetalleFormula();
  }

  changeFormulaByFIltersService(fullStringForCarousel: string) {
    this.pedidosService.getFormulaCarousel(fullStringForCarousel).subscribe(
        ({response}) => this.onSuccessDetailFormula(response)
        , error => this.errorService.httpError(error)
    ) ;
  }
}


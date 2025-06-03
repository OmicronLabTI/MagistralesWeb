import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatTableDataSource } from '@angular/material';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { BOOLEANS, ComponentSearch, CONST_CONTAINER, CONST_DETAIL_FORMULA, CONST_NUMBER, CONST_STRING } from 'src/app/constants/const';
import { Messages } from 'src/app/constants/messages';
import { IAddComponentsAndLotesTable, IComponentLotes } from 'src/app/model/http/addComponent';
import { IComponentsSaveReq, IFormulaDetalleReq, IFormulaReq, ISaveAssignedBatches } from 'src/app/model/http/detalleformula';
import { ILotesAsignadosReq, ILotesFormulaReq, ILotesReq, ILotesSelectedReq } from 'src/app/model/http/lotesformula';
import { BatchesService } from 'src/app/services/batches.service';
import { DataService } from 'src/app/services/data.service';
import { DateService } from 'src/app/services/date.service';
import { ErrorService } from 'src/app/services/error.service';
import { ObservableService } from 'src/app/services/observable.service';
import { PedidosService } from 'src/app/services/pedidos.service';

@Component({
  selector: 'app-add-component',
  templateUrl: './add-component.component.html',
  styleUrls: ['./add-component.component.scss']
})
export class AddComponentComponent implements OnInit {
  // MARK: ROUTES PARAMS
  document: number;
  ordenFabricacionId: string;
  hasMissingStock = false;
  description = CONST_STRING.empty;
  productId = CONST_STRING.empty;
  isFromDetail = false;
  detailOrders = CONST_STRING.empty;

  subscription = new Subscription();
  isReadyToSave = false;
  today = new Date();

  // MARK: VARIABLES DETALLE FORMULA
  warehouse = CONST_STRING.empty;
  comments = CONST_STRING.empty;
  endDateGeneral = new Date();
  catalogGroupName = CONST_STRING.empty;
  allComplete = false;
  isComponentsToDelete = false;

  // MARK: VARIABLES ADD-COMPONENT
  objectDataToSave: IComponentsSaveReq;
  componentsData: IFormulaDetalleReq[] = []; // Solo almacena la información de componentes del producto
  lotesData: ILotesFormulaReq[] = []; // Solo almacena la información de lotes del componente

  indexSelected = CONST_NUMBER.zero;
  plannedQuantityControl = CONST_NUMBER.zero;

  // MARK: ESTRUCTURAS DE TABLAS DE LOTES
  dataSourceComponents = new MatTableDataSource<IAddComponentsAndLotesTable>();
  dataSourceLotes = new MatTableDataSource<ILotesReq>();
  dataSourceLotesAsignados = new MatTableDataSource<ILotesAsignadosReq>();

  // MARK: COLUMNAS MAT-TABLE
  detailsColumns: string[] = [
    'cons',
    'codigoProducto',
    'descripcionProducto',
    'cantidadBase',
    'cantidadRequerida',
    'consumido',
    'disponible',
    'unidad',
    'almacen',
    'cantidadPendiente',
    'enStock',
    'cantidadAlmacen',
    'totalNecesario',
    'totalSeleccionado',
  ];
  lotesColumns: string[] = [
    'cons',
    'disponible',
    'seleccionada',
    'asignada',
    'opciones'
  ];
  lotesSelectedColumns: string[] = [
    'lote',
    'seleccionada',
    'opciones'
  ];

  constructor(
    private titleService: Title,
    private route: ActivatedRoute,
    private pedidosService: PedidosService,
    private errorService: ErrorService,
    private observableService: ObservableService,
    public dataService: DataService,
    private batchesService: BatchesService,
    private dateService: DateService,
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.document = Number(params.get('document'));
      this.ordenFabricacionId = params.get('ordenid');
      this.hasMissingStock = Number(params.get('hasMissingStock')) === CONST_NUMBER.one;
      this.description = params.get('description');
      this.productId = params.get('code');
      this.isFromDetail = Number(params.get('isFromDetail')) === CONST_NUMBER.one;
      this.detailOrders = params.get('detailOrders');
      this.titleService.setTitle('OmicronLab - Componentes/Lotes ' + this.ordenFabricacionId);
    });
    this.subscription.add(this.observableService.getNewComponentLotes().subscribe(resultNewFormulaComponent => {
      if (resultNewFormulaComponent) {
        this.getNewComponent(resultNewFormulaComponent);
      }
    }));
    this.reloadData();
  }

  async reloadData() {
    await Promise.all([
      this.getDetalleFormula(),
      this.getInventoryBatches()
    ]);
    this.generateObjects();
  }

  getDetalleFormula(): Promise<void> {
    return this.pedidosService.getFormulaDetail(this.ordenFabricacionId).toPromise().then(({ response }) => {
      this.onSuccessDetailFormula(response);
    }).catch(error => {
      this.errorService.httpError(error);
    });
  }

  onSuccessDetailFormula(response: IFormulaReq) {
    this.componentsData = response.details;
    const endDate = response.dueDate.split('/');
    this.endDateGeneral = new Date(`${endDate[1]}/${endDate[0]}/${endDate[2]}`);
    this.plannedQuantityControl = response.plannedQuantity;
    this.warehouse = response.warehouse;
    this.comments = response.comments || '';
    this.isReadyToSave = false;
    this.dataService.setIsToSaveAnything(false);
    this.catalogGroupName = response.catalogGroupName || '';
  }

  getInventoryBatches() {
    return this.batchesService.getInventoryBatches(this.ordenFabricacionId).toPromise().then(batchesRes => {
      batchesRes.response.forEach(batches => batches.descripcionProducto = batches.descripcionProducto.toUpperCase());
      this.lotesData = batchesRes.response;
    }).catch(error => {
      this.errorService.httpError(error);
    });
  }

  getNewComponent(componentLotes: IComponentLotes) {
    const queryParams = `?offset=0&limit=10&chips=${componentLotes.codigoProducto}&catalogGroup=${componentLotes.almacen}`;
    this.pedidosService.getComponents(queryParams, true).subscribe(resComponets => {
      if (resComponets) {
        this.generateNewComponentRow(resComponets.response[0], componentLotes);
      }
    });
  }

  generateObjects(): void {
    this.generateObjectToSave();
    this.generateTablesObjects();
  }

  generateObjectToSave(): void {
    const endDateToString = this.dateService.transformDate(this.endDateGeneral).split('/');
    this.objectDataToSave = {
      comments: '',
      fabOrderId: Number(this.ordenFabricacionId),
      fechaFin: `${endDateToString[2]}-${endDateToString[1]}-${endDateToString[0]}`,
      plannedQuantity: this.plannedQuantityControl,
      warehouse: this.warehouse,
      components: [],
    };
  }

  generateTablesObjects(): void {
    const dataTable: IAddComponentsAndLotesTable[] = [];

    this.componentsData.forEach((d, index) => {
      const dataRow: IAddComponentsAndLotesTable = {
        codigoProducto: d.productId,
        description: d.description,
        baseQuantity: d.baseQuantity,
        requiredQuantity: d.requiredQuantity,
        consumed: d.consumed,
        available: d.available,
        unit: d.unit,
        warehouse: d.warehouse,
        pendingQuantity: d.pendingQuantity,
        stock: d.stock,
        warehouseQuantity: d.warehouseQuantity,
        totalNecesario: this.lotesData[index].totalNecesario,
        totalSeleccionado: this.lotesData[index].totalSeleccionado,

        lotes: this.lotesData[index].lotes,
        lotesAsignados: this.lotesData[index].lotesAsignados,
        lotesSeleccionados: this.lotesData[index].lotesAsignados,
      };
      dataTable.push(dataRow);
    });

    dataTable[CONST_NUMBER.zero].selected = true;

    this.dataSourceComponents.data = [...dataTable];
    this.dataSourceLotes.data = this.dataSourceComponents.data[0].lotes;
    this.dataSourceLotesAsignados.data = this.dataSourceComponents.data[0].lotesAsignados;

    this.setSelectedQuantity(this.dataSourceComponents.data[0].totalNecesario);
    if (this.dataSourceLotes.data.length === CONST_NUMBER.one && this.dataSourceLotesAsignados.data.length === CONST_NUMBER.zero) {
      this.addLotes(this.dataSourceLotes.data[CONST_NUMBER.zero]);
    }
  }

  generateNewComponentRow(component: IFormulaDetalleReq, lotes: IComponentLotes) {
    const lotesToSave: ILotesReq[] = [];
    lotes.lotes.forEach(elements => {
      const lote: ILotesReq = {
        numeroLote: elements.numeroLote,
        cantidadDisponible: elements.cantidadDisponible,
        cantidadAsignada: elements.cantidadAsignada,
        sysNumber: elements.sysNumber,
        fechaExp: new Date()
      };
      lotesToSave.push(lote);
    });
    const dataRow: IAddComponentsAndLotesTable = {
      codigoProducto: component.productId,
      description: component.description,
      baseQuantity: component.baseQuantity,
      requiredQuantity: component.requiredQuantity,
      consumed: component.consumed,
      available: component.available,
      unit: component.unit,
      warehouse: component.warehouse,
      pendingQuantity: component.pendingQuantity,
      stock: component.stock,
      warehouseQuantity: component.warehouseQuantity,
      totalNecesario: component.requiredQuantity,
      totalSeleccionado: 0,
      lotes: lotesToSave,
      lotesAsignados: [],
      lotesSeleccionados: [],
      action: CONST_DETAIL_FORMULA.insert
    };
    this.dataSourceComponents.data.push(dataRow);
    this.dataSourceComponents._updateChangeSubscription();
  }

  setSelectedTr(elements?: IAddComponentsAndLotesTable): void {
    if (elements !== undefined) {
      this.indexSelected = this.dataSourceComponents.data.indexOf(elements);
      this.dataSourceComponents.data.forEach(item => {
        if (item.selected) {
          item.selected = BOOLEANS.falso;
        }
        elements.selected = !elements.selected;
      });
      this.getBatchesFromSelected(elements.codigoProducto);
    }
  }

  getBatchesFromSelected(codigoProducto?: string) {
    if (codigoProducto !== undefined) {
      const resultData = this.dataSourceComponents.data.filter(elements => (elements.codigoProducto === codigoProducto));
      this.dataSourceLotes.data = resultData[CONST_NUMBER.zero].lotes;
      this.dataSourceLotesAsignados.data = resultData[CONST_NUMBER.zero].lotesAsignados;
      if (this.dataSourceComponents.data[this.indexSelected].lotesSeleccionados == null) {
        this.dataSourceComponents.data[this.indexSelected].lotesSeleccionados = resultData[CONST_NUMBER.zero].lotesAsignados;
      }
      this.setSelectedQuantity(resultData[CONST_NUMBER.zero].totalNecesario);
    }
  }

  setSelectedQuantity(cantidadNecesaria?: number) {
    if (cantidadNecesaria !== undefined) {
      this.dataSourceLotes.data.forEach(element => {
        if (cantidadNecesaria <= element.cantidadDisponible) {
          element.cantidadSeleccionada = cantidadNecesaria;
        } else {
          element.cantidadSeleccionada = element.cantidadDisponible;
        }
      });
    }
  }

  addLotes(element: ILotesReq): void {
    if ((this.dataSourceComponents.data[this.indexSelected].totalNecesario - element.cantidadSeleccionada) >= CONST_NUMBER.zero) {
      if (element.cantidadSeleccionada === CONST_NUMBER.nulo || element.cantidadSeleccionada <= CONST_NUMBER.zero) {
        this.observableService.setGeneralNotificationMessage(Messages.batchesCantidadSeleccionadaZero);
        return;
      }
      if (element.cantidadDisponible - element.cantidadSeleccionada < CONST_NUMBER.zero) {
        this.observableService.setGeneralNotificationMessage(Messages.batchesNotAvailableQty);
        return;
      }
      const objetoNuevo: ILotesAsignadosReq = {
        numeroLote: element.numeroLote,
        cantidadSeleccionada: element.cantidadSeleccionada,
        sysNumber: element.sysNumber,
        action: CONST_DETAIL_FORMULA.insert,
        noidb: BOOLEANS.verdadero,
        isValid: element.isValid
      };
      this.dataSourceComponents.data[this.indexSelected].lotesSeleccionados = this.dataService.calculateTernary(
        this.dataSourceComponents.data[this.indexSelected].lotesSeleccionados == null,
        [],
        this.dataSourceComponents.data[this.indexSelected].lotesSeleccionados
      );
      this.dataSourceComponents.data[this.indexSelected].lotesSeleccionados.push(objetoNuevo);
      this.tableLotesView();
      element.cantidadDisponible = parseFloat((element
        .cantidadDisponible - element.cantidadSeleccionada).toFixed(6));
      this.setTotales(element.cantidadSeleccionada);
      this.isReadyToSave = true;

    }
  }

  deleteLotes(element?: ILotesAsignadosReq) {
    if (element !== undefined) {
      const indiceBorrar = this.dataSourceComponents.data[this.indexSelected].lotesAsignados.indexOf(element);
      if (indiceBorrar !== -1) {
        this.deleteDetails(element);
        this.dataSourceComponents.data[this.indexSelected].lotesAsignados.splice(indiceBorrar, CONST_NUMBER.one);
      }
      this.dataSourceLotesAsignados._updateChangeSubscription();
      this.dataSourceComponents.data[this.indexSelected].lotes.forEach(item => {
        if (item.numeroLote === element.numeroLote) {
          item.cantidadDisponible = parseFloat((item.cantidadDisponible + element.cantidadSeleccionada).toFixed(6));
        }
      });
      this.setTotales(-element.cantidadSeleccionada);
      this.isReadyToSave = true;
    }
    return false;
  }

  deleteDetails(element?: ILotesAsignadosReq) {
    if (element !== undefined) {
      const tomarEnCuenta = this.updateBatches(element);
      if (tomarEnCuenta) {
        this.dataSourceComponents.data[this.indexSelected].lotesSeleccionados.push({
          numeroLote: element.numeroLote,
          noidb: BOOLEANS.falso,
          sysNumber: element.sysNumber,
          cantidadSeleccionada: element.cantidadSeleccionada,
          action: CONST_DETAIL_FORMULA.delete
        });
      }
    }
    return false;
  }


  tableLotesView() {
    const dataSourceDetails = this.dataSourceComponents;
    const dataSourceLotesAsignados = this.dataSourceLotesAsignados;
    const indexSelected = this.indexSelected;
    const arrayObjetos: ILotesAsignadosReq[] = [];
    const arrayNoRepetir: string[] = [];

    this.dataSourceComponents.data[this.indexSelected].lotesSeleccionados.forEach(element => {
      if (element.action !== CONST_DETAIL_FORMULA.delete) {
        arrayNoRepetir.push(element.numeroLote);
        arrayObjetos.push(this.getAssignedLot(dataSourceDetails, element, indexSelected));
        if (dataSourceDetails.data[indexSelected].lotesAsignados == null) {
          dataSourceDetails.data[indexSelected].lotesAsignados = [];
        }
        if (dataSourceDetails.data[indexSelected].lotesAsignados == null) {
          dataSourceDetails.data[indexSelected].lotesAsignados = [];
        }
        dataSourceDetails.data[indexSelected].lotesAsignados = arrayObjetos;
        dataSourceLotesAsignados.data = dataSourceDetails.data[indexSelected].lotesAsignados;
        dataSourceLotesAsignados._updateChangeSubscription();
      }
    });
  }

  getAssignedLot(
    dataSourceDetails: MatTableDataSource<IAddComponentsAndLotesTable>,
    elementA: ILotesSelectedReq,
    indexSelected: number): ILotesAsignadosReq {
    const arraySum: ILotesSelectedReq[] = dataSourceDetails.data[indexSelected].lotesSeleccionados.
      filter(element => (element.numeroLote === elementA.numeroLote));

    const lastTotal = arraySum.reduce((total, actualValue) => this.dataService.calculateTernary(
      actualValue.action !== CONST_DETAIL_FORMULA.delete,
      total + actualValue.cantidadSeleccionada,
      total), 0);

    return {
      numeroLote: elementA.numeroLote,
      sysNumber: elementA.sysNumber,
      cantidadSeleccionada: this.dataService.calculateTernary(arraySum.length > CONST_NUMBER.one,
        parseFloat(lastTotal.toFixed(6)),
        elementA.cantidadSeleccionada
      ),
      isValid: elementA.isValid
    };
  }

  onBaseQuantityChange(baseQuantity: any, index: number) {
    if (baseQuantity !== null && baseQuantity > 0) {
      this.dataSourceComponents.data[index].requiredQuantity =
        Number((baseQuantity * this.plannedQuantityControl).toFixed(CONST_NUMBER.ten));
      this.dataSourceComponents.data[index].totalNecesario =
        Number((baseQuantity * this.plannedQuantityControl).toFixed(CONST_NUMBER.ten)) -
        this.dataSourceComponents.data[index].totalSeleccionado;
      this.getIsReadyTOSave();
      this.getAction(index);
    }
  }

  onRequiredQuantityChange(requiredQuantity: any, index: number) {
    if (requiredQuantity !== null && requiredQuantity > 0) {
      this.dataSourceComponents.data[index].baseQuantity =
        Number((requiredQuantity / this.plannedQuantityControl).toFixed(CONST_NUMBER.ten));
      this.dataSourceComponents.data[index].totalNecesario =
        Number((requiredQuantity / this.plannedQuantityControl).toFixed(CONST_NUMBER.ten)) -
        this.dataSourceComponents.data[index].totalSeleccionado;
      this.getIsReadyTOSave();
      this.getAction(index);
    }
  }

  setTotales(cantidadSeleccionada?: number) {
    if (cantidadSeleccionada !== undefined) {
      this.dataSourceComponents.data[this.indexSelected].totalSeleccionado = parseFloat(
        (parseFloat(this.dataSourceComponents.data[this.indexSelected].totalSeleccionado.toFixed(6)) + cantidadSeleccionada).toFixed(6)
      );
      this.dataSourceComponents.data[this.indexSelected].totalNecesario = parseFloat(
        (this.dataSourceComponents.data[this.indexSelected].totalNecesario - cantidadSeleccionada).toFixed(6)
      );
      this.setInputNecesaryQty();
    }
  }

  getAction(index: number) {
    this.dataSourceComponents.data[index].action =
      !this.dataSourceComponents.data[index].action ||
        (this.dataSourceComponents.data[index].action && this.dataSourceComponents.data[index].action !== CONST_DETAIL_FORMULA.insert) ?
        CONST_DETAIL_FORMULA.update : this.dataSourceComponents.data[index].action;
  }

  onSelectWareHouseChange(value: string, index: number) {
    this.dataSourceComponents.data[index].warehouse = value;
    this.dataSourceLotesAsignados.data.forEach(element => this.deleteLotes(element));
    this.getAction(index);
    this.getIsReadyTOSave();
  }

  getInsertElementsToSave() {
    const elementsToInsert = this.dataSourceComponents.data.filter(element => element.action === CONST_DETAIL_FORMULA.insert);
    elementsToInsert.forEach(element => {
      const lote = this.getBatches(element.lotesSeleccionados);
      const insertData: IFormulaDetalleReq = {
        action: element.action,
        available: element.available,
        baseQuantity: element.baseQuantity,
        consumed: element.consumed,
        description: element.description,
        isContainer: this.validateIsContainer(element.codigoProducto),
        isItemSelected: element.selected,
        orderFabId: 0,
        pendingQuantity: element.pendingQuantity,
        productId: element.codigoProducto,
        requiredQuantity: element.requiredQuantity,
        stock: element.stock,
        unit: element.unit,
        warehouse: element.warehouse,
        warehouseQuantity: element.warehouseQuantity,
        isChecked: false,
        assignedBatches: lote
      };
      this.objectDataToSave.components.push(insertData);
    });
  }

  getUpdateElementsToSave() {
    const elementsToInsert = this.dataSourceComponents.data.filter(element => element.action === CONST_DETAIL_FORMULA.update);
    elementsToInsert.forEach(element => {
      const lote = this.getBatches(element.lotesSeleccionados);
      const insertData: IFormulaDetalleReq = {
        action: element.action,
        available: element.available,
        baseQuantity: element.baseQuantity,
        consumed: element.consumed,
        description: element.description,
        isContainer: this.validateIsContainer(element.codigoProducto),
        isItemSelected: element.selected,
        orderFabId: 0,
        pendingQuantity: element.pendingQuantity,
        productId: element.codigoProducto,
        requiredQuantity: element.requiredQuantity,
        stock: element.stock,
        unit: element.unit,
        warehouse: element.warehouse,
        warehouseQuantity: element.warehouseQuantity,
        isChecked: false,
        assignedBatches: lote
      };
      this.objectDataToSave.components.push(insertData);
    });
  }

  setInputNecesaryQty() {
    const dataSourceDetails = this.dataSourceComponents;
    const indexSelected = this.indexSelected;
    // tslint:disable-next-line: no-shadowed-variable
    this.dataSourceComponents.data[this.indexSelected].lotes.forEach(element => {
      if (dataSourceDetails.data[indexSelected].totalNecesario <= element.cantidadDisponible) {
        element.cantidadSeleccionada = dataSourceDetails.data[indexSelected].totalNecesario;
      } else {
        element.cantidadSeleccionada = element.cantidadDisponible;
      }
    });
    return true;
  }

  getBatches(lotes: ILotesSelectedReq[]): ISaveAssignedBatches[] {
    const lotesToSave: ISaveAssignedBatches[] = [];
    lotes.forEach(element => {
      const loteToSave: ISaveAssignedBatches = {
        assignedQty: element.cantidadSeleccionada,
        batchNumber: element.numeroLote,
        areBatchesComplete: 0,
        sysNumber: element.sysNumber
      };
      lotesToSave.push(loteToSave);
    });
    return lotesToSave;
  }

  validateIsContainer(productId: string) {
    const productIdType = productId.split('-')[0];
    return productIdType === CONST_CONTAINER.en || productIdType === CONST_CONTAINER.em;
  }

  getIsReadyTOSave() {
    this.isReadyToSave = true;
    this.dataService.setIsToSaveAnything(true);
  }

  updateBatches(element?: ILotesAsignadosReq): boolean {
    let tomarEnCuenta = false;
    this.dataSourceComponents.data[this.indexSelected].lotesSeleccionados.forEach(ele => {
      if (ele.numeroLote === element.numeroLote) {
        ele.action = CONST_DETAIL_FORMULA.delete;
        if (this.dataService.calculateOrValueList([ele.noidb === undefined, ele.noidb === false])) {
          tomarEnCuenta = true;
        } else {
          tomarEnCuenta = false;
          return;
        }
      }
    });
    return tomarEnCuenta;
  }

  isDue(element: ILotesReq) {
    if (element.fechaExp !== null && element.fechaExp !== undefined) {
      const strFechaExp = String(element.fechaExp).split('/');
      const dtFechaExp = new Date(parseInt(strFechaExp[2], 10), parseInt(strFechaExp[1], 10) - 1, parseInt(strFechaExp[0], 10));
      element.isValid = (dtFechaExp >= this.today);
      return dtFechaExp < this.today;
    }
    element.isValid = true;
    return false;
  }

  getIsValid(element: ILotesReq) {
    this.dataSourceLotes.data.forEach(ele => {
      if (ele.numeroLote === element.numeroLote) {
        element.isValid = ele.isValid;
      }
    });
    return element.isValid;
  }

  buildObjectToSap(): void {
    this.objectDataToSave.components = [];
    this.getInsertElementsToSave();
    this.getUpdateElementsToSave();

    this.saveChanges();
  }

  saveChanges(): void {
    this.pedidosService.updateFormula(this.objectDataToSave).subscribe(() => {
      this.createMessageOkHttp();
      this.reloadData();
    }, error => {
      this.errorService.httpError(error);
      this.reloadData();
    });
  }

  createMessageOkHttp() {
    this.observableService.setMessageGeneralCallHttp({ title: Messages.success, icon: 'success', isButtonAccept: false });
  }

  openDialog() {
    this.observableService.setSearchComponentLoteModal({
      modalType: ComponentSearch.searchComponent,
      data: this.dataSourceComponents.data,
      catalogGroupName: this.catalogGroupName
    });
  }

  // MARK: ENRUTAMIENTO
  goToOrders(urlPath: string[]) {
    this.observableService.setPathUrl(urlPath);
  }

  goToDetailOrder(urlPath: (string | number)[]) {
    this.setPathUrlService(urlPath);
  }

  goToOrdenFab(urlPath: string[]) {
    this.observableService.setPathUrl(urlPath);
  }

  setPathUrlService(urlPath: any[]) {
    this.observableService.setPathUrl(urlPath);
  }

}

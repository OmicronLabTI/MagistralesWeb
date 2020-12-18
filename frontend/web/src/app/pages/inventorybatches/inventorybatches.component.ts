import { Component, OnInit } from '@angular/core';

import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { ILotesFormulaReq, ILotesReq, ILotesSelectedReq, ILotesAsignadosReq, ILotesToSaveReq} from 'src/app/model/http/lotesformula';
import { MatTableDataSource} from '@angular/material';
import { BatchesService } from 'src/app/services/batches.service';
import {
  CONST_NUMBER,
  BOOLEANS,
  CONST_DETAIL_FORMULA,
  MessageType,
  ClassNames,
  CONST_STRING
} from '../../constants/const';
import { Messages } from '../../constants/messages';
import {DataService} from '../../services/data.service';
import { ErrorService } from 'src/app/services/error.service';

@Component({
  selector: 'app-inventorybatches',
  templateUrl: './inventorybatches.component.html',
  styleUrls: ['./inventorybatches.component.scss']
})

export class InventorybatchesComponent implements OnInit {
  cantidadNecesariaInput = 0;
  indexSelected = 0;
  dataSelected: ILotesFormulaReq;
  document: number;
  ordenFabricacionId: string;
  dataSourceDetails = new MatTableDataSource<ILotesFormulaReq>();
  dataSourceLotes = new MatTableDataSource<ILotesReq>();
  dataSourceLotesAsignados = new MatTableDataSource<ILotesAsignadosReq>();
  isReadyToSave = false;
  objectToSave: ILotesToSaveReq[] = [];
  lotesSeleccionados: ILotesSelectedReq[];
  hasMissingStock = false;
  description = CONST_STRING.empty;
  productId = CONST_STRING.empty;
  element: any;
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
    'asignada',
    'opciones'
  ];
  lotesSelectedColumns: string[] = [
    'lote',
    'seleccionada',
    'opciones'
  ];
  today = new Date();
  constructor(
    private titleService: Title,
    private route: ActivatedRoute,
    private batchesService: BatchesService,
    private dataService: DataService,
    private errorService: ErrorService
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.document = Number(params.get('document'));
      this.ordenFabricacionId = params.get('ordenid');
      this.hasMissingStock = Number(params.get('hasMissingStock'))  === CONST_NUMBER.one;
      this.description = params.get('description');
      this.productId = params.get('code');
      this.titleService.setTitle('OmicronLab - Lotes ' + this.ordenFabricacionId);
      this.getInventoryBatches();
    });

  }

  // tslint:disable-next-line: no-shadowed-variable
  setSelectedTr(element?: ILotesFormulaReq){
    if (element !== undefined) {
      this.dataSelected = element;
      this.indexSelected = this.dataSourceDetails.data.indexOf(element);
      this.dataSourceDetails.data.forEach(item => {
        if (item.selected) {
          item.selected = BOOLEANS.falso;
        }
      });
      element.selected = !element.selected;
      this.getBatchesFromSelected(element.codigoProducto);
    }
    return true;
  }

  getBatchesFromSelected(codigoProducto?){
    if (codigoProducto !== undefined){
      // tslint:disable-next-line: no-shadowed-variable
      const resultData = this.dataSourceDetails.data.filter(element => (element.codigoProducto === codigoProducto));
      this.dataSourceLotes.data = resultData[CONST_NUMBER.zero].lotes;
      this.dataSourceLotesAsignados.data = resultData[CONST_NUMBER.zero].lotesAsignados;
      if (this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados == null) {
        this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados = resultData[CONST_NUMBER.zero].lotesAsignados;
      }
      this.setSelectedQuantity(resultData[CONST_NUMBER.zero].totalNecesario);
      if (this.dataSourceLotes.data.length === CONST_NUMBER.one && this.dataSourceLotesAsignados.data.length === CONST_NUMBER.zero) {
        this.addLotes(this.dataSourceLotes.data[CONST_NUMBER.zero]);
      }
      return true;
    }
    return false;
  }

  getInventoryBatches(){
    let resultData: ILotesFormulaReq[];
    this.batchesService.getInventoryBatches(this.ordenFabricacionId).subscribe(
      (batchesRes) => {
        batchesRes.response.forEach( batches => batches.descripcionProducto = batches.descripcionProducto.toUpperCase());
        this.dataSourceDetails.data = batchesRes.response;
        // tslint:disable-next-line: no-shadowed-variable
        resultData = this.dataSourceDetails.data.filter(element => (
          element.codigoProducto === this.dataSourceDetails.data[CONST_NUMBER.zero].codigoProducto)
        );
        resultData[CONST_NUMBER.zero].selected = BOOLEANS.verdadero;
        this.dataSourceLotes.data = resultData[CONST_NUMBER.zero].lotes;
        this.dataSourceLotesAsignados.data = resultData[CONST_NUMBER.zero].lotesAsignados;
        this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados = resultData[CONST_NUMBER.zero].lotesAsignados;
        this.setSelectedQuantity(resultData[CONST_NUMBER.zero].totalNecesario);
        if (this.dataSourceLotes.data.length === CONST_NUMBER.one && this.dataSourceLotesAsignados.data.length === CONST_NUMBER.zero) {
          this.addLotes(this.dataSourceLotes.data[CONST_NUMBER.zero]);
        }
      }
    );
    return true;
  }

  // tslint:disable-next-line: no-shadowed-variable
  addLotes(element: ILotesReq){
    if ((this.dataSourceDetails.data[this.indexSelected].totalNecesario - element.cantidadSeleccionada) >= CONST_NUMBER.zero){
      if (element.cantidadSeleccionada === CONST_NUMBER.nulo || element.cantidadSeleccionada <= CONST_NUMBER.zero) {
        this.dataService.setGeneralNotificationMessage(Messages.batchesCantidadSeleccionadaZero);
      } else {
        if (element.cantidadDisponible - element.cantidadSeleccionada < CONST_NUMBER.zero) {
          this.dataService.setGeneralNotificationMessage(Messages.batchesNotAvailableQty);
          return false;
        }
        const objetoNuevo: ILotesAsignadosReq = {
          numeroLote: element.numeroLote,
          cantidadSeleccionada: element.cantidadSeleccionada,
          sysNumber: element.sysNumber,
          action: CONST_DETAIL_FORMULA.insert,
          noidb: BOOLEANS.verdadero,
          isValid: element.isValid
        };
        if (this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados == null) {
          this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados = [];
        }
        this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados.push(objetoNuevo);
        this.tableLotesView();
        element.cantidadDisponible = parseFloat((element
          .cantidadDisponible - element.cantidadSeleccionada).toFixed(6));
        this.setTotales(element.cantidadSeleccionada);
        this.isReadyToSave = true;
      }
    }
  }

  tableLotesView(){
    const dataSourceDetails = this.dataSourceDetails;
    const dataSourceLotesAsignados = this.dataSourceLotesAsignados;
    const indexSelected = this.indexSelected;
    const arrayObjetos: ILotesAsignadosReq[] = [];
    let objetoLoteAsignado: ILotesAsignadosReq;
    const arrayNoRepetir: string[] = [];
    this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados.forEach(elementA => {
      if (elementA.action !== CONST_DETAIL_FORMULA.delete){
        if (!arrayNoRepetir.includes(elementA.numeroLote)){
          arrayNoRepetir.push(elementA.numeroLote);
          // tslint:disable-next-line: no-shadowed-variable
          const arraySum: ILotesSelectedReq[] = dataSourceDetails.data[indexSelected].lotesSeleccionados.filter(element => (
            element.numeroLote === elementA.numeroLote)
          );
          let suma = CONST_NUMBER.zero;
          if (arraySum.length > CONST_NUMBER.one) {
            arraySum.forEach(ele => {
              if (ele.action !== CONST_DETAIL_FORMULA.delete) {
                suma = suma + ele.cantidadSeleccionada;
              }
            });
            objetoLoteAsignado = {
              numeroLote: elementA.numeroLote,
              sysNumber: elementA.sysNumber,
              cantidadSeleccionada: parseFloat(suma.toFixed(6)),
              isValid: elementA.isValid
            }
          } else {
            objetoLoteAsignado = {
              numeroLote: elementA.numeroLote,
              sysNumber: elementA.sysNumber,
              cantidadSeleccionada: elementA.cantidadSeleccionada,
              isValid: elementA.isValid
            };
          }
          arrayObjetos.push(objetoLoteAsignado);
          if (dataSourceDetails.data[indexSelected].lotesAsignados == null){
            dataSourceDetails.data[indexSelected].lotesAsignados = []
          }
          dataSourceDetails.data[indexSelected].lotesAsignados = arrayObjetos;
          dataSourceLotesAsignados.data = dataSourceDetails.data[indexSelected].lotesAsignados;
          dataSourceLotesAsignados._updateChangeSubscription();
        }
      }
    });
  }

  // tslint:disable-next-line: no-shadowed-variable
  deleteLotes(element?: ILotesAsignadosReq){
    if (element !== undefined){
      const indiceBorrar = this.dataSourceDetails.data[this.indexSelected].lotesAsignados.indexOf(element);
      if ( indiceBorrar !== -1 ) {
        this.deleteDetails(element);
        this.dataSourceDetails.data[this.indexSelected].lotesAsignados.splice( indiceBorrar, CONST_NUMBER.one );
      }
      this.dataSourceLotesAsignados._updateChangeSubscription();
      this.dataSourceDetails.data[this.indexSelected].lotes.forEach(item => {
        if (item.numeroLote === element.numeroLote) {
          item.cantidadDisponible = parseFloat((item.cantidadDisponible + element.cantidadSeleccionada).toFixed(6));
        }
      });
      this.setTotales(-element.cantidadSeleccionada)
      this.isReadyToSave = true;
    }
    return false;
  }

  // tslint:disable-next-line: no-shadowed-variable
  deleteDetails(element?: ILotesAsignadosReq){
    if (element !== undefined) {
      let tomarEnCuenta = false;
      this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados.forEach(ele => {
        if (ele.numeroLote === element.numeroLote) {
          ele.action = CONST_DETAIL_FORMULA.delete;
          if (ele.noidb === undefined || ele.noidb === false) {
            tomarEnCuenta = true;
          } else {
            tomarEnCuenta = false;
            return;
          }
        }
      });
      if (tomarEnCuenta) {
        this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados.push({
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

  setSelectedQuantity(cantidadNecesaria?) {
    if (cantidadNecesaria !== undefined) {
      this.dataSourceDetails.data[this.indexSelected].lotes.forEach(ele => {
        if (cantidadNecesaria <= ele.cantidadDisponible) {
          ele.cantidadSeleccionada = cantidadNecesaria;
        } else {
          ele.cantidadSeleccionada = ele.cantidadDisponible;
        }
      });
    }
  }

  setTotales(cantidadSeleccionada?: number) {
    if (cantidadSeleccionada !== undefined) {
      this.dataSourceDetails.data[this.indexSelected].totalSeleccionado = parseFloat(
        (parseFloat(this.dataSourceDetails.data[this.indexSelected].totalSeleccionado.toFixed(6)) + cantidadSeleccionada).toFixed(6)
      );
      this.dataSourceDetails.data[this.indexSelected].totalNecesario = parseFloat(
        (this.dataSourceDetails.data[this.indexSelected].totalNecesario - cantidadSeleccionada).toFixed(6)
      );
      this.setInputNecesaryQty();
    }
  }

  setInputNecesaryQty() {
    const dataSourceDetails = this.dataSourceDetails;
    const indexSelected = this.indexSelected;
    // tslint:disable-next-line: no-shadowed-variable
    this.dataSourceDetails.data[this.indexSelected].lotes.forEach(element => {
      if (dataSourceDetails.data[indexSelected].totalNecesario <= element.cantidadDisponible) {
        element.cantidadSeleccionada = dataSourceDetails.data[indexSelected].totalNecesario;
      } else {
        element.cantidadSeleccionada = element.cantidadDisponible;
      }
    });
    return true;
  }

  buildObjectToSap(){
    let objectToSave = this.objectToSave;
    objectToSave = [];
    const ordenFabricacionId = this.ordenFabricacionId;
    // tslint:disable-next-line: no-shadowed-variable
    this.dataSourceDetails.data.forEach(element => {
      if (element.lotesSeleccionados != null) {
        element.lotesSeleccionados.forEach((lote, index) => {
          if (lote.action !== undefined) {
            if ((lote.noidb === BOOLEANS.falso || lote.noidb === undefined) || (lote.action === CONST_DETAIL_FORMULA.insert)) {
              const objectSAP: ILotesToSaveReq = {
                // tslint:disable-next-line: radix
                orderId: parseInt(ordenFabricacionId),
                itemCode: element.codigoProducto,
                assignedQty: parseFloat(lote.cantidadSeleccionada.toFixed(6)),
                action: lote.action,
                batchNumber: lote.numeroLote
              };
              objectToSave.push(objectSAP);
            }
          }
        });
      }
    });
    this.dataService.presentToastCustom(Messages.saveBatches, 'question', '', true, true).then( (resultSaveMessage: any) => {
      if (resultSaveMessage.isConfirmed) {

        this.batchesService.updateBatches(objectToSave).subscribe( resultSaveBatches => {
          if (resultSaveBatches.success && resultSaveBatches.response.length > 0) {
            const titleFinalizeWithError = this.dataService.getMessageTitle(
              resultSaveBatches.response, MessageType.saveBatches);
            this.dataService.presentToastCustom(titleFinalizeWithError, 'error',
            Messages.errorToAssignOrderAutomaticSubtitle, true, false, ClassNames.popupCustom);
          } else {
            this.dataService.presentToastCustom(Messages.successBatchesSave, 'success', '', true, false).then( (resultBatchSave: any) => {
              if (resultBatchSave.isConfirmed) {
                window.location.reload();
              }
            });
          }
        }, error => this.errorService.httpError(error));
      }
    });
  }

  goToDetailOrder(urlPath: (string | number)[]) {
    this.setPathUrlService(urlPath);
  }
  setPathUrlService(urlPath: any[]) {
    this.dataService.setPathUrl(urlPath);
  }

  goToOrdenFab(urlPath: string[]) {
    this.dataService.setPathUrl(urlPath);
  }

  // tslint:disable-next-line: no-shadowed-variable
  isDue(element: ILotesReq) {
    if (element.fechaExp !== null && element.fechaExp !== undefined) {
      const strFechaExp = String(element.fechaExp).split('/');
      // tslint:disable-next-line: radix
      const dtFechaExp = new Date(parseInt(strFechaExp[2]), parseInt(strFechaExp[1]) - 1, parseInt(strFechaExp[0]));
      element.isValid = !(dtFechaExp < this.today);
      return dtFechaExp < this.today;
    }
    element.isValid = true;
    return false;
  }

  // tslint:disable-next-line: no-shadowed-variable
  getIsValid(element: ILotesReq) {
    this.dataSourceLotes.data.forEach(ele => {
      if (ele.numeroLote === element.numeroLote) {
        element.isValid = ele.isValid;
      }
    });
    return element.isValid;
  }

    goToOrders(urlPath: string[]) {
      this.dataService.setPathUrl(urlPath);
    }
}

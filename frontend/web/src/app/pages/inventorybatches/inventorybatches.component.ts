import { Component, OnInit } from '@angular/core';

import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { ILotesFormulaReq, ILotesReq, ILotesSelectedReq, ILotesAsignadosReq, ILotesToSaveReq} from 'src/app/model/http/lotesformula';
import { MatTableDataSource} from '@angular/material';
import { BatchesService } from 'src/app/services/batches.service';
import { CONST_NUMBER, BOOLEANS, CONST_DETAIL_FORMULA, CONST_STRING } from '../../constants/const'
import { Messages } from '../../constants/messages'
import {DataService} from '../../services/data.service';
import { element } from 'protractor';

@Component({
  selector: 'app-inventorybatches',
  templateUrl: './inventorybatches.component.html',
  styleUrls: ['./inventorybatches.component.scss']
})

export class InventorybatchesComponent implements OnInit {
  cantidadNecesariaInput: number = 0;
  indexSelected: number = 0;
  dataSelected: ILotesFormulaReq;
  ordenFabricacionId: string;
  dataSourceDetails = new MatTableDataSource<ILotesFormulaReq>();
  dataSourceLotes = new MatTableDataSource<ILotesReq>();
  dataSourceLotesAsignados = new MatTableDataSource<ILotesAsignadosReq>();
  isReadyToSave = false;
  objectToSave: ILotesToSaveReq[] = [];
  lotesSeleccionados: ILotesSelectedReq[];
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
  ]
  constructor(
    private titleService: Title,
    private route: ActivatedRoute,
    private batchesService: BatchesService,
    private dataService: DataService
  ) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.ordenFabricacionId = params.get('ordenid');
      this.titleService.setTitle('OmicronLab - Lotes ' + this.ordenFabricacionId);
    });
    this.getInventoryBatches();
  }

  setSelectedTr(element?: ILotesFormulaReq){
    if (element != undefined){
      this.dataSelected = element;
      this.indexSelected = this.dataSourceDetails.data.indexOf(element);
      this.dataSourceDetails.data.filter(function(item){
        if (item.selected){
          item.selected = BOOLEANS.falso;
        }
    });
    element.selected = !element.selected;
    this.getBatchesFromSelected(element.codigoProducto);
    }
    return true;
  }

  getBatchesFromSelected(codigoProducto?){
    if (codigoProducto != undefined){
      let resultData = this.dataSourceDetails.data.filter(element => (element.codigoProducto === codigoProducto));
      this.dataSourceLotes.data = resultData[CONST_NUMBER.zero].lotes;
      this.dataSourceLotesAsignados.data = resultData[CONST_NUMBER.zero].lotesAsignados;
      if (this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados == null)
        this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados = resultData[CONST_NUMBER.zero].lotesAsignados;
      this.setSelectedQuantity(resultData[CONST_NUMBER.zero].totalNecesario);
      return true;
    }
    return false;
  }

  getInventoryBatches(){
    let resultData: ILotesFormulaReq[];
    this.batchesService.getInventoryBatches(this.ordenFabricacionId).subscribe(
      (batchesRes) => {
        this.dataSourceDetails.data = batchesRes.response;
        console.log("respuesta: ", batchesRes.response);
        resultData = this.dataSourceDetails.data.filter(element => (element.codigoProducto === this.dataSourceDetails.data[CONST_NUMBER.zero].codigoProducto))
        resultData[CONST_NUMBER.zero].selected = BOOLEANS.verdadero;
        this.dataSourceLotes.data = resultData[CONST_NUMBER.zero].lotes;
        this.dataSourceLotesAsignados.data = resultData[CONST_NUMBER.zero].lotesAsignados;
        this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados = resultData[CONST_NUMBER.zero].lotesAsignados;
        this.setSelectedQuantity(resultData[CONST_NUMBER.zero].totalNecesario);
      }
    );
    return true;
  }

  addLotes(element: ILotesReq){
    if ((this.dataSourceDetails.data[this.indexSelected].totalNecesario - element.cantidadSeleccionada) >= CONST_NUMBER.zero){
      if (element.cantidadSeleccionada === CONST_NUMBER.nulo || element.cantidadSeleccionada === CONST_NUMBER.zero){
        this.dataService.setGeneralNotificationMessage(Messages.batchesCantidadSeleccionadaZero);
      } else {
        if (element.cantidadDisponible <= CONST_NUMBER.zero) {
          console.log(element.cantidadDisponible);
          this.dataService.setGeneralNotificationMessage(Messages.batchesNotAvailableQty);
          return false;
        }
        const objetoNuevo: ILotesAsignadosReq = {
          numeroLote: element.numeroLote,
          cantidadSeleccionada: element.cantidadSeleccionada,
          sysNumber: element.sysNumber,
          action: CONST_DETAIL_FORMULA.insert,
          noidb: BOOLEANS.verdadero
        }
        if (this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados == null){
          this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados = [];
        }
        this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados.push(objetoNuevo);
        this.tableLotesView();
        this.setTotales(element.cantidadSeleccionada);
      }
      //this.setInputNecesaryQty();
    } else{
      this.dataService.setGeneralNotificationMessage(Messages.batchesSelectedQtyError);
    }
  }

  tableLotesView(){
    let dataSourceDetails = this.dataSourceDetails;
    let dataSourceLotesAsignados = this.dataSourceLotesAsignados;
    let indexSelected = this.indexSelected;
    let arrayObjetos: ILotesAsignadosReq[] = [];
    let objetoLoteAsignado: ILotesAsignadosReq;
    let arrayNoRepetir: string[] = [];
    this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados.forEach(function(elementA){
      if (elementA.action != CONST_DETAIL_FORMULA.delete){
        if (!arrayNoRepetir.includes(elementA.numeroLote)){
          arrayNoRepetir.push(elementA.numeroLote);
          let arraySum: ILotesSelectedReq[] = dataSourceDetails.data[indexSelected].lotesSeleccionados.filter(element => (element.numeroLote == elementA.numeroLote));
          let suma = CONST_NUMBER.zero;          
          if (arraySum.length > CONST_NUMBER.one){
            arraySum.forEach(function(ele){
              if(ele.action != CONST_DETAIL_FORMULA.delete)
                suma = suma + ele.cantidadSeleccionada;
            });
            objetoLoteAsignado = {
              numeroLote: elementA.numeroLote,
              sysNumber: elementA.sysNumber,
              cantidadSeleccionada: parseFloat(suma.toFixed(10))
            }
          }
          else{
            objetoLoteAsignado = {
              numeroLote: elementA.numeroLote,
              sysNumber: elementA.sysNumber,
              cantidadSeleccionada: elementA.cantidadSeleccionada
            }
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

  deleteLotes(element?: ILotesAsignadosReq){
    if(element != undefined){
      let indiceBorrar = this.dataSourceDetails.data[this.indexSelected].lotesAsignados.indexOf(element);
      if ( indiceBorrar !== -1 ) {
        this.deleteDetails(element);
        this.dataSourceDetails.data[this.indexSelected].lotesAsignados.splice( indiceBorrar, CONST_NUMBER.one );
      }
      this.dataSourceLotesAsignados._updateChangeSubscription();
      this.setTotales(-element.cantidadSeleccionada)
    }
    return false;
  }

  deleteDetails(element?:ILotesAsignadosReq){
    if (element != undefined){
      this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados.forEach(ele => {
        if (ele.numeroLote === element.numeroLote){
          ele.action = CONST_DETAIL_FORMULA.delete;
          ele.noidb = BOOLEANS.verdadero;
        }
      });
      this.dataSourceDetails.data[this.indexSelected].lotesSeleccionados.push({
        numeroLote: element.numeroLote,
        noidb: BOOLEANS.verdadero,
        sysNumber: element.sysNumber,
        cantidadSeleccionada: element.cantidadSeleccionada,
        action: CONST_DETAIL_FORMULA.delete
      });
    }
    return false;
  }

  setSelectedQuantity(cantidadNecesaria?){
    if (cantidadNecesaria != undefined){
      this.dataSourceDetails.data[this.indexSelected].lotes.forEach(ele => {
        ele.cantidadSeleccionada = cantidadNecesaria;
      });
    }
  }

  setTotales(cantidadSeleccionada?: number){
    if (cantidadSeleccionada != undefined){
      this.dataSourceDetails.data[this.indexSelected].totalSeleccionado = parseFloat((this.dataSourceDetails.data[this.indexSelected].totalSeleccionado + cantidadSeleccionada).toFixed(10));
      this.dataSourceDetails.data[this.indexSelected].totalNecesario = parseFloat((this.dataSourceDetails.data[this.indexSelected].totalNecesario - cantidadSeleccionada).toFixed(10)); 
      this.setInputNecesaryQty();
    }
  }

  setInputNecesaryQty(){
    let dataSourceDetails = this.dataSourceDetails;
    let indexSelected = this.indexSelected;
    this.dataSourceDetails.data[this.indexSelected].lotes.forEach(function(element){
      element.cantidadSeleccionada = dataSourceDetails.data[indexSelected].totalNecesario;
    });
    return true;
  }

  buildObjectToSap(){
    let objectToSave = this.objectToSave;
    objectToSave = [];
    let ordenFabricacionId = this.ordenFabricacionId;
    this.dataSourceDetails.data.forEach(function(element){
      if (element.lotesSeleccionados != null){
        element.lotesSeleccionados.forEach(function(lote){
          if (lote.noidb)
          {
            let objectSAP: ILotesToSaveReq = {
              orderId: parseInt(ordenFabricacionId),
              itemCode: element.codigoProducto,
              assignedQty: lote.cantidadSeleccionada,
              action: lote.action,
              batchNumber: lote.numeroLote
            }
            objectToSave.push(objectSAP);
          }          
        });
      }
    });
    this.dataService.presentToastCustom(Messages.saveBatches, 'question', '', true, true).then( (resultSaveMessage: any) => {
      if (resultSaveMessage.isConfirmed) {
        this.batchesService.updateBatches(objectToSave).subscribe( () => {
          //window.location.reload();
        }, error => console.log('error: ', error ));
      }
    }); 
  }
}

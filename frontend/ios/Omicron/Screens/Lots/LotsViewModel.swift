//
//  LotsViewModel.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxCocoa
import RxSwift
import Resolver

class LotsViewModel {
    
    // MARK: Variables
    var loading = PublishSubject<Bool>()
    var showMessage = PublishSubject<String>()
    var orderId = -1
    var disposeBag = DisposeBag()
    var dataOfLots = PublishSubject<[LotsAux]>()
    var dataLotsAvailable = PublishSubject<[LotsAvailableAux]>()
    var dataLotsSelected = PublishSubject<[LotsSelectedAux]>()
    var addLotDidTap = PublishSubject<Void>()
    var removeLotDidTap = PublishSubject<Void>()
    var saveLotsDidTap = PublishSubject<Void>()
    var quantitySelectedValue = ""
    var lotsSelectedCopy:[LotsSelectedAux] = []
    var quantitySelectedInput = BehaviorSubject<String>(value: "")
    var rowSelected = PublishSubject<Int>()
    var firstTime = PublishSubject<Void>()
    
    var lineDocumentsDataAux:[LotsAux] = []
    var lotsAvailablesAux:[LotsAvailableAux] = []
    var lotsSelectedAux:[LotsSelectedAux] = []
    
    var itemSelectedLineDocuments: Int = 0
    var itemLotSelected:LotsSelectedAux? = nil
    var prueba: [Any] = []
    init() {

        // Añade lotes de Lotes disponibles a Lotes Seleccionados
        let inputs = Observable.combineLatest(rowSelected, quantitySelectedInput)
        self.addLotDidTap.withLatestFrom(inputs).map({
            LotsAvailableInfo(row: $0, quantitySelected: $1)
        }).subscribe(onNext: { data in
            let lotSelected = LotsSelected(numeroLote: self.lotsAvailablesAux[data.row].numeroLote!, cantidadSeleccionada:  Double(data.quantitySelected) ?? 0.0, sysNumber: self.lotsAvailablesAux[data.row].sysNumber!)
            
            //Modifiacdo
//            if((lotSelected.cantidadSeleccionada! <= self.lotsAvailablesAux[data.row].cantidadDisponible!) && ( lotSelected.cantidadSeleccionada! <= self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalNecesario! )) {
//                let index = self.lotsSelectedAux.firstIndex(where: ({$0.numeroLote == lotSelected.numeroLote}))
//                if((index) != nil) {
//                    //Modifiacdo
//                    //self.lotsSelectedAux[index!].cantidadSeleccionada! += lotSelected.cantidadSeleccionada!
//                } else {
//                    //Modifiacdo
////                    self.lotsSelectedAux.append(lotSelected)
//                }
//
//                self.prueba.append(self.lotsSelectedAux)
//                //Modifiacdo
////                self.lotsAvailablesAux[data.row].cantidadDisponible! -= lotSelected.cantidadSeleccionada!
////                self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalNecesario! -= lotSelected.cantidadSeleccionada!
////                self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalSeleccionado! += lotSelected.cantidadSeleccionada!
//                //self.dataOfLots.onNext(self.lineDocumentsDataAux)
//                self.dataLotsSelected.onNext(self.lotsSelectedAux)
//                self.dataLotsAvailable.onNext(self.lotsAvailablesAux)
//            }
        }).disposed(by: self.disposeBag)
        
        // Remueve un lote de Lotes seleccionados y lo pasa a Lotes Disponibles
        self.removeLotDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { itemSelected in
            let index = self.lotsSelectedAux.firstIndex(where: ({$0.numeroLote == self.itemLotSelected?.numeroLote}))
            if (index != nil) {
                
                if let indexLotAvailable = self.lotsAvailablesAux.firstIndex(where: ({$0.numeroLote == self.itemLotSelected?.numeroLote})) {
//                    self.lotsAvailablesAux[indexLotAvailable].cantidadDisponible! += self.itemLotSelected!.cantidadSeleccionada!
//                    self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalNecesario! += self.itemLotSelected!.cantidadSeleccionada!
//                    self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalSeleccionado! -= self.itemLotSelected!.cantidadSeleccionada!
                    
                    
                    
                    
                    
                    
                    
                    self.lotsSelectedAux.remove(at: index!)
                } else {
                    for item in self.lineDocumentsDataAux {
                        //Modifiacdo
//                        if let indexOfTable = item.lotesDisponibles?.firstIndex(where: ({$0.numeroLote == self.itemLotSelected?.numeroLote})) {
//                            let lotAvailable = LotsAvailable(numeroLote: item.lotesDisponibles![indexOfTable].numeroLote , cantidadDisponible:  item.lotesDisponibles![indexOfTable].cantidadDisponible!, cantidadAsignada:  item.lotesDisponibles![indexOfTable].cantidadAsignada!, cantidadSeleccionada:  item.lotesDisponibles![indexOfTable].cantidadSeleccionada!, sysNumber:  item.lotesDisponibles![indexOfTable].sysNumber!)
//                            self.lotsAvailablesAux.append(lotAvailable)
//                        }
                    }
                }
                
                //Modifiacdo
                //self.dataOfLots.onNext(self.lineDocumentsDataAux)
                self.dataLotsAvailable.onNext(self.lotsAvailablesAux)
                self.dataLotsSelected.onNext(self.lotsSelectedAux)
            }
        }).disposed(by: self.disposeBag)
        
        
        // Guada los lotes selecionados y los manda al servicio
        self.saveLotsDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { _ in
            self.assingLots()
        }).self.disposed(by: self.disposeBag)
        
    }
    
    // MARK: -Functions
    func getLots() -> Void {
        self.loading.onNext(true)
        NetworkManager.shared.getLots(orderId: orderId).observeOn(MainScheduler.instance).subscribe(onNext: { data in
            self.loading.onNext(false)
            if let lotsData = data.response {
                if lotsData.first != nil {
                    
                    // Se asignan los valores a cada lote disponible su cantidad sugerida (total necesario)
                    self.firstTime.onNext(())
                    self.itemSelectedLineDocuments = 0
                    for lotData in lotsData {
                        for lot in lotData.lotesDisponibles!{
                            lot.cantidadSeleccionada = lotData.totalNecesario
                        }
                    }
                    for lot in lotsData[0].lotesDisponibles! {
                        lot.cantidadSeleccionada = lotsData[0].totalNecesario
                    }
                    
                    // Validación por si hay un solo lote se pasa directamemte a lotes seleccionados
                    if( lotsData[0].lotesDisponibles!.count == 1 && lotsData[0].lotesSelecionados!.count == 0 &&  lotsData[0].lotesDisponibles![0].cantidadSeleccionada! <= lotsData[0].totalNecesario!) {
                        let lotSelected = LotsSelected(numeroLote: lotsData[0].lotesDisponibles![0].numeroLote! , cantidadSeleccionada:  lotsData[0].lotesDisponibles![0].cantidadSeleccionada!, sysNumber:  lotsData[0].lotesDisponibles![0].sysNumber!)
                            
                        lotsData[0].totalNecesario = 0
                        lotsData[0].totalSeleccionado = lotsData[0].lotesDisponibles![0].cantidadSeleccionada!
                        //Modifiacdo
//                         self.lotsSelectedAux = [lotSelected]
//                        self.lotsSelectedCopy = [lotSelected]
                        self.lotsAvailablesAux = []
                    } else {
                        //Modifiacdo
//                        self.lotsAvailablesAux = lotsData[0].lotesDisponibles!
//                        self.lotsSelectedAux = lotsData[0].lotesSelecionados!
//                        self.lotsSelectedCopy = lotsData[0].lotesSelecionados!
                    }
                    
                    
                    var lotsAvailablesAux: [LotsAvailableAux] = []
                    var lotsSelectedAux: [LotsSelectedAux] = []
                    var dataLotsAux:[LotsAux] = []
                    for lineDocument in lotsData {
                        
                        for lotAvailables in lineDocument.lotesDisponibles! {
                            let lotAvailableAux =  LotsAvailableAux(numeroLote: lotAvailables.numeroLote , cantidadDisponible: NSDecimalNumber(floatLiteral: lotAvailables.cantidadDisponible!), cantidadAsignada: NSDecimalNumber(floatLiteral: lotAvailables.cantidadAsignada!), cantidadSeleccionada: NSDecimalNumber(floatLiteral: lotAvailables.cantidadSeleccionada!), sysNumber: lotAvailables.sysNumber!)
                            lotsAvailablesAux.append(lotAvailableAux)
                            self.lotsAvailablesAux.append(lotAvailableAux)
                        }
                        
                        
                        for lotSelected in lineDocument.lotesSelecionados! {
                            let lotSelectedAux = LotsSelectedAux(numeroLote: lotSelected.numeroLote!, cantidadSeleccionada: NSDecimalNumber(floatLiteral: lotSelected.cantidadSeleccionada!), sysNumber: lotSelected.sysNumber!)
                            lotsSelectedAux.append(lotSelectedAux)
                            self.lotsSelectedAux.append(lotSelectedAux)
                        }
                        
                        let lineDocumentAux = LotsAux(codigoProducto: lineDocument.codigoProducto!, descripcionProducto: lineDocument.descripcionProducto!
                            , almacen: lineDocument.almacen!, totalNecesario: NSDecimalNumber(floatLiteral: lineDocument.totalNecesario!), totalSeleccionado: NSDecimalNumber(floatLiteral: lineDocument.totalSeleccionado!), lotesSelecionados: lotsSelectedAux, lotesDisponibles: lotsAvailablesAux)
                        
                        dataLotsAux.append(lineDocumentAux)
                    }
                    
                    
                    self.dataLotsAvailable.onNext(self.lotsAvailablesAux)
                    self.dataLotsSelected.onNext(self.lotsSelectedAux)
                    self.dataOfLots.onNext(dataLotsAux)
                    //Modifiacdo
//                    self.lineDocumentsDataAux = lotsData
                }
            }
        }, onError: { error in
            self.loading.onNext(false)
            self.showMessage.onNext("Hubo un error al cargar los lotes, por favor de intentarlo de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func itemSelectedOfLineDocTable(lot: Lots) -> Void {
        
        // Si solo hay un elemento en la tabla de lotes disponibles y que la cantidad disponible es mayoy que la cantidad seleccionada
        if lot.lotesDisponibles?.count == 1 {
            if lot.lotesDisponibles![0].cantidadDisponible! >= lot.lotesDisponibles![0].cantidadSeleccionada! {
                let lotSelected = LotsSelected(numeroLote: lot.lotesDisponibles![0].numeroLote!  , cantidadSeleccionada:   lot.lotesDisponibles![0].cantidadSeleccionada! , sysNumber:  lot.lotesDisponibles![0].sysNumber!)
                //Modifiacdo
//                self.dataLotsSelected.onNext([lotSelected])
                self.dataLotsAvailable.onNext([])
                
                self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalNecesario = 0.0
                //Modifiacdo
//                self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalSeleccionado = lotSelected.cantidadSeleccionada
//                self.dataOfLots.onNext(self.lineDocumentsDataAux)
                
            }
        } else {
            if(lot.lotesDisponibles!.count > 1) {
                //Modifiacdo
//                self.dataLotsAvailable.onNext(lot.lotesDisponibles!)
//                self.lotsAvailablesAux = lot.lotesDisponibles!
            } else {
                self.dataLotsAvailable.onNext([])
                self.lotsAvailablesAux = []
            }
            
            if(lot.lotesSelecionados!.count > 1) {
                //Modifiacdo
//                self.dataLotsSelected.onNext(lot.lotesSelecionados!)
//                self.lotsSelectedAux = lot.lotesSelecionados!
            } else {
                self.dataLotsSelected.onNext([])
                //Modifiacdo
//                self.lotsSelectedAux = lot.lotesSelecionados!
            }
        }
        
    }
    
    func assingLots() -> Void {
        
        // Proceso de eliminación
        // Se busca lote del arreglo original (lotsSelectedCopy) en arreglo actual (lotsSelectedAux)
        // Si existe no se realiza nada
        // No existe, se crea el objeto para la eliminación en servicio
        var lotsRequest:[LotsRequest] = []
        
        for lso in self.lotsSelectedCopy {
            
            if (self.lotsSelectedAux.first(where: ({$0.numeroLote == lso.numeroLote})) == nil) {
                //Modifiacdo
//                let lotRequest = LotsRequest(orderId: self.orderId, assignedQty: lso.cantidadSeleccionada!, batchNumber: lso.numeroLote!, itemCode: self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!, action: "delete")
//                lotsRequest.append(lotRequest)
            }
        }
        
        // Proceso de inserción y actualización
        //Se busca lote del arreglo actual (lotsSelectedAux) en el arreglo original (lotsSelectedCopy)
        // Si existe se crea el objeto de eliminación del arreglo original, se obtiene el valor absoluto de la resta de cantidad seleccionada entre lotsSelectedAux y lotsSelectedCopy, por último se crea el objecto de actualización con el valor de la resta
        //No existe se crea un nuevo objeto de inserción
        
        for lsa in self.lotsSelectedAux {
            var lotRequest:LotsRequest? = nil
            
            if let index = self.lotsSelectedCopy.firstIndex(where: ({ $0.numeroLote == lsa.numeroLote })) {
                
                // Se crea el objeto de eliminación
                //Modifiacdo
//                lotRequest = LotsRequest(orderId: self.orderId, assignedQty: self.lotsSelectedCopy[index].cantidadSeleccionada!, batchNumber: self.lotsSelectedCopy[index].numeroLote!, itemCode: self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!, action: "delete")
//                lotsRequest.append(lotRequest!)
                
                //Se obtiene el valor absoluto de la resta de cantidad seleccionada entre lotsSelectedAux y lotsSelectedCopy, por último se crea el objecto de actualización con el valor de la resta
//                var  subtraction = lsa.cantidadSeleccionada! - self.lotsSelectedCopy[index].cantidadSeleccionada!
//                if(subtraction.isLess(than: 0.0)) {
//                    subtraction = (subtraction * -1)
//                }
                
//                lotRequest = LotsRequest(orderId: self.orderId, assignedQty: subtraction, batchNumber: self.lotsSelectedCopy[index].numeroLote!, itemCode: self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!, action: "update")
//                lotsRequest.append(lotRequest!)
            } else {
                 //No existe se crea un nuevo objeto de inserción
//                lotRequest = LotsRequest(orderId: self.orderId, assignedQty: lsa.cantidadSeleccionada!, batchNumber: lsa.numeroLote!, itemCode: self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!, action: "insert")
//                lotsRequest.append(lotRequest!)
            }
        }
        
    }
    
    
    func sendToServerAssignedLots(lotsToSend: [LotsRequest]) -> Void {
        self.loading.onNext(true)
        NetworkManager.shared.assingLots(lotsRequest: lotsToSend).observeOn(MainScheduler.instance).subscribe(onNext: { _ in
            self.loading.onNext(false)
            self.showMessage.onNext("Proceso realizado correctamente")
        }, onError:  { error in
            self.loading.onNext(false)
            self.showMessage.onNext("Hubo un error al asignar los lotes, por favor intentar de nuevo")
        }).disposed(by: self.disposeBag)
    }
}

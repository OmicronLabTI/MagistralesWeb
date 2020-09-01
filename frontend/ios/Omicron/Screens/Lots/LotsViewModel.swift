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
    var dataOfLots = BehaviorSubject<[Lots]>(value: [])
    var dataLotsAvailable = BehaviorSubject<[LotsAvailable]>(value: [])
    var dataLotsSelected = BehaviorSubject<[LotsSelected]>(value: [])
    var addLotDidTap = PublishSubject<Void>()
    var removeLotDidTap = PublishSubject<Void>()
    var saveLotsDidTap = PublishSubject<Void>()
    var quantitySelectedValue = ""
    var lotsSelectedCopy:[LotsSelected] = []
    var quantitySelectedInput = BehaviorSubject<String>(value: "")
    var rowSelected = PublishSubject<Int>()
    
    var lineDocumentsDataAux:[Lots] = []
    var lotsAvailablesAux:[LotsAvailable] = []
    // var lotsSelectedAux:[LotsSelected] = []
    
    var itemSelectedLineDocuments: Int = 0
    var itemDeselectedLineDocuments: Int = 0
    var itemLotSelected:LotsSelected? = nil
    //var prueba: [Any] = []
    
    var cache:[String: [LotsSelected]]  = [:]

    
    init() {

        // Añade lotes de Lotes disponibles a Lotes Seleccionados
        let inputs = Observable.combineLatest(rowSelected, quantitySelectedInput)
        self.addLotDidTap.withLatestFrom(inputs).map({
            LotsAvailableInfo(row: $0, quantitySelected: $1)
        }).subscribe(onNext: { data in
            
        
            let lotSelected = LotsSelected(numeroLote: self.lotsAvailablesAux[data.row].numeroLote!, cantidadSeleccionada: Decimal(string: data.quantitySelected) ?? 0.0, sysNumber: self.lotsAvailablesAux[data.row].sysNumber!)
            
            // Perdsiste la presición del problema con los decimales
            if((lotSelected.cantidadSeleccionada! <= self.lotsAvailablesAux[data.row].cantidadDisponible!) && ( lotSelected.cantidadSeleccionada! <= self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalNecesario! + 0.00000000000001)) {
                //let index = self.lotsSelectedAux.firstIndex(where: ({$0.numeroLote == lotSelected.numeroLote}))
                let index = self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]!.firstIndex(where: ({$0.numeroLote == lotSelected.numeroLote}))
                if((index) != nil) {
                    // self.lotsSelectedAux[index!].cantidadSeleccionada! += lotSelected.cantidadSeleccionada!
                    self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]![index!].cantidadSeleccionada! += lotSelected.cantidadSeleccionada!
                } else {
                    self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]!.append(lotSelected)
                    //self.lotsSelectedAux.append(lotSelected)
                }
                
                //self.prueba.append(self.lotsSelectedAux)
                
                self.lotsAvailablesAux[data.row].cantidadDisponible! -= lotSelected.cantidadSeleccionada!
                self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalNecesario! -= lotSelected.cantidadSeleccionada!
                self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalSeleccionado! += lotSelected.cantidadSeleccionada!
                self.dataOfLots.onNext(self.lineDocumentsDataAux)
                //self.dataLotsSelected.onNext(self.lotsSelectedAux)
                self.dataLotsSelected.onNext(self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]!)
                self.dataLotsAvailable.onNext(self.lotsAvailablesAux)
                 //self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!] = self.lotsSelectedAux //------------------------
            }
        }).disposed(by: self.disposeBag)
        
        // Remueve un lote de Lotes seleccionados y lo pasa a Lotes Disponibles
        self.removeLotDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { itemSelected in
//            let index = self.lotsSelectedAux.firstIndex(where: ({$0.numeroLote == self.itemLotSelected?.numeroLote}))
             let index = self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]!.firstIndex(where: ({$0.numeroLote == self.itemLotSelected?.numeroLote}))
            if (index != nil) {
                
                if let indexLotAvailable = self.lotsAvailablesAux.firstIndex(where: ({$0.numeroLote == self.itemLotSelected?.numeroLote})) {
                    self.lotsAvailablesAux[indexLotAvailable].cantidadDisponible! += self.itemLotSelected!.cantidadSeleccionada!
                    self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalNecesario! += self.itemLotSelected!.cantidadSeleccionada!
                    self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalSeleccionado! -= self.itemLotSelected!.cantidadSeleccionada!
                    //self.lotsSelectedAux.remove(at: index!)
                    self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]!.remove(at: index!)
                    //self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!] = self.lotsSelectedAux //------------------------
                } else {
                    for item in self.lineDocumentsDataAux {
                        if let indexOfTable = item.lotesDisponibles?.firstIndex(where: ({$0.numeroLote == self.itemLotSelected?.numeroLote})) {
                            let lotAvailable = LotsAvailable(numeroLote: item.lotesDisponibles![indexOfTable].numeroLote , cantidadDisponible:  item.lotesDisponibles![indexOfTable].cantidadDisponible!, cantidadAsignada:  item.lotesDisponibles![indexOfTable].cantidadAsignada!, cantidadSeleccionada:  item.lotesDisponibles![indexOfTable].cantidadSeleccionada!, sysNumber:  item.lotesDisponibles![indexOfTable].sysNumber!)
                            self.lotsAvailablesAux.append(lotAvailable)
                        }
                    }
                }
                
                self.dataOfLots.onNext(self.lineDocumentsDataAux)
                self.dataLotsAvailable.onNext(self.lotsAvailablesAux)
                //self.dataLotsSelected.onNext(self.lotsSelectedAux)
                self.dataLotsSelected.onNext(self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]!)
            }
        }).disposed(by: self.disposeBag)
        
        
        // Guada los lotes seleccionados y los manda al servicio
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
                     self.lineDocumentsDataAux = lotsData
                    // Se inicializa la caché
                    for lot in lotsData {
                        self.cache[lot.codigoProducto!] = []
                    }
                    
                    
                    // Se asignan los valores a cada lote disponible su cantidad sugerida (total necesario)
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
                        self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]! = [lotSelected]
//                         self.lotsSelectedAux = [lotSelected]
//                        self.cache[lotsData[0].codigoProducto!] =  self.lotsSelectedAux
                        self.lotsSelectedCopy = [lotSelected]
                        self.lotsAvailablesAux = []
                    } else {
                        self.lotsAvailablesAux = lotsData[0].lotesDisponibles!
                         self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]! =  lotsData[0].lotesSelecionados!
//                        self.lotsSelectedAux = lotsData[0].lotesSelecionados!
//                        self.cache[lotsData[0].codigoProducto!] =  self.lotsSelectedAux
                        self.lotsSelectedCopy = lotsData[0].lotesSelecionados!
                    }
                    
                    self.dataLotsAvailable.onNext(self.lotsAvailablesAux)
//                    self.dataLotsSelected.onNext(self.lotsSelectedAux)
                    self.dataLotsSelected.onNext(self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]! )
                    self.dataOfLots.onNext(lotsData)
                    //self.lineDocumentsDataAux = lotsData
                }
            }
        }, onError: { error in
            self.loading.onNext(false)
            self.showMessage.onNext("Hubo un error al cargar los lotes, por favor de intentarlo de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func itemSelectedOfLineDocTable(lot: Lots) -> Void {
        if (lot.lotesDisponibles!.count > 0) {
              self.dataLotsAvailable.onNext(lot.lotesDisponibles!)
                self.lotsAvailablesAux = lot.lotesDisponibles!
        } else {
            self.dataLotsAvailable.onNext([])
             self.lotsAvailablesAux = []
        }
        
        if(cache[lot.codigoProducto!]!.count > 0) {
            self.dataLotsSelected.onNext(cache[lot.codigoProducto!]!)
            //self.lotsSelectedAux = lot.lotesSelecionados!
        } else {
             self.dataLotsSelected.onNext([])
//            let lll = cache[lot.codigoProducto!]
//            self.lotsSelectedAux = lll!
        }
    }
        
    func assingLots() -> Void {
        print("caché \(self.cache)")
        // Proceso de eliminación
        // Se busca lote del arreglo original (lotsSelectedCopy) en arreglo actual (lotsSelectedAux)
        // Si existe no se realiza nada
        // No existe, se crea el objeto para la eliminación en servicio
        var lotsRequest:[LotsRequest] = []
        
        for lso in self.lotsSelectedCopy {
//            if (self.lotsSelectedAux.first(where: ({$0.numeroLote == lso.numeroLote})) == nil) {
             if ( self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]!.first(where: ({$0.numeroLote == lso.numeroLote})) == nil) {
                let lotRequest = LotsRequest(orderId: self.orderId, assignedQty: lso.cantidadSeleccionada!, batchNumber: lso.numeroLote!, itemCode: self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!, action: "delete")
                lotsRequest.append(lotRequest)
            }
        }
        
        // Proceso de inserción y actualización
        //Se busca lote del arreglo actual (lotsSelectedAux) en el arreglo original (lotsSelectedCopy)
        // Si existe se crea el objeto de eliminación del arreglo original, se obtiene el valor absoluto de la resta de cantidad seleccionada entre lotsSelectedAux y lotsSelectedCopy, por último se crea el objecto de actualización con el valor de la resta
        //No existe se crea un nuevo objeto de inserción
        for lsa in  self.cache[self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!]! {
            var lotRequest:LotsRequest? = nil
            
            if let index = self.lotsSelectedCopy.firstIndex(where: ({ $0.numeroLote == lsa.numeroLote })) {
                
                // Se crea el objeto de eliminación
                lotRequest = LotsRequest(orderId: self.orderId, assignedQty: self.lotsSelectedCopy[index].cantidadSeleccionada!, batchNumber: self.lotsSelectedCopy[index].numeroLote!, itemCode: self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!, action: "delete")
                lotsRequest.append(lotRequest!)
                
                //Se obtiene el valor absoluto de la resta de cantidad seleccionada entre lotsSelectedAux y lotsSelectedCopy, por último se crea el objecto de actualización con el valor de la resta
                var  subtraction = lsa.cantidadSeleccionada! - self.lotsSelectedCopy[index].cantidadSeleccionada!
                if(subtraction.isLess(than: 0.0)) {
                    subtraction = (subtraction * -1)
                }
                
                lotRequest = LotsRequest(orderId: self.orderId, assignedQty: subtraction, batchNumber: self.lotsSelectedCopy[index].numeroLote!, itemCode: self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!, action: "update")
                lotsRequest.append(lotRequest!)
            } else {
                 //No existe se crea un nuevo objeto de inserción
                lotRequest = LotsRequest(orderId: self.orderId, assignedQty: lsa.cantidadSeleccionada!, batchNumber: lsa.numeroLote!, itemCode: self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!, action: "insert")
                lotsRequest.append(lotRequest!)
            }
        }
        
     self.sendToServerAssignedLots(lotsToSend: lotsRequest)
    }
    
    
    func sendToServerAssignedLots(lotsToSend: [LotsRequest]) -> Void {
        self.loading.onNext(true)
        NetworkManager.shared.assingLots(lotsRequest: lotsToSend).observeOn(MainScheduler.instance).subscribe(onNext: { res in
            self.loading.onNext(false)
            if(res.response!.isEmpty) {
                self.showMessage.onNext("Proceso realizado correctamente")
                // actualiza la pantalla
                self.getLots()
                return
            }

            var badBatches = ""
            for batch in res.response! {
                badBatches += " \(batch)"
            }
            self.showMessage.onNext("Hubo un error al asignar los siguientes lotes\(badBatches)")
        }, onError:  { error in
            self.loading.onNext(false)
            self.showMessage.onNext("Hubo un error al asignar los lotes, por favor intentar de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func saveCacheLotsSelected(index: Int) -> Void {
//        let productCode = lineDocumentsDataAux[index].codigoProducto ?? ""
//        self.cache[productCode] = self.lotsSelectedAux
//        print("Valor del \(self.cache[productCode])")
    }
}

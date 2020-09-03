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

    var quantitySelectedInput = BehaviorSubject<String>(value: "")
    var rowSelected = PublishSubject<Int>()
    
    var lineDocumentsDataAux:[Lots] = []
    var lotsAvailablesAux:[LotsAvailable] = []
    
    var itemSelectedLineDocuments: Int?
    var itemDeselectedLineDocuments: Int?
    var itemLotSelected:LotsSelected? = nil
    
    var cache:[String: [LotsSelected]]  = [:]
    var cacheOriginal: [String: [LotsSelected]]  = [:]
    var cacheLineDocuments: [String:Lots] = [:]
        var firstTime = PublishSubject<Void>()
    
    private var selectedBatches: [BatchSelected] = []
    var documentSelected = PublishSubject<Lots>()
    var availableSelected = PublishSubject<LotsAvailable?>()
    var batchSelected = PublishSubject<LotsSelected>()
    var documentLines: [Lots] = []
    
    init() {
        // Añade lotes de Lotes disponibles a Lotes Seleccionados
        let inputs = Observable.combineLatest(documentSelected, availableSelected, quantitySelectedInput)
        self.addLotDidTap.withLatestFrom(inputs).map({
            LotsAvailableInfo(documentSelected: $0, availableSelected: $1, quantitySelected: ($2.isEmpty ? 0 : Decimal(string: $2)) ?? 0)
        }).subscribe(onNext: { data in
            self.availableSelected.onNext(nil)

            if (data.availableSelected == nil || data.quantitySelected == 0) {
                return
            }

            if let existing = self.selectedBatches.first(where: { batch in
                return batch.itemCode == data.documentSelected?.codigoProducto && batch.batchNumber == data.availableSelected?.numeroLote
            }) {
                existing.action = "delete"
                self.selectedBatches.append(BatchSelected(orderId: existing.orderId, assignedQty: data.quantitySelected, batchNumber: existing.batchNumber, itemCode: existing.itemCode, action: "insert", sysNumber: existing.sysNumber))
            } else {
                self.selectedBatches.append(BatchSelected(
                    orderId: self.orderId,
                    assignedQty: data.quantitySelected,
                    batchNumber: data.availableSelected?.numeroLote,
                    itemCode: data.documentSelected?.codigoProducto,
                    action: "insert",
                    sysNumber: data.availableSelected?.sysNumber))
                self.dataLotsSelected.onNext(self.selectedBatches.map({ $0.toLotsSelected() }))
                
                if let doc = self.documentLines.first(where: { $0.codigoProducto == data.documentSelected?.codigoProducto }) {
                    doc.totalNecesario = (doc.totalNecesario ?? 0) - data.quantitySelected
                    doc.totalSeleccionado = self.selectedBatches.filter({ data.documentSelected?.codigoProducto == $0.itemCode && $0.action != "delete" }).compactMap({ $0.assignedQty }).reduce(0, +)
                    
                    doc.lotesDisponibles?.forEach({ lot in
                        if (lot.numeroLote == data.availableSelected?.numeroLote) {
                            lot.cantidadDisponible = (lot.cantidadDisponible ?? 0) - data.quantitySelected
                        }
                        lot.cantidadSeleccionada = min(doc.totalNecesario ?? 0, lot.cantidadDisponible ?? 0)
                    })
                    
                    if let availableBatches = doc.lotesDisponibles {
                        self.dataLotsAvailable.onNext(availableBatches)
                    }
                }
                
                self.dataOfLots.onNext(self.documentLines)
            }
        }).disposed(by: self.disposeBag)
        
        // Remueve un lote de Lotes seleccionados y lo pasa a Lotes Disponibles
        let inputsRemove = Observable.combineLatest(documentSelected, batchSelected)
        self.removeLotDidTap.withLatestFrom(inputsRemove).map({
            ($0, $1)
        }).subscribe(onNext: { document, batch in
            if let existing = self.selectedBatches.first(where: { b in
                return b.batchNumber == batch.numeroLote
            }) {
                if (existing.action != nil) {
                    if let index = self.selectedBatches.firstIndex(where: { $0.batchNumber == existing.batchNumber }) {
                        self.selectedBatches.remove(at: index)
                        self.dataLotsSelected.onNext(self.selectedBatches.map({ $0.toLotsSelected() }))
                    }
                } else {
                    existing.action = "delete"
                    let newSelected = self.selectedBatches.filter({ $0.batchNumber == batch.numeroLote && $0.action != "delete" })
                    if (newSelected.count > 0) {
                        self.dataLotsSelected.onNext(newSelected.map({ $0.toLotsSelected() }))
                    } else {
                        self.dataLotsSelected.onNext([])
                    }
                }
                
                if let doc = self.documentLines.first(where: { $0.codigoProducto == document.codigoProducto }) {
                    doc.totalNecesario = (doc.totalNecesario ?? 0) + (batch.cantidadSeleccionada ?? 0)
                    doc.totalSeleccionado = self.selectedBatches.filter({ document.codigoProducto == $0.itemCode && $0.action != "delete" }).compactMap({ $0.assignedQty }).reduce(0, +)
                    
                    doc.lotesDisponibles?.forEach({ lot in
                        if (lot.numeroLote == batch.numeroLote) {
                            lot.cantidadDisponible = (lot.cantidadDisponible ?? 0) + (batch.cantidadSeleccionada ?? 0)
                        }
                        lot.cantidadSeleccionada = min(doc.totalNecesario ?? 0, lot.cantidadDisponible ?? 0)
                    })
                    
                    if let availableBatches = doc.lotesDisponibles {
                        self.dataLotsAvailable.onNext(availableBatches)
                    }
                }
                
                self.dataOfLots.onNext(self.documentLines)
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
                self.documentLines = lotsData
                self.selectedBatches = lotsData.map({ batch in
                    let selected: [BatchSelected] = batch.lotesSelecionados != nil ? batch.lotesSelecionados!.compactMap({ sel in
                        return BatchSelected(orderId: self.orderId, assignedQty: sel.cantidadSeleccionada, batchNumber: sel.numeroLote, itemCode: batch.codigoProducto, action: nil, sysNumber: sel.sysNumber)
                    }) : []
                    return selected
                }).reduce([], +)
                
                for lotData in lotsData {
                    for lot in lotData.lotesDisponibles ?? [] {
                        lot.cantidadSeleccionada = min(lotData.totalNecesario ?? 0, lot.cantidadDisponible ?? 0)
                    }
                }
                self.dataOfLots.onNext(lotsData)
            }
        }, onError: { error in
            self.loading.onNext(false)
            self.showMessage.onNext("Hubo un error al cargar los lotes, por favor de intentarlo de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func itemSelectedOfLineDocTable(lot: Lots) -> Void {
        if (lot.lotesDisponibles?.count ?? 0 > 0) {
              self.dataLotsAvailable.onNext(lot.lotesDisponibles!)
        } else {
            self.dataLotsAvailable.onNext([])
        }
        
        let selected = self.selectedBatches.filter({ b in
            b.itemCode == lot.codigoProducto && b.action != "delete" && b.assignedQty != 0
        }).map({ $0.toLotsSelected() })

        if(selected.count > 0) {
            self.dataLotsSelected.onNext(selected)
        } else {
            self.dataLotsSelected.onNext([])
        }
        
        // Selección automática de lote disponible
        if (lot.lotesDisponibles!.count == 1 && selected.count == 0) {
            if let firstAvailable = lot.lotesDisponibles?.first, let doc = self.documentLines.first(where: { $0.codigoProducto == lot.codigoProducto }) {
                let batch = BatchSelected(orderId: orderId, assignedQty: firstAvailable.cantidadSeleccionada, batchNumber: firstAvailable.numeroLote, itemCode: lot.codigoProducto, action: "insert", sysNumber: firstAvailable.sysNumber)
                    
                doc.totalNecesario = 0
                doc.totalSeleccionado = doc.lotesDisponibles![0].cantidadSeleccionada!
                self.selectedBatches.append(batch)
                
                self.dataOfLots.onNext(documentLines)
            }
        }
    }
        
    func assingLots() -> Void {
//        print("caché \(self.cache)")
//        // Proceso de eliminación
//        // Se busca lote del diccionario original (cacheOriginal) en arreglo actual (cache)
//        // Si existe no se realiza nada
//        // No existe, se crea el objeto para la eliminación en servicio
//        var lotsRequest:[LotsRequest] = []
//
//        for (key, value) in self.cacheOriginal {
//            for lso in value {
//                if ( self.cache[key]!.first(where: ({$0.numeroLote == lso.numeroLote})) == nil) {
//                    let lotRequest = LotsRequest(orderId: self.orderId, assignedQty: lso.cantidadSeleccionada!, batchNumber: lso.numeroLote!, itemCode: self.lineDocumentsDataAux[self.itemSelectedLineDocuments].codigoProducto!, action: "delete")
//                    lotsRequest.append(lotRequest)
//                }
//            }
//        }
//
//        // Proceso de inserción y actualización
//        //Se busca lote del arreglo actual (lotsSelectedAux) en el arreglo original (lotsSelectedCopy)
//        // Si existe se crea el objeto de eliminación del arreglo original, se obtiene el valor absoluto de la resta de cantidad seleccionada entre lotsSelectedAux y lotsSelectedCopy, por último se crea el objecto de actualización con el valor de la resta
//        //No existe se crea un nuevo objeto de inserción
//        var indexLineDocument: Int = 0
//        for (key, value) in self.cache {
//                for lsa in  value {
//                    var lotRequest:LotsRequest? = nil
//                    if let index = self.cacheOriginal[key]!.firstIndex(where: ({ $0.numeroLote == lsa.numeroLote})) {
//                        if self.cacheOriginal[key]!.firstIndex(where: ({ $0.cantidadSeleccionada != lsa.cantidadSeleccionada})) != nil {
//                            // Se crea el objeto de eliminación
//                            lotRequest = LotsRequest(orderId: self.orderId, assignedQty: self.cacheOriginal[key]![index].cantidadSeleccionada!, batchNumber: self.cacheOriginal[key]![index].numeroLote!, itemCode: self.lineDocumentsDataAux[indexLineDocument].codigoProducto!, action: "delete")
//                            lotsRequest.append(lotRequest!)
//
//                            //Se obtiene el valor absoluto de la resta de cantidad seleccionada entre lotsSelectedAux y lotsSelectedCopy, por último se crea el objecto de actualización con el valor de la resta
//                             var  subtraction = lsa.cantidadSeleccionada! -  self.cacheOriginal[key]![index].cantidadSeleccionada!
//                            if(subtraction.isLess(than: 0.0)) {
//                                subtraction = (subtraction * -1)
//                            }
//
//                            lotRequest = LotsRequest(orderId: self.orderId, assignedQty: subtraction, batchNumber:  self.cacheOriginal[key]![index].numeroLote!, itemCode:  self.cacheLineDocuments[key]!.codigoProducto! , action: "update")
//                            lotsRequest.append(lotRequest!)
//                        }
//                    } else {
//                         //No existe se crea un nuevo objeto de inserción
//                        lotRequest = LotsRequest(orderId: self.orderId, assignedQty: lsa.cantidadSeleccionada!, batchNumber: lsa.numeroLote!, itemCode:  self.cacheLineDocuments[key]!.codigoProducto!, action: "insert")
//                        lotsRequest.append(lotRequest!)
//                    }
//                }
//            indexLineDocument += 1
//        }
        let batchesToSend = self.selectedBatches.filter({ $0.action != nil })
        if (batchesToSend.count == 0) {
            self.showMessage.onNext("No se han realizado modificaciones de lotes")
            return
        }
        self.sendToServerAssignedLots(lotsToSend: batchesToSend)
    }
    
    
    func sendToServerAssignedLots(lotsToSend: [BatchSelected]) -> Void {
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

    func resetVariables() {
        self.cache = [:]
        self.cacheOriginal = [:]
        self.cacheLineDocuments = [:]
        self.lineDocumentsDataAux = []
        self.lineDocumentsDataAux = []
        self.lotsAvailablesAux = []
    }
}

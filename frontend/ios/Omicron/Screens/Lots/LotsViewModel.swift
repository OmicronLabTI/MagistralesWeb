//
//  LotsViewModel.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import RxCocoa
import RxSwift
import Resolver

class LotsViewModel {
    
    // MARK: Variables
    var loading = PublishSubject<Bool>()
    var showMessage = PublishSubject<String>()
    var orderId = -1
    var disposeBag = DisposeBag()
    
    var lastResponder = PublishSubject<Any?>()
    
    var dataOfLots = BehaviorSubject<[Lots]>(value: [])
    var dataLotsAvailable = BehaviorSubject<[LotsAvailable]>(value: [])
    var dataLotsSelected = BehaviorSubject<[LotsSelected]>(value: [])
    
    var indexProductSelected = BehaviorSubject<IndexPath?>(value: nil)

    var productSelected = BehaviorSubject<Lots?>(value: nil)
    var availableSelected = BehaviorSubject<LotsAvailable?>(value: nil)
    var batchSelected = BehaviorSubject<LotsSelected?>(value: nil)
    
    var addLotDidTap = PublishSubject<Void>()
    var removeLotDidTap = PublishSubject<Void>()
    var saveLotsDidTap = PublishSubject<Void>()
    
    var itemLotSelected:LotsSelected? = nil
    
    var finishOrderDidTap = PublishSubject<Void>()
    var backToInboxView = PublishSubject<Void>()
    private var selectedBatches: [BatchSelected] = []
    private var documentLines: [Lots] = []
    var technicalSignatureIsGet = false
    var qfbSignatureIsGet = false
    var askIfUserWantToFinalizeOrder = PublishSubject<String>()
    var showSignatureViewFromLotsView = PublishSubject<String>()
    var sqfbSignature = ""
    var technicalSignature = ""
    var showSignatureView = PublishSubject<String>()
    
    init() {
        
        // Finaliza la orden
        self.finishOrderDidTap.subscribe(onNext: { [weak self] in
            self?.askIfUserWantToFinalizeOrder.onNext("¿Deseas terminar la orden?")
        }).disposed(by: self.disposeBag)
        
        // Añade lotes de Lotes disponibles a Lotes Seleccionados
        let inputs = Observable.combineLatest(productSelected, availableSelected)
        self.addLotDidTap.withLatestFrom(inputs).subscribe(onNext: { [weak self] productSelected, availableSelected in
            self?.availableSelected.onNext(nil)
            
            guard let product = productSelected else { return }
            guard let available = availableSelected else { return }
            guard let doc = self?.documentLines.first(where: { $0.codigoProducto == product.codigoProducto }) else { return }
            
            let quantity = (available.cantidadSeleccionada ?? 0)
            
            if (quantity == 0 || quantity > (doc.totalNecesario ?? 0) || (available.cantidadDisponible ?? 0) == 0) {
                return
            }
            
            if let existing = self?.selectedBatches.first(where: { batch in
                return batch.itemCode == product.codigoProducto && batch.batchNumber == available.numeroLote && batch.action != "delete"
            }) {
                if existing.action == nil {
                    existing.action = "delete"
                    self?.selectedBatches.append(BatchSelected(
                        orderId: existing.orderId,
                        assignedQty: quantity,
                        batchNumber:
                        existing.batchNumber,
                        itemCode: existing.itemCode,
                        action: "insert",
                        sysNumber: existing.sysNumber))
                } else {
                    existing.assignedQty = (existing.assignedQty ?? 0) + (available.cantidadSeleccionada ?? 0)
                    let newSelected = self?.getFilteredSelected(itemCode: existing.itemCode) ?? []
                    if (newSelected.count > 0) {
                        self?.dataLotsSelected.onNext(newSelected)
                    } else {
                        self?.dataLotsSelected.onNext([])
                    }
                }
            } else {
                self?.selectedBatches.append(BatchSelected(
                    orderId: self?.orderId,
                    assignedQty: quantity,
                    batchNumber: available.numeroLote,
                    itemCode: product.codigoProducto,
                    action: "insert",
                    sysNumber: available.sysNumber))
                self?.dataLotsSelected.onNext(self?.getFilteredSelected(itemCode: product.codigoProducto) ?? [])
            }
            
            doc.totalNecesario = (doc.totalNecesario ?? 0) - quantity
            doc.totalSeleccionado = self?.selectedBatches.filter({ product.codigoProducto == $0.itemCode && $0.action != "delete" }).compactMap({ $0.assignedQty }).reduce(0, +)
            
            doc.lotesDisponibles?.forEach({ lot in
                if (lot.numeroLote == available.numeroLote) {
                    lot.cantidadDisponible = (lot.cantidadDisponible ?? 0) - quantity
                }
                lot.cantidadSeleccionada = min(doc.totalNecesario ?? 0, lot.cantidadDisponible ?? 0)
            })
            
            if let availableBatches = doc.lotesDisponibles {
                self?.dataLotsAvailable.onNext(availableBatches)
            }
            
            self?.dataOfLots.onNext(self?.documentLines ?? [])
        }).disposed(by: self.disposeBag)
        
        // Remueve un lote de Lotes seleccionados y lo pasa a Lotes Disponibles
        let inputsRemove = Observable.combineLatest(productSelected, batchSelected)
        self.removeLotDidTap.withLatestFrom(inputsRemove).subscribe(onNext: { [weak self] document, batch in
            if let existing = self?.selectedBatches.first(where: { b in
                return b.batchNumber == batch?.numeroLote
            }) {
                if (existing.action != nil) {
                    if let index = self?.selectedBatches.firstIndex(where: { $0.batchNumber == existing.batchNumber && $0.action != "delete" }) {
                        self?.selectedBatches.remove(at: index)
                        let newSelected = self?.getFilteredSelected(itemCode: existing.itemCode) ?? []
                        self?.dataLotsSelected.onNext(newSelected)
                    }
                } else {
                    existing.action = "delete"
                    let newSelected = self?.getFilteredSelected(itemCode: existing.itemCode) ?? []
                    if (newSelected.count > 0) {
                        self?.dataLotsSelected.onNext(newSelected)
                    } else {
                        self?.dataLotsSelected.onNext([])
                    }
                }
                
                if let doc = self?.documentLines.first(where: { $0.codigoProducto == document?.codigoProducto }) {
                    doc.totalNecesario = (doc.totalNecesario ?? 0) + (batch?.cantidadSeleccionada ?? 0)
                    doc.totalSeleccionado = self?.selectedBatches.filter({ doc.codigoProducto == $0.itemCode && $0.action != "delete" }).compactMap({ $0.assignedQty }).reduce(0, +)
                    
                    doc.lotesDisponibles?.forEach({ lot in
                        if (lot.numeroLote == batch?.numeroLote) {
                            lot.cantidadDisponible = (lot.cantidadDisponible ?? 0) + (batch?.cantidadSeleccionada ?? 0)
                        }
                        lot.cantidadSeleccionada = min(doc.totalNecesario ?? 0, lot.cantidadDisponible ?? 0)
                    })
                    
                    if let availableBatches = doc.lotesDisponibles {
                        self?.dataLotsAvailable.onNext(availableBatches)
                    }
                }
                
                self?.dataOfLots.onNext(self?.documentLines ?? [])
            }
        }).disposed(by: self.disposeBag)
        
        
        // Guada los lotes seleccionados y los manda al servicio
        self.saveLotsDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.assignLots()
        }).self.disposed(by: self.disposeBag)
        
    }
    
    // MARK: -Functions
    func getLots() -> Void {
        self.loading.onNext(true)
        NetworkManager.shared.getLots(orderId: orderId).observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] data in
            self?.loading.onNext(false)
            if let lotsData = data.response {
                if lotsData.count > 0 {
                    self?.documentLines = lotsData
                    self?.selectedBatches = lotsData.map({ batch in
                        let selected: [BatchSelected] = batch.lotesSelecionados != nil ? batch.lotesSelecionados!.compactMap({ sel in
                            return BatchSelected(orderId: self?.orderId, assignedQty: sel.cantidadSeleccionada, batchNumber: sel.numeroLote, itemCode: batch.codigoProducto, action: nil, sysNumber: sel.sysNumber)
                        }) : []
                        return selected
                    }).reduce([], +)
                    
                    for lotData in lotsData {
                        for lot in lotData.lotesDisponibles ?? [] {
                            lot.cantidadSeleccionada = min(lotData.totalNecesario ?? 0, lot.cantidadDisponible ?? 0)
                        }
                    }
                    self?.dataOfLots.onNext(lotsData)
                } else {
                    self?.showMessage.onNext("No hay lotes asignados")
                }
            }
            }, onError: { [weak self] error in
                self?.loading.onNext(false)
                self?.showMessage.onNext("Hubo un error al cargar los lotes, por favor de intentarlo de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func updateInfoSelectedBatch(lot: Lots) -> Void {
        if (lot.lotesDisponibles?.count ?? 0 > 0) {
            self.dataLotsAvailable.onNext(lot.lotesDisponibles ?? [])
        } else {
            self.dataLotsAvailable.onNext([])
        }
        
        let selected = self.getFilteredSelected(itemCode: lot.codigoProducto)
        
        if(selected.count > 0) {
            self.dataLotsSelected.onNext(selected)
        } else {
            self.dataLotsSelected.onNext([])
        }
        
        self.selectBatchIfNeeded(lot: lot, selected: selected)
    }
    
    func selectBatchIfNeeded(lot: Lots, selected: [LotsSelected]) -> Void {
        // Selección automática de lote disponible
        if (lot.lotesDisponibles!.count == 1 && selected.count == 0) {
            if let firstAvailable = lot.lotesDisponibles?.first, let doc = self.documentLines.first(where: { $0.codigoProducto == lot.codigoProducto }) {
                if ((firstAvailable.cantidadDisponible ?? 0) > 0) {
                    let batch = BatchSelected(orderId: orderId, assignedQty: firstAvailable.cantidadSeleccionada, batchNumber: firstAvailable.numeroLote, itemCode: lot.codigoProducto, action: "insert", sysNumber: firstAvailable.sysNumber)
                    doc.totalNecesario = 0
                    
                    guard let firstBatch = doc.lotesDisponibles?.first else { return }
                    doc.totalSeleccionado = firstBatch.cantidadSeleccionada ?? 0
                    self.selectedBatches.append(batch)
                    self.dataOfLots.onNext(documentLines)
                    self.dataLotsSelected.onNext(self.getFilteredSelected(itemCode: doc.codigoProducto))
                    
                    doc.lotesDisponibles?.forEach({ lot in
                        if (lot.numeroLote == firstBatch.numeroLote) {
                            lot.cantidadDisponible = (lot.cantidadDisponible ?? 0) - (firstBatch.cantidadSeleccionada ?? 0)
                        }
                        lot.cantidadSeleccionada = min(doc.totalNecesario ?? 0, lot.cantidadDisponible ?? 0)
                    })
                    
                    if let availableBatches = doc.lotesDisponibles {
                        self.dataLotsAvailable.onNext(availableBatches)
                    }
                    
                    self.dataOfLots.onNext(self.documentLines)
                }
            }
        }
    }
    
    func assignLots() -> Void {
        let batchesToSend = self.selectedBatches.filter({ $0.action != nil })
        if (batchesToSend.count == 0) {
            self.showMessage.onNext("No se han realizado modificaciones de lotes")
            return
        }
        self.sendToServerAssignedLots(lotsToSend: batchesToSend)
    }
    
    
    func sendToServerAssignedLots(lotsToSend: [BatchSelected]) -> Void {
        self.loading.onNext(true)
        NetworkManager.shared.assignLots(lotsRequest: lotsToSend).observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] res in
            self?.loading.onNext(false)
            if(res.response!.isEmpty) {
                self?.showMessage.onNext("Proceso realizado correctamente")
                // actualiza la pantalla
                self?.getLots()
                return
            }
            
            var badBatches = ""
            for batch in res.response! {
                badBatches += "\n\(batch)"
            }
            self?.showMessage.onNext("Hubo un error al asignar los siguientes lotes\(badBatches)")
            }, onError:  { [weak self] error in
                self?.loading.onNext(false)
                self?.showMessage.onNext("Hubo un error al asignar los lotes, por favor intentar de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    private func getFilteredSelected(itemCode: String?) -> [LotsSelected] {
        return self.selectedBatches
            .filter({ $0.itemCode == itemCode && $0.action != "delete" })
            .map({ $0.toLotsSelected() })
    }
    
    private func getFilteredSelected(itemCode: String?, batchNumber: String?) -> [LotsSelected] {
        return self.selectedBatches
            .filter({ $0.itemCode == itemCode && $0.batchNumber == batchNumber && $0.action != "delete" })
            .map({ $0.toLotsSelected() })
    }
    
    // Pregunta al server si la orden puede ser finaliada o no
    func validIfOrderCanBeFinalized() -> Void  {
        self.loading.onNext(true)
        NetworkManager.shared.askIfOrderCanBeFinalized(orderId: self.orderId).subscribe(onNext: { [weak self] res in
            self?.loading.onNext(false)
            self?.showSignatureView.onNext("Firma del  QFB")
            }, onError: { [weak self] error in
                self?.loading.onNext(false)
                self?.showMessage.onNext("La orden no puede ser terminada, revisa que todos los artículos tengan un lote asignado")
        }).disposed(by: self.disposeBag)
        
    }
    
    // Valida si el usuario obtuvo las firmas y finaliza la orden 
    func callFinishOrderService() -> Void {
        
        if(self.technicalSignatureIsGet && self.qfbSignatureIsGet) {
            self.loading.onNext(true)
            let finishOrder = FinishOrder(userId: Persistence.shared.getUserData()!.id!, fabricationOrderId: self.orderId, qfbSignature: "", technicalSignature: "")
            
            NetworkManager.shared.finishOrder(order: finishOrder).subscribe(onNext: { [weak self] _ in
                self?.loading.onNext(false)
                self?.backToInboxView.onNext(())
                }, onError: {[weak self] error in
                    self?.loading.onNext(false)
                    self?.showMessage.onNext("Ocurrió un error al finalizar la orden, por favor intentarlo de nuevo")
            }).disposed(by: self.disposeBag)
        }
    }
}

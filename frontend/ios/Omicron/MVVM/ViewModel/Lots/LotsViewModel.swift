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
    var itemLotSelected: LotsSelected?
    var finishOrderDidTap = PublishSubject<Void>()
    var backToInboxView = PublishSubject<Void>()
    var selectedBatches: [BatchSelected] = []
    var documentLines: [Lots] = []
    var technicalSignatureIsGet = false
    var qfbSignatureIsGet = false
    var askIfUserWantToFinalizeOrder = PublishSubject<String>()
    var showSignatureViewFromLotsView = PublishSubject<String>()
    var sqfbSignature = String()
    var technicalSignature = String()
    var showSignatureView = PublishSubject<String>()
    var updateComments = PublishSubject<OrderDetail>()
    var pendingButtonDidTap = PublishSubject<Void>()
    var askIfUserWantChageOrderToPendigStatus = PublishSubject<String>()
    var changeColorLabels = PublishSubject<Void>()
    var enableAddButton = PublishSubject<Bool>()
    var enableRemoveButton = PublishSubject<Bool>()

    @Injected var orderDetail: OrderDetailViewModel
    @Injected var rootViewModel: RootViewModel
    @Injected var networkManager: NetworkManager

    init() {
        // Finaliza la orden
        finishOrderDidTapBinding()
        // Pone en pendiente la orden
        pendingButtonDidTapBinding()
        // Añade lotes de Lotes disponibles a Lotes Seleccionados
        addLotDidTapAction()
        // Remueve un lote de Lotes seleccionados y lo pasa a Lotes Disponibles
        removeLotAction()
        // Guada los lotes seleccionados y los manda al servicio
        saveLotsDidTapBinding()
    }

    // MARK: - Functions
    func finishOrderDidTapBinding() {
        self.finishOrderDidTap.subscribe(onNext: { [weak self] in
            self?.validIfOrderCanBeFinalized()
        }).disposed(by: self.disposeBag)
    }

    func pendingButtonDidTapBinding() {
        self.pendingButtonDidTap.subscribe(onNext: { [weak self] _ in
            self?.askIfUserWantChageOrderToPendigStatus.onNext(CommonStrings.confirmationMessagePendingStatus)
        }).disposed(by: self.disposeBag)
    }

    func saveLotsDidTapBinding() {
        self.saveLotsDidTap.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.assignLots()
        }).self.disposed(by: self.disposeBag)
    }

    func addLotDidTapAction() {
        let inputs = Observable.combineLatest(productSelected, availableSelected)
        self.addLotDidTap.withLatestFrom(inputs).subscribe(onNext: { [weak self] productSelected, availableSelected in
            self?.availableSelected.onNext(nil)
            self?.enableAddButton.onNext(false)
            guard let product = productSelected else { return }
            guard let available = availableSelected else { return }
            guard let doc = self?.documentLines
                .first(where: { $0.codigoProducto == product.codigoProducto }) else { return }
            let quantity = available.cantidadSeleccionada ?? 0
            if quantity == 0 || quantity > (doc.totalNecesario ?? 0) || (available.cantidadDisponible ?? 0)
                == 0 || quantity > (available.cantidadDisponible ?? 0) {
                return
            }
            if let existing = self?.selectedBatches.first(where: { batch in
                return batch.itemCode == product.codigoProducto
                && batch.batchNumber == available.numeroLote && batch.action != Actions.delete.rawValue
            }) {
                existing.action == nil ? self?.doProcessWhenActionNotExist(existing, available, product) :
                    self?.doProcessWhenActionExist(existing, available)
            } else {
                self?.selectedBatches.append(BatchSelected(
                    orderId: self?.orderId, assignedQty: quantity, batchNumber: available.numeroLote,
                    itemCode: product.codigoProducto, action: Actions.insert.rawValue, sysNumber: available.sysNumber,
                    expiredBatch: available.expiredBatch, areBatchesComplete: 0))
                self?.dataLotsSelected.onNext(self?.getFilteredSelected(itemCode: product.codigoProducto) ?? [])
            }
            doc.totalNecesario = (doc.totalNecesario ?? 0) - quantity
            doc.totalSeleccionado = self?.selectedBatches
                .filter({ product.codigoProducto == $0.itemCode && $0.action != Actions.delete.rawValue })
                .compactMap({ $0.assignedQty }).reduce(0, +)
            doc.lotesDisponibles?.forEach({ lot in
                if lot.numeroLote == available.numeroLote {
                    lot.cantidadDisponible = (lot.cantidadDisponible ?? 0) - quantity
                }
                lot.cantidadSeleccionada = min(doc.totalNecesario ?? 0, lot.cantidadDisponible ?? 0)
            })
            if let availableBatches = doc.lotesDisponibles {
                self?.dataLotsAvailable.onNext(availableBatches)
            }
            self?.dataOfLots.onNext(self?.documentLines ?? [])
        }).disposed(by: self.disposeBag)
    }

    func doProcessWhenActionNotExist(_ existing: BatchSelected, _ available: LotsAvailable, _ product: Lots) {
        existing.action = Actions.delete.rawValue
        self.selectedBatches.append(BatchSelected(orderId: existing.orderId,
            assignedQty: (existing.assignedQty ?? 0) + (available.cantidadSeleccionada ?? 0),
            batchNumber: existing.batchNumber, itemCode: existing.itemCode, action: Actions.insert.rawValue,
            sysNumber: existing.sysNumber, expiredBatch: existing.expiredBatch, areBatchesComplete: 0))
        self.dataLotsSelected.onNext(self.getFilteredSelected(itemCode: product.codigoProducto))
    }
    func doProcessWhenActionExist(_ existing: BatchSelected, _ available: LotsAvailable) {
        existing.assignedQty = (existing.assignedQty ?? 0) + (available.cantidadSeleccionada ?? 0)
        let newSelected = self.getFilteredSelected(itemCode: existing.itemCode)
        newSelected.count > 0 ? self.dataLotsSelected.onNext(newSelected) :
        self.dataLotsSelected.onNext([])
    }

    func removeLotAction() {
        let inputsRemove = Observable.combineLatest(productSelected, batchSelected)
        self.removeLotDidTap.withLatestFrom(inputsRemove).subscribe(onNext: { [weak self] document, batch in
            self?.enableRemoveButton.onNext(false)
            if let existing = self?.selectedBatches.first(where: { batchItem in
                return batchItem.batchNumber == batch?.numeroLote
            }) {
                if existing.action != nil {
                    if let index = self?.selectedBatches
                        .firstIndex(
                            where: { $0.batchNumber == existing.batchNumber && $0.action != Actions.delete.rawValue && $0.itemCode == existing.itemCode }) {
                        self?.selectedBatches.remove(at: index)
                        let newSelected = self?.getFilteredSelected(itemCode: existing.itemCode) ?? []
                        self?.dataLotsSelected.onNext(newSelected)
                    }
                } else {
                    existing.action = Actions.delete.rawValue
                    let newSelected = self?.getFilteredSelected(itemCode: existing.itemCode) ?? []
                    if newSelected.count > 0 {
                        self?.dataLotsSelected.onNext(newSelected)
                    } else {
                        self?.dataLotsSelected.onNext([])
                    }
                }
                if let doc = self?.documentLines.first(where: { $0.codigoProducto == document?.codigoProducto }) {
                    doc.totalNecesario = (doc.totalNecesario ?? 0) + (batch?.cantidadSeleccionada ?? 0)
                    doc.totalSeleccionado = self?.selectedBatches
                        .filter({ doc.codigoProducto == $0.itemCode && $0.action != Actions.delete.rawValue })
                        .compactMap({ $0.assignedQty }).reduce(0, +)
                    doc.lotesDisponibles?.forEach({ lot in
                        if lot.numeroLote == batch?.numeroLote {
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
    }

    func updateInfoSelectedBatch(lot: Lots) {
        if lot.lotesDisponibles?.count ?? 0 > 0 {
            for lote in lot.lotesDisponibles! {
                lote.expiredBatch = UtilsManager.shared.calculateExpiredBatch(date: lote.fechaExp)
            }
            lot.lotesDisponibles?.sort { ($0.expiredBatch && !$1.expiredBatch) }
            self.dataLotsAvailable.onNext(lot.lotesDisponibles ?? [])
        } else {
            self.dataLotsAvailable.onNext([])
        }
        var selected = self.getFilteredSelected(itemCode: lot.codigoProducto)
        if selected.count > 0 {
            for select in selected {
                select.expiredBatch = ((lot.lotesDisponibles?.first(where: { lote -> Bool in
                    return lote.numeroLote == select.numeroLote && UtilsManager.shared.calculateExpiredBatch(date: lote.fechaExp)
                })) != nil)
            }
            selected.sort { ($0.expiredBatch && !$1.expiredBatch) }
            self.dataLotsAvailable.onNext(lot.lotesDisponibles ?? [])
            self.dataLotsSelected.onNext(selected)
        } else {
            self.dataLotsSelected.onNext([])
        }
        self.selectBatchIfNeeded(lot: lot, selected: selected)
    }

    func selectBatchIfNeeded(lot: Lots, selected: [LotsSelected]) {
        // Selección automática de lote disponible
        if lot.lotesDisponibles!.count == 1 && selected.count == 0 {
            if let firstAvailable = lot.lotesDisponibles?.first,
                let doc = self.documentLines.first(where: { $0.codigoProducto == lot.codigoProducto }) {
                if (firstAvailable.cantidadDisponible ?? 0) > 0 {
                    let batch = BatchSelected(
                        orderId: orderId, assignedQty: firstAvailable.cantidadSeleccionada,
                        batchNumber: firstAvailable.numeroLote, itemCode: lot.codigoProducto,
                        action: Actions.insert.rawValue, sysNumber: firstAvailable.sysNumber,
                        expiredBatch: firstAvailable.expiredBatch, areBatchesComplete: 0)
                    if let totalNecesario = lot.totalNecesario,
                       let cantidadSeleccionada = firstAvailable.cantidadSeleccionada {
                        let result = totalNecesario - cantidadSeleccionada
                        doc.totalNecesario = result.isSignMinus ? result * -1 : result
                    } else {
                        doc.totalNecesario = 0
                    }
                    guard let firstBatch = doc.lotesDisponibles?.first else { return }
                    doc.totalSeleccionado = firstBatch.cantidadSeleccionada ?? 0
                    self.selectedBatches.append(batch)
                    self.dataOfLots.onNext(documentLines)
                    self.dataLotsSelected.onNext(self.getFilteredSelected(itemCode: doc.codigoProducto))
                    doc.lotesDisponibles?.forEach({ lot in
                        if lot.numeroLote == firstBatch.numeroLote {
                            lot.cantidadDisponible = (lot.cantidadDisponible ?? 0) -
                                (firstBatch.cantidadSeleccionada ?? 0)
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
    func assignLots() {
        let batchesToSend = self.selectedBatches.filter({ $0.action != nil })
        if batchesToSend.count == 0 {
            self.showMessage.onNext(CommonStrings.noChanges)
            return
        }
        let areBatchesComplete = self.documentLines.filter({$0.totalNecesario == 0})
            .count == self.documentLines.count ? 1 : 0
        batchesToSend.forEach { (item) in
            item.areBatchesComplete = areBatchesComplete
        }
        self.sendToServerAssignedLots(lotsToSend: batchesToSend)
    }
}

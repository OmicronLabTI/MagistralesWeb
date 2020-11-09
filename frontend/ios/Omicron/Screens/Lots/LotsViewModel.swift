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
// swiftlint:disable type_body_length
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
    var itemLotSelected: LotsSelected?
    var finishOrderDidTap = PublishSubject<Void>()
    var backToInboxView = PublishSubject<Void>()
    var selectedBatches: [BatchSelected] = []
    var documentLines: [Lots] = []
    var technicalSignatureIsGet = false
    var qfbSignatureIsGet = false
    var askIfUserWantToFinalizeOrder = PublishSubject<String>()
    var showSignatureViewFromLotsView = PublishSubject<String>()
    var sqfbSignature = ""
    var technicalSignature = ""
    var showSignatureView = PublishSubject<String>()
    var updateComments = PublishSubject<OrderDetail>()
    var pendingButtonDidTap = PublishSubject<Void>()
    var askIfUserWantChageOrderToPendigStatus = PublishSubject<String>()
    var changeColorLabels = PublishSubject<Void>()
    @Injected var orderDetail: OrderDetailViewModel
    @Injected var rootViewModel: RootViewModel
    @Injected var networkManager: NetworkManager
    init() {
        // Finaliza la orden
        self.finishOrderDidTap.subscribe(onNext: { [weak self] in
            self?.askIfUserWantToFinalizeOrder.onNext("¿Deseas terminar la orden?")
        }).disposed(by: self.disposeBag)
        // Pone en pendiente la orden
        self.pendingButtonDidTap.subscribe(onNext: { [weak self] _ in
            self?.askIfUserWantChageOrderToPendigStatus.onNext(CommonStrings.confirmationMessagePendingStatus)
        }).disposed(by: self.disposeBag)
        // Añade lotes de Lotes disponibles a Lotes Seleccionados
        self.addLotDidTapAction()
        // Remueve un lote de Lotes seleccionados y lo pasa a Lotes Disponibles
        self.removeLotAction()
        // Guada los lotes seleccionados y los manda al servicio
        self.saveLotsDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.assignLots()
        }).self.disposed(by: self.disposeBag)
    }
    // MARK: - Functions
    // swiftlint:disable function_body_length
    func addLotDidTapAction() {
        let inputs = Observable.combineLatest(productSelected, availableSelected)
        self.addLotDidTap.withLatestFrom(inputs).subscribe(onNext: { [weak self] productSelected, availableSelected in
            self?.availableSelected.onNext(nil)
            guard let product = productSelected else { return }
            guard let available = availableSelected else { return }
            guard let doc = self?.documentLines
                .first(where: { $0.codigoProducto == product.codigoProducto }) else { return }
            let quantity = (available.cantidadSeleccionada ?? 0)
            if quantity == 0 || quantity > (doc.totalNecesario ?? 0) || (available.cantidadDisponible ?? 0) == 0 {
                return
            }
            if let existing = self?.selectedBatches.first(where: { batch in
                return batch.itemCode == product.codigoProducto
                    && batch.batchNumber == available.numeroLote && batch.action != "delete"
            }) {
                if existing.action == nil {
                    existing.action = "delete"
                    self?.selectedBatches.append(BatchSelected(orderId: existing.orderId,
                        assignedQty: (existing.assignedQty ?? 0) + (available.cantidadSeleccionada ?? 0),
                        batchNumber: existing.batchNumber, itemCode: existing.itemCode, action: "insert",
                        sysNumber: existing.sysNumber, expiredBatch: existing.expiredBatch))
                    self?.dataLotsSelected.onNext(self?.getFilteredSelected(itemCode: product.codigoProducto) ?? [])
                } else {
                    existing.assignedQty = (existing.assignedQty ?? 0) + (available.cantidadSeleccionada ?? 0)
                    let newSelected = self?.getFilteredSelected(itemCode: existing.itemCode) ?? []
                    if newSelected.count > 0 {
                        self?.dataLotsSelected.onNext(newSelected)
                    } else {
                        self?.dataLotsSelected.onNext([])
                    }
                }
            } else {
                self?.selectedBatches.append(BatchSelected(
                    orderId: self?.orderId, assignedQty: quantity, batchNumber: available.numeroLote,
                    itemCode: product.codigoProducto, action: "insert", sysNumber: available.sysNumber,
                    expiredBatch: available.expiredBatch))
                self?.dataLotsSelected.onNext(self?.getFilteredSelected(itemCode: product.codigoProducto) ?? [])
            }
            doc.totalNecesario = (doc.totalNecesario ?? 0) - quantity
            doc.totalSeleccionado = self?.selectedBatches
                .filter({ product.codigoProducto == $0.itemCode && $0.action != "delete" })
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
    func removeLotAction() {
        let inputsRemove = Observable.combineLatest(productSelected, batchSelected)
        self.removeLotDidTap.withLatestFrom(inputsRemove).subscribe(onNext: { [weak self] document, batch in
            if let existing = self?.selectedBatches.first(where: { batchItem in
                return batchItem.batchNumber == batch?.numeroLote
            }) {
                if existing.action != nil {
                    if let index = self?.selectedBatches
                        .firstIndex(where: { $0.batchNumber == existing.batchNumber && $0.action != "delete" }) {
                        self?.selectedBatches.remove(at: index)
                        let newSelected = self?.getFilteredSelected(itemCode: existing.itemCode) ?? []
                        self?.dataLotsSelected.onNext(newSelected)
                    }
                } else {
                    existing.action = "delete"
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
                        .filter({ doc.codigoProducto == $0.itemCode && $0.action != "delete" })
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
    func getLots() {
        self.loading.onNext(true)
        self.networkManager.getLots(orderId: orderId)
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] data in
            self?.loading.onNext(false)
            if let lotsData = data.response {
                if lotsData.count > 0 {
                    self?.documentLines = lotsData
                    self?.selectedBatches = lotsData.map({ batch in
                        let selected: [BatchSelected] = batch.lotesSelecionados != nil ?
                            batch.lotesSelecionados!.compactMap({ sel in
                            return BatchSelected(
                                orderId: self?.orderId, assignedQty: sel.cantidadSeleccionada,
                                batchNumber: sel.numeroLote, itemCode: batch.codigoProducto,
                                action: nil, sysNumber: sel.sysNumber, expiredBatch: sel.expiredBatch)
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
                self?.changeColorLabels.onNext(())
            }
            }, onError: { [weak self] error in
                self?.loading.onNext(false)
                self?.showMessage.onNext("Hubo un error al cargar los lotes, por favor de intentarlo de nuevo")
                print(error.localizedDescription)
        }).disposed(by: self.disposeBag)
    }
    func updateInfoSelectedBatch(lot: Lots) {
        if lot.lotesDisponibles?.count ?? 0 > 0 {
            for lote in lot.lotesDisponibles! {
                lote.expiredBatch = calculateExpiredBatch(date: lote.fechaExp)
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
                    return lote.numeroLote == select.numeroLote && calculateExpiredBatch(date: lote.fechaExp)
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
                        action: "insert", sysNumber: firstAvailable.sysNumber,
                        expiredBatch: firstAvailable.expiredBatch)
                    doc.totalNecesario = 0
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
            self.showMessage.onNext("No se han realizado modificaciones de lotes")
            return
        }
        self.sendToServerAssignedLots(lotsToSend: batchesToSend)
    }
    func sendToServerAssignedLots(lotsToSend: [BatchSelected]) {
        self.loading.onNext(true)
        self.networkManager.assignLots(lotsRequest: lotsToSend)
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] res in
            guard let self = self else { return }
            self.loading.onNext(false)
            if res.response!.isEmpty {
                self.showMessage.onNext("Proceso realizado correctamente")
                // actualiza la pantalla
                self.orderDetail.needsRefresh = true
                self.getLots()
                return
            }
            var badBatches = ""
            for batch in res.response! {
                badBatches += "\n\(batch)"
            }
            self.showMessage.onNext("Hubo un error al asignar los siguientes lotes\(badBatches)")
            }, onError: { [weak self] error in
                self?.loading.onNext(false)
                self?.showMessage.onNext("Hubo un error al asignar los lotes, por favor intentar de nuevo")
                print(error.localizedDescription)
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
    func validIfOrderCanBeFinalized() {
        self.loading.onNext(true)

        networkManager.getValidateOrder(orderId: orderId)
            .subscribe(onNext: { [weak self] response in
                guard let self = self else { return }
                self.loading.onNext(false)
                guard response.code == 400, !(response.success ?? false) else {
                    self.showSignatureView.onNext("Firma del  QFB")
                    return
                }
                guard let errors = response.response, errors.count > 0 else { return }
                var messageConcat = ""
                for error in errors {
                    if error.type == .some(.batches) && error.listItems?.count ?? 0 > 0 {
                        messageConcat += "No es posible Terminar, faltan lotes para: "
                        messageConcat += "\n"
                        messageConcat += error.listItems?.joined(separator: ", ") ?? ""
                        messageConcat += "\n\n"
                    } else if error.type == .some(.stock) && error.listItems?.count ?? 0 > 0 {
                        messageConcat += "No es posible Terminar, falta existencia para: "
                        messageConcat += "\n"
                        messageConcat += error.listItems?.joined(separator: ", ") ?? ""
                    }
                }
                self.showMessage.onNext(messageConcat)
            }, onError: { [weak self] _ in

                guard let self = self else { return }
                self.loading.onNext(false)
                self.showMessage.onNext("Error")

            }).disposed(by: disposeBag)
    }
    // Valida si el usuario obtuvo las firmas y finaliza la orden 
    func callFinishOrderService() {
        if self.technicalSignatureIsGet && self.qfbSignatureIsGet {
            self.loading.onNext(true)
            let finishOrder = FinishOrder(
                userId: Persistence.shared.getUserData()!.id!, fabricationOrderId: self.orderId,
                qfbSignature: self.sqfbSignature, technicalSignature: technicalSignature)
            self.networkManager.finishOrder(order: finishOrder).subscribe(onNext: { [weak self] _ in
                self?.loading.onNext(false)
                self?.backToInboxView.onNext(())
                self?.rootViewModel.needsRefresh = true
                }, onError: {[weak self] error in
                    self?.loading.onNext(false)
                    self?.showMessage.onNext("Ocurrió un error al finalizar la orden, por favor intentarlo de nuevo")
                    print(error.localizedDescription)
            }).disposed(by: self.disposeBag)
        }
    }
    // Se actualiza order detail para obtener los comentarios
    func updateOrderDetail() {
        loading.onNext(true)
        self.networkManager.getOrdenDetail(orderId: self.orderId)
            .observeOn(MainScheduler.instance).subscribe(onNext: {[weak self] res in
            self?.loading.onNext(false)
            if res.response != nil {
                self?.updateComments.onNext(res.response!)
            }
            self?.orderDetail.needsRefresh = true
        }, onError: { [weak self] error in
            self?.loading.onNext(false)
            self?.showMessage.onNext("Hubo un error al cargar el detalle de la orden de fabricación, intentar de nuevo")
            print(error.localizedDescription)
        }).disposed(by: self.disposeBag)
    }
    func changeOrderToPendingStatus() {
        self.loading.onNext(true)
        let orderToChageStatus = ChangeStatusRequest(
            userId: Persistence.shared.getUserData()!.id!,
            orderId: self.orderId, status: CommonStrings.pending)
        self.networkManager.changeStatusOrder(changeStatusRequest: [orderToChageStatus])
            .subscribe(onNext: { [weak self] _ in
            self?.loading.onNext(false)
            self?.backToInboxView.onNext(())
            self?.rootViewModel.needsRefresh = true
            }, onError: { [weak self] error in
                self?.loading.onNext(false)
                self?.showMessage.onNext(CommonStrings.errorToChangeStatus)
                print(error.localizedDescription)
        }).disposed(by: self.disposeBag)
    }
    // MARK: - Function Helpers
    func calculateExpiredBatch(date: String?) -> Bool {
        let date = date?.replacingOccurrences(of: "\"", with: "", options: String.CompareOptions.literal, range: nil)
        if let date = date {
            let formatter = DateFormatter()
            formatter.dateFormat = "dd/MM/yyyy"
            if let dateFormatter = formatter.date(from: date) {
                let roundedToday = Calendar.current.date(bySettingHour: 0, minute: 0, second: 0, of: Date())
                let roundedDate = Calendar.current.date(bySettingHour: 0, minute: 0, second: 0, of: dateFormatter)
                if roundedDate ?? Date() <= roundedToday ?? Date() {
                    return true
                }
            }
        }
        return false
    }
}

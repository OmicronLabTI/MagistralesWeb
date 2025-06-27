//
//  LotsViewModelExtension.swift
//  Omicron
//
//  Created by Sergio Flores Ramirez on 28/03/22.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift

extension LotsViewModel {
    func getLots() {
        self.loading.onNext(true)
        self.networkManager.getLots(orderId).observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] data in
            guard let self = self else { return }
            self.loading.onNext(false)
            if let lotsData = data.response {
                if lotsData.count > 0 {
                    self.documentLines = lotsData
                    self.selectedBatches = lotsData.map({ batch in
                        let selected: [BatchSelected] = batch.lotesSelecionados != nil ?
                            batch.lotesSelecionados!.compactMap({ sel in
                            return BatchSelected(
                                orderId: self.orderId, assignedQty: sel.cantidadSeleccionada,
                                batchNumber: sel.numeroLote, itemCode: batch.codigoProducto,
                                action: nil, sysNumber: sel.sysNumber,
                                expiredBatch: sel.expiredBatch, areBatchesComplete: 0)
                        }) : []
                        return selected
                    }).reduce([], +)
                    for lotData in lotsData {
                        for lot in lotData.lotesDisponibles ?? [] {
                            lot.cantidadSeleccionada = min(lotData.totalNecesario ?? 0, lot.cantidadDisponible ?? 0)
                        }
                    }
                    self.dataOfLots.onNext(lotsData)
                } else {
                    self.showMessage.onNext(CommonStrings.noBatchesAssigned)
                }
                self.changeColorLabels.onNext(())
            }
            }, onError: { [weak self] error in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.showMessage.onNext(Constants.Errors.loadBatches.rawValue)
                print(error.localizedDescription)
        }).disposed(by: self.disposeBag)
    }

    // Valida si el usuario obtuvo las firmas y finaliza la orden
    func callFinishOrderService() {
        if self.rootViewModel.getShowTwoSignatureModals([self.orderId]) {
            finishOrderWithoutTechnical()
        } else {
            finishOrderWithTechnical()
        }
    }

    func finishOrderWithoutTechnical() {
        if self.technicalSignatureIsGet && self.qfbSignatureIsGet {
            finishOrderService(qfbSignature: self.sqfbSignature,
                               technicalSignature: technicalSignature)
        }
    }
    func finishOrderWithTechnical() {
        if self.qfbSignatureIsGet {
            finishOrderService(qfbSignature: self.sqfbSignature,
                               technicalSignature: "")
        }
    }
    func finishOrderService(qfbSignature: String, technicalSignature: String) {
        self.loading.onNext(true)
        let finishOrder = FinishOrder(userId: Persistence.shared.getUserData()?.id ?? String(),
                                      fabricationOrderId: [self.orderId],
                                      qfbSignature: qfbSignature,
                                      technicalSignature: technicalSignature)
        self.networkManager.finishOrder(finishOrder)
            .subscribe(onNext: { [weak self] res in
                guard let self = self else { return }
                if res.code != 200 {
                    self.loading.onNext(false)
                    self.showMessage.onNext(res.userError ?? "")
                    return
                }
                self.loading.onNext(false)
                self.backToInboxView.onNext(())
                self.rootViewModel.needsRefresh = true
            }, onError: {[weak self] error in
                self?.loading.onNext(false)
                self?.showMessage.onNext(CommonStrings.errorFinishOrder)
                print(error.localizedDescription)
        }).disposed(by: self.disposeBag)
    }
    // Pregunta al server si la orden puede ser finaliada o no
    func validIfOrderCanBeFinalized() {
        self.loading.onNext(true)

        networkManager.validateOrders([orderId])
            .subscribe(onNext: { [weak self] response in
                guard let self = self else { return }
                self.loading.onNext(false)
                guard response.code == 400, !(response.success ?? false) else {
                    self.askIfUserWantToFinalizeOrder.onNext(CommonStrings.finishOrderMessage)
                    return
                }
                guard let errors = response.response, errors.count > 0 else { return }
                var messageConcat = String()
                for error in errors where error.listItems?.count ?? 0 > 0 {
                    let data = UtilsManager.shared.getValidationData(type: error.type ?? .signature)
                    let messageError = UtilsManager.shared.buildMessageError(error: error,
                                                                             message: data.error,
                                                                             lastSeparator: data.separator)
                    messageConcat = "\(messageConcat) \(messageError)"
                }
                self.showMessage.onNext(messageConcat)
            }, onError: { [weak self] _ in

                guard let self = self else { return }
                self.loading.onNext(false)
                self.showMessage.onNext(Constants.Errors.errorData.rawValue)

            }).disposed(by: disposeBag)
    }

    func changeOrderToPendingStatus() {
        self.loading.onNext(true)
        let orderToChageStatus = ChangeStatusRequest(
            userId: Persistence.shared.getUserData()?.id ?? String(),
            orderId: self.orderId, status: CommonStrings.pending, userType: rootViewModel.userType.rawValue)
        self.networkManager.changeStatusOrder([orderToChageStatus])
            .subscribe(onNext: { [weak self] res in
                guard let self = self else { return }
                if res.code == 200 {
                    self.loading.onNext(false)
                    self.backToInboxView.onNext(())
                    self.rootViewModel.needsRefresh = true
                    return
                }
                self.loading.onNext(false)
                self.showMessage.onNext(res.userError ?? CommonStrings.errorToChangeStatus)
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.showMessage.onNext(CommonStrings.errorToChangeStatus)
            }).disposed(by: self.disposeBag)
    }

    func sendToServerAssignedLots(lotsToSend: [BatchSelected]) {
        self.loading.onNext(true)
        self.networkManager.assignLots(lotsToSend)
            .subscribe(onNext: { [weak self] res in
            guard let self = self else { return }
            self.loading.onNext(false)
            if res.response!.isEmpty {
                self.showMessage.onNext(CommonStrings.processSuccess)
                // actualiza la pantalla
                self.orderDetail.needsRefresh = true
                self.getLots()
                return
            }
            var badBatches = ""
            for batch in res.response! {
                badBatches += "\n\(batch)"
            }
                self.showMessage.onNext("\(Constants.Errors.assignedBatches.rawValue) \(badBatches)")
            }, onError: { [weak self] error in
                self?.loading.onNext(false)
                self?.showMessage.onNext(Constants.Errors.assignedBatchesTryAgain.rawValue)
                print(error.localizedDescription)
        }).disposed(by: self.disposeBag)
    }

    // Se actualiza order detail para obtener los comentarios
    func updateOrderDetail() {
        loading.onNext(true)
        self.networkManager.getOrdenDetail(self.orderId)
            .subscribe(onNext: {[weak self] res in
                guard let self = self else { return }
                self.loading.onNext(false)
                if res.response != nil {
                    self.updateComments.onNext(res.response!)
                }
                self.orderDetail.needsRefresh = true
            }, onError: { [weak self] error in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.showMessage.onNext(Constants.Errors.loadOrdersDetail.rawValue)
                print(error.localizedDescription)
            }).disposed(by: self.disposeBag)
    }

    func getFilteredSelected(itemCode: String?) -> [LotsSelected] {
        return self.selectedBatches
            .filter({ $0.itemCode == itemCode && $0.action != Actions.delete.rawValue })
            .map({ $0.toLotsSelected() })
    }
    func getFilteredSelected(itemCode: String?, batchNumber: String?) -> [LotsSelected] {
        return self.selectedBatches
            .filter({ $0.itemCode == itemCode
                        && $0.batchNumber == batchNumber && $0.action != Actions.delete.rawValue })
            .map({ $0.toLotsSelected() })
    }
}

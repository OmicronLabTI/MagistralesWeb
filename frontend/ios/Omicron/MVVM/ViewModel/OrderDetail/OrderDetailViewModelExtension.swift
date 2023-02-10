//
//  OrderDetailViewModelExtension.swift
//  Omicron
//
//  Created by Daniel Velez on 27/01/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift

extension OrderDetailViewModel {
    func setComments(order: OrderDetail) {
        var iconName = ImageButtonNames.message
        if let comments = order.comments {
           iconName = (comments.trimmingCharacters(in: .whitespaces).isEmpty ?
                       ImageButtonNames.message : ImageButtonNames.messsageFill)
        }
        showIconComments.onNext(iconName)
    }

    func terminateOrChangeStatusOfAnOrder(actionType: String) {
        switch actionType {
        case StatusNameConstants.finishedStatus:        // Realiza el proceso para terminar la orden
            self.validIfOrderCanBeFinalized(orderId: orderId)
        case StatusNameConstants.inProcessStatus:       // Realiza el proceso para cambiar el estatus a proceso
            self.changeStatus(actionType: actionType)
        case StatusNameConstants.penddingStatus:        // Realiza el proceso para cambiar el status a pendiente
            self.changeStatus(actionType: actionType)
        default:
            break
        }
    }
    func sum(tableDetails: [Detail]) -> Double {
        guard tableDetails.count > 0 else { return 0.0 }
        return tableDetails.filter({
            $0.unit != CommonStrings.piece &&
            $0.unit != CommonStrings.millar &&
            $0.unit != CommonStrings.gameUnit
        }).map({$0.requiredQuantity ?? 0.0}).reduce(0.0, +)
    }

    func changeStatus(actionType: String) {
        self.loading.onNext(true)
        let status = actionType == StatusNameConstants.inProcessStatus ? CommonStrings.process : CommonStrings.pending
        let changeStatus = ChangeStatusRequest(
            userId: (Persistence.shared.getUserData()?.id) ?? String(),
            orderId: (self.tempOrderDetailData?.productionOrderID) ?? 0,
            status: status,
            userType: rootViewModel.userType.rawValue)
        self.networkManager.changeStatusOrder([changeStatus])
            .observeOn(MainScheduler.instance).subscribe(onNext: {[weak self] _ in
            self?.rootViewModel.needsRefresh = true
            self?.loading.onNext(false)
            self?.backToInboxView.onNext(())
        }, onError: { [weak self] error in
            self?.loading.onNext(false)
            self?.showAlert.onNext(CommonStrings.errorToChangeStatus)
            print(error.localizedDescription)
            }).disposed(by: self.disposeBag)
    }

    func addIndexDeleteTable(index: Int) {
        let existIndex = itemSelectedDetail.firstIndex(where: { $0 == index })
        if existIndex == nil {
            if itemSelectedDetail.count + 1 != auxTabledata.count {
                itemSelectedDetail.append(index)
            }
        } else {
            itemSelectedDetail.remove(at: existIndex!)
        }
        self.deleteManyButtonIsEnable.onNext(itemSelectedDetail.count>0)
    }

    func indexDeleteExist(_ index: Int) -> Bool {
        itemSelectedDetail.firstIndex(where: { $0 == index }) != nil
    }

    func deleteItemFromTable(indexs: [Int]) {
        self.loading.onNext(true)
        let itemsToDelete = indexs.map { index in
            auxTabledata[index]
        }
       let componets = itemsToDelete.map({ itemToDelete in
            Component(
                orderFabID: itemToDelete.orderFabID ?? 0,
                productId: itemToDelete.productID ?? String(),
                componentDescription: itemToDelete.detailDescription ?? String(),
                baseQuantity: itemToDelete.baseQuantity ?? 0.0,
                requiredQuantity: itemToDelete.requiredQuantity ?? 0.0,
                consumed: itemToDelete.consumed ?? 0.0, available: itemToDelete.available ?? 0.0,
                unit: itemToDelete.unit ?? String(), warehouse: itemToDelete.warehouse ?? String(),
                pendingQuantity: itemToDelete.pendingQuantity ?? 0.0,
                stock: itemToDelete.stock ?? 0.0,
                warehouseQuantity: itemToDelete.warehouseQuantity ?? 0.0,
                action: Actions.delete.rawValue)
        })
        let fechaFinFormated = UtilsManager.shared.formattedDateFromString(
            dateString: tempOrderDetailData?.dueDate ?? String(), withFormat: DateFormat.yyyymmdd)
        let order = OrderDetailRequest(
            fabOrderID: tempOrderDetailData?.productionOrderID ?? 0,
            plannedQuantity: tempOrderDetailData?.plannedQuantity ?? Decimal(0),
            fechaFin: fechaFinFormated ?? String(), comments: String(),
            warehouse: tempOrderDetailData?.warehouse ?? String(), components: componets)
        self.networkManager.updateDeleteItemOfTableInOrderDetail(order)
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            if self?.tempOrderDetailData != nil {
                self?.loading.onNext(false)
                self?.removeOfDetailsIndexs(indexs)
                self?.itemSelectedDetail = []
                self?.deleteManyButtonIsEnable.onNext(false)
                self?.auxTabledata = self?.tempOrderDetailData?.details ?? []
                self?.tableData.onNext(self?.tempOrderDetailData?.details ?? [])
                self?.sumFormula.accept(self?.sum(tableDetails: (self?.tempOrderDetailData?.details ?? [])) ?? 0.0)
            }
            }, onError: {  [weak self] error in
                self?.loading.onNext(false)
                self?.showAlert.onNext(CommonStrings.couldNotDeleteItem)
                print(error.localizedDescription)
        }).disposed(by: self.disposeBag)
    }

    func removeOfDetailsIndexs(_ indexs: [Int]) {
        self.tempOrderDetailData?.details =  self.tempOrderDetailData?.details?.enumerated()
            .filter { !indexs.contains($0.offset) }
            .map { $0.element }
    }

    func getDataTableToEdit() -> OrderDetail {
        return self.tempOrderDetailData!
    }
    // Valida si el usuario obtuvo las firmas y finaliza la orden
    func validSignatures() {
        if rootViewModel.requireTechnical {
            finishOrderWithTechnical()
        } else {
            finishOrderWithoutTechnical()
        }
    }
    func finishOrderWithTechnical() {
        if self.qfbSignatureIsGet {
            finishOrderService(qfbSignature: self.sqfbSignature,
                               technicalSignature: "")
        }
    }
    func finishOrderWithoutTechnical() {
        if self.technicalSignatureIsGet && self.qfbSignatureIsGet {
            finishOrderService(qfbSignature: self.sqfbSignature,
                               technicalSignature: self.technicalSignature)
        }
    }
    func finishOrderService(qfbSignature: String, technicalSignature: String) {
        self.loading.onNext(true)
        let finishOrder = FinishOrder(userId: Persistence.shared.getUserData()?.id ?? String(),
                                      fabricationOrderId: [self.orderId],
                                      qfbSignature: qfbSignature,
                                      technicalSignature: technicalSignature)
        self.networkManager.finishOrder(finishOrder)
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] res in
                guard let self = self else { return }
                if res.code != 200 {
                    self.loading.onNext(false)
                    self.showAlert.onNext(res.userError ?? "")
                    return
                }
                self.loading.onNext(false)
                self.backToInboxView.onNext(())
                self.rootViewModel.needsRefresh = true
            }, onError: { [weak self] _ in
                self?.loading.onNext(false)
            }).disposed(by: self.disposeBag)
    }
    func validIfOrderCanBeFinalized(orderId: Int) {
        self.loading.onNext(true)
        networkManager.validateOrders([orderId])
            .subscribe(onNext: { [weak self] response in
                guard let self = self else { return }
                self.loading.onNext(false)
                guard response.code == 400, !(response.success ?? false) else {
                    self.showSignatureView.onNext(CommonStrings.signatureViewTitleQFB)
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
                self.showAlert.onNext(messageConcat)
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.showAlert.onNext(Constants.Errors.errorData.rawValue)
            }).disposed(by: disposeBag)
    }
}

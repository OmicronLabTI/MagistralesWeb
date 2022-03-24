//
//  InboxViewModelExtension.swift
//  Omicron
//
//  Created by Sergio Flores Ramirez on 24/03/22.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift

extension InboxViewModel {

    func postOrderPDf(orders: [Int], needsError: Bool = false, statusCode: Int = 500, testData: Data = Data()) {
        networkManager.postOrdersPDF(
            orders: orders, needsError: needsError, statusCode: statusCode, testData: testData)
            .subscribe(onNext: { [weak self] response in
            guard let self = self, response.response?.count ?? 0 > 0 else { return }
            self.loading.onNext(false)
            self.orderURLPDF.onNext(response.response!.first!)
        }, onError: { [weak self] error in
            guard let self = self else { return }
            print(error.localizedDescription)
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorPDF)
        }).disposed(by: disposeBag)
    }

    func changeStatusService(_ orders: [ChangeStatusRequest], _ needsError: Bool, _ statusCode: Int, _ testData: Data) {
        networkManager.changeStatusOrder(
            changeStatusRequest: orders, needsError: needsError,
            statusCode: statusCode, testData: testData)
            .observeOn(MainScheduler.instance).subscribe(onNext: {[weak self] _ in
                guard let self = self else { return }
                self.processButtonIsEnable.onNext(false)
                self.pendingButtonIsEnable.onNext(false)
                self.rootViewModel.needsRefresh = true
                self.loading.onNext(false)
                self.refreshDataWhenChangeProcessIsSucces.onNext(())
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.showAlert.onNext(CommonStrings.errorToChangeStatus)
                self.processButtonIsEnable.onNext(true)
        }).disposed(by: self.disposeBag)
    }

    func getConnection(needsError: Bool = false, statusCode: Int = 500, testData: Data = Data()) {

        self.loading.onNext(true)
        networkManager.getConnect(needsError: needsError,
                                  statusCode: statusCode, testData: testData)
            .subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.hasConnection.onNext(true)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.hasConnection.onNext(false)
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorPDF)
        }).disposed(by: disposeBag)

    }

    func finisOrderService(_ finishOrder: FinishOrder, _ needsError: Bool, _ statusCode: Int, _ testData: Data) {
        networkManager.finishOrder(
            order: finishOrder, needsError: needsError, statusCode: statusCode, testData: testData)
            .subscribe(onNext: { [weak self] _ in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.isUserInteractionEnabled.onNext(true)
                self.refreshDataWhenChangeProcessIsSucces.onNext(())
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.showAlert.onNext(CommonStrings.errorFinishOrders)
            }).disposed(by: disposeBag)
    }

    func validateOrdersService(_ orderIds: [Int], _ needsError: Bool, _ statusCode: Int, _ testData: Data) {
        networkManager.validateOrders(
            orderIDs: orderIds, needsError: needsError, statusCode: statusCode, testData: testData)
            .subscribe(onNext: { [weak self] response in
            guard let self = self else { return }
            self.loading.onNext(false)
            guard response.code == 400, !(response.success ?? false) else {
                self.showSignatureVc.onNext(CommonStrings.signatureViewTitleQFB)
                return
            }
            guard let errors = response.response, errors.count > 0 else { return }
            var messageConcat = String()
            for error in errors {
                if error.type == .some(.batches) && error.listItems?.count ?? 0 > 0 {
                    messageConcat = UtilsManager.shared.messageErrorWhenNoBatches(error: error)
                } else if error.type == .some(.stock) && error.listItems?.count ?? 0 > 0 {
                    let errorMessage = UtilsManager.shared.messageErrorWhenOutOfStock(error: error)
                    messageConcat = "\(messageConcat) \(errorMessage)"
                }
            }
            self.showAlert.onNext(messageConcat)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.showAlert.onNext(Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag)
    }

    func getStatusName(index: Int) -> String {
        switch index {
        case 0: return StatusNameConstants.assignedStatus
        case 1: return StatusNameConstants.inProcessStatus
        case 2: return StatusNameConstants.penddingStatus
        case 3: return StatusNameConstants.finishedStatus
        case 4: return StatusNameConstants.reassignedStatus
        default: return CommonStrings.empty
        }
    }

    func getStatusId(name: String) -> Int {
        switch name {
        case StatusNameConstants.assignedStatus: return 0
        case StatusNameConstants.inProcessStatus: return 1
        case StatusNameConstants.penddingStatus: return 2
        case StatusNameConstants.finishedStatus: return 3
        case StatusNameConstants.reassignedStatus: return 4
        default: return -1
        }
    }

    func changeStatusSort(normal: Bool, similarity: Bool, grouped: Bool) {
        if normal {
            normalSort = true
            similaritySort = false
            groupSort = false
        } else if similarity {
            normalSort = false
            similaritySort = true
            groupSort = false
        } else {
            normalSort = false
            similaritySort = false
            groupSort = true
        }
    }

    func callFinishOrderService(needsError: Bool = false, statusCode: Int = 500, testData: Data = Data()) {
        if qfbSignatureIsGet && technicalSignatureIsGet {
            loading.onNext(true)
            guard let userID = Persistence.shared.getUserData()?.id,
                  let indexPathOfOrdersSelected = indexPathOfOrdersSelected else { return }
            let orderIds = getFabOrderIDs(indexPathOfOrdersSelected: indexPathOfOrdersSelected)
            let finishOrder = FinishOrder(
                userId: userID, fabricationOrderId: orderIds, qfbSignature: sqfbSignature,
                technicalSignature: technicalSignature)

            finisOrderService(finishOrder, needsError, statusCode, testData)
        }
    }

    func validOrders(
        indexPathOfOrdersSelected: [IndexPath]?, needsError: Bool = false,
        statusCode: Int = 500, testData: Data = Data()) {
            loading.onNext(true)
            self.indexPathOfOrdersSelected = indexPathOfOrdersSelected
            guard let indexPathOfOrdersSelected = indexPathOfOrdersSelected else { return }
            let orderIds = getFabOrderIDs(indexPathOfOrdersSelected: indexPathOfOrdersSelected)
            validateOrdersService(orderIds, needsError, statusCode, testData)
        }

    func getFabOrderIDs(indexPathOfOrdersSelected: [IndexPath]) -> [Int] {
        guard indexPathOfOrdersSelected.count > 0 else { return []}
        var fabOrderIDs = [Int]()
        indexPathOfOrdersSelected.forEach { [weak self] (indexPath) in
            let orderId = self?.sectionOrders[indexPath.section].items[indexPath.row].productionOrderId
            fabOrderIDs.append(orderId ?? 0)
        }
        return fabOrderIDs
    }

    // Cambia el estatus de una orden a proceso o pendiente
    func changeStatus(indexPath: [IndexPath]?, typeOfStatus: String,
                      needsError: Bool = false, statusCode: Int = 500, testData: Data = Data()) {
        self.loading.onNext(true)
        var status = CommonStrings.empty
        switch typeOfStatus {
        case StatusNameConstants.inProcessStatus:
            status = CommonStrings.process
        case StatusNameConstants.penddingStatus:
            status = CommonStrings.pending
        default:
            status = CommonStrings.empty
        }
        if status.isEmpty {
            self.showAlert.onNext(CommonStrings.errorToChangeStatus)
            return
        }
        // Obtiene las ordenes a cambialas de status mediante el indexPath
        var orders: [ChangeStatusRequest] = []
        for index in indexPath ?? [] {
            let card = self.sectionOrders[index.section].items[index.row]
            let order = ChangeStatusRequest(
                userId: (Persistence.shared.getUserData()?.id) ?? CommonStrings.empty,
                orderId: card.productionOrderId ?? 0, status: status)
            orders.append(order)
        }
        changeStatusService(orders, needsError, statusCode, testData)
    }

}

//
//  InboxViewModelExtension.swift
//  Omicron
//
//  Created by Sergio Flores Ramirez on 24/03/22.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxDataSources

extension InboxViewModel {

    func postOrderPDf(orders: [Int]) {
        networkManager.postOrdersPDF(orders)
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

    func changeStatusService(_ orders: [ChangeStatusRequest]) {
        networkManager.changeStatusOrder(orders)
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

    func downloadPDF(_ ordersId: [Int]) {
        self.loading.onNext(true)
        networkManager.getConnect()
            .subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
                self.postOrderPDf(orders: ordersId)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorPDF)
        }).disposed(by: disposeBag)
    }

    func finisOrderService(_ finishOrder: FinishOrder) {
        networkManager.finishOrder(finishOrder)
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

    func validateOrdersService(_ orderIds: [Int]) {
        networkManager.validateOrders(orderIds)
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
        case 1: return StatusNameConstants.assignedStatus
        case 2: return StatusNameConstants.inProcessStatus
        case 3: return StatusNameConstants.penddingStatus
        case 4: return StatusNameConstants.finishedStatus
        case 5: return StatusNameConstants.reassignedStatus
        default: return CommonStrings.empty
        }
    }

    func getStatusId(name: String) -> Int {
        switch name {
        case StatusNameConstants.assignedStatus: return 1
        case StatusNameConstants.inProcessStatus: return 2
        case StatusNameConstants.penddingStatus: return 3
        case StatusNameConstants.finishedStatus: return 4
        case StatusNameConstants.reassignedStatus: return 5
        default: return -1
        }
    }

    func changeStatusSort(_ type: ShortType) {
        shortType = type
        normalViewButtonIsEnable.onNext(true)
        similarityViewButtonIsEnable.onNext(true)
        groupedByOrderNumberIsEnable.onNext(true)
        groupedByShopTransactionIsEnable.onNext(true)
        switch shortType {
        case .normal:
            processNormalSort()
        case .similarity:
            processSimilaritySort()
        case .groupSort:
            proccessGroupShort()
        case.shopTransaction:
            processShopTransaction()
        }
    }

    func callFinishOrderService() {
        if qfbSignatureIsGet && technicalSignatureIsGet {
            loading.onNext(true)
            guard let userID = Persistence.shared.getUserData()?.id,
                  let indexPathOfOrdersSelected = indexPathOfOrdersSelected else { return }
            let orderIds = getFabOrderIDs(indexPathOfOrdersSelected: indexPathOfOrdersSelected)
            let finishOrder = FinishOrder(
                userId: userID, fabricationOrderId: orderIds, qfbSignature: sqfbSignature,
                technicalSignature: technicalSignature)

            finisOrderService(finishOrder)
        }
    }

    func validOrders(
        indexPathOfOrdersSelected: [IndexPath]?) {
            loading.onNext(true)
            self.indexPathOfOrdersSelected = indexPathOfOrdersSelected
            guard let indexPathOfOrdersSelected = indexPathOfOrdersSelected else { return }
            let orderIds = getFabOrderIDs(indexPathOfOrdersSelected: indexPathOfOrdersSelected)
            validateOrdersService(orderIds)
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
    func changeStatus(indexPath: [IndexPath]?, typeOfStatus: String) {
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
                orderId: card.productionOrderId ?? 0, status: status, userType: rootViewModel.userType.rawValue)
            orders.append(order)
        }
        changeStatusService(orders)
    }

    func proccessGroupShort() {
        let ordersGroupedAndSorted = sortOrderWithBatchesByOrderNumberView()
        sectionOrders = ordersGroupedAndSorted
        statusDataGrouped.onNext(ordersGroupedAndSorted)
        groupedByOrderNumberIsEnable.onNext(false)
        processButtonIsEnable.onNext(false)
    }

    func orderingOrders(section: SectionOrder) -> [Order] {
        var ordering: [Order] = []
        if section.statusName == StatusNameConstants.inProcessStatus ||
            section.statusName == StatusNameConstants.reassignedStatus {
            let ordersReadyToFinish =
                sortByBaseBocumentAscending(orders: section.orders.filter({ $0.areBatchesComplete == true }))
            let ordersNotReadyToFinish =
                sortByBaseBocumentAscending(orders: section.orders.filter({ $0.areBatchesComplete == false }))
            ordering.append(contentsOf: ordersReadyToFinish)
            ordering.append(contentsOf: ordersNotReadyToFinish)
        } else {
            ordering = sortByBaseBocumentAscending(orders: section.orders)
        }
        return ordering
    }

    func setFilter(orders: [Order]) {
        let ordering = self.sortByBaseBocumentAscending(orders: orders)
        self.statusDataGrouped.onNext([SectionModel(model: CommonStrings.empty, items: ordering)])
        self.sectionOrders = [SectionModel(model: CommonStrings.empty, items: ordering)]
        self.title.onNext(CommonStrings.search)
        self.ordersTemp = ordering
    }
    func sortByBaseBocumentAscending(orders: [Order]) -> [Order] {
        orders.sorted {
            switch ($0, $1) {
            case let (aCode, bCode):
                if aCode.baseDocument ?? 0 != bCode.baseDocument ?? 0 {
                    return aCode.baseDocument ?? 0 < bCode.baseDocument ?? 0
                } else {
                    return aCode.productionOrderId ?? 0 < bCode.productionOrderId ?? 0
                }
            }
        }
    }

    func sortByShopTransaction(orders: [Order], shopTransaction: String) -> [Order] {
        orders.filter { order in
            order.shopTransaction == shopTransaction
        }
    }

    func processNormalSort() {
        let ordering = self.sortOrderWithOrderBatchesCompleteByNormalView()
        statusDataGrouped.onNext([SectionModel(model: CommonStrings.empty, items: ordering)])
        sectionOrders = [SectionModel(model: CommonStrings.empty, items: ordering)]
        normalViewButtonIsEnable.onNext(false)
        processButtonIsEnable.onNext(false)
        pendingButtonIsEnable.onNext(false)
    }

    func processShopTransaction() {
            let ordering = self.sortOrderShopTransactionView()
            sectionOrders = ordering
            statusDataGrouped.onNext(ordering)
        processButtonIsEnable.onNext(false)
        pendingButtonIsEnable.onNext(false)
        groupedByShopTransactionIsEnable.onNext(false)
    }

    func processSimilaritySort() {
        for order in ordersTemp {
            let itemCodeInArray = order.itemCode?.components(separatedBy: CommonStrings.separationSpaces)
            if let codeProduct = itemCodeInArray?.first {
                order.productCode = codeProduct
            } else {
                order.productCode = CommonStrings.empty
            }
        }
        // Se agrupa las ordenes por código de producto
        let dataGroupedByProductCode = Dictionary(grouping: ordersTemp, by: {$0.productCode})
        let sectionModels = groupedWithSimilarityOrWithoutSimilarity(
            data: dataGroupedByProductCode, titleForOrdersWithoutSimilarity: CommonStrings.noSimilarity,
            titleForOrdersWithSimilarity: CommonStrings.product)
        sectionOrders = sectionModels
        statusDataGrouped.onNext(sectionModels)
        similarityViewButtonIsEnable.onNext(false)
    }

    func groupedByOrderNumber(data: [String?: [Order]]) -> [SectionModel<String, Order>] {
        var data1 = data
        var sectionModels: [SectionModel<String, Order>] = []
        resetData.onNext(())
        if let cero = data1[CommonStrings.zero] {
            sectionModels.append(SectionModel(model: "\(CommonStrings.ordersWithoutOrder)", items: cero))
            data1.removeValue(forKey: CommonStrings.zero)
        }
        let sections = data1.map({ [unowned self] (orders) -> SectionModel<String, Order> in
            return SectionModel(
                model: "\(CommonStrings.order) \(orders.key ?? CommonStrings.empty)",
                items: self.sortByBaseBocumentAscending(orders: orders.value))
        })
        let sortedSections = sections.sorted { $0.model < $1.model }
        sectionModels.append(contentsOf: sortedSections)
        return sectionModels
    }

    func groupedByShopTransaction(data: [String?: [Order]]) -> [SectionModel<String, Order>] {
        var sectionModels: [SectionModel<String, Order>] = []
        let sections = data.map({ [unowned self] (orders) -> SectionModel<String, Order> in
            return SectionModel(
                model: "\(CommonStrings.shopTransaction): \(orders.key?.suffix(6).uppercased() ?? "")",
                items: self.sortByShopTransaction(orders: orders.value, shopTransaction: orders.key ?? ""))
        }).filter({ $0.items.count > 0 })
        let sortedSections = sections.sorted { $0.model < $1.model }
        sectionModels.append(contentsOf: sortedSections)
        return sectionModels
    }

    func setSelection(section: SectionOrder, removeSelecteds: Bool = false) {
        currentSection = section
        let ordering = orderingOrders(section: section)
        ordersTemp = ordering
        if removeSelecteds {
            resetData.onNext(())
        }
        self.changeStatusSort(shortType)
        title.onNext(section.statusName)
        showKPIView.onNext(false)
        reloadData.onNext(())
    }
}

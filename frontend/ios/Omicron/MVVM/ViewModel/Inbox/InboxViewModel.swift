//
//  InboxViewModel.swift
//  Omicron
//
//  Created by Axity on 24/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa
import RxDataSources
import Resolver
import SwiftUI

class InboxViewModel {
    var finishedDidTap = PublishSubject<Void>()
    var pendingDidTap = PublishSubject<Void>()
    var processDidTap = PublishSubject<Void>()
    var indexSelectedOfTable = PublishSubject<Int>()
    var statusDataGrouped: BehaviorSubject<[SectionModel<String, Order>]> = BehaviorSubject(value: [])
    var loading =  PublishSubject<Bool>()
    var showAlertToChangeOrderOfStatus = PublishSubject<MessageToChangeStatus>()
    var refreshDataWhenChangeProcessIsSucces = PublishSubject<Void>()
    var showAlert = PublishSubject<String>()
    var title = PublishSubject<String>()
    var similarityViewButtonDidTap = PublishSubject<Void>()
    var similarityViewButtonIsEnable = PublishSubject<Bool>()
    var normalViewButtonDidTap = PublishSubject<Void>()
    var normalViewButtonIsEnable = PublishSubject<Bool>()
    var processButtonIsEnable = PublishSubject<Bool>()
    var pendingButtonIsEnable = PublishSubject<Bool>()
    var hideGroupingButtons = PublishSubject<Bool>()
    var groupByOrderNumberButtonDidTap = PublishSubject<Void>()
    var groupedByOrderNumberIsEnable = PublishSubject<Bool>()
    var showKPIView = PublishSubject<Bool>()
    var viewKPIDidPressed = PublishSubject<Void>()
    var deselectRow = PublishSubject<Bool>()
    var selectOrder = PublishSubject<Int?>()
    var orderURLPDF = PublishSubject<String>()
    var hasConnection = PublishSubject<Bool>()
    var resetData = PublishSubject<Void>()
    var showSignatureVc = PublishSubject<String>()
    var finishOrders = PublishSubject<Void>()
    var isUserInteractionEnabled = PublishSubject<Bool>()
    var reloadData = PublishSubject<Void>()

    var normalSort = true
    var similaritySort = false
    var groupSort = false
    var qfbSignatureIsGet = false
    var sqfbSignature = String()
    var technicalSignatureIsGet = false
    var technicalSignature = String()
    var selectedOrder: Order?
    var disposeBag = DisposeBag()
    var ordersTemp: [Order] = []
    var sectionOrders: [SectionModel<String, Order>] = []
    var indexPathOfOrdersSelected: [IndexPath]?
    var currentSection: SectionOrder = SectionOrder(
        statusId: 0, statusName: String(), numberTask: 0, imageIndicatorStatus: String(), orders: [])

    @Injected var rootViewModel: RootViewModel
    @Injected var networkManager: NetworkManager

    init() {
        // Funcionalidad para el botón de Pendiente
        pendingDidTapBinding()
        // Funcionalidad para el botón de En Proceso
        processDidTapBinding()
        // Funcionalidad para el botón de Terminado
        finishedDidTapBinding()
        // Funcionalidad para agrupar los cards por similitud
        similarityViewButtonAction()
        // Funcionalidad para mostrar la vista normal en los cards
        normalViewButtonDidTapBinding()
        // Funcionalidad para mostra la vista ordenada número de orden
        groupByOrderNumberButtonDidTapBinding()

        initExtension()
    }

    func pendingDidTapBinding() {
        pendingDidTap.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return}
            let message = MessageToChangeStatus(
                message: CommonStrings.confirmationMessagePendingStatus,
                typeOfStatus: StatusNameConstants.penddingStatus)
            self.showAlertToChangeOrderOfStatus.onNext(message)
        }).disposed(by: disposeBag)
    }

    func processDidTapBinding() {
        processDidTap.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            let message = MessageToChangeStatus(
                message: CommonStrings.confirmationMessageProcessStatus,
                typeOfStatus: StatusNameConstants.inProcessStatus)
            self.showAlertToChangeOrderOfStatus.onNext(message)
        }).disposed(by: disposeBag)
    }

    func finishedDidTapBinding() {
        finishedDidTap.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            let message = MessageToChangeStatus(
                message: CommonStrings.confirmationMessageFinishedStatus,
                typeOfStatus: StatusNameConstants.finishedStatus)
            self.showAlertToChangeOrderOfStatus.onNext(message)
        }).disposed(by: disposeBag)
    }

    func similarityViewButtonAction() {
        similarityViewButtonDidTap.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.processButtonIsEnable.onNext(false)
            self.pendingButtonIsEnable.onNext(false)
            self.resetData.onNext(())
            for order in self.ordersTemp {
                let itemCodeInArray = order.itemCode?.components(separatedBy: CommonStrings.separationSpaces)
                if let codeProduct = itemCodeInArray?.first {
                    order.productCode = codeProduct
                } else {
                    order.productCode = CommonStrings.empty
                }
            }
            // Se agrupa las ordenes por código de producto
            let dataGroupedByProductCode = Dictionary(grouping: self.ordersTemp, by: {$0.productCode})
            let sectionModels = self.groupedWithSimilarityOrWithoutSimilarity(
                data: dataGroupedByProductCode, titleForOrdersWithoutSimilarity: CommonStrings.noSimilarity,
                titleForOrdersWithSimilarity: CommonStrings.product)
            self.sectionOrders = sectionModels
            self.statusDataGrouped.onNext(sectionModels)
            self.similarityViewButtonIsEnable.onNext(false)
            self.normalViewButtonIsEnable.onNext(true)
            self.groupedByOrderNumberIsEnable.onNext(true)
            self.changeStatusSort(normal: false, similarity: true, grouped: false)
        }).disposed(by: self.disposeBag)
    }

    func normalViewButtonDidTapBinding() {
        normalViewButtonDidTap.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.processButtonIsEnable.onNext(false)
            self.pendingButtonIsEnable.onNext(false)
            let ordering = self.sortOrderWithOrderBatchesCompleteByNormalView()
            self.sectionOrders = [SectionModel(model: CommonStrings.empty, items: ordering)]
            self.statusDataGrouped.onNext([SectionModel(model: CommonStrings.empty, items: ordering)])
            self.similarityViewButtonIsEnable.onNext(true)
            self.normalViewButtonIsEnable.onNext(false)
            self.groupedByOrderNumberIsEnable.onNext(true)
            self.changeStatusSort(normal: true, similarity: false, grouped: false)
            self.resetData.onNext(())
        }).disposed(by: self.disposeBag)
    }

    func groupByOrderNumberButtonDidTapBinding() {
        groupByOrderNumberButtonDidTap.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.processButtonIsEnable.onNext(false)
            self.pendingButtonIsEnable.onNext(false)
            let ordersGroupedAndSorted = self.sortOrderWithBatchesByOrderNumberView()
            self.sectionOrders = ordersGroupedAndSorted
            self.statusDataGrouped.onNext(ordersGroupedAndSorted)
            self.normalViewButtonIsEnable.onNext(true)
            self.similarityViewButtonIsEnable.onNext(true)
            self.groupedByOrderNumberIsEnable.onNext(false)
            self.changeStatusSort(normal: false, similarity: false, grouped: true)
            self.resetData.onNext(())
        }).disposed(by: self.disposeBag)
    }

    func initExtension() {
        viewKPIDidPressed.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.showKPIView.onNext(true)
            self.deselectRow.onNext(true)
        }).disposed(by: disposeBag)
        selectOrder.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] orders in
            guard let orders = orders, let self = self else { return }
            self.postOrderPDf(orders: [orders])
        }).disposed(by: disposeBag)
    }

//  Separa las ordenes con lotes completos y las ordena por documento base de forma ascendente para la vista normal
    func sortOrderWithOrderBatchesCompleteByNormalView() -> [Order] {
        var ordering: [Order] = []
        if self.currentSection.statusName == StatusNameConstants.inProcessStatus ||
            self.currentSection.statusName == StatusNameConstants.reassignedStatus {
            let orderReadyToFinished =
                self.sortByBaseBocumentAscending(orders: self.ordersTemp.filter({$0.areBatchesComplete == true}))
            let ordersNotReadyToFinished =
                self.sortByBaseBocumentAscending(orders: self.ordersTemp.filter({$0.areBatchesComplete == false}))
            ordering.append(contentsOf: orderReadyToFinished)
            ordering.append(contentsOf: ordersNotReadyToFinished)
        } else {
            ordering.append(contentsOf: self.sortByBaseBocumentAscending(orders: self.ordersTemp))
        }
        return ordering
    }

//  Separa las ordenes tengan lotes completos y las ordena por documento base para la vista de agrupamiento 
    func sortOrderWithBatchesByOrderNumberView() -> [SectionModel<String, Order>] {
        var ordersGroupedAndSorted: [SectionModel<String, Order>] = []
        var dataGroupedByBaseDocument: [String: [Order]] = [:]
        if self.currentSection.statusName == StatusNameConstants.inProcessStatus ||
            self.currentSection.statusName == StatusNameConstants.reassignedStatus {
            let ordersReadyToFinish =
                Dictionary(grouping:
                            self.ordersTemp
                            .filter({ $0.areBatchesComplete == true}), by: { "\($0.baseDocument ?? 0)" })
            let ordersNotReadyToFinish =
                Dictionary(
                    grouping: self.ordersTemp
                        .filter({ $0.areBatchesComplete == false}), by: { "\($0.baseDocument ?? 0)" })
            ordersGroupedAndSorted.append(contentsOf: self.groupedByOrderNumber(data: ordersReadyToFinish))
            ordersGroupedAndSorted.append(contentsOf: self.groupedByOrderNumber(data: ordersNotReadyToFinish))
        } else {
            dataGroupedByBaseDocument = Dictionary(grouping: self.ordersTemp,
                                                   by: { "\($0.baseDocument ?? 0)" })
            ordersGroupedAndSorted.append(contentsOf: self.groupedByOrderNumber(data: dataGroupedByBaseDocument))
        }
        return ordersGroupedAndSorted
    }

    // Se agrupan ordenes por similitud o sin similitud
    func groupedWithSimilarityOrWithoutSimilarity(
        data: [String? : [Order]], titleForOrdersWithoutSimilarity: String,
        titleForOrdersWithSimilarity: String) -> [SectionModel<String, Order>] {
        var sectionModels: [SectionModel<String, Order>] = []
        // Se extraen las ordenes que contengan más de una coincidencia por código de producto
        // y se agrupan por "Producto: [productCode]"
        resetData.onNext(())
        let groupBySimilarity = data
            .filter { $0.value.count > 1 }
            .sorted { ($0.key ?? CommonStrings.empty)
                .localizedStandardCompare( $1.key ?? CommonStrings.empty) == .orderedAscending }
        if groupBySimilarity.count > 0 {
            let sectionsModelsBySimilarity = groupBySimilarity
                .map({ [unowned self] (orders) -> SectionModel<String, Order> in
                return SectionModel(
                    model: "\(titleForOrdersWithSimilarity) \(orders.key ?? CommonStrings.empty)",
                    items: self.sortByBaseBocumentAscending(orders: orders.value))
            })
            sectionModels.append(contentsOf: sectionsModelsBySimilarity)
        }
        // Se extraen las ordenes que solo contengan una coincidencia
        // por código de producto y agruparlas por "Sin similitud"
        let groupWithoutSimilarity = data.filter { $0.value.count == 1 }
        if groupWithoutSimilarity.count > 0 {
            var orders: [Order] = []
            for order in groupWithoutSimilarity {
                orders.append(contentsOf: order.value)
            }
            let orderedCars = self.sortByBaseBocumentAscending(orders: orders)
            sectionModels.append(SectionModel(model: titleForOrdersWithoutSimilarity, items: orderedCars ))
        }
        return sectionModels
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
    func setSelection(section: SectionOrder, removeSelecteds: Bool = false) {
        currentSection = section
        let ordering = orderingOrders(section: section)
        ordersTemp = ordering
        if removeSelecteds {
            resetData.onNext(())
        }
        if normalSort {
            processNormalSort()
        } else if similaritySort {
            processSimilaritySort()
        } else {
            proccessGroupShort()
        }
        title.onNext(section.statusName)
        showKPIView.onNext(false)
        reloadData.onNext(())
    }

    func processNormalSort() {
        let ordering = self.sortOrderWithOrderBatchesCompleteByNormalView()
        statusDataGrouped.onNext([SectionModel(model: CommonStrings.empty, items: ordering)])
        sectionOrders = [SectionModel(model: CommonStrings.empty, items: ordering)]
        similarityViewButtonIsEnable.onNext(true)
        groupedByOrderNumberIsEnable.onNext(true)
        normalViewButtonIsEnable.onNext(false)
        processButtonIsEnable.onNext(false)
        pendingButtonIsEnable.onNext(false)
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
        normalViewButtonIsEnable.onNext(true)
        groupedByOrderNumberIsEnable.onNext(true)
    }

    func proccessGroupShort() {
        processButtonIsEnable.onNext(false)
        let ordersGroupedAndSorted = sortOrderWithBatchesByOrderNumberView()
        sectionOrders = ordersGroupedAndSorted
        statusDataGrouped.onNext(ordersGroupedAndSorted)
        normalViewButtonIsEnable.onNext(true)
        similarityViewButtonIsEnable.onNext(true)
        groupedByOrderNumberIsEnable.onNext(false)
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
}

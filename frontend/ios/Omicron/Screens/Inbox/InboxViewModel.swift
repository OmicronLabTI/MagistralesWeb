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
// swiftlint:disable type_body_length
class InboxViewModel {
    var finishedDidTap = PublishSubject<Void>()
    var pendingDidTap = PublishSubject<Void>()
    var processDidTap = PublishSubject<Void>()
    var indexSelectedOfTable = PublishSubject<Int>()
    var statusDataGrouped: BehaviorSubject<[SectionModel<String, Order>]> = BehaviorSubject(value: [])
    var ordersTemp: [Order] = []
    var sectionOrders: [SectionModel<String, Order>] = []
    var loading =  PublishSubject<Bool>()
    var showAlertToChangeOrderOfStatus = PublishSubject<MessageToChangeStatus>()
    var refreshDataWhenChangeProcessIsSucces = PublishSubject<Void>()
    var showAlert = PublishSubject<String>()
    var title = PublishSubject<String>()
    var selectedOrder: Order?
    var disposeBag = DisposeBag()
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
    @Injected var rootViewModel: RootViewModel
    @Injected var networkManager: NetworkManager
    var normalSort = true
    var similaritySort = false
    var groupSort = false
    // swiftlent:disable function_body_length
    init() {
        pendingDidTap.subscribe(onNext: { [weak self] _ in
            let message = MessageToChangeStatus(
                message: CommonStrings.confirmationMessagePendingStatus,
                typeOfStatus: StatusNameConstants.penddingStatus)
            self?.showAlertToChangeOrderOfStatus.onNext(message)
        }).disposed(by: disposeBag)
        // Funcionalidad para el botón de En Proceso
        processDidTap.subscribe(onNext: { [weak self] _ in
            let message = MessageToChangeStatus(
                message: CommonStrings.confirmationMessageProcessStatus,
                typeOfStatus: StatusNameConstants.inProcessStatus)
            self?.showAlertToChangeOrderOfStatus.onNext(message)
        }).disposed(by: disposeBag)
        // Funcionalidad para agrupar los cards por similitud
        self.similarityViewButtonAction()
        // Funcionalidad para mostrar la vista normal en los cards
        normalViewButtonDidTap.subscribe(onNext: { [weak self] _ in
            self?.processButtonIsEnable.onNext(false)
            let ordering = self?.sortByBaseBocumentAscending(orders: self!.ordersTemp)
            self?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: ordering ?? [])]
            self?.statusDataGrouped.onNext([SectionModel(model: CommonStrings.empty, items: ordering ?? [])])
            self?.similarityViewButtonIsEnable.onNext(true)
            self?.normalViewButtonIsEnable.onNext(false)
            self?.groupedByOrderNumberIsEnable.onNext(true)
            self?.changeStatusSort(normal: true, similarity: false, grouped: false)
        }).disposed(by: self.disposeBag)
        // Funcionalidad para mostra la vista ordenada número de orden
        groupByOrderNumberButtonDidTap.subscribe(onNext: { [weak self] _ in
            self?.processButtonIsEnable.onNext(false)
            if self?.ordersTemp != nil {
                let dataGroupedByBaseDocument = Dictionary(grouping: self!.ordersTemp, by: { "\($0.baseDocument!)" })
                let ordersGroupedAndSorted = self?.groupedByOrderNumber(data: dataGroupedByBaseDocument)
                self?.sectionOrders = ordersGroupedAndSorted ?? []
                self?.statusDataGrouped.onNext(ordersGroupedAndSorted ?? [])
            }
            self?.normalViewButtonIsEnable.onNext(true)
            self?.similarityViewButtonIsEnable.onNext(true)
            self?.groupedByOrderNumberIsEnable.onNext(false)
            self?.changeStatusSort(normal: false, similarity: false, grouped: true)
        }).disposed(by: self.disposeBag)
        initExtension()
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

    private func postOrderPDf(orders: [Int]) {
        networkManager.postOrdersPDF(orders: orders).subscribe(onNext: { [weak self] response in
            guard let self = self, response.response?.count ?? 0 > 0 else { return }
            self.loading.onNext(false)
            self.orderURLPDF.onNext(response.response!.first!)
        }, onError: { [weak self] error in
            guard let self = self else { return }
            print(error.localizedDescription)
            self.loading.onNext(false)
            self.showAlert.onNext("Por el momento no es posible mostrar el PDF del pedido, intenta más tarde")
        }).disposed(by: disposeBag)
    }

    func similarityViewButtonAction() {
        similarityViewButtonDidTap.subscribe(onNext: { [weak self] _ in
            self?.processButtonIsEnable.onNext(false)
            if self?.ordersTemp != nil {
                for order in self!.ordersTemp {
                    let itemCodeInArray = order.itemCode?.components(separatedBy: CommonStrings.separationSpaces)
                    if let codeProduct = itemCodeInArray?.first {
                        order.productCode = codeProduct
                    } else {
                        order.productCode = CommonStrings.empty
                    }
                }
                // Se agrupa las ordenes por código de producto
                let dataGroupedByProductCode = Dictionary(grouping: self!.ordersTemp, by: {$0.productCode})
                let sectionModels = self?.groupedWithSimilarityOrWithoutSimilarity(
                    data: dataGroupedByProductCode, titleForOrdersWithoutSimilarity: CommonStrings.noSimilarity,
                    titleForOrdersWithSimilarity: CommonStrings.product)
                self?.sectionOrders = sectionModels ?? []
                self?.statusDataGrouped.onNext(sectionModels ?? [])
            }
            self?.similarityViewButtonIsEnable.onNext(false)
            self?.normalViewButtonIsEnable.onNext(true)
            self?.groupedByOrderNumberIsEnable.onNext(true)
            self?.changeStatusSort(normal: false, similarity: true, grouped: false)
        }).disposed(by: self.disposeBag)
    }
    // Se agrupan ordenes por similitud o sin similitud
    func groupedWithSimilarityOrWithoutSimilarity(
        data: [String? : [Order]], titleForOrdersWithoutSimilarity: String,
        titleForOrdersWithSimilarity: String) -> [SectionModel<String, Order>] {
        var sectionModels: [SectionModel<String, Order>] = []
        // Se extraen las ordenes que contengan más de una coincidencia por código de producto
        // y se agrupan por "Producto: [productCode]"
        let groupBySimilarity = data
            .filter { $0.value.count > 1 }
            .sorted { ($0.key ?? "").localizedStandardCompare( $1.key ?? "") == .orderedAscending }
        if groupBySimilarity.count > 0 {
            let sectionsModelsBySimilarity = groupBySimilarity
                .map({ [unowned self] (orders) -> SectionModel<String, Order> in
                return SectionModel(
                    model: "\(titleForOrdersWithSimilarity) \(orders.key ?? "")",
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
        if let cero = data1["0"] {
            sectionModels.append(SectionModel(model: "\(CommonStrings.ordersWithoutOrder)", items: cero))
            data1.removeValue(forKey: "0")
        }
        let sections = data1.map({ [unowned self] (orders) -> SectionModel<String, Order> in
            return SectionModel(
                model: "\(CommonStrings.order) \(orders.key ?? "")",
                items: self.sortByBaseBocumentAscending(orders: orders.value))
        })
        let sortedSections = sections.sorted { $0.model < $1.model }
        sectionModels.append(contentsOf: sortedSections)
        return sectionModels
    }
    func setSelection(section: SectionOrder) {
        let ordering = sortByBaseBocumentAscending(orders: section.orders)
        ordersTemp = ordering
        if normalSort {
            statusDataGrouped.onNext([SectionModel(model: CommonStrings.empty, items: ordering)])
            sectionOrders = [SectionModel(model: CommonStrings.empty, items: ordering)]
            similarityViewButtonIsEnable.onNext(true)
            groupedByOrderNumberIsEnable.onNext(true)
            normalViewButtonIsEnable.onNext(false)
            processButtonIsEnable.onNext(false)
            pendingButtonIsEnable.onNext(false)
        } else if similaritySort {
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
        } else {
            processButtonIsEnable.onNext(false)
            let dataGroupedByBaseDocument = Dictionary(grouping: ordersTemp, by: { "\($0.baseDocument!)" })
            let ordersGroupedAndSorted = groupedByOrderNumber(data: dataGroupedByBaseDocument)
            sectionOrders = ordersGroupedAndSorted
            statusDataGrouped.onNext(ordersGroupedAndSorted)
            normalViewButtonIsEnable.onNext(true)
            similarityViewButtonIsEnable.onNext(true)
            groupedByOrderNumberIsEnable.onNext(false)
        }
        title.onNext(section.statusName)
        showKPIView.onNext(false)
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
                if aCode.baseDocument! != bCode.baseDocument! {
                    return aCode.baseDocument! < bCode.baseDocument!
                } else {
                    return aCode.productionOrderId! < bCode.productionOrderId!
                }
            }
        }
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
        if !status.isEmpty {
            // Obtiene las ordenes a cambialas de status mediante el indexPath
            var orders: [ChangeStatusRequest] = []
            for index in indexPath! {
                let card = self.sectionOrders[index.section].items[index.row]
                let order = ChangeStatusRequest(
                    userId: (Persistence.shared.getUserData()?.id)!, orderId: card.productionOrderId!, status: status)
                orders.append(order)
            }
            self.networkManager.changeStatusOrder(changeStatusRequest: orders)
                .observeOn(MainScheduler.instance).subscribe(onNext: {[weak self] _ in
                self?.loading.onNext(false)
                self?.refreshDataWhenChangeProcessIsSucces.onNext(())
                self?.processButtonIsEnable.onNext(false)
                self?.pendingButtonIsEnable.onNext(false)
                self?.rootViewModel.needsRefresh = true
                }, onError: { [weak self] _ in
                    self?.loading.onNext(false)
                    self?.showAlert.onNext(CommonStrings.errorToChangeStatus)
                    self?.processButtonIsEnable.onNext(true)
            }).disposed(by: self.disposeBag)
        } else {
            self.showAlert.onNext(CommonStrings.errorToChangeStatus)
        }
    }
    func getStatusName(index: Int) -> String {
        switch index {
        case 0:
            return StatusNameConstants.assignedStatus
        case 1:
            return StatusNameConstants.inProcessStatus
        case 2:
            return StatusNameConstants.penddingStatus
        case 3:
            return StatusNameConstants.finishedStatus
        case 4:
            return StatusNameConstants.reassignedStatus
        default:
            return CommonStrings.empty
        }
    }
    func getStatusId(name: String) -> Int {
        switch name {
        case StatusNameConstants.assignedStatus:
            return 0
        case StatusNameConstants.inProcessStatus:
            return 1
        case StatusNameConstants.penddingStatus:
            return 2
        case StatusNameConstants.finishedStatus:
            return 3
        case StatusNameConstants.reassignedStatus:
            return 4
        default:
            return -1
        }
    }
    private func changeStatusSort(normal: Bool, similarity: Bool, grouped: Bool) {
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

    func getConnection() {

        self.loading.onNext(true)
        networkManager.getConnect().subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.hasConnection.onNext(true)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.hasConnection.onNext(false)
            self.loading.onNext(false)
            self.showAlert.onNext("Por el momento no es posible mostrar el PDF del pedido, intenta más tarde")
        }).disposed(by: disposeBag)

    }

}

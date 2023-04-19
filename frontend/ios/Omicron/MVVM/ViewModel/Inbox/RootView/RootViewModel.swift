//
//  RootViewModel.swift
//  Omicron
//
//  Created by Axity on 29/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import RxSwift
import RxCocoa
import Moya // Borrar cuando se consuma bien el servicio
import Resolver

class RootViewModel {
    // MARK: - Variables
    var sections: [SectionOrder] = []
    var selectedRow: BehaviorSubject<IndexPath?> = BehaviorSubject<IndexPath?>(value: nil)
    public var dataStatus: BehaviorSubject<[SectionOrder]> = BehaviorSubject(value: [])
    var dataFilter = PublishSubject<[Order]?>()
    var loading: BehaviorSubject<Bool> = BehaviorSubject(value: false)
    var refreshSelection: PublishSubject<Int> = PublishSubject()
    var error: PublishSubject<String> = PublishSubject()
    let disposeBag = DisposeBag()
    var showRefreshControl: PublishSubject<Void> = PublishSubject<Void>()
    var logoutDidTap = PublishSubject<Void>()
    var goToLoginViewController = PublishSubject<Void>()
    var searchFilter = PublishSubject<String>()
    var needsRefresh = true
    var removeSelecteds = false
    var refreshSearch = PublishSubject<String>()
    var searchStore = String()
    var needSearch = false
    var orders: [SectionOrder] = []
    var showTwoModals = false
    var completeOrderList: [Order] = []
    var modalHideAuto: BehaviorSubject<String> = BehaviorSubject(value: "")
    @Injected var chartViewModel: ChartViewModel
    @Injected var networkManager: NetworkManager
    var userType: UserType = UserType.technical
    var idUser: String = CommonStrings.empty
    init() {
        logoutDidTapBinding()
        searchFilterBinding()
        userData()
    }

    func userData() {
        let rol = Persistence.shared.getUserData()?.role ?? UserType.technical.rawValue
        self.idUser = Persistence.shared.getUserData()?.id ?? CommonStrings.empty
        self.userType = UserType(rawValue: rol)!
    }
    // MARK: - Functions
    func logoutDidTapBinding() {
        self.logoutDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(true)

            DispatchQueue.main.asyncAfter(deadline: .now() + 1.5) {
                Persistence.shared.removePersistenceData()
                self.goToLoginViewController.onNext(())
                self.loading.onNext(false)
            }
        }).disposed(by: self.disposeBag)
    }

    func searchFilterBinding() {
        self.searchFilter.subscribe(onNext: { [weak self] text in
            guard let self = self else { return }
            self.searchStore = text
            if text.count == 0 {
                self.needSearch = false
                self.dataFilter.onNext(nil)
                return
            }
            self.needSearch = true
            let orders = self.sections.map({ $0.orders }).reduce([], +)
            let filter = self.getOrdersFilters(orders: orders, text: text)
            self.dataFilter.onNext(filter)
        }).disposed(by: disposeBag)
    }

    func getOrdersFilters(orders: [Order], text: String) -> [Order] {
        return orders.filter({ order in
            guard let orderId = order.productionOrderId else { return false }
            guard let baseDocument = order.baseDocument else { return false }
            guard let itemCode = order.itemCode else { return false }
            guard let description = order.descriptionProduct else { return false }
            var shopTransaction = ""
            if order.shopTransaction != nil {
                shopTransaction = String(order.shopTransaction ?? "").suffix(6).uppercased()
            }
            return String(orderId).contains(text)
                || String(baseDocument).contains(text)
                || String(shopTransaction).contains(text.uppercased())
                || String(itemCode).contains(text.uppercased())
                || description.uppercased().contains(text.uppercased())
        })
    }

    func getShowTwoSignatureModals(_ fabricationOrders: [Int]) -> Bool {
        let selectedOrders = self.completeOrderList.filter {
            fabricationOrders.contains($0.productionOrderId ?? 0)
        }
        return !selectedOrders.allSatisfy { $0.hasTechnicalAssigned ?? false}
    }

    func sectionOrderSwitched(statusId: Int, orders: [Order]) -> SectionOrder? {
        switch statusId {
        case 1:
            return SectionOrder(statusId: statusId, statusName: StatusNameConstants.assignedStatus,
                                numberTask: orders.count,
                                imageIndicatorStatus: IndicatorImageStatus.assigned, orders: orders)
        case 2:
            return SectionOrder(statusId: statusId, statusName: StatusNameConstants.inProcessStatus,
                                numberTask: orders.count,
                                imageIndicatorStatus: IndicatorImageStatus.inProcess,
                                orders: orders)
        case 3:
            return SectionOrder(statusId: statusId, statusName: StatusNameConstants.penddingStatus,
                                numberTask: orders.count,
                                imageIndicatorStatus: IndicatorImageStatus.pendding,
                                orders: orders)
        case 4:
            return SectionOrder(statusId: statusId, statusName: StatusNameConstants.finishedStatus,
                                numberTask: orders.count,
                                imageIndicatorStatus: IndicatorImageStatus.finished,
                                orders: orders)
        case 5:
            return SectionOrder(statusId: statusId,
                                statusName: StatusNameConstants.reassignedStatus,
                                numberTask: orders.count,
                                imageIndicatorStatus: IndicatorImageStatus.reassined,
                                orders: orders)
        default:
            return nil
        }
    }
    func getOrders(isUpdate: Bool = false) {
        if isUpdate { needsRefresh = true }
        needsRefresh = true
        if let userData = Persistence.shared.getUserData(), let userId = userData.id {
            if needsRefresh { self.loading.onNext(true) }
            chartViewModel.getWorkloads()
            removeSelecteds = needsRefresh
            getOrdersService(userId: userId, isUpdate: isUpdate)
        } else {
            self.error.onNext(CommonStrings.errorLoadingOrders)
            self.showRefreshControl.onNext(())
        }
    }

    func getOrdersService(userId: String, isUpdate: Bool) {
        self.networkManager.getStatusList(userId).subscribe(onNext: { [weak self] res in
            guard let self = self else { return }
            let sections = self.getSections(res: res)
            let ordersByStatus: [[Order]] = res.response?.status?.map { $0.orders ?? [] } ?? []
            var ordersTemp: [Order] = []
            for orders in ordersByStatus {
                ordersTemp.append(contentsOf: orders )
            }
            self.completeOrderList = ordersTemp
            self.sections = sections
            self.dataStatus.onNext(sections)
            self.orders = sections
            self.refreshSelection.onNext(sections.count)
            self.needRefreshAction()
            self.needIsUpdate(isUpdate: isUpdate)
            self.needSearchAction(self.needSearch, self.searchStore)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.showRefreshControl.onNext(())
            self.error.onNext(CommonStrings.errorLoadingOrders)
            self.needRefreshAction()
        }).disposed(by: disposeBag)
    }

    func needRefreshAction() {
        if self.needsRefresh {
            self.loading.onNext(false)
            self.needsRefresh.toggle()
        }
    }

    func needIsUpdate(isUpdate: Bool) {
        if isUpdate {
            self.showRefreshControl.onNext(())
        }
    }

    func needSearchAction(_ needSearch: Bool, _ searchStore: String) {
        if needSearch {
            self.refreshSearch.onNext(searchStore)
        }
    }

    func getSections(res: StatusResponse) -> [SectionOrder] {
        if self.userType == UserType.technical {
            res.response?.status = res.response?.status?.filter({
                $0.statusId! != StatusOrders.inProcess.rawValue &&
                $0.statusId! != StatusOrders.finished.rawValue })
        }
        return res.response?.status.map({ status in
            return status.map({ detail -> SectionOrder? in
                let orders = detail.orders ?? []
                if let statusId = detail.statusId {
                    return self.sectionOrderSwitched(statusId: statusId, orders: orders)
                }
                return nil
            })
        })?.compactMap({ $0 }) ?? []
    }

    func resetFilter() {
        self.dataFilter.onNext(nil)
    }
}

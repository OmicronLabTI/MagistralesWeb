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
    @Injected var chartViewModel: ChartViewModel
    @Injected var networkManager: NetworkManager
    init() {
        self.logoutDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.loading.onNext(true)
            DispatchQueue.main.asyncAfter(deadline: .now() + 1.5) {
                Persistence.shared.removePersistenceData()
                self?.goToLoginViewController.onNext(())
                self?.loading.onNext(false)
            }
        }).disposed(by: self.disposeBag)
        self.searchFilter.subscribe(onNext: { [weak self] text in
            if text.count == 0 {
                self?.dataFilter.onNext(nil)
                return
            }
            let orders = self?.sections.map({ $0.orders }).reduce([], +)
            let filter = orders?.filter({ order in
                guard let orderId = order.productionOrderId else { return false }
                guard let baseDocument = order.baseDocument else { return false }
                return String(orderId).contains(text) || String(baseDocument).contains(text)
            })
            self?.dataFilter.onNext(filter ?? [])
        }).disposed(by: disposeBag)
    }
    // MARK: - Functions
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
            chartViewModel.getWorkload()
            removeSelecteds = needsRefresh
            self.networkManager.getStatusList(userId: userId).subscribe(onNext: { [weak self] res in
                guard let self = self else { return }
                let sections = res.response?.status.map({ status in
                    return status.map({ detail -> SectionOrder? in
                        let orders = detail.orders ?? []
                        if let statusId = detail.statusId {
                            return self.sectionOrderSwitched(statusId: statusId, orders: orders)
                        }
                        return nil
                    })
                })?.compactMap({ $0 }) ?? []
                self.sections = sections
                self.dataStatus.onNext(sections)
                self.refreshSelection.onNext(sections.count)
                if self.needsRefresh {
                    self.loading.onNext(false)
                    self.needsRefresh.toggle()
                }
                if isUpdate {
                    self.showRefreshControl.onNext(())
                }
                }, onError: { [weak self] err in
                    guard let self = self else { return }
                    print(err)
                    self.showRefreshControl.onNext(())
                    self.error.onNext(CommonStrings.errorLoadingOrders)
                    if self.needsRefresh {
                        self.loading.onNext(false)
                        self.needsRefresh.toggle()
                    }
            }).disposed(by: disposeBag)
        } else {
            self.error.onNext(CommonStrings.errorLoadingOrders)
            self.showRefreshControl.onNext(())
        }
    }
    func resetFilter() {
        self.dataFilter.onNext(nil)
    }
}

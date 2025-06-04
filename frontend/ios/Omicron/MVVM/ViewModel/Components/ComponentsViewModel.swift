//
//  ComponentsViewModel.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import Resolver

class ComponentsViewModel {
    var disposeBag = DisposeBag()
    var searchFilter = BehaviorSubject<String>(value: String())
    var searchDidTap = PublishSubject<Void>()
    var removeChip = PublishSubject<String>()
    var dataChips = BehaviorSubject<[String]>(value: [])
    var dataResults = BehaviorSubject<[ComponentO]>(value: [])
    var dataError = PublishSubject<String>()
    var loading = PublishSubject<Bool>()
    var selectedComponent = BehaviorSubject<ComponentO?>(value: nil)
    var saveDidTap = PublishSubject<ComponentFormValues>()
    var saveSuccess = PublishSubject<Void>()
    var bindingData = BehaviorSubject<[ComponentO]>(value: [])
    var componentsList: [ComponentO] = []
    @Injected var inboxViewModel: InboxViewModel
    @Injected var orderDetailViewModel: OrderDetailViewModel
    @Injected var networkManager: NetworkManager
    var typeOpen = TypeComponentsOpenDialog.detailOrder
    let onScroll = PublishSubject<Void>()
    var chips: [String] = []
    var offset = 0
    var totalInfo = 0

    init() {
        searchDidTapBinding()
        removeChipBinding()
        dataChipsBinding()
        saveDidTapBinding()
        bindOnScroll()
    }

    func clearObservables() {
        self.chips = []
        self.offset = 0
        self.totalInfo = 0
        self.dataResults.onNext([])
        self.dataChips.onNext([])
    }
    func searchDidTapBinding() {
        searchDidTap.withLatestFrom(Observable.combineLatest(searchFilter, dataChips))
            .subscribe(onNext: { [weak self] text, chips in
                guard let self = self, text.count >= 2 else { return }
                self.onSuccessChips(text: text, chips: chips)
            }).disposed(by: disposeBag)
    }

    func onSuccessChips(text: String, chips: [String]) {
        if chips.first(where: { $0 == text }) != nil {
            return
        }
        let newChips = chips + [text]
        dataChips.onNext(newChips)
    }
    func bindOnScroll() {
        self.onScroll.subscribe(onNext: {[weak self] _ in
            guard let self = self else { return }
            if self.totalInfo > self.componentsList.count {
                self.offset += Constants.Components.limit.rawValue
                self.getComponents(chips: self.chips, offset: self.offset)
            }
        }).disposed(by: disposeBag)
    }

    func removeChipBinding() {
        removeChip.withLatestFrom(dataChips, resultSelector: { [weak self] removeItem, data in
            guard let self = self else { return }
            let newChips = data.filter({ $0 != removeItem })
            self.dataChips.onNext(newChips)
        }).subscribe().disposed(by: disposeBag)
    }

    func dataChipsBinding() {
        dataChips.subscribe(onNext: { [weak self] data in
            guard let self = self else { return }
            self.offset = 0
            self.componentsList = []
            if data.count == 0 {
                self.dataResults.onNext([])
                return
            }
            self.chips = data
            self.getComponents(chips: data, offset: self.offset)
        }).disposed(by: disposeBag)
    }

    func saveDidTapBinding() {
        saveDidTap.subscribe(onNext: {[weak self] values in
            guard let self = self else { return }
            guard let order = self.inboxViewModel.selectedOrder else { return }
            let component = Component(
                orderFabID: order.productionOrderId ?? 0, productId: values.selectedComponent.productId ?? String(),
                componentDescription: values.selectedComponent.description ?? String(),
                baseQuantity: values.baseQuantity,
                requiredQuantity: values.requiredQuantity,
                consumed: NSDecimalNumber(decimal: values.selectedComponent.consumed ?? 0).doubleValue,
                available: NSDecimalNumber(decimal: values.selectedComponent.available ?? 0).doubleValue,
                unit: values.selectedComponent.unit ?? String(), warehouse: values.warehouse,
                pendingQuantity: NSDecimalNumber(decimal: values.selectedComponent.pendingQuantity ?? 0).doubleValue,
                stock: NSDecimalNumber(decimal: values.selectedComponent.stock ?? 0).doubleValue,
                warehouseQuantity: NSDecimalNumber(decimal:
                                                    values.selectedComponent.warehouseQuantity ?? 0).doubleValue,
                action: Actions.insert.rawValue,
                assignedBatches: [])
            let orderDetailReq = OrderDetailRequest(
                fabOrderID: component.orderFabId,
                plannedQuantity: order.plannedQuantity ?? 0, fechaFin: (order.finishDate != nil ?
                    UtilsManager.shared.formattedDateFromString(
                        dateString: order.finishDate ?? String(),
                        withFormat: DateFormat.yyyymmdd) : String()) ?? String(),
                comments: String(),
                warehouse: self.orderDetailViewModel.tempOrderDetailData?.warehouse ?? CommonStrings.empty,
                components: [component])
            self.saveComponent(req: orderDetailReq)
        }).disposed(by: disposeBag)
    }

    func saveComponent(req: OrderDetailRequest) {
        inboxViewModel.rootViewModel.loading.onNext(true)
        self.networkManager.updateDeleteItemOfTableInOrderDetail(req)
            .subscribe(onNext: { [weak self] _ in
                guard let self = self else { return }
                self.inboxViewModel.rootViewModel.loading.onNext(false)
                self.saveSuccess.onNext(())
                self.orderDetailViewModel.getOrdenDetail(isRefresh: true)
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.inboxViewModel.rootViewModel.loading.onNext(false)
                self.dataError.onNext(Constants.Errors.errorSave.rawValue)
            }).disposed(by: disposeBag)
    }
    func getComponents(chips: [String], offset: Int) {
        let catalogGroup = typeOpen == .detailOrder ?
            orderDetailViewModel.getCatalogGroup() :
            ""
        let userId = Persistence.shared.getUserData()?.id ?? ""
        let request = ComponentRequest(
            offset: offset,
            limit: Constants.Components.limit.rawValue,
            chips: chips,
            catalogGroup: catalogGroup,
            userId: typeOpen != .detailOrder ? userId : "")
        loading.onNext(true)
        self.networkManager.getComponents(request).subscribe(onNext: { [weak self] res in
            guard let self = self else { return }
            self.componentsList.append(contentsOf: res.response ?? [])
            self.totalInfo = res.comments ?? 0
            self.dataResults.onNext(self.componentsList)
            self.loading.onNext(false)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.dataError.onNext(Constants.Errors.errorData.rawValue)
            self.loading.onNext(false)
        }).disposed(by: disposeBag)
    }

    func getMostCommonComponentsService(type: String) {
        loading.onNext(true)
        let userID = Persistence.shared.getUserData()?.id ?? ""
        let catalogGroup = type == TypeMostCommonRequest.detailOrder.rawValue ?
            orderDetailViewModel.getCatalogGroup(): String()
        let reqParams = CommonComponentRequest(catalogGroup: catalogGroup,
                                               userId: userID,
                                               type: type)
        networkManager.getMostCommonComponents(reqParams).subscribe(onNext: {
            [weak self] res in
            guard let self = self else { return }
            self.loading.onNext(false)
            if let components = res.response {
                self.bindingData.onNext(components)
            }
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.dataError.onNext(Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag)
    }
}

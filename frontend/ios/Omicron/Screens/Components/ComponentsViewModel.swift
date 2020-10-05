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
    var searchFilter = BehaviorSubject<String>(value: "")
    var searchDidTap = PublishSubject<Void>()
    var removeChip = PublishSubject<String>()
    var dataChips = BehaviorSubject<[String]>(value: [])
    var dataResults = BehaviorSubject<[ComponentO]>(value: [])
    var dataError = PublishSubject<String>()
    var loading = PublishSubject<Bool>()
    var selectedComponent = BehaviorSubject<ComponentO?>(value: nil)
    var saveDidTap = PublishSubject<ComponentFormValues>()
    var saveSuccess = PublishSubject<Void>()
    
    @Injected var inboxViewModel: InboxViewModel
    @Injected var orderDetailViewModel: OrderDetailViewModel
    @Injected var networkManager: NetworkManager
    
    init() {
        searchDidTap.withLatestFrom(Observable.combineLatest(searchFilter, dataChips))
            .subscribe(onNext: { [weak self] text, chips in
                guard text.count >= 2 else { return }
                if let _ = chips.first(where: { $0 == text }) {
                    return
                }
                
                let newChips = chips + [text]
                self?.dataChips.onNext(newChips)
            }).disposed(by: disposeBag)
        
        removeChip.withLatestFrom(dataChips, resultSelector: { [weak self] removeItem, data in
            let newChips = data.filter({ $0 != removeItem })
            self?.dataChips.onNext(newChips)
        }).subscribe().disposed(by: disposeBag)
        
        dataChips.subscribe(onNext: { [weak self] data in
            if data.count == 0 {
                self?.dataResults.onNext([])
                return
            }
            self?.getComponents(chips: data)
        }).disposed(by: disposeBag)
        
        saveDidTap.withLatestFrom(selectedComponent, resultSelector: { [weak self] values, data in
            guard let comp = data else { return }
            guard let order = self?.inboxViewModel.selectedOrder else { return }
            let component = Component(
                orderFabID: order.productionOrderId ?? 0,
                productId: comp.productId ?? "",
                componentDescription: comp.description ?? "",
                baseQuantity: values.baseQuantity,
                requiredQuantity: values.requiredQuantity,
                consumed: NSDecimalNumber(decimal: comp.consumed ?? 0).doubleValue,
                available: NSDecimalNumber(decimal: comp.available ?? 0).doubleValue,
                unit: comp.unit ?? "",
                warehouse: values.warehouse,
                pendingQuantity: NSDecimalNumber(decimal: comp.pendingQuantity ?? 0).doubleValue,
                stock: NSDecimalNumber(decimal: comp.stock ?? 0).doubleValue,
                warehouseQuantity: NSDecimalNumber(decimal: comp.warehouseQuantity ?? 0).doubleValue,
                action: "insert")
            
            let orderDetailReq = OrderDetailRequest(
                fabOrderID: component.orderFabId,
                plannedQuantity: order.plannedQuantity ?? 0,
                fechaFin: (order.finishDate != nil ? UtilsManager.shared.formattedDateFromString(dateString: order.finishDate!, withFormat: "yyyy-MM-dd") : "") ?? "",
                comments: "",
                components: [component])
            
            self?.saveComponent(req: orderDetailReq)
        }).subscribe().disposed(by: disposeBag)
    }
    
    func saveComponent(req: OrderDetailRequest) {
        loading.onNext(true)
        self.networkManager.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: req).subscribe(onNext: { [weak self] _ in
            self?.loading.onNext(false)
            self?.saveSuccess.onNext(())
            self?.orderDetailViewModel.getOrdenDetail(isRefresh: true)
        }, onError: { [weak self] _ in
            self?.loading.onNext(false)
            self?.dataError.onNext(Constants.Errors.errorSave.rawValue)
        }).disposed(by: disposeBag)
    }
    
    func getComponents(chips: [String]) {
        let request = ComponentRequest(
            offset: Constants.Components.offset.rawValue,
            limit: Constants.Components.limit.rawValue,
            chips: chips)

        loading.onNext(true)
        self.networkManager.getComponents(data: request).subscribe(onNext: { [weak self] res in
            self?.dataResults.onNext(res.response ?? [])
            self?.loading.onNext(false)
        }, onError: { [weak self] _ in
            self?.dataError.onNext(Constants.Errors.errorData.rawValue)
            self?.loading.onNext(false)
        }).disposed(by: disposeBag)
    }
}

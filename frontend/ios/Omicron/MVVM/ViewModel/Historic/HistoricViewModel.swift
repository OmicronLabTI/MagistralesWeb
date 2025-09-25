//
//  HistoricViewModel.swift
//  Omicron
//
//  Created by Josue Castillo on 11/09/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import Resolver

class HistoricViewModel {
    // MARK: Dependencias
    @Injected var networkManager: NetworkManager
    var disposeBag: DisposeBag = DisposeBag()
    
    // MARK: Observables
    var loading: PublishSubject<Bool> = PublishSubject()
    var tableData = PublishSubject<[ParentOrders]>()
    var searchDidTap = PublishSubject<Void>()
    var searchFilter = BehaviorSubject<String>(value: String())
    var onScroll = PublishSubject<Void>()
    var endRefreshing = PublishSubject<Void>()
    var restartTable: PublishSubject<Bool> = PublishSubject()
    
    var showAlert: PublishSubject<String> = PublishSubject()
    // MARK: Controles
    var orders: [ParentOrders] = []
    var dataOffset: Int = 0
    let limit: Int = 10
    var chips = String()
    
    init() {
        searchDidTapBinding()
        bindOnScroll()
    }
    
    func searchDidTapBinding() {
        searchDidTap.withLatestFrom(searchFilter).subscribe(onNext: { [weak self] text in
            let isValid = self?.validateOrdersToSearch(text)
            if isValid ?? false {
                self?.dataOffset = 0
                self?.chips = text
                self?.orders = []
                self?.restartTable.onNext(true)
                self?.getHistoricData(orders: self?.chips ?? String(), offset: self?.dataOffset ?? 0, limit: 10)
            }
        }).disposed(by: disposeBag)
        
    }
                                                            
    
    func validateOrdersToSearch(_ text: String) -> Bool {
        let segments = text.split(separator: ",")
        guard segments.count <= 10 else {
            return false
        }
        for segment in segments {
            if !segment.allSatisfy({ $0.isNumber }) {
                return false
            }
        }
        return true
    }
    
    func updateData(isRefresh: Bool = false) {
        if isRefresh {
            self.dataOffset = 0
            self.orders = []
            restartTable.onNext(isRefresh)
            getHistoricData(orders: self.chips, offset: self.dataOffset, limit: 10, isRefresh: isRefresh)
        }
    }
    
    
    func bindOnScroll() {
        self.onScroll.subscribe(onNext: {[weak self] _ in
            guard let self = self else { return }
            self.dataOffset += 10
            self.getHistoricData(orders: self.chips, offset: self.dataOffset, limit: 10)
        }).disposed(by: disposeBag)
    }
    
    func getHistoricData(orders: String, offset: Int, limit: Int, isRefresh: Bool = false) {
        let request = getRequestData(orders: orders, offset: offset, limit: limit)
        
        self.loading.onNext(true)
        networkManager.getHistoric(request).subscribe(onNext: {[weak self] res in
            dump(res)
            guard let self = self else { return }
            self.loading.onNext(false)
            if res.code == 200 {
                self.orders.append(contentsOf: res.response ?? [])
                self.endRefreshing.onNext(())
                tableData.onNext(res.response ?? [])
                return
            }
            let errorMessage = res.userError ?? String()
            self.showAlert.onNext(errorMessage)
        }, onError: {[weak self] _ in
            guard let self = self else { return }
            self.endRefreshing.onNext(())
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorSplitOrder)
        }).disposed(by: disposeBag)
    }
    
    func getRequestData(orders: String, offset: Int, limit: Int) -> HistoricRequestModel {
        return HistoricRequestModel(
            qfb: Persistence.shared.getUserData()?.id ?? String(),
            orders: orders,
            offset: offset,
            limit: limit
        )
    }
    
    func endRefreshingAction(_ isRefresh: Bool) {
        if isRefresh {
            self.endRefreshing.onNext(())
        }
    }
}

//
//  HistoryViewModel.swift
//  Omicron
//
//  Created by Daniel Vargas on 15/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa
import CryptoKit
import Resolver

class HistoryViewModel {
    var disposeBag: DisposeBag = DisposeBag()
    @Injected var networkManager: NetworkManager
    var selectedRangeDateObs = PublishSubject<(startDate: Date, endDate: Date)>()
    var selectedStatusObs = PublishSubject<[String]>()
    var selectedHistoryList = PublishSubject<[RawMaterialItem]>()
    var changeFilters = PublishSubject<Void>()
    var onScroll = PublishSubject<Void>()
    var startDate = Date()
    var endDate = Date()
    var selectedStatus: [String] = ["Abierto"]
    var offset = 0
    var limit = 20
    var totalData = 0
    var loading = PublishSubject<Bool>()
    var showAlert = PublishSubject<String>()
    var historyList: [RawMaterialItem] = []
    init() {
        startDate = Calendar.current.date(byAdding: .day, value: -6, to: self.endDate) ?? Date()
        bindChangeDateRange()
        bindChangeStatus()
        bindOnScroll()
    }
    
    func resetValues() {
        endDate = Date()
        startDate = Calendar.current.date(byAdding: .day, value: -6, to: self.endDate) ?? Date()
        historyList = []
        offset = 0
        totalData = 0
        selectedStatus = ["Abierto"]
    }

    func bindChangeDateRange() {
        selectedRangeDateObs.subscribe(onNext: { [weak self]dates in
            guard let self = self else { return }
            self.startDate = dates.startDate
            self.endDate = dates.endDate
            self.offset = 0
            self.historyList = []
            self.changeFilters.onNext(())
            self.getHistory(offset: self.offset, limit: self.limit)
        }).disposed(by: disposeBag)
    }
    func bindChangeStatus() {
        selectedStatusObs.subscribe(onNext: {[weak self] status in
            guard let self = self else { return }
            self.selectedStatus = status
            self.offset = 0
            self.historyList = []
            self.changeFilters.onNext(())
            self.getHistory(offset: self.offset, limit: self.limit)
        }).disposed(by: disposeBag)
    }
    func bindOnScroll() {
        onScroll.subscribe(onNext: {[weak self] _ in
            guard let self = self else { return }
            if self.totalData > self.historyList.count {
                self.offset += self.limit
                self.getHistory(offset: self.offset, limit: self.limit)
            }
        }).disposed(by: disposeBag)
    }

    func getHistory(offset: Int,
                    limit: Int) {
        self.loading.onNext(true)
        let userId = Persistence.shared.getUserData()?.id ?? String()
        let req = RawMaterialHistoryReq(offset: offset,
                                        limit: limit,
                                        fini: getStringDate(self.startDate),
                                        ffin: getStringDate(self.endDate),
                                        status: self.selectedStatus,
                                        userId: userId)
        networkManager.getRawMaterialRequest(req).subscribe(onNext: {[weak self] res in
            guard let self = self else { return }
            let responseList = res.response ?? []
            self.historyList.append(contentsOf: responseList)
            self.totalData = res.comments ?? 0
            self.selectedHistoryList.onNext(self.historyList)
            self.loading.onNext(false)
        }, onError: { [weak self] error in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorGetHistoryOrders)
        }).disposed(by: disposeBag)
    }

    func getStringDate(_ date: Date) -> String {
        let formatter = DateFormatter()
        formatter.dateFormat = "dd/MM/yyyy"
        return formatter.string(from: date)
    }
}

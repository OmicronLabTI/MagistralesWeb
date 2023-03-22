//
//  BulkOrderViewModel.swift
//  Omicron
//
//  Created by Daniel Velez on 01/03/23.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import RxSwift
import RxCocoa
import Resolver

class BulkOrderViewModel {
    var searchBulk = PublishSubject<String>()
    var okCreateOrder = PublishSubject<String>()
    var searchDidEnter = PublishSubject<Void>()
    var removeChip = PublishSubject<String>()
    var dataChips = BehaviorSubject<[String]>(value: [])
    var dataResults = BehaviorSubject<[BulkProduct]>(value: [])
    let onScroll = PublishSubject<Void>()
    var bulkProducts: [BulkProduct] = []
    var offset = 0
    var totalInfo = 0
    var chips: [String] = []
    @Injected var networkManager: NetworkManager
    @Injected var rootViewModel: RootViewModel
    let disposeBag = DisposeBag()
    init() {
        searchBulkBinding()
        dataChipsBinding()
        removeChipBinding()
        bindOnScroll()
    }

    func searchBulkBinding() {
        searchDidEnter.withLatestFrom(Observable.combineLatest(searchBulk, dataChips))
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

    func dataChipsBinding() {
        dataChips.subscribe(onNext: { [weak self] data in
            guard let self = self else { return }
            self.offset = 0
            self.bulkProducts = []
            if data.count == 0 {
                self.dataResults.onNext([])
                return
            }
            self.chips = data
            self.searchBulksCall(data, self.offset)
        }).disposed(by: disposeBag)
    }

    func removeChipBinding() {
        removeChip.withLatestFrom(dataChips, resultSelector: { [weak self] removeItem, data in
            guard let self = self else { return }
            let newChips = data.filter({ $0 != removeItem })
            self.dataChips.onNext(newChips)
        }).subscribe().disposed(by: disposeBag)
    }

    func searchBulksCall(_ chips: [String], _ offset: Int) {
        rootViewModel.loading.onNext(true)
        let request = BulkListRequest(
            offset: offset,
            limit: Constants.Components.limit.rawValue,
            chips: chips,
            catalogGroup: "MN")
        self.networkManager.getBulks(request).subscribe(onNext: { [weak self] res in
            guard let self = self else { return }
            self.bulkProducts.append(contentsOf: res.response ?? [])
            self.rootViewModel.loading.onNext(false)
            self.totalInfo = res.comments ?? 0
            self.dataResults.onNext(self.bulkProducts)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.rootViewModel.loading.onNext(false)
            self.rootViewModel.error.onNext(CommonStrings.errorLoadingOrders)
        }).disposed(by: disposeBag)
    }

    func createBulkOrder(_ bulk: BulkProduct) {
        rootViewModel.loading.onNext(true)
        let req = BulkOrderCreate(
            productCode: bulk.productoId ?? "",
            userId: rootViewModel.idUser,
            isFromQfbProfile: true)
        self.networkManager.createOrderBulks(req).subscribe(onNext: { [weak self] res in
            guard let self = self else { return }
            if res.response != 0 {
                self.rootViewModel.getOrders(isUpdate: true)
                self.okCreateOrder.onNext(CommonStrings.okCreateBulkOrder)
            } else {
                self.okCreateOrder.onNext(res.userError ?? CommonStrings.errorCreateBulkOrder)
            }
            self.rootViewModel.loading.onNext(false)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.rootViewModel.loading.onNext(false)
            self.okCreateOrder.onNext(CommonStrings.errorCreateBulkOrder)
        }).disposed(by: disposeBag)
    }

    func bindOnScroll() {
        self.onScroll.subscribe(onNext: {[weak self] _ in
            guard let self = self else { return }
            if self.totalInfo > self.bulkProducts.count {
                self.offset += Constants.Components.limit.rawValue
                self.searchBulksCall( self.chips, self.offset)
            }
        }).disposed(by: disposeBag)
    }
}

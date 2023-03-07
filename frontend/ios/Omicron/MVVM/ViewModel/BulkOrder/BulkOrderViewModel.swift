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
    var okCreateOrder = PublishSubject<Bool>()
    var searchDidEnter = PublishSubject<Void>()
    var removeChip = PublishSubject<String>()
    var dataChips = BehaviorSubject<[String]>(value: [])
    var dataResults = BehaviorSubject<[BulkProduct]>(value: [])
    @Injected var networkManager: NetworkManager
    @Injected var rootViewModel: RootViewModel
    let disposeBag = DisposeBag()
    init(){
        searchBulkBinding()
        dataChipsBinding()
        removeChipBinding()
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
            if data.count == 0 {
                self.dataResults.onNext([])
                return
            }
            self.searchBulksCall(data)
        }).disposed(by: disposeBag)
    }

    func removeChipBinding() {
        removeChip.withLatestFrom(dataChips, resultSelector: { [weak self] removeItem, data in
            guard let self = self else { return }
            let newChips = data.filter({ $0 != removeItem })
            self.dataChips.onNext(newChips)
        }).subscribe().disposed(by: disposeBag)
    }
    
    
    func searchBulksCall(_ chips: [String]) {
        rootViewModel.loading.onNext(true)
        let request = BulkListRequest(
            offset: Constants.Components.offset.rawValue,
            limit: Constants.Components.limit.rawValue,
            chips: chips,
            catalogGroup: "MN")
        self.networkManager.getBulks(request).subscribe(onNext: { [weak self] res in
            guard let self = self else { return }
            self.rootViewModel.loading.onNext(false)
            self.dataResults.onNext(res.response ?? [])
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.rootViewModel.loading.onNext(false)
            self.rootViewModel.error.onNext(CommonStrings.errorLoadingOrders)
        }).disposed(by: disposeBag)
    }
    
    func createBulkOrder(_ bulk:BulkProduct){
        rootViewModel.loading.onNext(true)
        let req = BulkOrderCreate(
            productCode: bulk.productoId ?? "",
            userId: rootViewModel.idUser,
            isFromQfbProfile: true)
        self.networkManager.createOrderBulks(req).subscribe(onNext: { [weak self] res in
            guard let self = self else { return }
            self.rootViewModel.getOrders()
            self.okCreateOrder.onNext(true)
            self.rootViewModel.loading.onNext(false)
        }, onError: { [weak self] erro in
            guard let self = self else { return }
            self.rootViewModel.loading.onNext(false)
            self.rootViewModel.error.onNext(CommonStrings.errorLoadingOrders)
        }).disposed(by: disposeBag)
    }
    
}

//
//  SupplieViewModel.swift
//  Omicron
//
//  Created by Daniel Vargas on 28/02/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//
import Foundation
import RxSwift
import RxCocoa
import CryptoKit
import Resolver

class SupplieViewModel {
    var disposeBag: DisposeBag = DisposeBag()
    var supplieList: [ComponentO] = []
    @Injected var networkManager: NetworkManager
    let componentsList = BehaviorSubject<[ComponentO]>(value: [])
    let addComponent = PublishSubject<ComponentO>()
    init() {
        bindProperties()
    }
    func bindProperties() {
        self.addComponent.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] supplie in
            guard let self = self else { return }
            self.supplieList.append(supplie)
            self.componentsList.onNext(self.supplieList)
        }).disposed(by: disposeBag)
    }
}

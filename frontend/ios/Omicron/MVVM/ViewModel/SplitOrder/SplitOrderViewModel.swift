//
//  Untitled.swift
//  Omicron
//
//  Created by Josue Castillo on 29/07/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import Resolver

class SplitOrderViewModel {
    var disposeBag: DisposeBag = DisposeBag()
    @Injected var networkManager: NetworkManager
    var loading: PublishSubject<Bool> = PublishSubject()
    var showAlert: PublishSubject<String> = PublishSubject()
    var closeModal: PublishSubject<String> = PublishSubject()
    
    func saveChanges(_ data: SplitOrderRequest, section: String) {
        self.loading.onNext(true)
        let mssg = CommonStrings.succesSplitOrder.replacingOccurrences(of: "[status]", with: section)
        networkManager.postSplitOrder(data).subscribe(onNext: {[weak self] res in
            guard let self = self else { return }
            self.loading.onNext(false)
            if res.code == 200 {
                self.closeModal.onNext(mssg)
                return
            }
            let errorMessage = res.userError ?? String()
            self.showAlert.onNext(errorMessage)
        }, onError: {[weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorSplitOrder)
        }).disposed(by: disposeBag)
    }
}

//
//  ContainerViewModel.swift
//  Omicron
//
//  Created by Vicente Cantu Garcia on 19/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa
import Resolver

class ContainerViewModel {

    @Injected var networkManager: NetworkManager

    var containerData = BehaviorSubject<[Container]>(value: [])
    var loading: BehaviorSubject<Bool> = BehaviorSubject(value: false)
    var disposeBag = DisposeBag()

    func getContainerData() {
        guard let userData = Persistence.shared.getUserData(), let userId = userData.id else { return }
        loading.onNext(true)
        networkManager
            .getContainer(userId: userId)
            .subscribe(onNext: { [weak self] containerResponse in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.containerData.onNext(containerResponse.response ?? [])
                }, onError: { error in
                    print(error)
                    self.loading.onNext(false)
                })
            .disposed(by: disposeBag)
    }

}

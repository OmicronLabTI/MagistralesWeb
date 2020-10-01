//
//  ChartViewModel.swift
//  Omicron
//
//  Created by Vicente Cantú on 21/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa
import Resolver

class ChartViewModel {
    

    var workloadData = ReplaySubject<[Workload]>.create(bufferSize: 1)
    var disposeBag = DisposeBag()
    @Injected var networkManager: NetworkManager
    
    init() { }
    
    func getWorkload() {
        let fini =
            UtilsManager.shared.formattedDateToString(date: Date().startOfMonth)
            + "-"
            + UtilsManager.shared.formattedDateToString(date: Date().endOfMonth)
        guard let userData = Persistence.shared.getUserData(), let userId = userData.id else { return }

        self.networkManager.getWordLoad(data: WorkloadRequest(fini: fini, qfb: userId))
            .subscribe(onNext: { [weak self] workloadResponse in
                guard let self = self else { return }
                if let workload = workloadResponse.response {
                    
                    self.workloadData.onNext(workload)
                }
                
                }, onError: { [weak self] error in
                    print(error)
            }).disposed(by: disposeBag)
    }
    
}


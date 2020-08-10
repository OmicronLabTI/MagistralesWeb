//
//  OrderDetailViewModel.swift
//  Omicron
//
//  Created by Axity on 07/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa

class OrderDetailViewModel {
    var disposeBag: DisposeBag = DisposeBag()
    init() {
        NetworkManager.shared.getOrdenDetail(orderId: 1).subscribe(onNext: {res in
            print("Respuesta: \(res)")
        }).disposed(by: self.disposeBag)
    }
}

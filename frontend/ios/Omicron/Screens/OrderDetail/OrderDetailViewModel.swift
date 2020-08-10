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
    // MARK: Variables
    var userId: Int = 1
    var disposeBag: DisposeBag = DisposeBag()
    var orderDetailData: BehaviorRelay<[OrderDetail]> = BehaviorRelay<[OrderDetail]>(value: [])
    var tableData: BehaviorRelay<[Detail]> = BehaviorRelay<[Detail]>(value: [])
    // MARK: Init
    init() {
        NetworkManager.shared.getOrdenDetail(orderId: userId).subscribe(onNext: {res in
            self.orderDetailData.accept([res.response!])
            self.tableData.accept(res.response!.details!)
        }).disposed(by: self.disposeBag)
    }
}

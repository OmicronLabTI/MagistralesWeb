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
    var disposeBag: DisposeBag = DisposeBag()
    var orderDetailData: BehaviorRelay<[OrderDetail]> = BehaviorRelay<[OrderDetail]>(value: [])
    var tableData: BehaviorRelay<[Detail]> = BehaviorRelay<[Detail]>(value: [])
    var error: PublishSubject<String> = PublishSubject()
    var loading: BehaviorSubject<Bool> = BehaviorSubject<Bool>(value: false)
    
    // MARK: Init
    init() {

    }
    
    // MARK: Functions
    func getOrdenDetail(orderId: Int) -> Void {
        loading.onNext(true)
        NetworkManager.shared.getOrdenDetail(orderId: orderId).observeOn(MainScheduler.instance).subscribe(onNext: {res in
            self.orderDetailData.accept([res.response!])
            self.tableData.accept(res.response!.details!)
            self.loading.onNext(false)
        }, onError: { error in
            self.loading.onNext(false)
            self.error.onNext("Hubo un error al cargar el detalle de la orden de fabricación, intentar de nuevo")
        }).disposed(by: self.disposeBag)
    }
}

//
//  ComentsViewModel.swift
//  Omicron
//
//  Created by Axity on 14/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import RxSwift
import RxSwift
import UIKit

class CommentsViewModel {
    
    // MARK: - Variables
    var cancelDidTap = PublishSubject<Void>()
    var aceptDidTap = PublishSubject<Void>()
    var disposeBag = DisposeBag()
    var textView = BehaviorSubject<String>(value: "")
    var loading = PublishSubject<Bool>()
    var showAlert = PublishSubject<String>()
    var orderDetail: [OrderDetail] = []
    var backToOrderDetail = PublishSubject<Void>()
    
    
    init() {
        
        self.aceptDidTap.withLatestFrom(textView).subscribe(onNext: { data in
            
            if (self.orderDetail.first != nil) {
                let order = OrderDetailRequest(fabOrderID: self.orderDetail[0].productionOrderID!, plannedQuantity: self.orderDetail[0].plannedQuantity!, fechaFin: UtilsManager.shared.formattedDateFromString(dateString: self.orderDetail[0].dueDate! , withFormat: "yyyy-MM-dd")!, comments: data, components: [])
                self.loading.onNext(true)
                self.backToOrderDetail.onNext(())
                NetworkManager.shared.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order).observeOn(MainScheduler.instance).subscribe(onNext: { _ in
                    self.loading.onNext(false)
                }, onError: { error in
                    self.loading.onNext(false)
                    self.showAlert.onNext("Ocurrió un error al guardar los comentarios, por favor intentarlo de nuevo")
                }).disposed(by: self.disposeBag)
            }
        }).disposed(by: self.disposeBag)
    }
}

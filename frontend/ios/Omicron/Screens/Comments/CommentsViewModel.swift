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
    var backToLots = PublishSubject<Void>()
    var originView = ""
    
    init() {
        
        self.aceptDidTap.withLatestFrom(textView).subscribe(onNext: { [weak self] data in
            
            if (self?.orderDetail.first != nil) {
                let order = OrderDetailRequest(fabOrderID: (self?.orderDetail[0].productionOrderID!)!, plannedQuantity: (self?.orderDetail[0].plannedQuantity!)!, fechaFin: UtilsManager.shared.formattedDateFromString(dateString: (self?.orderDetail[0].dueDate!)! , withFormat: "yyyy-MM-dd")!, comments: data, components: [])
                self?.loading.onNext(true)
                NetworkManager.shared.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order).observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
                    self?.loading.onNext(false)
                    self?.backToOriginView()
                }, onError: { [weak self] error in
                    self?.loading.onNext(false)
                    self?.showAlert.onNext("Ocurrió un error al guardar los comentarios, por favor intentarlo de nuevo")
                }).disposed(by: self!.disposeBag)
            }
        }).disposed(by: self.disposeBag)
    }
    
    func backToOriginView() {
        switch self.originView {
        case ViewControllerIdentifiers.lotsViewController:
            self.backToLots.onNext(())
        case ViewControllerIdentifiers.orderDetailViewController:
            self.backToOrderDetail.onNext(())
        default:
            print("")
        }
    }
}

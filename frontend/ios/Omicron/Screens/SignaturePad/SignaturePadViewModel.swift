//
//  SignaturePadViewModel.swift
//  Omicron
//
//  Created by Axity on 26/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxCocoa
import RxSwift
import Resolver

class SignaturePadViewModel  {
    
    //MARK: - Variables
    var acceptDidTap = PublishSubject<Void>()
    var getSignature = BehaviorSubject<String>(value: "")
    var signatureIsDone = BehaviorSubject<Bool>(value: false)
    var getOrder = BehaviorSubject<Int>(value: 0)
    var disposeBag = DisposeBag()
    var signature = ""
    let canGetSignature: Driver<Bool>
    var loading = PublishSubject<Bool>()
    let showAlertMessage = PublishSubject<String>()
    var dismissSignatureView = PublishSubject<Void>()
    @Injected var orderDetailVC: OrderDetailViewModel
    
    
    init() {
        
        let input = Observable.combineLatest(self.getOrder,self.getSignature)
        let isValid = self.signatureIsDone.map({$0})
        self.canGetSignature = isValid.asDriver(onErrorJustReturn: false)
    
        self.acceptDidTap.withLatestFrom(input).map({OrderSignature(order: $0, signature: $1)}).subscribe(onNext: { data in
            self.finishOrder(data: data)
        }).disposed(by: self.disposeBag)
    
    }
    
    func finishOrder(data: OrderSignature) -> Void {
        self.loading.onNext(true)
        let finishOrder = FinishOrder(userId: Persistence.shared.getUserData()!.id!, fabricationOrderId: data.order, signature: data.signature)
        
        NetworkManager.shared.finishOrder(order: finishOrder).observeOn(MainScheduler.instance).subscribe(onNext: { _ in
            self.loading.onNext(false)
            self.dismissSignatureView.onNext(())
            self.orderDetailVC.backToInboxView.onNext(())
        }, onError: { error in
            self.loading.onNext(false)
            self.showAlertMessage.onNext("La orden no puede ser Terminada, revisa que todos los artículos tengan un lote asignado")
        }).disposed(by: self.disposeBag)
    }
}

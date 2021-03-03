//
//  ComentsViewModel.swift
//  Omicron
//
//  Created by Axity on 14/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import RxSwift
import UIKit
import Resolver

class CommentsViewModel {
    // MARK: - Variables
    var cancelDidTap = PublishSubject<Void>()
    var aceptDidTap = PublishSubject<Void>()
    var disposeBag = DisposeBag()
    var textView = BehaviorSubject<String>(value: String())
    var loading = PublishSubject<Bool>()
    var showAlert = PublishSubject<String>()
    var orderDetail: [OrderDetail] = []
    var backToOrderDetail = PublishSubject<Void>()
    var backToLots = PublishSubject<Void>()
    var originView = String()
    var needsError = false
    @Injected var networkmanager: NetworkManager
    init() {
        self.aceptDidTap.withLatestFrom(textView).subscribe(onNext: { [weak self] data in
            guard let self = self else { return }
            if self.orderDetail.first != nil {
                let dateFormated = UtilsManager.shared.formattedDateFromString(
                    dateString: self.orderDetail[0].dueDate ?? String() ,
                    withFormat: DateFormat.yyyymmdd) ?? String()
                let order = OrderDetailRequest(
                    fabOrderID: self.orderDetail[0].productionOrderID ?? 0,
                    plannedQuantity: self.orderDetail[0].plannedQuantity ?? Decimal(0),
                    fechaFin: dateFormated,
                    comments: data, warehouse: self.orderDetail[0].warehouse ?? String(),
                    components: [])
                self.loading.onNext(true)
                self.networkmanager.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order,needsError: self.needsError)
                    .observeOn(MainScheduler.instance)
                    .subscribe(onNext: { [weak self] _ in
                        guard let self = self else { return }
                        self.loading.onNext(false)
                        self.backToOriginView()
                        }, onError: { [weak self] _ in
                            guard let self = self else { return }
                            self.loading.onNext(false)
                            self.showAlert
                                .onNext(CommonStrings.errorInComments)
                    }).disposed(by: self.disposeBag)
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
            break
        }
    }
}

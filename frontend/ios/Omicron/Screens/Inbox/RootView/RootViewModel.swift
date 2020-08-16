//
//  RootViewModel.swift
//  Omicron
//
//  Created by Axity on 29/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import RxSwift
import RxCocoa
import Moya // Borrar cuando se consuma bien el servicio
class RootViewModel {
    
    // MARK: Variables
    var assignedOrders: [Order] = []
    var inProcessOrdes: [Order] = []
    var penddingOrders: [Order] = []
    var finishedOrders: [Order] = []
    var reassignedOrders: [Order] = []
    
    public var dataStatus: BehaviorSubject<[Section]> = BehaviorSubject(value: [])
    var loading: BehaviorSubject<Bool> = BehaviorSubject(value: false)
    var error: PublishSubject<String> = PublishSubject()
    let disposeBag = DisposeBag()
    var refreshDataWhenChangeProcessIsSucces = PublishSubject<Void>()
    
    init() {
    }

    // MARK: Functions

    func getOrders() -> Void {
        if let userData = Persistence.shared.getUserData(), let userId = userData.id {
            self.loading.onNext(true)
            NetworkManager.shared.getStatusList(qfbId: userId).subscribe(onNext: { [weak self] res in
                
                for status in res.response!.status! {
                    switch status.statusId {
                    case 1:
                        self!.assignedOrders = status.orders!
                    case 2:
                        self!.inProcessOrdes = status.orders!
                    case 3:
                        self!.penddingOrders = status.orders!
                    case 4:
                        self!.finishedOrders = status.orders!
                    case 5:
                        self!.reassignedOrders = status.orders!
                    default:
                        print("")
                    }
                }
                let data = [
                    Section(statusName: StatusNameConstants.assignedStatus, numberTask:  self!.assignedOrders.count, imageIndicatorStatus: IndicatorImageStatus.assigned, orders: self!.assignedOrders),
                    Section(statusName: StatusNameConstants.inProcessStatus, numberTask: self!.inProcessOrdes.count, imageIndicatorStatus: IndicatorImageStatus.inProcess, orders: self!.inProcessOrdes),
                    Section(statusName: StatusNameConstants.penddingStatus, numberTask: self!.penddingOrders.count, imageIndicatorStatus: IndicatorImageStatus.pendding, orders:  self!.penddingOrders),
                    Section(statusName: StatusNameConstants.finishedStatus, numberTask: self!.finishedOrders.count, imageIndicatorStatus: IndicatorImageStatus.finished, orders: self!.finishedOrders),
                    Section(statusName: StatusNameConstants.reassignedStatus, numberTask: self!.reassignedOrders.count, imageIndicatorStatus: IndicatorImageStatus.reassined, orders: self!.reassignedOrders)
                ]
                self!.dataStatus.onNext(data)
                self!.loading.onNext(false)
            }, onError: { err in
                print(err)
                self.error.onNext("Hubo un error al cargar las ordenes de fabricación, por favor intentarlo de nuevo")
                self.loading.onNext(false)
            }).disposed(by: disposeBag)
        } else {
            self.error.onNext("Hubo un error al cargar las ordenes de fabricación, por favor intentarlo de nuevo")
        }
    }
}

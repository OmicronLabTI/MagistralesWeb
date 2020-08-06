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
    
    public var dataStatus: BehaviorRelay<[Section]> = BehaviorRelay(value: [])
    let disposeBag = DisposeBag()
    
    init() {
        NetworkManager.shared.getStatusList(qfbId: StatusRequest.init(qfbId: 1)).subscribe(onNext: { res in
            for status in res.status! {
                switch status.statusId {
                case 1:
                    self.assignedOrders = status.orders!
                case 2:
                    self.inProcessOrdes = status.orders!
                case 3:
                    self.penddingOrders = status.orders!
                case 4:
                    self.finishedOrders = status.orders!
                case 5:
                    self.reassignedOrders = status.orders!
                default:
                    print("")
                }
            }
            let data = [
                Section(statusName: StatusNameConstants.assignedStatus, numberTask:  self.assignedOrders.count, imageIndicatorStatus: IndicatorImageStatus.assigned, orders: self.assignedOrders),
                Section(statusName: StatusNameConstants.inProcessStatus, numberTask: self.inProcessOrdes.count, imageIndicatorStatus: IndicatorImageStatus.inProcess, orders: self.inProcessOrdes),
                Section(statusName: StatusNameConstants.penddingStatus, numberTask: self.penddingOrders.count, imageIndicatorStatus: IndicatorImageStatus.pendding, orders:  self.penddingOrders),
                Section(statusName: StatusNameConstants.finishedStatus, numberTask: self.finishedOrders.count, imageIndicatorStatus: IndicatorImageStatus.finished, orders: self.finishedOrders),
                Section(statusName: StatusNameConstants.reassignedStatus, numberTask: self.reassignedOrders.count, imageIndicatorStatus: IndicatorImageStatus.reassined, orders: self.reassignedOrders)
            ]
            self.dataStatus.accept(data)
        }).disposed(by: disposeBag)
    }
}

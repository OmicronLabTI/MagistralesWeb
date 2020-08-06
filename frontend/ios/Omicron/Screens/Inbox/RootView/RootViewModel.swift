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
                Section(statusName: "Asignadas", numberTask:  self.assignedOrders.count, imageIndicatorStatus: "assignedStatus"),
                Section(statusName: "En Proceso", numberTask: self.inProcessOrdes.count, imageIndicatorStatus: "processStatus"),
                Section(statusName: "Pendientes", numberTask: self.penddingOrders.count, imageIndicatorStatus: "pendingStatus"),
                Section(statusName: "Terminado", numberTask: self.finishedOrders.count, imageIndicatorStatus: "finishedStatus"),
                Section(statusName: "Reasignado", numberTask: self.reassignedOrders.count, imageIndicatorStatus: "reassignedStatus")
            ]
            self.dataStatus.accept(data)
        }).disposed(by: disposeBag)
    }
}

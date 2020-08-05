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
    
    public let dataStatus: PublishSubject<[Section]> = PublishSubject()
    let disposeBag = DisposeBag()
    
    init() {
        
        let manager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
        let statusRequest: StatusRequest = StatusRequest(qfbId: 1)
        manager.getStatusList(qfbId: statusRequest).subscribe(onNext: { res in

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
        }).disposed(by: disposeBag)
        
    }
    
}

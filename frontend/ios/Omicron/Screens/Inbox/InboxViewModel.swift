//
//  InboxViewModel.swift
//  Omicron
//
//  Created by Axity on 24/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa

class  InboxViewModel {
    var finishedDidTap = PublishSubject<Void>()
    var pendingDidTap = PublishSubject<Void>()
    var processDidTap = PublishSubject<Void>()
    var indexSelectedOfTable = PublishSubject<Int>()
    var statusData: BehaviorSubject<[Order]> = BehaviorSubject(value: [])
    var ordersTemp: [Order] = []
    var loading =  PublishSubject<Bool>()
    var showConfirmationAlerChangeStatusProcess = PublishSubject<String>()
    var refreshDataWhenChangeProcessIsSucces = PublishSubject<Void>()
    var showAlert = PublishSubject<String>()
    var title = PublishSubject<String>()
    weak var selectedOrder: Order?

    var disposeBag = DisposeBag();
    init() {
        // Funcionalidad para el botón de Terminar
        finishedDidTap.subscribe(onNext: { () in
        }).disposed(by: disposeBag)
        
        // Funcionalidad para el botón de pendiente
        pendingDidTap.subscribe(onNext: {
        }).disposed(by: disposeBag)
        
        // Funcionalidad para el botón de En Proceso
        processDidTap.subscribe(onNext: {
             self.showConfirmationAlerChangeStatusProcess.onNext("La orden cambiará a estatus En proceso ¿quieres continuar?")
            }).disposed(by: disposeBag)
    }
    
    func setSelection(section: SectionOrder) -> Void {
        let ordering = section.orders.sorted  {
            switch ($0, $1) {
            // Order errors by code
            case let (aCode, bCode):
                return aCode.baseDocument! < bCode.baseDocument!
            }
        }

        self.statusData.onNext(ordering)
        self.title.onNext(section.statusName)
        self.ordersTemp = ordering
    }
    
    func setFilter(orders: [Order]) -> Void {
        let ordering = orders.sorted  {
            switch ($0, $1) {
            // Order errors by code
            case let (aCode, bCode):
                return aCode.baseDocument! < bCode.baseDocument!
            }
        }

        self.statusData.onNext(ordering)
        self.title.onNext("Búsqueda")
        self.ordersTemp = ordering
    }
    
    func changeStatus(indexPath: [IndexPath]) -> Void {
        self.loading.onNext(true)
        var orders:[ChangeStatusRequest] = []
        for index in indexPath {
            let order = ChangeStatusRequest(userId: (Persistence.shared.getUserData()?.id)!, orderId: ordersTemp[index.row].productionOrderId!, status: "Proceso")
            orders.append(order)
        }
        
        NetworkManager.shared.changeStatusOrder(changeStatusRequest: orders).observeOn(MainScheduler.instance).subscribe(onNext: {_ in
            self.loading.onNext(false)
            self.refreshDataWhenChangeProcessIsSucces.onNext(())
        }, onError: { error in
            self.loading.onNext(false)
            self.showAlert.onNext("Ocurrió un error al cambiar de estatus la orden, por favor intente de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func getStatusName(index: Int) -> String {
        switch index {
        case 0:
            return StatusNameConstants.assignedStatus
        case 1:
            return StatusNameConstants.inProcessStatus
        case 2:
            return StatusNameConstants.penddingStatus
        case 3:
            return StatusNameConstants.finishedStatus
        case 4:
            return StatusNameConstants.reassignedStatus
        default:
            return ""
        }
    }
    
    func getStatusName(id: Int) -> String {
        switch id {
        case 1:
            return StatusNameConstants.assignedStatus
        case 2:
            return StatusNameConstants.inProcessStatus
        case 3:
            return StatusNameConstants.penddingStatus
        case 4:
            return StatusNameConstants.finishedStatus
        case 5:
            return StatusNameConstants.reassignedStatus
        default:
            return ""
        }
    }
    
    
    func getStatusId(name: String) -> Int {
        switch name {
        case StatusNameConstants.assignedStatus:
            return 1
        case StatusNameConstants.inProcessStatus:
            return 2
        case StatusNameConstants.penddingStatus:
            return 3
        case StatusNameConstants.finishedStatus:
            return 4
        case StatusNameConstants.reassignedStatus:
            return 5
        default:
            return -1
        }
    }
}

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
import RxDataSources

class  InboxViewModel {
    var finishedDidTap = PublishSubject<Void>()
    var pendingDidTap = PublishSubject<Void>()
    var processDidTap = PublishSubject<Void>()
    var indexSelectedOfTable = PublishSubject<Int>()
    var statusData: BehaviorSubject<[Order]> = BehaviorSubject(value: [])
    var statusDataGrouped: BehaviorSubject<[SectionModel<String, Order>]> = BehaviorSubject(value: [])
    var ordersTemp: [Order] = []
    var loading =  PublishSubject<Bool>()
    var showConfirmationAlerChangeStatusProcess = PublishSubject<String>()
    var refreshDataWhenChangeProcessIsSucces = PublishSubject<Void>()
    var showAlert = PublishSubject<String>()
    var title = PublishSubject<String>()
    weak var selectedOrder: Order?
    var disposeBag = DisposeBag();
    var similarityViewButtonDidTap = PublishSubject<Void>()
    var similarityViewButtonIsEnable = PublishSubject<Bool>()
    var normalViewButtonDidTap = PublishSubject<Void>()
    var normalViewButtonIsEnable = PublishSubject<Bool>()
    
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
        
        // Funcionalidad para agrupar los cards por similitud
        similarityViewButtonDidTap.subscribe(onNext: { [weak self] _ in
            if (self?.ordersTemp != nil) {
                let ordering = self?.sortByBaseBocumentAscending(orders: self!.ordersTemp)
                
                for order in ordering! {
                    let itemCodeInArray = order.itemCode?.components(separatedBy: "   ")
                    if let codeProduct = itemCodeInArray?.first {
                        order.productCode = codeProduct
                    } else {
                        order.productCode = ""
                    }
                }
                
                let  groupBySimilarity = Dictionary(grouping: ordering!, by: {$0.productCode})
                
                let sectionsModels = groupBySimilarity.map({ (orders) -> SectionModel<String, Order> in
                    
                    return SectionModel(model: orders.key ?? "", items: orders.value)
                    
                })
                
                self?.statusDataGrouped.onNext(sectionsModels)

            }
            
            self?.similarityViewButtonIsEnable.onNext(false)
            self?.normalViewButtonIsEnable.onNext(true)
        }).disposed(by: self.disposeBag)
        
        // Funcionalidad para mostrar la vista normal en los cards
        normalViewButtonDidTap.subscribe(onNext: { [weak self] _ in
            let ordering = self?.sortByBaseBocumentAscending(orders: self!.ordersTemp)
            self?.statusData.onNext(ordering ?? [])
            self?.similarityViewButtonIsEnable.onNext(true)
            self?.normalViewButtonIsEnable.onNext(false)
        }).disposed(by: self.disposeBag)
    }
    
    func setSelection(section: SectionOrder) -> Void {
        let ordering = self.sortByBaseBocumentAscending(orders: section.orders)
//        let ordering = section.orders.sorted  {
//            switch ($0, $1) {
//            // Order errors by code
//            case let (aCode, bCode):
//                return aCode.baseDocument! < bCode.baseDocument!
//            }
//        }

        self.statusData.onNext(ordering)
        self.title.onNext(section.statusName)
        self.ordersTemp = ordering
    }
    
    func setFilter(orders: [Order]) -> Void {
        let ordering = self.sortByBaseBocumentAscending(orders: orders)
//        let ordering = orders.sorted  {
//            switch ($0, $1) {
//            // Order errors by codeorders
//            case let (aCode, bCode):
//                return aCode.baseDocument! < bCode.baseDocument!
//            }
//        }

        self.statusData.onNext(ordering)
        self.title.onNext("Búsqueda")
        self.ordersTemp = ordering
    }
    
    func sortByBaseBocumentAscending(orders: [Order]) -> [Order]{
        orders.sorted  {
            switch ($0, $1) {
            case let (aCode, bCode):
                return aCode.baseDocument! < bCode.baseDocument!
            }
        }
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

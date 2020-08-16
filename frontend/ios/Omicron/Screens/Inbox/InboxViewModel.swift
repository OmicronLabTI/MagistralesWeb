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
    
    var finishedDidTab = PublishSubject<Void>();
    var pendingDidTab = PublishSubject<Void>();
    var processDidTab = PublishSubject<Void>();
    var indexSelectedOfTable = PublishSubject<Int>()
    var statusData: BehaviorRelay<[Order]> = BehaviorRelay(value: [])
    var nameStatus: BehaviorRelay<String> = BehaviorRelay(value: "")
    var validateStatusData: BehaviorRelay<ValidStatusData> = BehaviorRelay(value: ValidStatusData(indexStatusSelected: -1, orders: []))
    let rootViewModel = RootViewModel()
    var ordersTemp: [Order] = []
    var loading =  PublishSubject<Bool>()
    var showConfirmationAlerChangeStatusProcess = PublishSubject<String>()
    var showAlert = PublishSubject<String>()

    var disposeBag = DisposeBag();
    init() {
        // Funcionalidad para el botón de Terminar
        finishedDidTab.subscribe(onNext: { () in
        }).disposed(by: disposeBag)
        
        // Funcionalidad para el botón de pendiente
        pendingDidTab.subscribe(onNext: {
        }).disposed(by: disposeBag)
        
        // Funcionalidad para el botón de En Proceso
        processDidTab.subscribe(onNext: {
             self.showConfirmationAlerChangeStatusProcess.onNext("La orden cambiará a estatus En proceso ¿quieres continuar?")
            }).disposed(by: disposeBag)
        
        rootViewModel.dataStatus.subscribe(onNext: { data in
            if let assignedData = data.first?.orders {
                self.statusData.accept(assignedData)
                self.validateStatusData.accept(ValidStatusData(indexStatusSelected: 0, orders: assignedData))
            }
        }).disposed(by: disposeBag)
    }
    
    func setSelection(index: Int, section: Section) -> Void {
        self.indexSelectedOfTable.onNext(index)
        self.statusData.accept(section.orders)
        self.nameStatus.accept(section.statusName)
        self.ordersTemp = section.orders
        self.validateStatusData.accept(ValidStatusData(indexStatusSelected: index, orders: section.orders))
    }
    
    func changeStatus(indexPath: [IndexPath]) -> Void {
        self.loading.onNext(true)
        var orders:[ChangeStatusRequest] = []
        for index in indexPath {
            let order = ChangeStatusRequest(userId: (Persistence.shared.getUserData()?.id)!, orderId: ordersTemp[index.row].productionOrderId!, status: "Proceso")
            orders.append(order)
        }
        
        for index in indexPath {
            ordersTemp.remove(at: index.row)
        }
        self.statusData.accept(ordersTemp)
        
        NetworkManager.shared.changeStatusOrder(changeStatusRequest: orders).observeOn(MainScheduler.instance).subscribe(onNext: {_ in
            self.loading.onNext(false)
            self.rootViewModel.refreshDataWhenChangeProcessIsSucces.onNext(())
        }, onError: { error in
            self.loading.onNext(false)
            self.showAlert.onNext("Ocurrió un error al cambiar de estatus la orden, por favor intente de nuevo")
        }).disposed(by: self.disposeBag)
    }
}

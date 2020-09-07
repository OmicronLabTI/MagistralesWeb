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
    var normalViewDidTap = PublishSubject<Void>()
    var similarityViewDidTap = PublishSubject<Void>()
    var indexSelectedOfTable = PublishSubject<Int>()
    var statusData: BehaviorRelay<[Order]> = BehaviorRelay(value: [])
    var nameStatus: BehaviorRelay<String> = BehaviorRelay(value: "")
    var validateStatusData: BehaviorRelay<ValidStatusData> = BehaviorRelay(value: ValidStatusData(indexStatusSelected: -1, orders: []))
    let rootViewModel = RootViewModel()
    var ordersTemp: [Order] = []
    var loading =  PublishSubject<Bool>()
    var showConfirmationAlerChangeStatusProcess = PublishSubject<String>()
    var refreshDataWhenChangeProcessIsSucces = PublishSubject<Void>()
    var showAlert = PublishSubject<String>()

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
        
        rootViewModel.dataStatus.subscribe(onNext: { data in
            if let assignedData = data.first?.orders {
                self.statusData.accept(assignedData)
                self.validateStatusData.accept(ValidStatusData(indexStatusSelected: 0, orders: assignedData))
            }
        }).disposed(by: disposeBag)
        
        self.similarityViewDidTap.subscribe(onNext: { [weak self]  _ in
            print("Se pintó el botón para agruopar por similarydad ")
        }).disposed(by: self.disposeBag)
        
        
        self.normalViewDidTap.subscribe(onNext: { [weak self] _ in
            print("Se pintó el botón para mostra vista normal")
        }).disposed(by: self.disposeBag)
        
    }
    
    func setSelection(index: Int, section: SectionOrder) -> Void {
        let ordering = section.orders.sorted  {
            switch ($0, $1) {
            // Order errors by code
            case let (aCode, bCode):
                return aCode.baseDocument! < bCode.baseDocument!
            }
        }
        self.indexSelectedOfTable.onNext(index)
        self.statusData.accept(ordering)
        self.nameStatus.accept(section.statusName)
        self.ordersTemp = ordering
        self.validateStatusData.accept(ValidStatusData(indexStatusSelected: index, orders: ordering))
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
}

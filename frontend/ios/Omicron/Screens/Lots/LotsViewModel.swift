//
//  LotsViewModel.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxCocoa
import RxSwift

class LotsViewModel {
    
    // MARK: Variables
    var loading = PublishSubject<Bool>()
    var showMessage = PublishSubject<String>()
    var orderId = -1
    var disposeBag = DisposeBag()
    var dataOfLots = BehaviorSubject<[Lots]>(value: [])
    var dataLotsAvailable = BehaviorSubject<[LotsAvailable]>(value: [])
    var dataLotsSelected = BehaviorSubject<[LotsSelected]>(value: [])
    init() {
        
    }
    
    // MARK: -Functions
    func getLots() -> Void {
        self.loading.onNext(true)
        NetworkManager.shared.getLots(orderId: orderId).observeOn(MainScheduler.instance).subscribe(onNext: { data in
            self.loading.onNext(false)
            if let lotsData = data.response {
                self.dataOfLots.onNext(lotsData)
                if let lots = lotsData.first {
                    self.dataLotsAvailable.onNext(lots.lotesDisponibles!)
                    self.dataLotsSelected.onNext(lots.lotesSelecionados!)
                }
            }
        }, onError: { error in
            self.loading.onNext(false)
            self.showMessage.onNext("Hubo un error al cargar los lotes, por favor de intentarlo de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func itemSelectedOfLineDocTable(lot: Lots) -> Void {
        print("lote selecionado \(lot)")
        if (lot.lotesDisponibles!.count > 0) {
              self.dataLotsAvailable.onNext(lot.lotesDisponibles!)
        } else {
            self.dataLotsAvailable.onNext([])
        }
        
        if(lot.lotesSelecionados!.count > 0) {
            self.dataLotsSelected.onNext(lot.lotesSelecionados!)
        } else {
             self.dataLotsSelected.onNext([])
        }
    }
}

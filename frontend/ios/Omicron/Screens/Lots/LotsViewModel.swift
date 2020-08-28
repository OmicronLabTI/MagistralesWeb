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
import Resolver

class LotsViewModel {
    
    // MARK: Variables
    var loading = PublishSubject<Bool>()
    var showMessage = PublishSubject<String>()
    var orderId = -1
    var disposeBag = DisposeBag()
    var dataOfLots = BehaviorSubject<[Lots]>(value: [])
    var dataLotsAvailable = BehaviorSubject<[LotsAvailable]>(value: [])
    var dataLotsSelected = BehaviorSubject<[LotsSelected]>(value: [])
    var addLotDidTap = PublishSubject<Void>()
    var removeLotDidTap = PublishSubject<Void>()
    var quantitySelectedValue = ""
    
    var quantitySelectedInput = BehaviorSubject<String>(value: "")
    var rowSelected = PublishSubject<Int>()
    
    var lineDocumentsDataAux:[Lots] = []
    var lotsAvailablesAux:[LotsAvailable] = []
    var lotsSelectedAux:[LotsSelected] = []
    
    
    init() {
    
        // Obtiene los valores para poder añadir lotes disponibles a seleccionados por checar
        let inputs = Observable.combineLatest(rowSelected, quantitySelectedInput)
        
        self.addLotDidTap.withLatestFrom(inputs).map({
            LotsAvailableInfo(row: $0, quantitySelected: $1)
        }).subscribe(onNext: { data in
            let lotSelected = LotsSelected(numeroLote: self.lotsAvailablesAux[data.row].numeroLote!, cantidadSeleccionada:  Double(data.quantitySelected) ?? 0.0, sysNumber: self.lotsAvailablesAux[data.row].sysNumber!)

            let index = self.lotsSelectedAux.firstIndex(where: ({$0.numeroLote == lotSelected.numeroLote}))
            if((index) != nil) {
                self.lotsSelectedAux[index!].cantidadSeleccionada = lotSelected.cantidadSeleccionada
            } else {
                self.lotsSelectedAux.append(lotSelected)
            }
            self.dataLotsSelected.onNext(self.lotsSelectedAux)
        }).disposed(by: self.disposeBag)
        
        self.removeLotDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { itemSelected in
            
        }).disposed(by: self.disposeBag)
        
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
                    self.lineDocumentsDataAux = lotsData
                    self.lotsAvailablesAux = lotsData[0].lotesDisponibles!
                    self.lotsSelectedAux = lotsData[0].lotesSelecionados!
                }
            }
        }, onError: { error in
            self.loading.onNext(false)
            self.showMessage.onNext("Hubo un error al cargar los lotes, por favor de intentarlo de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func itemSelectedOfLineDocTable(lot: Lots) -> Void {
        if (lot.lotesDisponibles!.count > 0) {
              self.dataLotsAvailable.onNext(lot.lotesDisponibles!)
                self.lotsAvailablesAux = lot.lotesDisponibles!
        } else {
            self.dataLotsAvailable.onNext([])
             self.lotsAvailablesAux = []
        }
        
        if(lot.lotesSelecionados!.count > 0) {
            self.dataLotsSelected.onNext(lot.lotesSelecionados!)
            self.lotsSelectedAux = lot.lotesSelecionados!
        } else {
             self.dataLotsSelected.onNext([])
            self.lotsSelectedAux = lot.lotesSelecionados!
        }
    }
    
    func getLotOfLotsAvailableTable(lot: LotsAvailable) -> Void {
        print("Value cuando se seleccionó un elemento \(self.quantitySelectedValue)")
        print("Objecto completo de lotes disponibles: \(lot)")
    }
}

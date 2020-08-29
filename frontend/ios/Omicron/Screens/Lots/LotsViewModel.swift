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
    
    var itemSelectedLineDocuments: Int = 0
    var itemLotSelected:LotsSelected? = nil
    
    init() {
    
        // Obtiene los valores para poder añadir lotes disponibles a seleccionados por checar
        let inputs = Observable.combineLatest(rowSelected, quantitySelectedInput)
        
        self.addLotDidTap.withLatestFrom(inputs).map({
            LotsAvailableInfo(row: $0, quantitySelected: $1)
        }).subscribe(onNext: { data in
            
            print("elemento selecionado: \(self.itemSelectedLineDocuments)")
            
            
            let lotSelected = LotsSelected(numeroLote: self.lotsAvailablesAux[data.row].numeroLote!, cantidadSeleccionada:  Double(data.quantitySelected) ?? 0.0, sysNumber: self.lotsAvailablesAux[data.row].sysNumber!)

            if((lotSelected.cantidadSeleccionada! <= self.lotsAvailablesAux[data.row].cantidadDisponible!) && ( lotSelected.cantidadSeleccionada! <= self.lotsAvailablesAux[data.row].cantidadSeleccionada! )) {
                print("Si se pasa")

                let index = self.lotsSelectedAux.firstIndex(where: ({$0.numeroLote == lotSelected.numeroLote}))
                if((index) != nil) {
                    self.lotsSelectedAux[index!].cantidadSeleccionada! += lotSelected.cantidadSeleccionada!
                } else {
                    self.lotsSelectedAux.append(lotSelected)
                }
                
                self.lotsAvailablesAux[data.row].cantidadDisponible! -= lotSelected.cantidadSeleccionada!
                self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalNecesario! -= lotSelected.cantidadSeleccionada!
                self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalSeleccionado! += lotSelected.cantidadSeleccionada!
                self.dataOfLots.onNext(self.lineDocumentsDataAux)
                self.dataLotsSelected.onNext(self.lotsSelectedAux)
                self.dataLotsAvailable.onNext(self.lotsAvailablesAux)
            }
            
        }).disposed(by: self.disposeBag)
        
        self.removeLotDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { itemSelected in
            let index = self.lotsSelectedAux.firstIndex(where: ({$0.numeroLote == self.itemLotSelected?.numeroLote}))
            if (index != nil) {
                
                if let indexLotAvailable = self.lotsAvailablesAux.firstIndex(where: ({$0.numeroLote == self.itemLotSelected?.numeroLote})) {
                    self.lotsAvailablesAux[indexLotAvailable].cantidadDisponible! += self.itemLotSelected!.cantidadSeleccionada!
                    self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalNecesario! += self.itemLotSelected!.cantidadSeleccionada!
                    self.lineDocumentsDataAux[self.itemSelectedLineDocuments].totalSeleccionado! -= self.itemLotSelected!.cantidadSeleccionada!
                    self.dataOfLots.onNext(self.lineDocumentsDataAux)
                    self.dataLotsAvailable.onNext(self.lotsAvailablesAux)
                    self.lotsSelectedAux.remove(at: index!)
                    self.dataLotsSelected.onNext(self.lotsSelectedAux)
                }
            }
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
                    // Se asignan los valores a cada lote disponible su cantidad sugerida (total necesario)
                    for lotData in lotsData {
                        for lot in lotData.lotesDisponibles!{
                            lot.cantidadSeleccionada = lotData.totalNecesario
                        }
                    }
                    for lot in lotsData[0].lotesDisponibles! {
                        lot.cantidadSeleccionada = lotsData[0].totalNecesario
                    }
                    
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

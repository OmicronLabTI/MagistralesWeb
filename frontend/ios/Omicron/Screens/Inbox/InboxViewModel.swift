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
//    var finishedButtonIsHidden: Driver<Bool>
//    var pendingButtonIsHidden: Driver<Bool>
//    var processButtonIsHidden: Driver<Bool>
    
    
    var disposeBag = DisposeBag();
    init() {
        // Funcionalidad para el botón de Terminar
        finishedDidTab.subscribe(onNext: { () in
            print("Se oprmió el botón de terminar")
        }).disposed(by: disposeBag)
        
        // Funcionalidad para el botón de pendiente
        pendingDidTab.subscribe(onNext: {
            print("Boton pendiente")
        }).disposed(by: disposeBag)
        
        // Funcionalidad para el botón de En Proceso
        processDidTab.subscribe(onNext: {
            print("Botón de proceso")
            }).disposed(by: disposeBag)
    }
    
    func setSelection(index: Int) -> Void {
        
         //Aqui se consume servicio
//        let assigned =
//        [
//            Orden(No: 1, BaseDocument: "SFJDFJDF123", Container: "Envasd", Tag: "34", PlannedQuantity: "Envase", startDate: "27/03/1994", finishDate: "27/03/1994", descriptionProduct: "Descripción del producto")
//        ]
        print("Index desde table: \(index)")
        self.indexSelectedOfTable.onNext(index)
    }
}

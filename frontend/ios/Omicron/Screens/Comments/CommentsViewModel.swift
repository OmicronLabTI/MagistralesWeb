//
//  ComentsViewModel.swift
//  Omicron
//
//  Created by Axity on 14/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import RxSwift
import RxSwift
import UIKit

class CommentsViewModel {
    
        // MARK: - Variables
    var cancelDidTap = PublishSubject<Void>()
    var aceptDidTap = PublishSubject<Void>()
    var disposeBag = DisposeBag()
    
    init() {
        self.cancelDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { _ in
            print("Se apretó el botón de cancelar")
        }).disposed(by: self.disposeBag)
        
        self.aceptDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { _ in
            print("Se apretó el botón de aceptar")
        }).disposed(by: self.disposeBag)
    }
}

//
//  SplitViewController.swift
//  Omicron
//
//  Created by Axity on 30/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa

class SplitViewController: UISplitViewController {

    let disposeBag: DisposeBag = DisposeBag()
    override func viewDidLoad() {
        super.viewDidLoad()
        let username = Persistence.shared.getUserName()
        NetworkManager.shared.getInfoUser(userId: username).subscribe(onNext: { res in
            Persistence.shared.saveUserData(user: res.response!)
        }).disposed(by: self.disposeBag)
    }
}

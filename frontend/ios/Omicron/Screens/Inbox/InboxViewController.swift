//
//  InboxViewController.swift
//  Omicron
//
//  Created by Axity on 24/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa

class InboxViewController: UIViewController {
    
    // MARK: Outlets
    @IBOutlet weak var statusNameLabel: UILabel!
    @IBOutlet weak var finishedButton: UIButton!
    @IBOutlet weak var pendingButton: UIButton!
    @IBOutlet weak var processButton: UIButton!
    @IBOutlet weak var collectionView: UICollectionView!
    
    // MARK:  Variables
    lazy var inboxModel: InboxViewModel = InboxViewModel()
    let disposeBag = DisposeBag()
    private let cardWidth = UIScreen.main.bounds.width / 2.5
    
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        viewModelBinding()
        self.initComponents()
        collectionView.dataSource = self
        collectionView.delegate = self
        collectionView.register(UINib(nibName:
            ViewControllerIdentifiers.cardCollectionViewCell, bundle: nil), forCellWithReuseIdentifier: ViewControllerIdentifiers.cardReuseIdentifier)
        finishedButton.isHidden = true
    }
    
    // MARK: Functions
    func viewModelBinding() -> Void {
        [
            finishedButton.rx.tap.bind(to: inboxModel.finishedDidTab),
            pendingButton.rx.tap.bind(to: inboxModel.pendingDidTab),
            processButton.rx.tap.bind(to: inboxModel.processDidTab)
        ].forEach({ $0.disposed(by: disposeBag) })
     
        inboxModel.indexSelectedOfTable
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { res in
                self.chageStatusName(index: res)
                self.hideButtons(index: res)
            }).disposed(by: disposeBag)
    }
    
    func initComponents() -> Void {
        self.statusNameLabel.text = "Asignadas"
        self.statusNameLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 39)
        self.setStyleButton(button: self.finishedButton, title: "Terminado", color: OmicronColors.finishedStatus)
        self.setStyleButton(button: self.pendingButton, title: "Pendiente", color: OmicronColors.pendingStatus)
        self.setStyleButton(button: self.processButton, title: "En proceso", color: OmicronColors.processStatus)
    }
    
    func setStyleButton( button: UIButton ,title: String, color: UIColor) {
        button.setTitle(title, for: .normal)
        button.setTitleColor(color, for: .normal)
        button.layer.borderWidth = 1
        button.layer.cornerRadius = 10
        button.layer.borderColor = color.cgColor
        button.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayBold, size: 16)
    }
    
    func chageStatusName(index: Int) -> Void {
        switch index {
        case 0:
            self.statusNameLabel.text = "Asignadas"
        case 1:
            self.statusNameLabel.text = "En Proceso"
        case 2:
            self.statusNameLabel.text = "Pendientes"
        case 3:
            self.statusNameLabel.text = "Terminado"
        case 4:
            self.statusNameLabel.text = "Reasignado"
        default:
            print("")
        }
    }
    
    private func hideButtons(index: Int) {
        switch index {
        case 0:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: false, finishedButtonIsHidden: true, pendingButtonIsHidden: false)
        case 1:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: false, pendingButtonIsHidden: false)
        case 2:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: false, finishedButtonIsHidden: true, pendingButtonIsHidden: false)
        case 3:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        case 4:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: false, pendingButtonIsHidden: true)
        default:
            print("")
        }
    }
    
    private func changePropertyIsHiddenStatusButtons(processButtonIsHidden: Bool, finishedButtonIsHidden: Bool, pendingButtonIsHidden: Bool) -> Void {
        self.processButton.isHidden = processButtonIsHidden
        self.finishedButton.isHidden = finishedButtonIsHidden
        self.pendingButton.isHidden = pendingButtonIsHidden
    }
}


extension InboxViewController: UICollectionViewDataSource {
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        return 0
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "card", for: indexPath) as? CardCollectionViewCell
        return cell!
    }
}

extension InboxViewController: UICollectionViewDelegate {
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
        
    }
}

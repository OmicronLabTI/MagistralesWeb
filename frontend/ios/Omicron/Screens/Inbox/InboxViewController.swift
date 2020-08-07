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
    lazy var inboxViewModel: InboxViewModel = InboxViewModel()
    let disposeBag = DisposeBag()
    private let cardWidth = UIScreen.main.bounds.width / 2.5
    private var typeCard: Int = 0
    let rootViewModel = RootViewModel();
    
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        viewModelBinding()
        self.initComponents()
        collectionView.register(UINib(nibName:
            ViewControllerIdentifiers.cardCollectionViewCell, bundle: nil), forCellWithReuseIdentifier: ViewControllerIdentifiers.cardReuseIdentifier)
        finishedButton.isHidden = true
    }
        
    // MARK: Functions
    func viewModelBinding() -> Void {

        collectionView.rx.itemSelected.subscribe(onNext:{ _ in
            print("Se ejecutó")

            self.performSegue(withIdentifier: ViewControllerIdentifiers.orderDetailViewController, sender: nil)
//                let vc  = UIStoryboard.init(name: ViewControllerIdentifiers.storieboardName, bundle: Bundle.main).instantiateViewController(identifier: ViewControllerIdentifiers.orderDetailViewController) as! OrderDetailViewController
//                self.navigationController?.pushViewController(vc, animated: true)
            }).disposed(by: disposeBag)

        
        [
            finishedButton.rx.tap.bind(to: inboxViewModel.finishedDidTab),
            pendingButton.rx.tap.bind(to: inboxViewModel.pendingDidTab),
            processButton.rx.tap.bind(to: inboxViewModel.processDidTab)
        ].forEach({ $0.disposed(by: disposeBag) })
     
        inboxViewModel.indexSelectedOfTable
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { res in
                self.chageStatusName(index: res)
                self.hideButtons(index: res)
                self.typeCard = res
            }).disposed(by: disposeBag)
        
        inboxViewModel.statusData.bind(to: self.collectionView.rx.items(cellIdentifier: ViewControllerIdentifiers.cardReuseIdentifier, cellType: CardCollectionViewCell.self)) { row, data, cell in
            self.changepropertiesOfCard(cell: cell)
            cell.numberDescriptionLabel.text = "\(data.no!)"
            cell.baseDocumentDescriptionLabel.text = data.baseDocument!
            cell.containerDescriptionLabel.text = data.container!
            cell.tagDescriptionLabel.text = data.tag!
            cell.plannedQuantityDescriptionLabel.text = data.plannedQuantity!
            cell.startDateDescriptionLabel.text = data.startDate!
            cell.finishDateDescriptionLabel.text = data.finishDate!
            cell.productDescriptionLabel.text = data.descriptionProduct!
        }.disposed(by: disposeBag)
        
        inboxViewModel.validateStatusData.observeOn(MainScheduler.instance).subscribe(onNext: { data in
            var message: String = ""
            if(data.orders.count == 0 && data.indexStatusSelected >= 0) {
                switch data.indexStatusSelected {
                case 0:
                    message = "No tienes ordenes Asignadas"
                case 1:
                    message = "No tienes ordenes En proceso"
                case 2:
                    message = "No tienes ordenes Pendientes"
                case 3:
                    message = "No tienes ordenes Terminadas"
                case 4:
                    message = "No tienes ordenes Reasignadas"
                default:
                    message = ""
                }
            }
            self.collectionView.setEmptyMessage(message)
        }).disposed(by: disposeBag)
    }
    
    func changepropertiesOfCard(cell: CardCollectionViewCell) {
        switch self.typeCard {
        case 0:
            self.propertyCard(cell: cell, borderColor: OmicronColors.assignedStatus, iconName: ButtonNameOfCards.assigned)
        case 1:
            self.propertyCard(cell: cell, borderColor: OmicronColors.processStatus, iconName: ButtonNameOfCards.inProcess)
        case 2:
            self.propertyCard(cell: cell, borderColor: OmicronColors.pendingStatus, iconName: ButtonNameOfCards.pendding)
        case 3:
            self.propertyCard(cell: cell, borderColor: OmicronColors.finishedStatus, iconName: ButtonNameOfCards.finished)
        case 4:
            self.propertyCard(cell: cell, borderColor: OmicronColors.reassignedStatus, iconName: ButtonNameOfCards.reasigned)
        default:
            print("")
        }
    }
    
    func propertyCard(cell: CardCollectionViewCell, borderColor: UIColor, iconName: String) {
        cell.assignedStyleCard(color: borderColor.cgColor)
        cell.changeIconButton(iconName: iconName)
    }
    
    func initComponents() -> Void {
        self.statusNameLabel.text = StatusNameConstants.assignedStatus
        self.statusNameLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 39)
        self.setStyleButton(button: self.finishedButton, title: StatusNameConstants.finishedStatus, color: OmicronColors.finishedStatus)
        self.setStyleButton(button: self.pendingButton, title: StatusNameConstants.penddingStatus, color: OmicronColors.pendingStatus)
        self.setStyleButton(button: self.processButton, title: StatusNameConstants.inProcessStatus, color: OmicronColors.processStatus)
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
            self.statusNameLabel.text = StatusNameConstants.assignedStatus
        case 1:
            self.statusNameLabel.text = StatusNameConstants.inProcessStatus
        case 2:
            self.statusNameLabel.text = StatusNameConstants.penddingStatus
        case 3:
            self.statusNameLabel.text = StatusNameConstants.finishedStatus
        case 4:
            self.statusNameLabel.text = StatusNameConstants.reassignedStatus
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

//func prepare(for segue: UIStoryboardSegue, sender: Any?) {
//   if segue.identifier == "segueToNext" {
//       if let destination = segue.destination as? OrderDetailViewController {
//           destination.nomb = nombres // you can pass value to destination view controller
//       }
//   }
//}

extension UICollectionView {

    func setEmptyMessage(_ message: String) {
        let messageLabel = UILabel(frame: CGRect(x: 0, y: 0, width: self.bounds.size.width, height: self.bounds.size.height))
        messageLabel.text = message
        messageLabel.textColor = .black
        messageLabel.numberOfLines = 0;
        messageLabel.textAlignment = .center;
        messageLabel.font = UIFont(name: "TrebuchetMS", size: 15)
        messageLabel.sizeToFit()

        self.backgroundView = messageLabel;
    }

    func restore() {
        self.backgroundView = nil
    }
}

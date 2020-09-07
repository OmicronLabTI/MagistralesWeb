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
import Resolver

class InboxViewController: UIViewController {
    
    // MARK: Outlets
    @IBOutlet weak var finishedButton: UIButton!
    @IBOutlet weak var pendingButton: UIButton!
    @IBOutlet weak var processButton: UIButton!
    @IBOutlet weak var collectionView: UICollectionView!
    @IBOutlet weak var similarityViewButton: UIButton!
    @IBOutlet weak var normalViewButton: UIButton!
    
    // MARK:  Variables
    @Injected var inboxViewModel: InboxViewModel
    @Injected var rootViewModel: RootViewModel
    @Injected var lottieManager: LottieManager
    let disposeBag = DisposeBag()
    private let cardWidth = UIScreen.main.bounds.width / 2.5
    private var typeCard: Int = 0
    var orderId:Int = -1
    var statusType: String = ""
    
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        self.title = StatusNameConstants.assignedStatus
        self.collectionView.allowsMultipleSelection = true
        viewModelBinding()
        self.initComponents()
        collectionView.register(UINib(nibName:
            ViewControllerIdentifiers.cardCollectionViewCell, bundle: nil), forCellWithReuseIdentifier: ViewControllerIdentifiers.cardReuseIdentifier)
        finishedButton.isHidden = true
        self.normalViewButton.isEnabled = false
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(true)
         self.splitViewController?.preferredDisplayMode = UISplitViewController.DisplayMode.allVisible
        self.splitViewController?.presentsWithGesture = false
    }

        
    // MARK: Functions
    func viewModelBinding() -> Void {
        
        self.similarityViewButton.rx.tap.bind(to: inboxViewModel.similarityViewDidTap).disposed(by: self.disposeBag)
        self.normalViewButton.rx.tap.bind(to: inboxViewModel.normalViewDidTap).disposed(by: self.disposeBag)
        
        inboxViewModel.refreshDataWhenChangeProcessIsSucces.observeOn(MainScheduler.instance).subscribe(onNext: { _ in
            self.rootViewModel.getOrders()
        }).disposed(by: self.disposeBag)
        
        // Identifica cuando un card ha sido selecionado y se habilita o deshabilita el botón proceso
        collectionView.rx.itemSelected.observeOn(MainScheduler.instance).subscribe(onNext:{ indexpath in
            if self.collectionView.indexPathsForSelectedItems?.count ?? 0 > 0 {
                self.processButton.isEnabled = true
            }
        }).disposed(by: self.disposeBag)
        
        collectionView.rx.itemDeselected.subscribe(onNext: { _ in
            if self.collectionView.indexPathsForSelectedItems?.count == 0 {
                self.processButton.isEnabled = false
            }
        }).disposed(by: disposeBag)

        // Muestra o oculta el loading
        inboxViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { showLoading in
            if(showLoading) {
                self.lottieManager.showLoading()
                return
            }
            self.lottieManager.hideLoading()
        }).disposed(by: self.disposeBag)
        
        // Muestra un mensaje AlertViewController
        inboxViewModel.showAlert.observeOn(MainScheduler.instance).subscribe(onNext: { message in
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.disposeBag)
        
        // Muestra un alert para la confirmación de cambiar el status o no
        inboxViewModel.showConfirmationAlerChangeStatusProcess.observeOn(MainScheduler.instance).subscribe(onNext: { message in
            let alert = UIAlertController(title: CommonStrings.Emty, message: message, preferredStyle: .alert)
            let cancelAction = UIAlertAction(title: "Cancelar", style: .cancel, handler: nil)
            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler:  { _ in self.inboxViewModel.changeStatus(indexPath: self.collectionView.indexPathsForSelectedItems!) })  // Si la respuesta es OK, se mandan los index selecionados para cambiar el status
                       alert.addAction(cancelAction)
                       alert.addAction(okAction)
                       self.present(alert, animated: true, completion: nil)
        }).disposed(by: self.disposeBag)
        
        [
            finishedButton.rx.tap.bind(to: inboxViewModel.finishedDidTap),
            pendingButton.rx.tap.bind(to: inboxViewModel.pendingDidTap),
            processButton.rx.tap.bind(to: inboxViewModel.processDidTap)
        ].forEach({ $0.disposed(by: disposeBag) })
     
        // Lógica cuando se seleciona un item de la tabla
        inboxViewModel.indexSelectedOfTable
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { res in
                self.chageStatusName(index: res)
                self.hideButtons(index: res)
                self.typeCard = res
            }).disposed(by: disposeBag)
        
        // Pinta la cards
        inboxViewModel.statusData.bind(to: self.collectionView.rx.items(cellIdentifier: ViewControllerIdentifiers.cardReuseIdentifier, cellType: CardCollectionViewCell.self)) { row, data, cell in
            self.changepropertiesOfCard(cell: cell)
            cell.row = row
            cell.numberDescriptionLabel.text = "\(data.productionOrderId ?? 0)"
            cell.baseDocumentDescriptionLabel.text = "\(data.baseDocument ?? 0)"
            cell.containerDescriptionLabel.text = data.container ?? ""
            cell.tagDescriptionLabel.text = data.tag ?? ""
            cell.plannedQuantityDescriptionLabel.text = "\(data.plannedQuantity ?? 0)"
            cell.startDateDescriptionLabel.text = data.startDate ?? ""
            cell.finishDateDescriptionLabel.text = data.finishDate ?? ""
            cell.productDescriptionLabel.text = data.descriptionProduct ?? ""
        }.disposed(by: disposeBag)
        
        // retorna mensaje si no hay card para cada status
        inboxViewModel.validateStatusData.observeOn(MainScheduler.instance).subscribe(onNext: { data in
            var message: String = ""
            if(data.orders.count == 0 && data.indexStatusSelected >= 0) {
                switch data.indexStatusSelected {
                case 0:
                    message = "No tienes órdenes Asignadas"
                case 1:
                    message = "No tienes órdenes En proceso"
                case 2:
                    message = "No tienes órdenes Pendientes"
                case 3:
                    message = "No tienes órdenes Terminadas"
                case 4:
                    message = "No tienes órdenes Reasignadas"
                default:
                    message = ""
                }
            }
            self.collectionView.setEmptyMessage(message)
        }).disposed(by: disposeBag)
    }
    
    // Cambiar colores para cada tipo de estatus selecionado
    func changepropertiesOfCard(cell: CardCollectionViewCell) {
        switch self.typeCard {
        case 0:
            self.propertyCard(cell: cell, borderColor: OmicronColors.assignedStatus, iconName: ImageButtonNames.assigned)
        case 1:
            self.propertyCard(cell: cell, borderColor: OmicronColors.processStatus, iconName: ImageButtonNames.inProcess)
        case 2:
            self.propertyCard(cell: cell, borderColor: OmicronColors.pendingStatus, iconName: ImageButtonNames.pendding)
        case 3:
            self.propertyCard(cell: cell, borderColor: OmicronColors.finishedStatus, iconName: ImageButtonNames.finished)
        case 4:
            self.propertyCard(cell: cell, borderColor: OmicronColors.reassignedStatus, iconName: ImageButtonNames.reasigned)
        default:
            print("")
        }
    }
    
    func propertyCard(cell: CardCollectionViewCell, borderColor: UIColor, iconName: String) {
        cell.assignedStyleCard(color: borderColor.cgColor)
        cell.delegate = self
        UtilsManager.shared.changeIconButton(button: cell.showDetail, iconName: iconName)
    }
    
    func initComponents() -> Void {
        self.processButton.isEnabled = false
        UtilsManager.shared.setStyleButtonStatus(button: self.finishedButton, title: StatusNameConstants.finishedStatus, color: OmicronColors.finishedStatus, titleColor: OmicronColors.finishedStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.pendingButton, title: StatusNameConstants.penddingStatus, color: OmicronColors.pendingStatus, titleColor: OmicronColors.pendingStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.processButton, title: StatusNameConstants.inProcessStatus, color: OmicronColors.processStatus, titleColor: OmicronColors.processStatus)
        
        self.similarityViewButton.setTitle("", for: .normal)
        self.similarityViewButton.setImage(UIImage(systemName: ImageButtonNames.similarityView), for: .normal)
        self.normalViewButton.setTitle("", for: .normal)
        self.normalViewButton.setImage(UIImage(systemName: ImageButtonNames.normalView), for: .normal)
    }
    
    func chageStatusName(index: Int) -> Void {
        switch index {
        case 0:
            self.title = StatusNameConstants.assignedStatus
        case 1:
             self.title = StatusNameConstants.inProcessStatus
        case 2:
             self.title = StatusNameConstants.penddingStatus
        case 3:
             self.title = StatusNameConstants.finishedStatus
        case 4:
             self.title = StatusNameConstants.reassignedStatus
        default:
            print("")
        }
    }
    
    private func hideButtons(index: Int) {
        switch index {
        case 0:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: false, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        case 1:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        case 2:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        case 3:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        case 4:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        default:
            print("")
        }
    }
    
    private func changePropertyIsHiddenStatusButtons(processButtonIsHidden: Bool, finishedButtonIsHidden: Bool, pendingButtonIsHidden: Bool) -> Void {
        self.processButton.isHidden = processButtonIsHidden
        self.finishedButton.isHidden = finishedButtonIsHidden
        self.pendingButton.isHidden = pendingButtonIsHidden
    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
       if segue.identifier == ViewControllerIdentifiers.orderDetailViewController {
           if let destination = segue.destination as? OrderDetailViewController {
            destination.orderId = self.orderId // you can pass value to destination view controller
            destination.statusType = self.statusType
           }
       }
    }
}

// MARK: Extencions
extension InboxViewController: CardCellDelegate {
    
    // Chec this
    func detailTapped(row: Int) {
        self.inboxViewModel.statusData.subscribe(onNext: { res in
            if (res.count > 0 && res.count > row) {
                self.orderId = res[row].productionOrderId!
            }
        }).disposed(by: self.disposeBag)
        
        self.inboxViewModel.nameStatus.subscribe(onNext: { statusName in
            self.statusType = statusName
        }).disposed(by: self.disposeBag)
        
        self.performSegue(withIdentifier: ViewControllerIdentifiers.orderDetailViewController, sender: nil)
    }
}

extension UICollectionView {

    func setEmptyMessage(_ message: String) {
        let messageLabel = UILabel(frame: CGRect(x: 0, y: 0, width: self.bounds.size.width, height: self.bounds.size.height))
        messageLabel.text = message
        messageLabel.textColor = .black
        messageLabel.numberOfLines = 0;
        messageLabel.textAlignment = .center;
        messageLabel.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 15)
        messageLabel.sizeToFit()

        self.backgroundView = messageLabel;
    }

    func restore() {
        self.backgroundView = nil
    }
}

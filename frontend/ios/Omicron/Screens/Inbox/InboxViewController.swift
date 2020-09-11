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
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(true)
         self.splitViewController?.preferredDisplayMode = UISplitViewController.DisplayMode.allVisible
        self.splitViewController?.presentsWithGesture = false
    }

        
    // MARK: Functions
    func viewModelBinding() -> Void {
        inboxViewModel.title.subscribe(onNext: { [weak self] title in
            self?.title = title
            guard let statusId = self?.inboxViewModel.getStatusId(name: title) else { return }
            self?.hideButtons(id: statusId)
        }).disposed(by: disposeBag)
        
        inboxViewModel.refreshDataWhenChangeProcessIsSucces.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.rootViewModel.getOrders()
        }).disposed(by: self.disposeBag)
        
        // Identifica cuando un card ha sido selecionado y se habilita o deshabilita el botón proceso
        collectionView.rx.itemSelected.observeOn(MainScheduler.instance).subscribe(onNext:{ [weak self] indexpath in
            if self?.collectionView.indexPathsForSelectedItems?.count ?? 0 > 0 {
                self?.processButton.isEnabled = true
            }
        }).disposed(by: self.disposeBag)
        
        collectionView.rx.itemDeselected.subscribe(onNext: { [weak self] _ in
            if self?.collectionView.indexPathsForSelectedItems?.count == 0 {
                self?.processButton.isEnabled = false
            }
        }).disposed(by: disposeBag)

        // Muestra o oculta el loading
        inboxViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] showLoading in
            if(showLoading) {
                self?.lottieManager.showLoading()
                return
            }
            self?.lottieManager.hideLoading()
        }).disposed(by: self.disposeBag)
        
        // Muestra un mensaje AlertViewController
        inboxViewModel.showAlert.observeOn(MainScheduler.instance).subscribe(onNext: { [unowned self] message in
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.disposeBag)
        
        inboxViewModel.similarityViewButtonIsEnable.subscribe(onNext: { [weak self] isEnabled in
            self?.similarityViewButton.isEnabled = isEnabled
        }).disposed(by: self.disposeBag)
        
        inboxViewModel.normalViewButtonIsEnable.subscribe(onNext: { [weak self] isEnabled in
            self?.normalViewButton.isEnabled = isEnabled
        }).disposed(by: self.disposeBag)
        
        // Muestra un alert para la confirmación de cambiar el status o no
        inboxViewModel.showConfirmationAlerChangeStatusProcess.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] message in
            let alert = UIAlertController(title: CommonStrings.Emty, message: message, preferredStyle: .alert)
            let cancelAction = UIAlertAction(title: "Cancelar", style: .cancel, handler: nil)
            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler:  { _ in
                self?.inboxViewModel.changeStatus(indexPath: self?.collectionView.indexPathsForSelectedItems ?? [])
            })  // Si la respuesta es OK, se mandan los index selecionados para cambiar el status
            alert.addAction(cancelAction)
            alert.addAction(okAction)
            self?.present(alert, animated: true, completion: nil)
        }).disposed(by: self.disposeBag)
        
        [
            finishedButton.rx.tap.bind(to: inboxViewModel.finishedDidTap),
            pendingButton.rx.tap.bind(to: inboxViewModel.pendingDidTap),
            processButton.rx.tap.bind(to: inboxViewModel.processDidTap),
            similarityViewButton.rx.tap.bind(to: inboxViewModel.similarityViewButtonDidTap),
            normalViewButton.rx.tap.bind(to: inboxViewModel.normalViewButtonDidTap),
        ].forEach({ $0.disposed(by: disposeBag) })
     
        rootViewModel.selectedRow.subscribe(onNext: { [weak self] index in
            guard let row = index?.row else { return }
            self?.chageStatusName(index: row)
            self?.hideButtons(index: row)
        }).disposed(by: disposeBag)

        // Pinta la cards
        inboxViewModel.statusData.bind(to: self.collectionView.rx.items(cellIdentifier: ViewControllerIdentifiers.cardReuseIdentifier, cellType: CardCollectionViewCell.self)) { [weak self] row, data, cell in
            cell.row = row
            cell.order = data
            cell.numberDescriptionLabel.text = "\(data.productionOrderId ?? 0)"
            cell.baseDocumentDescriptionLabel.text = "\(data.baseDocument ?? 0)"
            cell.containerDescriptionLabel.text = data.container ?? ""
            cell.tagDescriptionLabel.text = data.tag ?? ""
            cell.plannedQuantityDescriptionLabel.text = "\(data.plannedQuantity ?? 0)"
            cell.startDateDescriptionLabel.text = data.startDate ?? ""
            cell.finishDateDescriptionLabel.text = data.finishDate ?? ""
            cell.productDescriptionLabel.text = data.descriptionProduct ?? ""
            cell.delegate = self
        }.disposed(by: disposeBag)
        
//        inboxViewModel.statusData.bind(to: self.collectionView.rx.items(dataSource: RxCollectionViewDataSourceType & UICollectionViewDataSource))
        
        // retorna mensaje si no hay card para cada status
        inboxViewModel.title.withLatestFrom(inboxViewModel.statusData, resultSelector: { [weak self] title, data in
            let statusId = self?.inboxViewModel.getStatusId(name: title) ?? -1
            var message: String = ""
            if (data.count == 0 && statusId != -1) {
                message = "No tienes órdenes \(title)"
            }
            self?.collectionView.setEmptyMessage(message)
        }).subscribe().disposed(by: disposeBag)
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
        let name = self.inboxViewModel.getStatusName(index: index)
        self.inboxViewModel.title.onNext(name)
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
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        }
    }
    
    private func hideButtons(id: Int) {
        switch id {
        case 1:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: false, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        case 2:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        case 3:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        case 4:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        case 5:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
        default:
            self.changePropertyIsHiddenStatusButtons(processButtonIsHidden: true, finishedButtonIsHidden: true, pendingButtonIsHidden: true)
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
            guard let orderId = self.inboxViewModel.selectedOrder?.productionOrderId else { return }
            guard let statusId = self.inboxViewModel.selectedOrder?.statusId else { return }
            guard let destiny = self.inboxViewModel.selectedOrder?.destiny else { return }
            destination.orderId = orderId // you can pass value to destination view controller
            destination.statusType = self.inboxViewModel.getStatusName(id: statusId)
            destination.destiny = destiny
           }
       }
    }
}

// MARK: Extencions
extension InboxViewController: CardCellDelegate {
    
    // Chec this
    func detailTapped(order: Order) {
        self.inboxViewModel.selectedOrder = order
        self.view.endEditing(true)
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

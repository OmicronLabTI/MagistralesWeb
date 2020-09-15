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
import RxDataSources

class InboxViewController: UIViewController {
    
    // MARK: Outlets
    @IBOutlet weak var finishedButton: UIButton!
    @IBOutlet weak var pendingButton: UIButton!
    @IBOutlet weak var processButton: UIButton!
    @IBOutlet weak var collectionView: UICollectionView!
    @IBOutlet weak var similarityViewButton: UIButton!
    @IBOutlet weak var normalViewButton: UIButton!
    
    @IBOutlet weak var heigthCollectionViewConstraint: NSLayoutConstraint!
    // MARK:  Variables
    
    private var bindingCollectionView = true
    
    @Injected var inboxViewModel: InboxViewModel
    @Injected var rootViewModel: RootViewModel
    @Injected var lottieManager: LottieManager

    let disposeBag = DisposeBag()
    
    
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
        collectionView.register(UINib(nibName: ViewControllerIdentifiers.headerCollectionViewCell, bundle: nil), forSupplementaryViewOfKind: UICollectionView.elementKindSectionHeader, withReuseIdentifier: ViewControllerIdentifiers.headerReuseIdentifier)
        finishedButton.isHidden = true
        pendingButton.isHidden = true
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(true)
        self.splitViewController?.preferredDisplayMode = UISplitViewController.DisplayMode.allVisible
        self.splitViewController?.presentsWithGesture = false
        
        if bindingCollectionView {
            viewModelBindingCollectionView()
            bindingCollectionView.toggle()
        }
        
    }
    
    func viewModelBindingCollectionView() {
        
        // Pinta la cards
        let dataSource = RxCollectionViewSectionedReloadDataSource<SectionModel<String, Order>>(configureCell: { [weak self] (dataSource, cv, indexPath, element) in
            
            let cell = cv.dequeueReusableCell(withReuseIdentifier: ViewControllerIdentifiers.cardReuseIdentifier, for: indexPath) as! CardCollectionViewCell
            cell.row = indexPath.row
            cell.order = element
            cell.numberDescriptionLabel.text = "\(element.productionOrderId ?? 0)"
            cell.baseDocumentDescriptionLabel.text = "\(element.baseDocument ?? 0)"
            cell.containerDescriptionLabel.text = element.container ?? ""
            cell.tagDescriptionLabel.text = element.tag ?? ""
            cell.plannedQuantityDescriptionLabel.text = "\(element.plannedQuantity ?? 0)"
            cell.startDateDescriptionLabel.text = element.startDate ?? ""
            cell.finishDateDescriptionLabel.text = element.finishDate ?? ""
            cell.productDescriptionLabel.text = element.descriptionProduct ?? ""
            cell.delegate = self
            return cell
            
        })
        
        dataSource.configureSupplementaryView = { (dataSource, collectionView, kind, indexPath) -> UICollectionReusableView in
            let header = collectionView.dequeueReusableSupplementaryView(ofKind: UICollectionView.elementKindSectionHeader, withReuseIdentifier: ViewControllerIdentifiers.headerReuseIdentifier, for: indexPath) as! HeaderCollectionViewCell
            header.productID.text = dataSource.sectionModels[indexPath.section].identity
            return header
        }
        
        inboxViewModel.statusDataGrouped
            .bind(to: collectionView.rx.items(dataSource: dataSource))
            .disposed(by: disposeBag)
        
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
        
        // Habilita o deshabilita el botón para cambiar a proceso
        inboxViewModel.processButtonIsEnable.subscribe(onNext: { [weak self] isEnable in
            self?.processButton.isEnabled = isEnable
        }).disposed(by: self.disposeBag)

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
        
        // Habilita o deshabilita el botón de agrupamiento por similaridad
        inboxViewModel.similarityViewButtonIsEnable.subscribe(onNext: { [weak self] isEnabled in
            guard let self = self else { return }
            self.similarityViewButton.isEnabled = isEnabled
            self.heigthCollectionViewConstraint.constant = !isEnabled ? 8 : -40
        }).disposed(by: self.disposeBag)
        
        // Habilita o deshabilita el botón de agrupamiento por vista normal
        inboxViewModel.normalViewButtonIsEnable.subscribe(onNext: { [weak self] isEnabled in
            guard let self = self else { return }
            self.normalViewButton.isEnabled = isEnabled
        }).disposed(by: self.disposeBag)
        
        // Oculta o muestra los botones de agrupamiento cuando se se realiza una búsqueda
        inboxViewModel.hideGroupingButtons.subscribe(onNext: { [weak self] isHidden in
            self?.similarityViewButton.isHidden = isHidden
            self?.normalViewButton.isHidden = isHidden
        }).disposed(by: self.disposeBag)
        
        // Muestra un alert para la confirmación de cambiar el status o no
        inboxViewModel.showConfirmationAlerChangeStatusProcess.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] message in
            let alert = UIAlertController(title: message, message: nil, preferredStyle: .alert)
            let cancelAction = UIAlertAction(title: "Cancelar", style: .cancel, handler: nil)
            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler:  { _ in
                self?.inboxViewModel.changeStatus(indexPath: self?.collectionView.indexPathsForSelectedItems)
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
                
        // retorna mensaje si no hay card para cada status
        inboxViewModel.title.withLatestFrom(inboxViewModel.statusDataGrouped, resultSelector: { [weak self] title, data in
            let statusId = self?.inboxViewModel.getStatusId(name: title) ?? -1
            var message: String = ""
            if (data.count == 0 && statusId != -1) {
                message = "No tienes órdenes \(title)"
            }
            self?.collectionView.setEmptyMessage(message)
        }).subscribe().disposed(by: disposeBag)
    }

    func initComponents() {
        self.processButton.isEnabled = false
        UtilsManager.shared.setStyleButtonStatus(button: self.finishedButton, title: StatusNameConstants.finishedStatus, color: OmicronColors.finishedStatus, titleColor: OmicronColors.finishedStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.pendingButton, title: StatusNameConstants.penddingStatus, color: OmicronColors.pendingStatus, titleColor: OmicronColors.pendingStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.processButton, title: StatusNameConstants.inProcessStatus, color: OmicronColors.processStatus, titleColor: OmicronColors.processStatus)
        self.similarityViewButton.setTitle("", for: .normal)
        self.similarityViewButton.setImage(UIImage(systemName: ImageButtonNames.similarityView), for: .normal)
        self.normalViewButton.setTitle("", for: .normal)
        self.normalViewButton.setImage(UIImage(systemName: ImageButtonNames.normalView), for: .normal)
        
        let layout = UICollectionViewFlowLayout()
        layout.headerReferenceSize = CGSize(width: UIScreen.main.bounds.width, height: 40)
        layout.itemSize = CGSize(width: 355, height: 250)
        collectionView.setCollectionViewLayout(layout, animated: true)
        heigthCollectionViewConstraint.constant = -40
        print(UIScreen.main.bounds.width)
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

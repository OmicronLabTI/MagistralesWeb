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
import Charts
import PDFKit

// swiftlint:disable type_body_length
class InboxViewController: UIViewController {
    // MARK: Outlets
    @IBOutlet weak var finishedButton: UIButton!
    @IBOutlet weak var pendingButton: UIButton!
    @IBOutlet weak var processButton: UIButton!
    @IBOutlet weak var packageButton: UIButton!
    @IBOutlet weak var collectionView: UICollectionView!
    @IBOutlet weak var similarityViewButton: UIButton!
    @IBOutlet weak var normalViewButton: UIButton!
    @IBOutlet weak var groupByOrderNumberButton: UIButton!
    @IBOutlet weak var groupByShopTransactionButton: UIButton!
    @IBOutlet weak var heigthCollectionViewConstraint: NSLayoutConstraint!
    @IBOutlet weak var chartViewContainer: UIView!
    @IBOutlet weak var cardsView: UIView!
    @IBOutlet weak var removeOrdersSelectedView: UIView!
    @IBOutlet weak var removeOrdersSelectedVerticalSpace: NSLayoutConstraint!
    @IBOutlet weak var showContainersButtons: UIButton!
    // MARK: - Variables
    @Injected var inboxViewModel: InboxViewModel
    @Injected var rootViewModel: RootViewModel
    @Injected var lottieManager: LottieManager

    private var bindingCollectionView = true
    let disposeBag = DisposeBag()
    var productID = 0
    var indexPathsSelected: [IndexPath] = []
    var lastColor = UIColor.blue
    var lastIndexPath: IndexPath?
    var lastRect: CGRect?

    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.title = StatusNameConstants.assignedStatus
        collectionView.allowsMultipleSelection = true
        viewModelBinding()
        initComponents()
        extensionInitComponents()
        registerCellsOfCollectionView()
        tapBindingButtons()
        finishedButton.isHidden = true
        pendingButton.isHidden = true
        navigationItem.rightBarButtonItem = getOmniconLogo()
        let lpgr = UILongPressGestureRecognizer(target: self,
                                                action: #selector(InboxViewController.handleLongPress(gesture:)))
        lpgr.minimumPressDuration = 0.5
        lpgr.delaysTouchesBegan = true
        self.collectionView.addGestureRecognizer(lpgr)
        removeOrdersSelectedView.layer.cornerRadius = removeOrdersSelectedView.frame.width / 2
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        self.splitViewController?.preferredDisplayMode = UISplitViewController.DisplayMode.allVisible
        self.splitViewController?.presentsWithGesture = false
        if bindingCollectionView {
            viewModelBindingCollectionView()
            bindingCollectionView.toggle()
        }
    }

    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        splitViewController?.preferredDisplayMode = .primaryHidden
    }
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == ViewControllerIdentifiers.orderDetailViewController {
            if let destination = segue.destination as? OrderDetailViewController {
                guard let orderId = self.inboxViewModel.selectedOrder?.productionOrderId else { return }
                guard let statusId = self.inboxViewModel.selectedOrder?.statusId else { return }
                guard let destiny = self.inboxViewModel.selectedOrder?.destiny else { return }
                destination.orderId = orderId // you can pass value to destination view controller
                destination.statusType = self.inboxViewModel.getStatusName(index: statusId)
                destination.destiny = destiny
            }
        } else if segue.identifier == ViewControllerIdentifiers.patientListViewController {
            if let destination = segue.destination as? PatientListViewController {
                guard let data = sender as? PatientListData else { return }
                destination.name = data.title
                destination.list = data.list
            }
        }
    }

    // MARK: - Functions
    func getDecimalPartOfDouble(number: Double) -> Double {
        return number.truncatingRemainder(dividingBy: 1)
    }

    func tapBindingButtons() {
        [similarityViewButton.rx.tap.bind(to: inboxViewModel.similarityViewButtonDidTap),
         normalViewButton.rx.tap.bind(to: inboxViewModel.normalViewButtonDidTap),
         groupByOrderNumberButton.rx.tap.bind(to: inboxViewModel.groupByOrderNumberButtonDidTap),
         groupByShopTransactionButton.rx.tap.bind(to: inboxViewModel.groupByShopTransactionButtonDidTap),
         finishedButton.rx.tap.bind(to: inboxViewModel.finishedDidTap),
         pendingButton.rx.tap.bind(to: inboxViewModel.pendingDidTap),
         processButton.rx.tap.bind(to: inboxViewModel.processDidTap) ].forEach({ $0.disposed(by: disposeBag) })
    }

    func viewModelBindingCollectionView() {
        // Pinta las cards
        let dataSource = RxCollectionViewSectionedReloadDataSource<SectionModel<String, Order>>(
            configureCell: { [weak self] (_, _, indexPath, element) in
                guard let self = self else { return CardCollectionViewCell()}
                let decimalPart = self.getDecimalPartOfDouble(
                    number: NSDecimalNumber(decimal: element.plannedQuantity ?? 0.0).doubleValue)
                if element.baseDocument != 0 {
                    let cell = self.returnCardCollectionViewCell(
                        indexPath: indexPath, element: element, decimalPart: decimalPart)
                    cell.delegate = self
                    return cell
                } else {
                    let cell = self.returnCardIsolateOrderCollectionViewCell(
                        indexPath: indexPath, element: element, decimalPart: decimalPart)
                    cell.delegate = self
                    return cell
                }
            })
        dataSource
            .configureSupplementaryView = { [weak self] (dataSource, collectionView, _, indexPath)
                -> UICollectionReusableView in
                guard let header = collectionView.dequeueReusableSupplementaryView(
                    ofKind: UICollectionView.elementKindSectionHeader,
                    withReuseIdentifier: ViewControllerIdentifiers.headerReuseIdentifier,
                    for: indexPath) as? HeaderCollectionViewCell else { return HeaderCollectionViewCell() }
                let headerText = dataSource.sectionModels[indexPath.section].identity
                let items = dataSource.sectionModels[indexPath.section].items
                return self!.generateHeaderOptions(headerText, items, header)
            }
        inboxViewModel.statusDataGrouped
            .bind(to: collectionView.rx.items(dataSource: dataSource))
            .disposed(by: disposeBag)
    }

    func viewModelBinding() {
        inboxViewModel.title.subscribe(onNext: { [weak self] title in
            guard let self = self else { return }
            self.title = title
            self.hideButtons(title: title)
            self.removeOrdersSelectedView.backgroundColor = self.updateRemoveViewColor(title: title)
        }).disposed(by: disposeBag)
        inboxViewModel.refreshDataWhenChangeProcessIsSucces
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
                guard let self = self else { return }
                self.rootViewModel.getOrders()
            }).disposed(by: self.disposeBag)
        inboxViewModel.processButtonIsEnable.subscribe(onNext: { [weak self] isEnable in
            guard let self = self else { return }
            self.processButton.isEnabled = isEnable
        }).disposed(by: self.disposeBag)
        // Habilita o deshabilita el botón para cambiar a pendiente
        inboxViewModel.pendingButtonIsEnable.subscribe(onNext: { [weak self] isEnable in
            guard let self = self else { return }
            self.pendingButton.isEnabled = isEnable
        }).self.disposed(by: self.disposeBag)
        inboxViewModel.orderURLPDF.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] urlPDF in
            guard let self = self else { return }
            DispatchQueue.main.async {
                let pdfViewController = PDFViewController()
                pdfViewController.pdfURL = URL(string: urlPDF)
                self.present(pdfViewController, animated: true, completion: nil)
            }
        }).disposed(by: disposeBag)
        self.modelBindingGrouped()
        self.modelBindingExtension1()
        self.modelBindingExtension3()
        self.modelBindingExtension4()
        self.showSignatureVC()
        isUserInteractionEnabledBinding()
    }

    func modelBindingExtension1() {
        // Muestra un alert para la confirmación de cambio de estatus de una orden
        inboxViewModel.showAlertToChangeOrderOfStatus
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] data in
                guard let self = self else { return }
                let alert = UIAlertController(title: data.message, message: nil, preferredStyle: .alert)
                let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive, handler: nil)
                let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default, handler: { [weak self] _ in
                    guard let self = self else { return }
                    self.view.isUserInteractionEnabled = false
                    if data.typeOfStatus == StatusNameConstants.finishedStatus {
                        self.inboxViewModel.validOrders(indexPathOfOrdersSelected: self.indexPathsSelected)
                    } else {
                        self.inboxViewModel.changeStatus(
                            indexPath: self.indexPathsSelected, typeOfStatus: data.typeOfStatus)
                    }
                })
                alert.addAction(cancelAction)
                alert.addAction(okAction)
                self.present(alert, animated: true, completion: nil)
            }).disposed(by: self.disposeBag)
        didScrollBinding()
        showKPIViewBinding()
    }

    func didScrollBinding() {
        collectionView.rx.didScroll.subscribe({ [weak self] _ in
            guard let self = self else { return }
            self.collectionView.removeMoreIndicator()
        }).disposed(by: disposeBag)
    }

    func showKPIViewBinding() {
        inboxViewModel.showKPIView.observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] show in
                guard let self = self else { return }
                if show { self.title = "Indicadores" }
                self.chartViewContainer.isHidden = !show
                self.cardsView.isHidden = show
            }).disposed(by: disposeBag)
    }
    func initComponents() {
        self.processButton.isEnabled = false
        self.pendingButton.isEnabled = false
        self.finishedButton.isEnabled = false
        self.packageButton.isEnabled = false
        UtilsManager.shared.setStyleButtonStatus(
            button: self.finishedButton,
            title: StatusNameConstants.finishedStatus,
            color: OmicronColors.finishedStatus,
            titleColor: OmicronColors.finishedStatus)
        UtilsManager.shared.setStyleButtonStatus(
            button: self.pendingButton,
            title: StatusNameConstants.penddingStatus,
            color: OmicronColors.pendingStatus,
            titleColor: OmicronColors.pendingStatus)
        UtilsManager.shared.setStyleButtonStatus(
            button: self.processButton,
            title: StatusNameConstants.inProcessStatus,
            color: OmicronColors.processStatus,
            titleColor: OmicronColors.processStatus)
        UtilsManager.shared.setStyleButtonStatus(
            button: self.packageButton,
            title: StatusNameConstants.package,
            color: OmicronColors.packageButton,
            titleColor: OmicronColors.packageButton)
    }
    func extensionInitComponents() {
        self.similarityViewButton.setTitle("", for: .normal)
        self.similarityViewButton.setImage(UIImage(systemName: ImageButtonNames.similarityView), for: .normal)
        self.normalViewButton.setTitle("", for: .normal)
        self.normalViewButton.setImage(UIImage(systemName: ImageButtonNames.normalView), for: .normal)
        self.normalViewButton.isEnabled = false
        self.groupByOrderNumberButton.setTitle("", for: .normal)
        self.groupByOrderNumberButton.setImage(UIImage(systemName: ImageButtonNames.rectangule3offgrid), for: .normal)
        let layout = UICollectionViewFlowLayout()
        layout.headerReferenceSize = CGSize(width: UIScreen.main.bounds.width, height: 60)
        let size = rootViewModel.userType != .technical ? 180 : 210
        layout.itemSize = CGSize(width: 700, height: size)
        layout.minimumLineSpacing = 16
        collectionView.setCollectionViewLayout(layout, animated: true)
        heigthCollectionViewConstraint.constant = -60
        inboxViewModel.resetData.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] in
            guard let self = self else { return }
            self.indexPathsSelected.removeAll()
            self.collectionView.reloadData()
            if self.removeOrdersSelectedVerticalSpace.constant > 0 {
                UIView.animate(withDuration: 0.2, animations: { [weak self] in
                    guard let self = self else { return }
                    self.removeOrdersSelectedVerticalSpace.constant = -60
                    self.view.layoutIfNeeded()
                })
            }
            self.processButton.isEnabled = false
            self.pendingButton.isEnabled = false
            self.finishedButton.isEnabled = false
            self.packageButton.isEnabled = false
        }).disposed(by: disposeBag)
    }

    func showMoreIndicators() {
        collectionView.removeMoreIndicator()
        let itemsCount = Array(0..<collectionView.numberOfSections)
            .map { collectionView.numberOfItems(inSection: $0) }
            .reduce(0, +)
        DispatchQueue.main.async {
            if itemsCount > 4 {
                self.collectionView.addMoreIndicator(size: 50)
            } else {
                self.collectionView.removeMoreIndicator()
            }
        }
    }
    func goToTop() {
        collectionView.setContentOffset(.zero, animated: false)
    }
    private func hideButtons(title: String) {
        showContainersButtons.isHidden = true
        if rootViewModel.userType == .technical {
            hideButtonsTechnical(title)
        } else {
            hideButtonsQfB(title)
        }
    }

    func hideButtonsTechnical(_ title: String) {
        switch title {
        case StatusNameConstants.assignedStatus:
            self.changePropertyIsHiddenStatusButtons(true, true, false, false)
            showContainersButtons.isHidden = false
        case StatusNameConstants.penddingStatus: self.changePropertyIsHiddenStatusButtons(true, true, true, true)
        case StatusNameConstants.reassignedStatus: self.changePropertyIsHiddenStatusButtons(true, true, false, false)
        default: self.changePropertyIsHiddenStatusButtons(true, true, true, false)
        }
    }

    func hideButtonsQfB(_ title: String) {
        switch title {
        case StatusNameConstants.assignedStatus:
            self.changePropertyIsHiddenStatusButtons(false, true, false, true)
            showContainersButtons.isHidden = false
        case StatusNameConstants.inProcessStatus: self.changePropertyIsHiddenStatusButtons(true, false, false, true)
        case StatusNameConstants.penddingStatus: self.changePropertyIsHiddenStatusButtons(false, true, true, true)
        case StatusNameConstants.finishedStatus: self.changePropertyIsHiddenStatusButtons(true, true, true, true)
        case StatusNameConstants.reassignedStatus: self.changePropertyIsHiddenStatusButtons(true, false, false, true)
        default: self.changePropertyIsHiddenStatusButtons(true, true, true, true)
        }
    }

    private func changePropertyIsHiddenStatusButtons(
        _ processButtonIsHidden: Bool,
        _ finishedButtonIsHidden: Bool,
        _ pendingButtonIsHidden: Bool,
        _ packageButtonIsHidden: Bool) {
            self.processButton.isHidden = processButtonIsHidden
            self.finishedButton.isHidden = finishedButtonIsHidden
            self.pendingButton.isHidden = pendingButtonIsHidden
            self.packageButton.isHidden = packageButtonIsHidden || self.rootViewModel.userType != UserType.technical
        }

    // aqui se detecta cuando se mantuvo presionada la celda para seleccionarla
    @objc func handleLongPress(gesture: UILongPressGestureRecognizer!) {
        if gesture.state != .ended { return }
        let position = gesture.location(in: self.collectionView)
        if let indexPath = self.collectionView.indexPathForItem(at: position) {
            self.updateCellWithIndexPath(indexPath)
        }
    }

    @IBAction func packageOrdersDidPressed(_ sender: Any) {
        if !self.inboxViewModel.validateSelectedOrdersAreSameSAPId(indexPathOfOrdersSelected: indexPathsSelected) {
            rootViewModel.error.onNext(Constants.Errors.invalidSapOrderId.rawValue)
            return
        }
        inboxViewModel.indexPathOfOrdersSelected = indexPathsSelected
        inboxViewModel.showSignatureVc.onNext(CommonStrings.signatureViewTitleTechnical)
    }
    @IBAction func removeSelectedOrdersDidPressed(_ sender: Any) {
        self.indexPathsSelected.removeAll()
        DispatchQueue.main.async {
            self.collectionView.reloadData()
            self.processButton.isEnabled = false
            self.pendingButton.isEnabled = false
            self.finishedButton.isEnabled = false
            self.packageButton.isEnabled = false
        }
        if self.removeOrdersSelectedVerticalSpace.constant > 0 {
            UIView.animate(withDuration: 0.2, animations: { [weak self] in
                guard let self = self else { return }
                self.removeOrdersSelectedVerticalSpace.constant = -60
                self.view.layoutIfNeeded()
            })
        }
    }
}

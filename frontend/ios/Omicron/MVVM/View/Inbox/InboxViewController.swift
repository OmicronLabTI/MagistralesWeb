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
    @IBOutlet weak var collectionView: UICollectionView!
    @IBOutlet weak var similarityViewButton: UIButton!
    @IBOutlet weak var normalViewButton: UIButton!
    @IBOutlet weak var groupByOrderNumberButton: UIButton!
    @IBOutlet weak var heigthCollectionViewConstraint: NSLayoutConstraint!
    @IBOutlet weak var chartViewContainer: UIView!
    @IBOutlet weak var cardsView: UIView!
    @IBOutlet weak var removeOrdersSelectedView: UIView!
    @IBOutlet weak var removeOrdersSelectedVerticalSpace: NSLayoutConstraint!
    @IBOutlet weak var showContainersButtons: UIButton!
    var order = PublishSubject<Int>()

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
        finishedButton.isHidden = true
        pendingButton.isHidden = true
        navigationItem.rightBarButtonItem = getOmniconLogo()
        let lpgr = UILongPressGestureRecognizer(target: self,
                                                action: #selector(InboxViewController.handleLongPress(gesture:)))
        lpgr.minimumPressDuration = 0.5
        lpgr.delaysTouchesBegan = true
        self.collectionView.addGestureRecognizer(lpgr)
//        let tapGestureRecognizer = UITapGestureRecognizer(
//            target: self,
//            action: #selector(InboxViewController.tappedPressed(gesture:))
//        )
//        self.view.addGestureRecognizer(tapGestureRecognizer)
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
//        guard let lastIndexPath = lastIndexPath else { return }
//        collectionView.scrollRectToVisible(<#T##rect: CGRect##CGRect#>, animated: <#T##Bool#>)
//        guard let lastRect = lastRect else { return }
//        collectionView.scrollRectToVisible(lastRect, animated: true)
    }

    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        splitViewController?.preferredDisplayMode = .primaryHidden
    }

    // MARK: - Functions
    func registerCellsOfCollectionView() {
        collectionView.register(
            UINib(
                nibName: ViewControllerIdentifiers.cardCollectionViewCell,
                bundle: nil), forCellWithReuseIdentifier: ViewControllerIdentifiers.cardReuseIdentifier)
        collectionView.register(
            UINib(
                nibName: ViewControllerIdentifiers.cardIsolatedOrderCollectionViewCell,
                bundle: nil), forCellWithReuseIdentifier: ViewControllerIdentifiers.cardIsolatedOrderReuseIdentifier)
        collectionView.register(
            UINib(
                nibName: ViewControllerIdentifiers.headerCollectionViewCell,
                bundle: nil),
            forSupplementaryViewOfKind: UICollectionView.elementKindSectionHeader,
            withReuseIdentifier: ViewControllerIdentifiers.headerReuseIdentifier)
    }

    func getDecimalPartOfDouble(number: Double) -> Double {
        return number.truncatingRemainder(dividingBy: 1)
    }

    func returnCardIsolateOrderCollectionViewCell(
        indexPath: IndexPath, element: SectionModel<String, Order>.Item,
        decimalPart: Double?) -> CardIsolatedOrderCollectionViewCell {
            guard let cell = collectionView.dequeueReusableCell(
                withReuseIdentifier: ViewControllerIdentifiers.cardIsolatedOrderReuseIdentifier,
                for: indexPath) as? CardIsolatedOrderCollectionViewCell
            else { return CardIsolatedOrderCollectionViewCell() }
            cell.row = indexPath.row
            cell.order = element
            cell.numberDescriptionLabel.text = "\(element.productionOrderId ?? 0)"
            cell.plannedQuantityDescriptionLabel.text = decimalPart  ?? 0.0 > 0.0 ?
            String(format: DecimalFormat.six.rawValue,
                   NSDecimalNumber(decimal: element.plannedQuantity ?? 0.0).doubleValue) :
            "\(element.plannedQuantity ?? 0.0)"
            cell.startDateDescriptionLabel.text = element.startDate ?? CommonStrings.empty
            cell.finishDateDescriptionLabel.text = element.finishDate ?? CommonStrings.empty
            cell.productDescriptionLabel.text = element.descriptionProduct?.uppercased() ?? CommonStrings.empty
            cell.missingStockImage.isHidden = !element.hasMissingStock
            cell.isSelected = indexPathsSelected.contains(indexPath)
            cell.itemCode.text = element.itemCode
            return cell
        }

    func returnCardCollectionViewCell(indexPath: IndexPath, element: SectionModel<String, Order>.Item,
                                      decimalPart: Double?) -> CardCollectionViewCell {
        guard let cell = collectionView.dequeueReusableCell(
            withReuseIdentifier: ViewControllerIdentifiers.cardReuseIdentifier,
            for: indexPath) as? CardCollectionViewCell else { return CardCollectionViewCell() }
        cell.row = indexPath.row
        cell.order = element
        cell.numberDescriptionLabel.text = "\(element.productionOrderId ?? 0)"
        cell.baseDocumentDescriptionLabel.text = element.baseDocument == 0 ?
            CommonStrings.empty : "\(element.baseDocument ?? 0)"
        cell.tagDescriptionLabel.text = element.tag
        cell.containerLabel.text = element.container
        cell.tagDescriptionLabel.textColor = !element.finishedLabel ? .red : .systemGreen
        cell.plannedQuantityDescriptionLabel.text = decimalPart  ?? 0.0 > 0.0 ?
            String(format: DecimalFormat.six.rawValue,
                   NSDecimalNumber(decimal: element.plannedQuantity ?? 0.0).doubleValue) :
            "\(element.plannedQuantity ?? 0.0)"
        cell.startDateDescriptionLabel.text = element.startDate ?? CommonStrings.empty
        cell.finishDateDescriptionLabel.text = element.finishDate ?? CommonStrings.empty
        cell.productDescriptionLabel.text = element.descriptionProduct?.uppercased() ?? CommonStrings.empty
        cell.missingStockImage.isHidden = !element.hasMissingStock
        cell.isSelected = indexPathsSelected.contains(indexPath)
        cell.itemCode.text = element.itemCode
        cell.destiny.text = element.destiny
        let patientName = (element.patientName != "") ? "patientName" : "noPatientName"
        cell.patientListButton.setImage(UIImage(named: patientName),for: .normal)
        cell.pdfDownloadButton.isHidden = !rootViewModel.needSearch
        cell.patientListButton.isHidden = !groupByOrderNumberButton.isEnabled && !rootViewModel.needSearch
        return cell
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
                header.productID.text = headerText
                if headerText.contains(CommonStrings.orderTitile) {
                    let productId = headerText
                        .components(separatedBy: CharacterSet.decimalDigits.inverted)
                        .joined()
                    header.productId = Int(productId) ?? 0
                    header.delegate = self
                    header.pdfImageView.isHidden = false
                    header.patientListButton.isHidden = false
                    header.doctorName.isHidden = false
                    if let order = self?.getNamesByOrder(productID: Int(productId) ?? 0 ) {
                        let patientName = (order.patientName != "") ? "patientName" : "noPatientName"
                        header.patientListButton.setImage(UIImage(named: patientName),for: .normal)
                        header.doctorName.text = order.clientDxp
                    }
                } else {
                    header.doctorName.isHidden = true
                    header.productId = 0
                    header.delegate = nil
                    header.pdfImageView.isHidden = true
                    header.patientListButton.isHidden = true
                }
                return header
            }
        inboxViewModel.statusDataGrouped
            .bind(to: collectionView.rx.items(dataSource: dataSource))
            .disposed(by: disposeBag)
    }

    func viewModelBinding() {
        inboxViewModel.title.subscribe(onNext: { [weak self] title in
            guard let self = self else { return }
            self.title = title
            let statusId = self.inboxViewModel.getStatusId(name: title)
            self.hideButtons(index: statusId)
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
        self.modelBindingExtention1()
        self.modelBindingExtension2()
        self.modelBindingExtension3()
        self.modelBindingExtension4()
        self.showSignatureVC()
        isUserInteractionEnabledBinding()
    }
    func modelBindingExtention1() {
        // Habilita o deshabilita el botón de agrupamiento por similaridad
        inboxViewModel.similarityViewButtonIsEnable.subscribe(onNext: { [weak self] isEnabled in
            guard let self = self else { return }
            self.similarityViewButton.isEnabled = isEnabled
            self.showMoreIndicators()
            self.goToTop()
        }).disposed(by: self.disposeBag)
        // Habilita o deshabilita el botón de agrupamiento por vista normal
        inboxViewModel.normalViewButtonIsEnable.subscribe(onNext: { [weak self] isEnabled in
            guard let self = self else { return }
            self.normalViewButton.isEnabled = isEnabled
            self.heigthCollectionViewConstraint.constant = isEnabled ? 8 : -60
            self.showMoreIndicators()
            self.goToTop()
        }).disposed(by: self.disposeBag)
        // Habilita o deshabilita el botón de agrupamiento por número de orden
        inboxViewModel.groupedByOrderNumberIsEnable.subscribe(onNext: { [weak self] isEnabled in
            guard let self = self else { return }
            self.groupByOrderNumberButton.isEnabled = isEnabled
            self.showMoreIndicators()
            self.goToTop()
        }).disposed(by: self.disposeBag)
        // Oculta o muestra los botones de agrupamiento cuando se se realiza una búsqueda
        inboxViewModel.hideGroupingButtons.subscribe(onNext: { [weak self] isHidden in
            guard let self = self else { return }
            self.similarityViewButton.isHidden = isHidden
            self.normalViewButton.isHidden = isHidden
            self.groupByOrderNumberButton.isHidden = isHidden
            self.showMoreIndicators()
            self.goToTop()
        }).disposed(by: self.disposeBag)
        // retorna mensaje si no hay card para cada status
        inboxViewModel.title
            .withLatestFrom(inboxViewModel.statusDataGrouped, resultSelector: { [weak self] title, data in
                guard let self = self else { return }
                let statusId = self.inboxViewModel.getStatusId(name: title)
                var message = String()
                if let orders = data.first {
                    if orders.items.count == 0 && statusId != -1 {
                        message = "No tienes órdenes \(title)"
                    }
                } else {
                    message = "No tienes órdenes \(title)"
                }
                self.collectionView.setEmptyMessage(message)
            }).subscribe().disposed(by: disposeBag)
    }
    func modelBindingExtension2() {
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
        [
            finishedButton.rx.tap.bind(to: inboxViewModel.finishedDidTap),
            pendingButton.rx.tap.bind(to: inboxViewModel.pendingDidTap),
            processButton.rx.tap.bind(to: inboxViewModel.processDidTap),
            similarityViewButton.rx.tap.bind(to: inboxViewModel.similarityViewButtonDidTap),
            normalViewButton.rx.tap.bind(to: inboxViewModel.normalViewButtonDidTap),
            groupByOrderNumberButton.rx.tap.bind(to: inboxViewModel.groupByOrderNumberButtonDidTap)
        ].forEach({ $0.disposed(by: disposeBag) })
        order.bind(to: inboxViewModel.selectOrder).disposed(by: disposeBag)
        selectedRowBinding()
        didScrollBinding()
        showKPIViewBinding()
    }

    func selectedRowBinding() {
        rootViewModel.selectedRow.subscribe(onNext: { [weak self] index in
            guard let self = self, let row = index?.row else { return }
            self.chageStatusName(index: row)
            self.hideButtons(index: row)
            self.goToTop()
        }).disposed(by: disposeBag)
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

    func modelBindingExtension3() {
        // Muestra o oculta el loading
        inboxViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] showLoading in
            guard let self = self else { return }
            self.view.isUserInteractionEnabled = true
            if showLoading {
                self.lottieManager.showLoading()
                return
            }
            self.lottieManager.hideLoading()
        }).disposed(by: self.disposeBag)
        // Muestra un mensaje AlertViewController
        inboxViewModel.showAlert.observeOn(MainScheduler.instance)
            .subscribe(onNext: { [unowned self] message in
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.disposeBag)
        inboxViewModel.hasConnection.observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] hasConnection in
                guard let self = self else { return }
                if hasConnection { self.order.onNext(self.productID) }
            }).disposed(by: disposeBag)

        collectionView.rx.itemSelected.observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] indexPath in
                guard let self = self else { return }
                guard self.indexPathsSelected.count > 0 else {
                    self.lastRect = self.collectionView.layoutAttributesForItem(at: indexPath)?.frame
                    self.lastIndexPath = indexPath
                    let orders = self.inboxViewModel.sectionOrders
                    let orderOptional = orders[safe: indexPath.section]?.items[safe: indexPath.row]
                    guard let order = orderOptional else { return }
                    self.detailTapped(order: order)
                    return
                }
                self.updateCellWithIndexPath(indexPath)
        }).disposed(by: self.disposeBag)
    }

    func modelBindingExtension4() {
        collectionView.rx.itemDeselected.subscribe(onNext: { [weak self] indexPath in
            guard let self = self else { return }
            guard self.indexPathsSelected.count > 0 else {
                self.lastRect = self.collectionView.layoutAttributesForItem(at: indexPath)?.frame
                self.lastIndexPath = indexPath
                let orders = self.inboxViewModel.sectionOrders
                let orderOptional = orders[safe: indexPath.section]?.items[safe: indexPath.row]
                guard let order = orderOptional else { return }
                self.detailTapped(order: order)
                return
            }
            self.updateCellWithIndexPath(indexPath)
        }).disposed(by: disposeBag)

        inboxViewModel
            .reloadData
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] _ in
                guard let self = self, let lastRect = self.lastRect else { return }
                self.collectionView.scrollRectToVisible(lastRect, animated: true)
            })
            .disposed(by: self.disposeBag)
    }
    func initComponents() {
        self.processButton.isEnabled = false
        self.pendingButton.isEnabled = false
        self.finishedButton.isEnabled = false
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
        layout.itemSize = CGSize(width: 700, height: 180)
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
        }).disposed(by: disposeBag)
    }
    func chageStatusName(index: Int) {
        let name = self.inboxViewModel.getStatusName(index: index)
        self.inboxViewModel.title.onNext(name)
    }
    private func showMoreIndicators() {
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
    private func goToTop() {
        collectionView.setContentOffset(.zero, animated: false)
    }
    private func hideButtons(index: Int) {
        showContainersButtons.isHidden = true
        switch index {
        case 0:
            self.changePropertyIsHiddenStatusButtons(false, true, false)
            showContainersButtons.isHidden = false
        case 1: self.changePropertyIsHiddenStatusButtons(true, false, false)
        case 2: self.changePropertyIsHiddenStatusButtons(false, true, true)
        case 3: self.changePropertyIsHiddenStatusButtons(true, true, true)
        case 4: self.changePropertyIsHiddenStatusButtons(true, false, false)
        default: self.changePropertyIsHiddenStatusButtons(true, true, true)
        }
    }
    private func changePropertyIsHiddenStatusButtons(
        _ processButtonIsHidden: Bool,
        _ finishedButtonIsHidden: Bool,
        _ pendingButtonIsHidden: Bool) {
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
            destination.statusType = self.inboxViewModel.getStatusName(index: statusId - 1)
            destination.destiny = destiny
           }
       }else if (segue.identifier == ViewControllerIdentifiers.patientListViewController) {
           if let destination = segue.destination as? PatientListViewController {
               guard let orderId = self.inboxViewModel.selectedOrder?.baseDocument else { return }
               guard let patientList = self.inboxViewModel.selectedOrder?.patientName else { return }
               
               destination.order = orderId
               destination.patientList = patientList
           }
        
        }
    }

    func detailTapped(order: Order) {
        self.inboxViewModel.selectedOrder = order
        self.view.endEditing(true)
        self.performSegue(withIdentifier: ViewControllerIdentifiers.orderDetailViewController, sender: nil)
    }

    @objc func handleLongPress(gesture: UILongPressGestureRecognizer!) {

        if gesture.state != .ended { return }

        let position = gesture.location(in: self.collectionView)

        if let indexPath = self.collectionView.indexPathForItem(at: position) {
            self.updateCellWithIndexPath(indexPath)
        }
    }

    func updateCellWithIndexPath(_ indexPath: IndexPath) {
        indexPathsSelected.contains(indexPath) ?
            collectionView.deselectItem(at: indexPath, animated: true) :
            collectionView.selectItem(at: indexPath, animated: true, scrollPosition: .centeredVertically)

        if let index = indexPathsSelected.firstIndex(of: indexPath) {
            indexPathsSelected.remove(at: index)
        } else {
            indexPathsSelected.append(indexPath)
        }

        if indexPathsSelected.count > 0 {
            UIView.animate(withDuration: 0.2, animations: { [weak self] in
                guard let self = self else { return }
                self.removeOrdersSelectedVerticalSpace.constant = 48
                self.view.layoutIfNeeded()
            })
            processButton.isEnabled = true
            pendingButton.isEnabled = true
            finishedButton.isEnabled = true
        } else {
            UIView.animate(withDuration: 0.2, animations: { [weak self] in
                guard let self = self else { return }
                self.removeOrdersSelectedVerticalSpace.constant = -60
                self.view.layoutIfNeeded()
            })
            processButton.isEnabled = false
            pendingButton.isEnabled = false
            finishedButton.isEnabled = false
        }

    }

    @IBAction func removeSelectedOrdersDidPressed(_ sender: Any) {
        self.indexPathsSelected.removeAll()
        DispatchQueue.main.async {
            self.collectionView.reloadData()
            self.processButton.isEnabled = false
            self.pendingButton.isEnabled = false
            self.finishedButton.isEnabled = false
        }
        if self.removeOrdersSelectedVerticalSpace.constant > 0 {
            UIView.animate(withDuration: 0.2, animations: { [weak self] in
                guard let self = self else { return }
                self.removeOrdersSelectedVerticalSpace.constant = -60
                self.view.layoutIfNeeded()
            })
        }
    }

    private func updateRemoveViewColor(title: String) -> UIColor {
        switch title {
        case StatusNameConstants.assignedStatus:
            lastColor = OmicronColors.assignedStatus
            return OmicronColors.assignedStatus
        case StatusNameConstants.inProcessStatus:
            lastColor = OmicronColors.processStatus
            return OmicronColors.processStatus
        case StatusNameConstants.penddingStatus:
            lastColor = OmicronColors.pendingStatus
            return OmicronColors.pendingStatus
        case StatusNameConstants.finishedStatus:
            lastColor = OmicronColors.finishedStatus
            return OmicronColors.finishedStatus
        case StatusNameConstants.reassignedStatus:
            lastColor = OmicronColors.reassignedStatus
            return OmicronColors.reassignedStatus
        default: return lastColor
        }
    }
    
    func getNamesByOrder(productID: Int) -> Order? {
        let order = inboxViewModel.sectionOrders.first(where: { value -> Bool in
            value.model == "Pedido: \(productID)"
        })
        let orderSend = order?.items[0]
        orderSend?.baseDocument = productID
        return orderSend;
    }

}

// MARK: Extencions
extension InboxViewController: CardCellDelegate {
    func downloadPdf(id: Int) {
        inboxViewModel.getConnection()
        self.productID = id
    }
    
    func patientList(order: Order) {
        self.inboxViewModel.selectedOrder = order
        self.performSegue(withIdentifier: ViewControllerIdentifiers.patientListViewController, sender: nil)
    }
    
    // Chec this
//    func detailTapped(order: Order) {
//        self.inboxViewModel.selectedOrder = order
//        self.view.endEditing(true)
//        self.performSegue(withIdentifier: ViewControllerIdentifiers.orderDetailViewController, sender: nil)
//    }
}

extension UICollectionView {
    func setEmptyMessage(_ message: String) {
        let messageLabel = UILabel(
            frame: CGRect(x: 0, y: 0, width: self.bounds.size.width, height: self.bounds.size.height))
        messageLabel.text = message
        messageLabel.textColor = .black
        messageLabel.numberOfLines = 0
        messageLabel.textAlignment = .center
        messageLabel.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 15)
        messageLabel.sizeToFit()
        self.backgroundView = messageLabel
    }
    func restore() {
        self.backgroundView = nil
    }
    // swiftlint:disable file_length
}

// MARK: - HeaderSelectedDelegate

extension InboxViewController: HeaderSelectedDelegate {

    func headerSelected(productID: Int) {
        inboxViewModel.getConnection()
        self.productID = productID
    }
    
    func tapPatientList(productID: Int) {
        if let order = self.getNamesByOrder(productID: productID) {
            if order.patientName != "" {
                self.inboxViewModel.selectedOrder = order
                self.performSegue(withIdentifier: ViewControllerIdentifiers.patientListViewController, sender: nil)
            }
        }
    }

}

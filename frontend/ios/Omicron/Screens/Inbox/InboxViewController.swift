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
    var order = PublishSubject<Int>()
    // MARK: - Variables
    private var bindingCollectionView = true
    @Injected var inboxViewModel: InboxViewModel
    @Injected var rootViewModel: RootViewModel
    @Injected var lottieManager: LottieManager
    @Injected var orderDetailVM: OrderDetailViewModel
    let disposeBag = DisposeBag()
    var productID = 0
    var indexPathsSelected: [IndexPath] = []
    var lastColor = UIColor.blue
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.title = StatusNameConstants.assignedStatus
        self.collectionView.allowsMultipleSelection = true

        viewModelBinding()
        self.initComponents()
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
        finishedButton.isHidden = true
        pendingButton.isHidden = true
        navigationItem.rightBarButtonItem = getOmniconLogo()
        let lpgr = UILongPressGestureRecognizer(target: self,
                                                action: #selector(InboxViewController.handleLongPress(gesture:)))
        lpgr.minimumPressDuration = 0.5
        lpgr.delaysTouchesBegan = true
        self.collectionView.addGestureRecognizer(lpgr)
        let tapGestureRecognizer = UITapGestureRecognizer(
            target: self,
            action: #selector(InboxViewController.tappedPressed(gesture:))
        )
        self.collectionView.addGestureRecognizer(tapGestureRecognizer)
        removeOrdersSelectedView.layer.cornerRadius = removeOrdersSelectedView.frame.width / 2

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
    func getDecimalPartOfDouble(number: Double) -> Double {
        return number.truncatingRemainder(dividingBy: 1)
    }
    func returnCardIsolateOrderCollectionViewCell(
        indexPath: IndexPath, element: SectionModel<String, Order>.Item,
        decimalPart: Double?) -> CardIsolatedOrderCollectionViewCell {
        let cell = collectionView.dequeueReusableCell(
            withReuseIdentifier: ViewControllerIdentifiers.cardIsolatedOrderReuseIdentifier,
            for: indexPath) as? CardIsolatedOrderCollectionViewCell
        cell?.row = indexPath.row
        cell?.order = element
        cell?.numberDescriptionLabel.text = "\(element.productionOrderId ?? 0)"
        cell?.plannedQuantityDescriptionLabel.text = decimalPart  ?? 0.0 > 0.0 ?
            String(format: "%6f", NSDecimalNumber(decimal: element.plannedQuantity ?? 0.0).doubleValue) :
            "\(element.plannedQuantity ?? 0.0)"
        cell?.startDateDescriptionLabel.text = element.startDate ?? CommonStrings.empty
        cell?.finishDateDescriptionLabel.text = element.finishDate ?? CommonStrings.empty
        cell?.productDescriptionLabel.text = element.descriptionProduct ?? CommonStrings.empty
        cell?.missingStockImage.isHidden = !element.hasMissingStock
        if indexPathsSelected.contains(indexPath) {
            cell?.isSelected = true
        } else {
            cell?.isSelected = false
        }
        return cell!
    }
    func returnCardCollectionViewCell(indexPath: IndexPath, element: SectionModel<String, Order>.Item,
                                      decimalPart: Double?) -> CardCollectionViewCell {
        let cell = collectionView.dequeueReusableCell(
            withReuseIdentifier: ViewControllerIdentifiers.cardReuseIdentifier, for: indexPath)
            as? CardCollectionViewCell
        cell?.row = indexPath.row
        cell?.order = element
        cell?.numberDescriptionLabel.text = "\(element.productionOrderId ?? 0)"
        cell?.baseDocumentDescriptionLabel.text = element.baseDocument == 0 ?
            CommonStrings.empty : "\(element.baseDocument ?? 0)"
        cell?.tagDescriptionLabel.text = element.tag
        cell?.containerLabel.text = element.container
        if !element.finishedLabel {
            cell?.tagDescriptionLabel.textColor = .red
        } else {
            cell?.tagDescriptionLabel.textColor = .systemGreen
        }
        cell?.plannedQuantityDescriptionLabel.text = decimalPart  ?? 0.0 > 0.0 ?
            String(format: "%6f", NSDecimalNumber(decimal: element.plannedQuantity ?? 0.0).doubleValue) :
            "\(element.plannedQuantity ?? 0.0)"
        cell?.startDateDescriptionLabel.text = element.startDate ?? CommonStrings.empty
        cell?.finishDateDescriptionLabel.text = element.finishDate ?? CommonStrings.empty
        cell?.productDescriptionLabel.text = element.descriptionProduct ?? CommonStrings.empty
        cell?.missingStockImage.isHidden = !element.hasMissingStock
        if indexPathsSelected.contains(indexPath) {
            cell?.isSelected = true
        } else {
            cell?.isSelected = false
        }
        return cell!
    }
    func viewModelBindingCollectionView() {
        // Pinta las cards
        let dataSource = RxCollectionViewSectionedReloadDataSource<SectionModel<String, Order>>(
            configureCell: { [weak self] (_, _, indexPath, element) in
            let decimalPart = self?.getDecimalPartOfDouble(
                number: NSDecimalNumber(decimal: element.plannedQuantity ?? 0.0).doubleValue)
            if element.baseDocument != 0 {
                let cell = self?.returnCardCollectionViewCell(
                    indexPath: indexPath, element: element, decimalPart: decimalPart)
                cell?.delegate = self
                return cell!
            } else {
                let cell = self?.returnCardIsolateOrderCollectionViewCell(
                    indexPath: indexPath, element: element, decimalPart: decimalPart)
                cell!.delegate = self
                return cell!
            }
        })
        dataSource
            .configureSupplementaryView = { (dataSource, collectionView, kind, indexPath) -> UICollectionReusableView in
                let header = collectionView.dequeueReusableSupplementaryView(
                    ofKind: UICollectionView.elementKindSectionHeader,
                    withReuseIdentifier: ViewControllerIdentifiers.headerReuseIdentifier,
                    for: indexPath) as? HeaderCollectionViewCell
                let headerText = dataSource.sectionModels[indexPath.section].identity
                header?.productID.text = headerText
                if headerText.contains("Pedido") {
                    let productId = headerText
                        .components(separatedBy: CharacterSet.decimalDigits.inverted)
                        .joined()
                    header?.productId = Int(productId) ?? 0
                    header?.delegate = self
                    header?.pdfImageView.isHidden = false
                } else {
                    header?.productId = 0
                    header?.delegate = nil
                    header?.pdfImageView.isHidden = true
                }
                return header!
        }
        inboxViewModel.statusDataGrouped
            .bind(to: collectionView.rx.items(dataSource: dataSource))
            .disposed(by: disposeBag)
    }
    // MARK: - Functions
    func viewModelBinding() {
        inboxViewModel.title.subscribe(onNext: { [weak self] title in
            self?.title = title
            guard let statusId = self?.inboxViewModel.getStatusId(name: title) else { return }
            self?.hideButtons(index: statusId)
            self?.removeOrdersSelectedView.backgroundColor = self?.updateRemoveViewColor(title: title)
        }).disposed(by: disposeBag)
        inboxViewModel.refreshDataWhenChangeProcessIsSucces
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.rootViewModel.getOrders()
        }).disposed(by: self.disposeBag)
        inboxViewModel.processButtonIsEnable.subscribe(onNext: { [weak self] isEnable in
            self?.processButton.isEnabled = isEnable
        }).disposed(by: self.disposeBag)
        // Habilita o deshabilita el botón para cambiar a pendiente
        inboxViewModel.pendingButtonIsEnable.subscribe(onNext: { [weak self] isEnable in
            self?.pendingButton.isEnabled = isEnable
        }).self.disposed(by: self.disposeBag)
        inboxViewModel.orderURLPDF.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] urlPDF in
            guard let self = self else { return }
            print(urlPDF)
            DispatchQueue.main.async {
                let pdfViewController = PDFViewController()
                pdfViewController.pdfURL = URL(string: urlPDF)
                self.present(pdfViewController, animated: true, completion: nil)
            }
        }).disposed(by: disposeBag)
        self.modelBindingExtention1()
        self.modelBindingExtension2()
        self.modelBindingExtension3()
        self.showSignatureVC()
        self.finishOrders()
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
            let statusId = self?.inboxViewModel.getStatusId(name: title) ?? -1
            var message: String = ""
            if let orders = data.first {
                if orders.items.count == 0 && statusId != -1 {
                    message = "No tienes órdenes \(title)"
                }
            } else {
                message = "No tienes órdenes \(title)"
            }
            self?.collectionView.setEmptyMessage(message)
        }).subscribe().disposed(by: disposeBag)
    }
    func modelBindingExtension2() {
        // Muestra un alert para la confirmación de cambio de estatus de una orden
        inboxViewModel.showAlertToChangeOrderOfStatus
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] data in
            let alert = UIAlertController(title: data.message, message: nil, preferredStyle: .alert)
            let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive, handler: nil)
            let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default, handler: { [weak self] _ in
                self?.view.isUserInteractionEnabled = false
                if data.typeOfStatus == StatusNameConstants.finishedStatus {
                    // Primero se verifica si se pueden finalizar
                    self?.inboxViewModel.showSignatureVc.onNext(CommonStrings.signatureViewTitleQFB)
                } else {
                    // Si la respuesta es OK se cambiará a status pendiente, se mandan los index selecionados para cambiar el status
                    self?.inboxViewModel.changeStatus(
                        indexPath: self?.indexPathsSelected,
                        typeOfStatus: data.typeOfStatus)
                }
            })
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
            groupByOrderNumberButton.rx.tap.bind(to: inboxViewModel.groupByOrderNumberButtonDidTap)
        ].forEach({ $0.disposed(by: disposeBag) })
        order.bind(to: inboxViewModel.selectOrder).disposed(by: disposeBag)
        rootViewModel.selectedRow.subscribe(onNext: { [weak self] index in
            guard let row = index?.row else { return }
            self?.chageStatusName(index: row)
            self?.hideButtons(index: row)
            self?.goToTop()
        }).disposed(by: disposeBag)
        collectionView.rx.didScroll.subscribe({ _ in
            self.collectionView.removeMoreIndicator()
        }).disposed(by: disposeBag)
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
            self?.view.isUserInteractionEnabled = true
            if showLoading {
                self?.lottieManager.showLoading()
                return
            }
            self?.lottieManager.hideLoading()
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
                    self?.removeOrdersSelectedVerticalSpace.constant = -60
                    self?.view.layoutIfNeeded()
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
        switch index {
        case 0:
            self.changePropertyIsHiddenStatusButtons(
                processButtonIsHidden: false,
                finishedButtonIsHidden: true,
                pendingButtonIsHidden: false)
        case 1:
            self.changePropertyIsHiddenStatusButtons(
                processButtonIsHidden: true,
                finishedButtonIsHidden: false,
                pendingButtonIsHidden: false)
        case 2:
            self.changePropertyIsHiddenStatusButtons(
                processButtonIsHidden: false,
                finishedButtonIsHidden: true,
                pendingButtonIsHidden: true)
        case 3:
            self.changePropertyIsHiddenStatusButtons(
                processButtonIsHidden: true,
                finishedButtonIsHidden: true,
                pendingButtonIsHidden: true)
        case 4:
            self.changePropertyIsHiddenStatusButtons(
                processButtonIsHidden: true,
                finishedButtonIsHidden: true,
                pendingButtonIsHidden: true)
        default:
            self.changePropertyIsHiddenStatusButtons(
                processButtonIsHidden: true,
                finishedButtonIsHidden: true,
                pendingButtonIsHidden: true)
        }
    }
    private func changePropertyIsHiddenStatusButtons(
        processButtonIsHidden: Bool,
        finishedButtonIsHidden: Bool,
        pendingButtonIsHidden: Bool) {
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
            DispatchQueue.main.async {
                self.updateCellWithIndexPath(indexPath)
            }
        }
    }

    @objc func tappedPressed(gesture: UITapGestureRecognizer!) {

        let position = gesture.location(in: self.collectionView)
        if let indexPath = self.collectionView.indexPathForItem(at: position) {
            if indexPathsSelected.count > 0 {
                DispatchQueue.main.async {
                    self.updateCellWithIndexPath(indexPath)
                }
            } else {
                let orders = self.inboxViewModel.sectionOrders
                let orderOptional = orders[safe: indexPath.section]?.items[safe: indexPath.row]
                guard let order = orderOptional else { return }
                self.detailTapped(order: order)
            }
        }

    }

    func updateCellWithIndexPath(_ indexPath: IndexPath) {

        if let cell = self.collectionView.cellForItem(at: indexPath) as? CardCollectionViewCell {
            if indexPathsSelected.contains(indexPath) {
                cell.isSelected = false
            } else {
                cell.isSelected = true
            }
        } else if let cell = self.collectionView.cellForItem(at: indexPath) as? CardIsolatedOrderCollectionViewCell {
            if indexPathsSelected.contains(indexPath) {
                cell.isSelected = false
            } else {
                cell.isSelected = true
            }
        }

        let indexOptional = indexPathsSelected.firstIndex(of: indexPath)
        if let index = indexOptional {
            indexPathsSelected.remove(at: index)
        } else {
            indexPathsSelected.append(indexPath)
        }

        if indexPathsSelected.count > 0 {
            UIView.animate(withDuration: 0.2, animations: { [weak self] in
                self?.removeOrdersSelectedVerticalSpace.constant = 48
                self?.view.layoutIfNeeded()
            })
            processButton.isEnabled = true
            pendingButton.isEnabled = true
            finishedButton.isEnabled = true
        } else {
            UIView.animate(withDuration: 0.2, animations: { [weak self] in
                self?.removeOrdersSelectedVerticalSpace.constant = -60
                self?.view.layoutIfNeeded()
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
        }
        if self.removeOrdersSelectedVerticalSpace.constant > 0 {
            UIView.animate(withDuration: 0.2, animations: { [weak self] in
                self?.removeOrdersSelectedVerticalSpace.constant = -60
                self?.view.layoutIfNeeded()
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

}

// MARK: Extencions
extension InboxViewController: CardCellDelegate {
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

}

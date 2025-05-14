//
//  OrderDetailViewController.swift
//  Omicron
//
//  Created by Axity on 07/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa
import Resolver
// swiftlint:disable type_body_length
class OrderDetailViewController: UIViewController {
    // Outlets
    @IBOutlet weak var deleteManyButton: UIButton!
    @IBOutlet weak var processButton: UIButton!
    @IBOutlet weak var finishedButton: UIButton!
    @IBOutlet weak var addComponentButton: UIButton!
    @IBOutlet weak var saveButton: UIButton!
    @IBOutlet weak var seeLotsButton: UIButton!
    @IBOutlet weak var penddingButton: UIButton!
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var codeDescriptionLabel: UILabel!
    @IBOutlet weak var documentBaseDescriptionLabel: UILabel!
    @IBOutlet weak var containerDescriptionLabel: UILabel!
    @IBOutlet weak var tagDescriptionLabel: UILabel!
    @IBOutlet weak var sumFormulaDescriptionLabel: UILabel!
    @IBOutlet weak var quantityPlannedDescriptionLabel: UILabel!
    @IBOutlet weak var startDateDescriptionLabel: UILabel!
    @IBOutlet weak var finishedDateDescriptionLabel: UILabel!
    @IBOutlet weak var productDescritionLabel: UILabel!
    @IBOutlet weak var infoView: UIView!
    @IBOutlet weak var hashtagLabel: UILabel!
    // MARK: Outlets from table header
    @IBOutlet weak var htCode: UILabel!
    @IBOutlet weak var htDescription: UILabel!
    @IBOutlet weak var htBaseQuantity: UILabel!
    @IBOutlet weak var htrequiredQuantity: UILabel!
    @IBOutlet weak var htUnit: UILabel!
    @IBOutlet weak var htWerehouse: UILabel!
    @IBOutlet weak var detailTable: UITableView!
    @IBOutlet weak var tableView: UITableView!
    @IBOutlet weak var destinyLabel: UILabel!
    @IBOutlet weak var labelSpaceQuantity: UILabel!
    @IBOutlet weak var quantityTextField: UITextField!
    @IBOutlet weak var quantityButtonChange: UIButton!

    // MARK: Variables
    @Injected var orderDetailViewModel: OrderDetailViewModel
    @Injected var rootViewModel: RootViewModel
    @Injected var lottieManager: LottieManager

    var disposeBag: DisposeBag = DisposeBag()
    var orderId: Int = -1
    var statusType = String()
    var indexOfTableToEditItem: Int = -1
    let formatter = UtilsManager.shared.formatterDoublesTo6Decimals()
    var orderDetail: [OrderDetail] = []
    var refreshControl = UIRefreshControl()
    var destiny = String()
    var isolatedOrder = false
    var emptyStockProductId: [String] = []
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.tableView.delegate = self
        self.quantityTextField.delegate = self
        self.title = CommonStrings.formulaDetail
        self.showButtonsByStatusType(statusType: statusType)
        self.tableView.allowsMultipleSelectionDuringEditing = false
        tableView.setEditing(false, animated: true)
        self.orderDetailViewModel.orderId = self.orderId
        infoView.layer.cornerRadius = 10
    }
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(true)
        self.disposeBag = DisposeBag()
        self.initComponents()
        self.viewModelBinding()
        quantityButtonBindind()
        self.orderDetailViewModel.getOrdenDetail()
        self.refreshViewControl()
    }
    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        disposeBag = DisposeBag()
    }
    // MARK: - Functions
    @objc func goToCommentsViewController() {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let commentsVC = storyboard.instantiateViewController(
            withIdentifier: ViewControllerIdentifiers.commentsViewController) as? CommentsViewController
        commentsVC?.orderDetail = self.orderDetail
        commentsVC?.originView = ViewControllerIdentifiers.orderDetailViewController
        commentsVC?.modalPresentationStyle = .overCurrentContext
        commentsVC?.modalTransitionStyle = .crossDissolve
        self.present(commentsVC ?? CommentsViewController(), animated: true, completion: nil)
    }
    // click boton de agregar componente
    func goToComponentsViewController() {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let componentsVC = storyboard.instantiateViewController(
            withIdentifier: ViewControllerIdentifiers.componentsViewController) as? ComponentsViewController
        componentsVC!.clearObservables()
        let navigationVC = UINavigationController(rootViewController: componentsVC ?? ComponentsViewController())
        navigationVC.modalPresentationStyle = .formSheet
        self.present(navigationVC, animated: true, completion: nil)
    }
    // Inicia la ejecución del refresh control
    func refreshViewControl() {
        self.refreshControl.tintColor = OmicronColors.blue
        self.refreshControl.attributedTitle = NSAttributedString(string: CommonStrings.updatingData)
        refreshControl.addTarget(self, action: #selector(refresh(_:)), for: .valueChanged)
        self.tableView.addSubview(self.refreshControl)
    }
    @objc func refresh(_ sender: AnyObject) {
        self.orderDetailViewModel.getOrdenDetail(isRefresh: true)
    }
    func viewModelBinding() {
        self.viewModelBinding1()
        self.viewModelBinding2()
        self.viewModelBinding3()
        self.viewModelBinding4()
        self.deleteManyDidEnableBinding()
        self.orderDetailViewModel.showIconComments.observe(on: MainScheduler.instance)
            .subscribe(onNext: { [weak self] iconName in
                guard let self = self else { return }
                let comments = UIBarButtonItem(image: UIImage(systemName: iconName),
                                               style: .plain, target: self,
                                               action: #selector(self.goToCommentsViewController))
                self.navigationItem.rightBarButtonItems = [self.getOmniconLogo(), comments]
            }).disposed(by: self.disposeBag)
        orderDetailViewModel.showAlert.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] message in
            guard let self = self else { return }
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.disposeBag)
        orderDetailViewModel.loading.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] showLoading in
            if showLoading {
                self?.lottieManager.showLoading()
            } else {
                self?.lottieManager.hideLoading()
            }
        }).disposed(by: self.disposeBag)
        self.orderDetailViewModel.showSignatureView
            .observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] titleView in
                let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
                let signatureVC = storyboard.instantiateViewController(
                    identifier: ViewControllerIdentifiers.signaturePadViewController) as? SignaturePadViewController
                signatureVC?.titleView = titleView
                signatureVC?.originView = ViewControllerIdentifiers.orderDetailViewController
                signatureVC?.modalPresentationStyle = .overCurrentContext
                signatureVC?.modalTransitionStyle = .crossDissolve
                self?.present(signatureVC ?? SignaturePadViewController(), animated: true, completion: nil)
            }).disposed(by: self.disposeBag)
    }
    func viewModelBinding1() {
        // Termina la ejecución del refresh control
        self.orderDetailViewModel.endRefreshing.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self]  _ in
            self?.refreshControl.endRefreshing()
        }).disposed(by: self.disposeBag)
        self.orderDetailViewModel.backToInboxView.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.navigationController?.popToRootViewController(animated: true)
        }).disposed(by: self.disposeBag)
        self.orderDetailViewModel.goToSeeLotsViewController.observe(on: MainScheduler.instance)
            .subscribe(onNext: { [weak self] _ in
                let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
                let lotsVC = storyboard.instantiateViewController(
                    identifier: ViewControllerIdentifiers.lotsViewController) as? LotsViewController
                if self?.orderId != nil && self?.statusType != nil && self?.orderDetail != nil {
                    lotsVC?.orderId = self?.orderId ?? 0
                    if let self = self { lotsVC?.emptyStockProductId = self.emptyStockProductId }
                    lotsVC?.statusType = self?.statusType ?? String()
                    lotsVC?.orderDetail = self?.orderDetail ?? []
                    if let order = self?.orderDetail.first {
                        if order.productDescription != nil && order.code != nil &&
                            order.productionOrderID != nil && order.baseDocument != nil {
                            lotsVC?.orderNumber =  "\(order.baseDocument ?? 0)"
                            lotsVC?.manufacturingOrder = "\(order.productionOrderID ?? 0)"
                            lotsVC?.codeDescription =
                                "\(order.code ?? String())  \(order.productDescription ?? String())"
                        }
                    }
                    self?.navigationController?.pushViewController(lotsVC ?? LotsViewController(), animated: true)
                }
            }).disposed(by: self.disposeBag)
        self.processButton.rx.tap.bind(to: orderDetailViewModel.processButtonDidTap).disposed(by: self.disposeBag)
        self.seeLotsButton.rx.tap.bind(to: orderDetailViewModel.seeLotsButtonDidTap).disposed(by: self.disposeBag)
        self.finishedButton.rx.tap.bind(to: orderDetailViewModel.finishedButtonDidTap).disposed(by: self.disposeBag)
        self.penddingButton.rx.tap.bind(to: orderDetailViewModel.pendingButtonDidTap).disposed(by: self.disposeBag)
        self.deleteManyButton.rx.tap.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.deleteConfirmationDialog()
        }).disposed(by: self.disposeBag)
        self.addComponentButton.rx.tap.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.goToComponentsViewController()
        }).disposed(by: disposeBag)
    }
    func getDecimalPartOfDouble(number: Double) -> Double {
        return number.truncatingRemainder(dividingBy: 1)
    }
    func initComponents() {
        UtilsManager.shared.setStyleButtonStatus(
            button: self.deleteManyButton,
            title: StatusNameConstants.deleteComponents,
                                                 color: OmicronColors.processStatus,
                                                 titleColor: OmicronColors.processStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.finishedButton, title: StatusNameConstants.finishedStatus,
                                                 color: OmicronColors.finishedStatus,
                                                 titleColor: OmicronColors.finishedStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.penddingButton, title: StatusNameConstants.penddingStatus,
                                                 color: OmicronColors.pendingStatus,
                                                 titleColor: OmicronColors.pendingStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.processButton, title: StatusNameConstants.inProcessStatus,
                                                 color: OmicronColors.processStatus,
                                                 titleColor: OmicronColors.processStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.addComponentButton,
                                                 title: StatusNameConstants.addComponent,
                                                 color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.saveButton, title: StatusNameConstants.save,
                                                 color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.seeLotsButton, title: StatusNameConstants.seeLots,
                                                 color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.labelsStyle(label: self.titleLabel, text: CommonStrings.components, fontSize: 22)
        UtilsManager.shared.labelsStyle(label: self.htCode, text: CommonStrings.code, fontSize: 19,
                                        typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.hashtagLabel, text: CommonStrings.hashtag, fontSize: 19,
                                        typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htBaseQuantity, text: CommonStrings.baseQuantity, fontSize: 19,
                                        typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htrequiredQuantity, text: CommonStrings.pQuantity, fontSize: 19,
                                        typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htUnit, text: CommonStrings.unit, fontSize: 19,
                                        typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htWerehouse, text: CommonStrings.warehouse, fontSize: 19,
                                        typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htDescription, text: CommonStrings.description, fontSize: 19,
                                        typeFont: CommonStrings.bold)
    }

    func initComponents2() {
        self.productDescritionLabel.textColor = .white
        // Se cambian de color los Labels
        self.changeTextColorLabel(color: OmicronColors.ligthGray)
        self.productDescritionLabel.font = .fontDefaultBold(22)
        self.detailTable.tableFooterView = UIView()
        self.infoView.layer.cornerRadius = 10.0
        self.infoView.backgroundColor = OmicronColors.ligthGray
        self.changeTextColorHtLabels(color: .white)
    }

    func changeTextColorLabel(color: UIColor) {
        self.codeDescriptionLabel.textColor = color
        self.containerDescriptionLabel.textColor = color
        self.tagDescriptionLabel.textColor = color
        self.documentBaseDescriptionLabel.textColor = color
        self.sumFormulaDescriptionLabel.textColor = color
        self.quantityPlannedDescriptionLabel.textColor = color
        self.startDateDescriptionLabel.textColor = color
        self.finishedDateDescriptionLabel.textColor = color
        self.destinyLabel.textColor = color
    }
    func changeTextColorHtLabels(color: UIColor) {
        self.htCode.textColor = color
        self.htDescription.textColor = color
        self.htBaseQuantity.textColor = color
        self.htrequiredQuantity.textColor = color
        self.htUnit.textColor = color
        self.htWerehouse.textColor = color
        self.hashtagLabel.textColor = color
        self.titleLabel.textColor = color
    }
    func showButtonsByStatusType(statusType: String) {
        var hideBtn = HideButtons(
            process: true, finished: true, pending: true, addComp: true, save: true, seeBatches: true)
        switch statusType {
        case StatusNameConstants.assignedStatus:
            hideBtn = HideButtons(process: false, finished: true, pending: false,
                                  addComp: true, save: true, seeBatches: true)
        case StatusNameConstants.inProcessStatus:
            hideBtn = HideButtons(process: true, finished: false, pending: false,
                                  addComp: false, save: true, seeBatches: false)
        case StatusNameConstants.penddingStatus:
            hideBtn = HideButtons(process: false, finished: true, pending: true,
                                  addComp: true, save: true, seeBatches: false)
        case StatusNameConstants.finishedStatus:
            hideBtn = HideButtons(process: true, finished: true, pending: true,
                                  addComp: true, save: true, seeBatches: false)
        case StatusNameConstants.reassignedStatus:
            hideBtn = HideButtons(process: true, finished: false, pending: false,
                                  addComp: false, save: true, seeBatches: false)
        default: break
        }
        self.changeHidePropertyOfButtons(hideBtn)
    }

    func changeHidePropertyOfButtons(_ hideBtns: HideButtons) {
        self.processButton.isHidden = hideBtns.process
        self.finishedButton.isHidden = hideBtns.finished
        self.penddingButton.isHidden = hideBtns.pending
        self.addComponentButton.isHidden = hideBtns.addComp
        self.saveButton.isHidden = hideBtns.save
        self.seeLotsButton.isHidden = hideBtns.seeBatches
    }
    func sendIndexToDelete(index: Int) {
        orderDetailViewModel.deleteItemFromTable(indexs: [index])
    }
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        if segue.identifier == ViewControllerIdentifiers.orderDetailFormViewController {
            if let destination = segue.destination as? OrderDetailFormViewController {
                let orderDetailToEdit = orderDetailViewModel.getDataTableToEdit()
                destination.dataOfTable = orderDetailToEdit
                destination.indexOfItemSelected = self.indexOfTableToEditItem
            }
        }
    }

    func deleteManyDidEnableBinding() {
        self.orderDetailViewModel.deleteManyButtonIsEnable.observe(on: MainScheduler.instance)
            .subscribe(onNext: { [weak self] isEnable in
            self?.deleteManyButton.isHidden = !isEnable
        }).disposed(by: self.disposeBag)
    }

    func deleteConfirmationDialog() {
        let alert = UIAlertController(title: CommonStrings.deleteComponentMessage, message: nil, preferredStyle: .alert)
        let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive,
                                         handler: {[weak self] _ in
                                            self?.dismiss(animated: true, completion: nil)})
        let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default,
                                     handler: { [weak self] _ in
            self?.orderDetailViewModel.deleteItemsFromTableDidTap()
        })
        alert.addAction(cancelAction)
        alert.addAction(okAction)
        self.present(alert, animated: true, completion: nil)
    }
}

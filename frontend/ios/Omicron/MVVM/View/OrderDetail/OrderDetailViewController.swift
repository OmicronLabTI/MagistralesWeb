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
class OrderDetailViewController: UIViewController, SelectedPickerInput, AcceptButtonPressed {
    
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
    @IBOutlet weak var saveWarehousesChangesButton: UIButton!
    @IBOutlet weak var splitButton: UIButton!
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
    @Injected var inboxViewModel: InboxViewModel
    @Injected var splitOrderViewModel: SplitOrderViewModel
    

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
    var componentsToUpdate: [Component] = []
    var onGoingSplitProcess: Bool = false
    
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.tableView.delegate = self
        self.quantityTextField.delegate = self
        self.title = CommonStrings.formulaDetail
        self.showButtonsByStatusType(statusType: statusType)
        saveWarehousesChangesButton.isEnabled = false
        self.tableView.allowsMultipleSelectionDuringEditing = false
        tableView.setEditing(false, animated: true)
        self.orderDetailViewModel.orderId = self.orderId
        infoView.layer.cornerRadius = 10
        setupDismissPickerOnTap()
        initNavigationBar()
    }

    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(true)
        self.infoView.isHidden = true
        cleanLabels()
        self.initComponents()
        self.viewModelBinding()
        quantityButtonBindind()
        self.orderDetailViewModel.getOrdenDetail()
        self.refreshViewControl()
        self.componentsToUpdate = []
    }
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        splitButton.isEnabled = false
        self.productDescritionLabel.isHidden = true

    }
    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        disposeBag = DisposeBag()
        cleanLabels()
        orderDetailViewModel.tableData.onNext([])
        saveWarehousesChangesButton.isEnabled = false
        self.componentsToUpdate = []
        splitButton.isEnabled = false
        self.productDescritionLabel.isHidden = true

    }

    // MARK: - Functions
    func initNavigationBar() {
        let title = inboxViewModel.currentSection.statusName
        let backButton = UIButton(type: .system)
        backButton.setTitle("  \(title)", for: .normal)
        backButton.setImage(UIImage(systemName: "chevron.left"), for: .normal)
        backButton.tintColor = .systemBlue
        backButton.titleLabel?.font = UIFont.systemFont(ofSize: 19, weight: .medium)

        backButton.addTarget(self, action: #selector(backBtnAction(_:)), for: .touchUpInside)

        let barButton = UIBarButtonItem(customView: backButton)
        navigationItem.leftBarButtonItem = barButton
    }

    @objc func backBtnAction(_ sender: UIBarButtonItem) {
        if componentsToUpdate.isEmpty {
            returnBack()
            return
        }
        self.presentConfirmDialog()
    }

    func presentConfirmDialog() {
        let cancelAction = UIAlertAction(title: "Cancelar", style: .destructive, handler: nil)
        let okAction = UIAlertAction(title: CommonStrings.OKConst,
                                     style: .default, handler: { [weak self] _ in self?.resetValues()})
        AlertManager.shared.showAlert(title: CommonStrings.warehousesChangesConfirm,
                                      message: String(),
                                      actions: [cancelAction, okAction],
                                      view: self)
    }
    
    func resetValues() {
        componentsToUpdate = []
        returnBack()
    }

    func returnBack() {
        self.navigationController?.popViewController(animated: true)
    }
    
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
        self.orderDetailViewModel.disableSaveButton.subscribe(onNext: { [weak self] _ in
            self?.saveWarehousesChangesButton.isEnabled = false
        }).disposed(by: disposeBag)
        self.orderDetailViewModel.clearComponentsToUpdate.subscribe(onNext: { [weak self] _ in
            self?.componentsToUpdate = []
        }).disposed(by: disposeBag)
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
                guard let self = self else { return }
                
                self.validateNavigation(identifier: ViewControllerIdentifiers.lotsViewController,
                                                  controllerType: LotsViewController.self)
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
            if (orderDetailViewModel.warehousesOptions.count == 0) {
                AlertManager.shared.showAlert(message:"\(Constants.Errors.nowarehouses.rawValue) \(orderDetailViewModel.itemCode)",
                                              view: self)
                return
            }
            self.validateNavigation(identifier: ViewControllerIdentifiers.addComponentViewController,
                          controllerType: AddComponentViewController.self)
        }).disposed(by: disposeBag)
    }
    func getDecimalPartOfDouble(number: Double) -> Double {
        return number.truncatingRemainder(dividingBy: 1)
    }
    func validateNavigation<T: LotsBaseViewController>(identifier: String, controllerType: T.Type) {
        if (!componentsToUpdate.isEmpty) {
            let cancelAction = UIAlertAction(title: "Cancelar", style: .destructive, handler: nil)
            let okAction = UIAlertAction(title: CommonStrings.OKConst,
                                         style: .default, handler: { [weak self] _ in self?.goToPage(identifier: identifier,controllerType: controllerType)})
            AlertManager.shared.showAlert(title: CommonStrings.warehousesChangesConfirm,
                                          message: String(),
                                          actions: [cancelAction, okAction],
                                          view: self)
            return
        }

        self.goToPage(identifier: identifier,controllerType: controllerType)
    }

    func goToPage<T: LotsBaseViewController>(identifier: String, controllerType: T.Type) {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        
        guard let viewController = storyboard.instantiateViewController(withIdentifier: identifier) as? T else {
            print("⚠️ No se pudo instanciar \(T.self) con el identificador \(identifier)")
            return
        }
        
        if self.orderId != nil && self.statusType != nil && self.orderDetail != nil {
            viewController.orderId = self.orderId
            viewController.emptyStockProductId = self.emptyStockProductId
            viewController.statusType = self.statusType
            viewController.orderDetail = self.orderDetail
            if let order = self.orderDetail.first {
                if order.productDescription != nil && order.code != nil &&
                    order.productionOrderID != nil && order.baseDocument != nil {
                    viewController.orderNumber =  "\(order.baseDocument ?? 0)"
                    viewController.manufacturingOrder = "\(order.productionOrderID ?? 0)"
                    viewController.codeDescription =
                    "\(order.code ?? String())  \(order.productDescription ?? String())"
                }
            }
            viewController.warehousesOptions = self.orderDetailViewModel.warehousesOptions
            self.componentsToUpdate = []
            self.navigationController?.pushViewController(viewController, animated: true)
        }
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
        UtilsManager.shared.setStyleButtonStatus(button: self.saveWarehousesChangesButton, title: StatusNameConstants.save,
                                                 color: OmicronColors.blue,
                                                 titleColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.splitButton, title: StatusNameConstants.splitOrder,
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
        splitButton.isEnabled = false
    }
    
    func setupDismissPickerOnTap() {
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(dismissPicker))
        tapGesture.cancelsTouchesInView = false
        view.addGestureRecognizer(tapGesture)
    }

    @objc func dismissPicker() {
        view.endEditing(true)
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
            process: true, finished: true, pending: true, addComp: true, save: true, seeBatches: true, saveChanges: true, splitOrder: true)
        switch statusType {
        case StatusNameConstants.assignedStatus:
            hideBtn = HideButtons(process: false, finished: true, pending: false,
                                  addComp: true, save: true, seeBatches: true, saveChanges: true, splitOrder: true)
        case StatusNameConstants.inProcessStatus:
            hideBtn = HideButtons(process: true, finished: false, pending: false,
                                  addComp: false, save: true, seeBatches: false, saveChanges: false, splitOrder: false)
        case StatusNameConstants.penddingStatus:
            hideBtn = HideButtons(process: false, finished: true, pending: true,
                                  addComp: true, save: true, seeBatches: false, saveChanges: true, splitOrder: true)
        case StatusNameConstants.finishedStatus:
            hideBtn = HideButtons(process: true, finished: true, pending: true,
                                  addComp: false, save: true, seeBatches: false, saveChanges: true, splitOrder: true)
        case StatusNameConstants.reassignedStatus:
            hideBtn = HideButtons(process: true, finished: false, pending: false,
                                  addComp: false, save: true, seeBatches: false, saveChanges: false, splitOrder: false)
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
        self.saveWarehousesChangesButton.isHidden = hideBtns.saveChanges
        self.splitButton.isHidden = hideBtns.splitOrder
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
    
    func okAction(selectedOption: String, productId: String) {
        saveWarehousesChangesButton.isEnabled = true
        let index = orderDetail[0].details?.firstIndex(where: ({$0.productID == productId})) ?? 0
        orderDetail[0].details?[index].warehouse = selectedOption
        
        let orderFabId = orderDetail[0].productionOrderID ?? 0
        let component = orderDetail[0].details?[index]
        
        let existsIndex = componentsToUpdate.firstIndex(where: ({$0.productId == productId})) ?? -1
        if (existsIndex != -1) {
            componentsToUpdate.remove(at: existsIndex)
        }
        
        let componentToUpdate: Component = Component(
            orderFabID: orderFabId, productId: productId, componentDescription: component?.detailDescription ?? "",
            baseQuantity: component?.baseQuantity ?? 0, requiredQuantity: component?.requiredQuantity ?? 0, consumed: component?.consumed ?? 0,
            available: component?.available ?? 0, unit: component?.unit ?? "", warehouse: component?.warehouse ?? "",
            pendingQuantity: component?.pendingQuantity ?? 0, stock: component?.stock ?? 0, warehouseQuantity: component?.warehouseQuantity ?? 0,
            action: Actions.update.rawValue, assignedBatches: [])
        
        componentsToUpdate.append(componentToUpdate)
    }
    
    @IBAction func saveChangesComponents(_ sender: Any) {
        orderDetailViewModel.updateObjectToSend.components = componentsToUpdate
        let alert = UIAlertController(
            title: "Se actualizará el almacén en la orden de fabricación ¿quieres continuar?",
            message: nil,
            preferredStyle: .alert)
        let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive, handler: nil)
        let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default, handler: { [weak self] _ in
            guard let self = self else { return }
            orderDetailViewModel.updateComponents()
        })
        alert.addAction(cancelAction)
        alert.addAction(okAction)
        self.present(alert, animated: true, completion: nil)
    }
    
    @IBAction func splitOrderAction(_ sender: Any) {
        let someHasBatches = someComponentHasBatches()
        let fabOrderId = orderDetail[0].productionOrderID ?? 0
        if someHasBatches || onGoingSplitProcess {
            let mssg = someHasBatches
                ? Constants.Errors.unassignBatches.rawValue.replacingOccurrences(of: "[fabOrder]", with: String(fabOrderId))
                : Constants.Errors.onGoingSplitProcess.rawValue
            let alert = UIAlertController(
                title: mssg,
                message: nil,
                preferredStyle: .alert)
            let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default, handler: nil)
            alert.addAction(okAction)
            self.present(alert, animated: true, completion: nil)
            return
        } else {
            let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
            let componentsVC = storyboard.instantiateViewController(
                withIdentifier: ViewControllerIdentifiers.splitOrderViewController) as? SplitOrderViewController
            let navigationVC = UINavigationController(rootViewController: componentsVC ?? SplitOrderViewController())
            navigationVC.modalPresentationStyle = .formSheet
            let availablePieces = orderDetail[0].availablePieces ?? 0
            componentsVC?.delegate = self
            componentsVC?.fabOrderId = fabOrderId
            componentsVC?.dxpTransactionId = orderDetail[0].shopTransaction
            componentsVC?.orderId = orderDetail[0].number
            componentsVC?.totalPieces = NSDecimalNumber(decimal: orderDetail[0].plannedQuantity ?? 0).intValue
            componentsVC?.availableQuantity = availablePieces
            let originalQuantity = NSDecimalNumber(decimal: (orderDetail[0].plannedQuantity ?? 0)).intValue
            componentsVC?.originalQuantityOrder = originalQuantity
            componentsVC?.orderType = orderDetail[0].orderRelationType ?? OrderRelationTypes.completa
            componentsVC?.showOnGoingProcessMessage = { value in
                self.onGoingSplitProcess = value
            }
            self.present(navigationVC, animated: true, completion: nil)
        }
    }
    
    func splitOrderAcceptButton(request: SplitOrderRequest) {
        splitOrderViewModel.saveChanges(request, section: statusType)
    }
    
    func someComponentHasBatches() -> Bool {
        let someHasBatches = orderDetail[0].details?.contains{ $0.hasBatches ?? false }
        return someHasBatches ?? false
    }
    
    func cleanLabels() {
        titleLabel.text = ""
        codeDescriptionLabel.text = ""
        documentBaseDescriptionLabel.text = ""
        containerDescriptionLabel.text = ""
        tagDescriptionLabel.text = ""
        sumFormulaDescriptionLabel.text = ""
        quantityPlannedDescriptionLabel.text = ""
        startDateDescriptionLabel.text = ""
        finishedDateDescriptionLabel.text = ""
        productDescritionLabel.text = ""
        hashtagLabel.text = ""
        htCode.text = ""
        htDescription.text = ""
        htBaseQuantity.text = ""
        htrequiredQuantity.text = ""
        htUnit.text = ""
        htWerehouse.text = ""
        destinyLabel.text = ""
        labelSpaceQuantity.text = ""
    }
    
    func emptyOptions() {
        AlertManager.shared.showAlert(message:"\(Constants.Errors.nowarehouses.rawValue) \(orderDetailViewModel.itemCode)",
                                                     view: self)
    }
    
    func hassBatches() {
        AlertManager.shared.showAlert(message:Constants.Errors.hasBatches.rawValue, view: self)
    }
}

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

class OrderDetailViewController: UIViewController {

    // Outlets
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
    
    
    // Comentado para ampliar el campo descripción
//    @IBOutlet weak var htAvailable: UILabel!
//    @IBOutlet weak var htAmountPendingLabel: UILabel!
//    @IBOutlet weak var htStockLabel: UILabel!
//    @IBOutlet weak var htQuantityInStockLabel: UILabel!
    
    // MARK: Variables
    @Injected var orderDetailViewModel: OrderDetailViewModel
    @Injected var lottieManager: LottieManager
    var disposeBag: DisposeBag = DisposeBag()
    var orderId: Int = -1
    var statusType: String = ""
    var indexOfTableToEditItem: Int = -1
    let formatter = UtilsManager.shared.formatterDoublesTo6Decimals()
    var orderDetail: [OrderDetail] = []
    var refreshControl = UIRefreshControl()
    var destiny = ""
    
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.tableView.delegate = self
        self.title = CommonStrings.formulaDetail
        splitViewController!.preferredDisplayMode = .allVisible
        self.showButtonsByStatusType(statusType: statusType)
        self.initComponents()
        self.tableView.allowsMultipleSelectionDuringEditing = false
        tableView.setEditing(false, animated: true)
        self.orderDetailViewModel.orderId = self.orderId
        self.viewModelBinding()
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(true)
        splitViewController!.preferredDisplayMode = .allVisible
        self.orderDetailViewModel.getOrdenDetail()
        self.refreshViewControl()
    }
        
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(true)
        self.splitViewController?.preferredDisplayMode = UISplitViewController.DisplayMode.primaryHidden
    }
    
    //MARK: Functions
    @objc func goToCommentsViewController() {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let commentsVC = storyboard.instantiateViewController(withIdentifier: ViewControllerIdentifiers.commentsViewController) as! CommentsViewController
        commentsVC.orderDetail = self.orderDetail
        commentsVC.originView = ViewControllerIdentifiers.orderDetailViewController
        commentsVC.modalPresentationStyle = .overCurrentContext
        self.present(commentsVC, animated: true, completion: nil)
    
    }
    
    func goToComponentsViewController() {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let componentsVC = storyboard.instantiateViewController(withIdentifier: ViewControllerIdentifiers.componentsViewController) as! ComponentsViewController
        let navigationVC = UINavigationController(rootViewController: componentsVC)
        navigationVC.modalPresentationStyle = .formSheet
        
        self.present(navigationVC, animated: true, completion: nil)
    }
    
    // Inicia la ejecución del refresh control
    func refreshViewControl() -> Void {
        self.refreshControl.tintColor = OmicronColors.blue
        self.refreshControl.attributedTitle = NSAttributedString(string: CommonStrings.updatingData)
        refreshControl.addTarget(self, action: #selector(refresh(_:)), for: .valueChanged)
        self.tableView.addSubview(self.refreshControl)
    }
    
    @objc func refresh(_ sender: AnyObject) -> Void {
        self.orderDetailViewModel.getOrdenDetail(isRefresh: true)
    }
    
    func viewModelBinding() {
        
        // Termina la ejecución del refresh control
        self.orderDetailViewModel.endRefreshing.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self]  _ in
            self?.refreshControl.endRefreshing()
        }).disposed(by: self.disposeBag)
        
        self.orderDetailViewModel.backToInboxView.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.navigationController?.popToRootViewController(animated: true)
        }).disposed(by: self.disposeBag)
        
        self.orderDetailViewModel.goToSeeLotsViewController.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
            let lotsVC = storyboard.instantiateViewController(identifier: ViewControllerIdentifiers.lotsViewController) as! LotsViewController
            if (self?.orderId != nil && self?.statusType != nil && self?.orderDetail != nil) {
                lotsVC.orderId = self!.orderId
                lotsVC.statusType = self!.statusType
                lotsVC.orderDetail = self!.orderDetail
                if let order = self?.orderDetail.first {
                    if (order.productDescription != nil && order.code != nil && order.productionOrderID != nil && order.baseDocument != nil) {
                        lotsVC.orderNumber =  "\(order.baseDocument!)"
                        lotsVC.manufacturingOrder = "\(order.productionOrderID!)"
                        lotsVC.codeDescription = "\(order.code!)  \(order.productDescription!)"
                    }
                }
                self?.navigationController?.pushViewController(lotsVC, animated: true)
            }
        }).disposed(by: self.disposeBag)
        
        self.processButton.rx.tap.bind(to: orderDetailViewModel.processButtonDidTap).disposed(by: self.disposeBag)
        self.seeLotsButton.rx.tap.bind(to: orderDetailViewModel.seeLotsButtonDidTap).disposed(by: self.disposeBag)
        self.finishedButton.rx.tap.bind(to: orderDetailViewModel.finishedButtonDidTap).disposed(by: self.disposeBag)
        self.penddingButton.rx.tap.bind(to: orderDetailViewModel.pendingButtonDidTap).disposed(by: self.disposeBag)
        self.addComponentButton.rx.tap.subscribe(onNext: { [weak self] _ in
            self?.goToComponentsViewController()
        }).disposed(by: disposeBag)
        
        self.orderDetailViewModel.showAlertConfirmation.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] data in
            let alert = UIAlertController(title: data.message, message: nil, preferredStyle: .alert)
            let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive, handler: {[weak self] _ in self?.dismiss(animated: true, completion: nil)})
            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler:  { [weak self] _ in self?.orderDetailViewModel.terminateOrChangeStatusOfAnOrder(actionType: data.typeOfStatus)})
            alert.addAction(cancelAction)
            alert.addAction(okAction)
            self?.present(alert, animated: true, completion: nil)
        }).disposed(by: self.disposeBag)
        
        // Muestra un mensaje de confirmación para poner la orden en status finalizado
//        self.orderDetailViewModel.showAlert.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] message in
//            let alert = UIAlertController(title: message, message: nil, preferredStyle: .alert)
//            let cancelAction = UIAlertAction(title: "Cancelar", style: .destructive, handler: { [weak self] _ in self?.dismiss(animated: true)})
//            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler:  { [weak self] _ in self?.orderDetailViewModel.validIfOrderCanBeFinalized()  })
//            alert.addAction(cancelAction)
//            alert.addAction(okAction)
//            self?.present(alert, animated: true, completion: nil)
//        }).disposed(by: self.disposeBag)
        
        self.orderDetailViewModel.sumFormula.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] sum in
            self?.sumFormulaDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.sumOfFormula)\(self?.formatter.string(from: NSNumber(value: sum)) ?? CommonStrings.empty)", textToBold: "\(CommonStrings.sumOfFormula)")
        }).disposed(by: self.disposeBag)
                
        self.orderDetailViewModel.orderDetailData.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] res in
            
            guard let self = self else { return }
            
            if res.first != nil {
                
                self.orderDetail = res
                let detail = res.first!
                let number = detail.baseDocument == 0 ? CommonStrings.empty : "\(detail.baseDocument ?? 0)"
                self.codeDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.orderNumber) \(number)", textToBold: CommonStrings.orderNumber)
                self.containerDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.container) \(detail.container ?? CommonStrings.empty)", textToBold: CommonStrings.container)
                self.tagDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.tag) \(detail.productLabel ?? CommonStrings.empty)", textToBold: CommonStrings.tag)

                self.documentBaseDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.manufacturingOrder) \(detail.productionOrderID ?? 0)", textToBold: CommonStrings.manufacturingOrder)
                self.quantityPlannedDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.plannedQuantity) \(detail.plannedQuantity ?? 0)", textToBold: CommonStrings.plannedQuantity)
                self.startDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.manufacturingDate) \(detail.startDate ?? CommonStrings.empty)", textToBold: CommonStrings.manufacturingDate)
                self.finishedDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.finishdate) \(detail.dueDate ?? CommonStrings.empty)", textToBold: CommonStrings.finishdate)
//                self.productDescritionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(detail.code ?? CommonStrings.empty) | \(detail.productDescription ?? CommonStrings.empty)", textToBold: detail.code, textColor: OmicronColors.blue)
                let code = UtilsManager.shared.boldSubstring(text: "\(detail.code ?? CommonStrings.empty)", textToBold: detail.code, fontSize: 22, textColor: OmicronColors.blue)
                let description = UtilsManager.shared.boldSubstring(text: "\(detail.productDescription ?? CommonStrings.empty)", textToBold: detail.productDescription, fontSize: 22, textColor: .gray)
                let pipe = UtilsManager.shared.boldSubstring(text: " | ", textToBold: " | ", fontSize: 22)
                let richText = NSMutableAttributedString()
                richText.append(code)
                richText.append(pipe)
                richText.append(description)
                self.productDescritionLabel.attributedText = richText
                self.destinyLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.destiny) \(self.destiny)", textToBold: CommonStrings.destiny)
                if detail.baseDocument == 0 {
                    self.destinyLabel.text = ""
                }
                
            }
                }).disposed(by: self.disposeBag)
        
        self.orderDetailViewModel.tableData.bind(to: tableView.rx.items(cellIdentifier: ViewControllerIdentifiers.detailTableViewCell, cellType: DetailTableViewCell.self)){ [weak self] row, data, cell in
            cell.hashTagLabel.text = "\(row + 1)"
            cell.codeLabel.text = "\(data.productID!)"
            cell.descriptionLabel.text = data.detailDescription?.uppercased()
            cell.baseQuantityLabel.text =  data.unit == CommonStrings.piece ? String(format: "%.0f", data.baseQuantity ?? 0.0) : self?.formatter.string(from: NSNumber(value: data.baseQuantity ?? 0.0))
            cell.requiredQuantityLabel.text = data.unit == CommonStrings.piece ? String(format: "%.0f", data.requiredQuantity ?? 0.0) : self?.formatter.string(from: NSNumber(value: data.requiredQuantity ?? 0.0))
            cell.unitLabel.text = data.unit!
            cell.werehouseLabel.text = data.warehouse
//            cell.consumedLabel.text = self?.formatter.string(from: NSNumber(value: data.consumed ?? 0.0))
//            cell.availableLabel.text =  self?.formatter.string(from: NSNumber(value: data.available ?? 0.0))
//            cell.quantityPendingLabel.text = self?.formatter.string(from: NSNumber(value: data.pendingQuantity ?? 0.0))
//            cell.stockLabel.text =  self?.formatter.string(from: NSNumber(value: data.stock ?? 0.0))
//            cell.storedQuantity.text =  self?.formatter.string(from: NSNumber(value: data.warehouseQuantity ?? 0.0))
        }.disposed(by: disposeBag)
        
        self.orderDetailViewModel.showIconComments.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] iconName in
            guard let self = self else { return }
            let comments = UIBarButtonItem(image: UIImage(systemName: iconName), style: .plain, target: self, action: #selector(self.goToCommentsViewController))
            self.navigationItem.rightBarButtonItems = [self.getOmniconLogo(), comments]
        }).disposed(by: self.disposeBag)
        
        orderDetailViewModel.showAlert.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] message in
            AlertManager.shared.showAlert(message: message, view: self!)
        }).disposed(by: self.disposeBag)
        
        orderDetailViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] showLoading in
            if(showLoading) {
                self?.lottieManager.showLoading()
            } else {
                self?.lottieManager.hideLoading()
            }
        }).disposed(by: self.disposeBag)
        
        self.orderDetailViewModel.showSignatureView.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] titleView in
            let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
            let signatureVC = storyboard.instantiateViewController(identifier: ViewControllerIdentifiers.signaturePadViewController) as! SignaturePadViewController
            //signatureVC.orderId = self.orderId
            signatureVC.titleView = titleView
            signatureVC.originView = ViewControllerIdentifiers.orderDetailViewController
            signatureVC.modalPresentationStyle = .overCurrentContext
            self?.present(signatureVC, animated: true, completion: nil)
        }).disposed(by: self.disposeBag)
    }
    
    func initComponents() -> Void {

        UtilsManager.shared.setStyleButtonStatus(button: self.finishedButton, title: StatusNameConstants.finishedStatus, color: OmicronColors.finishedStatus, titleColor: OmicronColors.finishedStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.penddingButton, title: StatusNameConstants.penddingStatus, color: OmicronColors.pendingStatus, titleColor: OmicronColors.pendingStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.processButton, title: StatusNameConstants.inProcessStatus, color: OmicronColors.processStatus, titleColor: OmicronColors.processStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.addComponentButton, title: StatusNameConstants.addComponent, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.saveButton, title: StatusNameConstants.save, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.seeLotsButton, title: StatusNameConstants.seeLots, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.labelsStyle(label: self.titleLabel, text: CommonStrings.components, fontSize: 22)
        UtilsManager.shared.labelsStyle(label: self.htCode, text: CommonStrings.code, fontSize: 19, typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.hashtagLabel, text: CommonStrings.hashtag, fontSize: 19, typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htBaseQuantity, text: CommonStrings.baseQuantity, fontSize: 19, typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htrequiredQuantity, text: CommonStrings.pQuantity, fontSize: 19, typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htUnit, text: CommonStrings.unit, fontSize: 19, typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htWerehouse, text: CommonStrings.warehouse, fontSize: 19, typeFont: CommonStrings.bold)
        UtilsManager.shared.labelsStyle(label: self.htDescription, text: CommonStrings.description, fontSize: 19, typeFont: CommonStrings.bold)
        
        // Comentado para ampliar campo descripción
//        UtilsManager.shared.labelsStyle(label: self.htConsumed, text: "Consumido", fontSize: 19, typeFont: "bold")
//        UtilsManager.shared.labelsStyle(label: self.htAvailable, text: "Disponible", fontSize: 19, typeFont: "bold")
//        UtilsManager.shared.labelsStyle(label: self.htAmountPendingLabel, text: "Cant. Pendiente", fontSize: 19, typeFont: "bold")
//        UtilsManager.shared.labelsStyle(label: self.htStockLabel, text: "En stock", fontSize: 19, typeFont: "bold")
//        UtilsManager.shared.labelsStyle(label: self.htQuantityInStockLabel, text: "Cant. Almacén", fontSize: 19, typeFont: "bold")
        
        self.codeDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: CommonStrings.manufacturingOrder, textToBold: CommonStrings.manufacturingOrder)
        self.containerDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: CommonStrings.container, textToBold: CommonStrings.container)
        self.tagDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: CommonStrings.tag, textToBold: CommonStrings.tag)
        self.documentBaseDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: CommonStrings.baseDocument, textToBold: CommonStrings.baseDocument)
        self.sumFormulaDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: CommonStrings.sumOfFormula, textToBold: CommonStrings.sumOfFormula)
        self.quantityPlannedDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: CommonStrings.plannedQuantity, textToBold: CommonStrings.plannedQuantity)
        self.startDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: CommonStrings.manufacturingDate, textToBold: CommonStrings.manufacturingDate)
        self.finishedDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: CommonStrings.finishdate, textToBold: CommonStrings.finishdate)
        self.destinyLabel.attributedText = UtilsManager.shared.boldSubstring(text: CommonStrings.destiny, textToBold: CommonStrings.destiny)
        self.productDescritionLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 22)
        self.detailTable.tableFooterView = UIView()
        
        self.infoView.layer.cornerRadius = 10.0
        self.infoView.backgroundColor = OmicronColors.ligthGray
    }
    
    func showButtonsByStatusType(statusType: String) -> Void {
        switch statusType {
        case StatusNameConstants.assignedStatus:
            self.changeHidePropertyOfButtons(hideProcessBtn: false, hideFinishedBtn: true, hidePendinBtn: false, hideAddCompBtn: true, hideSaveBtn: true, hideSeeLotsBtn: true)
        case StatusNameConstants.inProcessStatus:
            self.changeHidePropertyOfButtons(hideProcessBtn: true, hideFinishedBtn: false, hidePendinBtn: false, hideAddCompBtn: false, hideSaveBtn: true, hideSeeLotsBtn: false)
        case StatusNameConstants.penddingStatus:
            self.changeHidePropertyOfButtons(hideProcessBtn: false, hideFinishedBtn: true, hidePendinBtn: true, hideAddCompBtn: true, hideSaveBtn: true, hideSeeLotsBtn: false)
        case StatusNameConstants.finishedStatus:
            self.changeHidePropertyOfButtons(hideProcessBtn: true, hideFinishedBtn: true, hidePendinBtn: true, hideAddCompBtn: true, hideSaveBtn: true, hideSeeLotsBtn: false)
        case StatusNameConstants.reassignedStatus:
            self.changeHidePropertyOfButtons(hideProcessBtn: true, hideFinishedBtn: false, hidePendinBtn: true, hideAddCompBtn: false, hideSaveBtn: true, hideSeeLotsBtn: false)
        default:
            print("")
        }
    }
    
    func changeHidePropertyOfButtons(hideProcessBtn: Bool, hideFinishedBtn: Bool, hidePendinBtn: Bool,hideAddCompBtn: Bool,hideSaveBtn: Bool,hideSeeLotsBtn: Bool) -> Void {
        self.processButton.isHidden = hideProcessBtn
        self.finishedButton.isHidden = hideFinishedBtn
        self.penddingButton.isHidden = hidePendinBtn
        self.addComponentButton.isHidden = hideAddCompBtn
        self.saveButton.isHidden = hideSaveBtn
        self.seeLotsButton.isHidden = hideSeeLotsBtn
    }
        
    
    func sendIndexToDelete(index: Int) -> Void  {
        orderDetailViewModel.deleteItemFromTable(index: index)
    }
    
//    func changeStatusToProcess() -> Void  {
//        orderDetailViewModel.changeStatus()
//    }
    
//    func showQfbSignatureView() {
//        self.orderDetailViewModel.showSignatureView.onNext("Firma del  QFB")
//    }
    
//    func showSignatureView(title: String) {
//        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
//        let signatureVC = storyboard.instantiateViewController(identifier: ViewControllerIdentifiers.signaturePadViewController) as! SignaturePadViewController
//        signatureVC.orderId = self.orderId
//        signatureVC.modalPresentationStyle = .overCurrentContext
//        self.present(signatureVC, animated: true, completion: nil)
//    }
    
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
       if segue.identifier == ViewControllerIdentifiers.orderDetailFormViewController {
           if let destination = segue.destination as? OrderDetailFormViewController {
            let orderDetailToEdit = orderDetailViewModel.getDataTableToEdit()
            destination.dataOfTable = orderDetailToEdit
            destination.indexOfItemSelected = self.indexOfTableToEditItem
           }
       }
    }
}

extension OrderDetailViewController: UITableViewDelegate {
    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
        cell.selectionStyle = .none
        if(indexPath.row%2 == 0) {
            cell.backgroundColor = OmicronColors.tableColorRow
        } else {
            cell.backgroundColor = .white
        }
    }
    
        func tableView(_ tableView: UITableView, trailingSwipeActionsConfigurationForRowAt indexPath: IndexPath) -> UISwipeActionsConfiguration? {
            if (self.statusType == StatusNameConstants.inProcessStatus || self.statusType == StatusNameConstants.reassignedStatus) {
                // Lógica para editar un item de la tabla
                let editItem = UIContextualAction(style: .normal, title: CommonStrings.edit) { [weak self] (contextualAction, view, boolValue) in
                    self?.indexOfTableToEditItem = indexPath.row
                                
                    self?.performSegue(withIdentifier: ViewControllerIdentifiers.orderDetailFormViewController , sender: nil)
                }
                
                // Logica para borrar un elemento de la tabla
                let deleteItem = UIContextualAction(style: .destructive, title: CommonStrings.delete) {  [weak self] (contextualAction, view, boolValue) in
                    let alert = UIAlertController(title: CommonStrings.deleteComponentMessage, message: nil, preferredStyle: .alert)
                    let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive, handler: { [weak self] _ in self?.dismiss(animated: true)})
                    let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler:  { [weak self] res in self?.sendIndexToDelete(index: indexPath.row)})
                    alert.addAction(cancelAction)
                    alert.addAction(okAction)
                    self?.present(alert, animated: true)
                }
                let swipeActions = UISwipeActionsConfiguration(actions: [editItem, deleteItem])
                return swipeActions
            }
            return nil
        }
}


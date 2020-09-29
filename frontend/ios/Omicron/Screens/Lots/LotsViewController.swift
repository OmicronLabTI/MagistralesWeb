//
//  LotsViewController.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa
import Resolver

class LotsViewController: UIViewController {
    
    // MARK: -OUTLEST
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var hashtagLabel: UILabel!
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    @IBOutlet weak var warehouseCodeLabel: UILabel!
    @IBOutlet weak var totalNeededLabel: UILabel!
    @IBOutlet weak var totalSelectedLabel: UILabel!
    
    @IBOutlet weak var lotsAvailableLabel: UILabel!
    @IBOutlet weak var laLotsLabel: UILabel!
    @IBOutlet weak var laQuantityAvailableLabel: UILabel!
    @IBOutlet weak var laQuantitySelectedLabel: UILabel!
    @IBOutlet weak var laQuantityAssignedLabel: UILabel!
    @IBOutlet weak var lotsSelectedLabel: UILabel!
    @IBOutlet weak var lsLotsLabel: UILabel!
    @IBOutlet weak var lsQuantityAvailableLabel: UILabel!
    @IBOutlet weak var addLotButton: UIButton!
    @IBOutlet weak var removeLotButton: UIButton!
    @IBOutlet weak var pendingButton: UIButton!
    
    @IBOutlet weak var lineOfDocumentsView: UIView!
    @IBOutlet weak var lotsAvailable: UIView!
    @IBOutlet weak var lotsSelected: UIView!
    
    @IBOutlet weak var saveLotsButton: UIButton!
    
    @IBOutlet weak var lineDocTable: UITableView!
    @IBOutlet weak var lotsAvailablesTable: UITableView!
    @IBOutlet weak var lotsSelectedTable: UITableView!
    
    @IBOutlet weak var codeDescriptionLabel: UILabel!
    @IBOutlet weak var orderNumberLabel: UILabel!
    @IBOutlet weak var manufacturingOrderLabel: UILabel!
    @IBOutlet weak var finishOrderButton: UIButton!
    
    // MARK: -Variables
    @Injected var lotsViewModel: LotsViewModel
    @Injected var lottieManager: LottieManager
    let disposeBag = DisposeBag()
    var orderId = -1
    var formatter = UtilsManager.shared.formatterDoublesTo6Decimals()
    var statusType = CommonStrings.empty
    var codeDescription = CommonStrings.empty
    var orderNumber = CommonStrings.empty
    var manufacturingOrder = CommonStrings.empty
    var comments = CommonStrings.empty
    var orderDetail:[OrderDetail] = []
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.initComponents()
        self.viewModelBinding()
        self.lotsViewModel.orderId = self.orderId
        self.lotsViewModel.getLots()
        self.setupKeyboard()
        
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(true)
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)

        //Selecciona el primer elemento de productos cuando termina la carga de datos
        Observable.combineLatest(self.lotsViewModel.dataOfLots, self.lotsViewModel.indexProductSelected, resultSelector: { [weak self] data, indexPath in
            if data.count > 0, let selectedRow = indexPath {
                self?.lineDocTable.selectRow(at: selectedRow, animated: false, scrollPosition: .none)
            } else if data.count > 0, let weakSelf = self {
                let firstRow = IndexPath(row: 0, section: 0)
                weakSelf.lineDocTable.selectRow(at: firstRow, animated: false, scrollPosition: .none)
                weakSelf.lineDocTable.delegate?.tableView?(weakSelf.lineDocTable, didSelectRowAt: firstRow)
            }
        }).subscribe().disposed(by: disposeBag)
        
    }
        
    // MARK: - Functions
    func viewModelBinding() -> Void {
        
        self.addLotButton.rx.tap.bind(to: self.lotsViewModel.addLotDidTap).disposed(by: self.disposeBag)
        self.removeLotButton.rx.tap.bind(to: self.lotsViewModel.removeLotDidTap).disposed(by: self.disposeBag)
        self.saveLotsButton.rx.tap.bind(to: self.lotsViewModel.saveLotsDidTap).disposed(by: self.disposeBag)
        self.finishOrderButton.rx.tap.bind(to: self.lotsViewModel.finishOrderDidTap).disposed(by: self.disposeBag)
        self.pendingButton.rx.tap.bind(to: self.lotsViewModel.pendingButtonDidTap).disposed(by: self.disposeBag)
        // Muestra la vista de la firma
        self.lotsViewModel.showSignatureViewFromLotsView.subscribe(onNext: { [weak self] signatureTitleView in
            let storieboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
            let signatureVC = storieboard.instantiateViewController(identifier: ViewControllerIdentifiers.signaturePadViewController) as! SignaturePadViewController
            signatureVC.titleView = signatureTitleView
            signatureVC.originView = ViewControllerIdentifiers.lotsViewController
            signatureVC.modalPresentationStyle = .overCurrentContext
            self?.present(signatureVC, animated: true, completion: nil)
        }).disposed(by: self.disposeBag)
        
        // Cambia de color los labels a negro cuando ya termino de cargar toda la información
        self.lotsViewModel.changeColorLabels.subscribe(onNext: { [weak self] _ in
            self?.changeTextColorOfLabels(color: .black)
        }).disposed(by: self.disposeBag)
        
        // Actualizan los comentarios
        self.lotsViewModel.updateComments.subscribe(onNext: {[weak self] orderDetail in
            self?.orderDetail = [orderDetail]
            self?.showIconMessage()
        }).disposed(by: self.disposeBag)
        
        // Muestra el componente de firma
        self.lotsViewModel.showSignatureView.subscribe(onNext: { [weak self] titleView in
            let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
            let signatureVC = storyboard.instantiateViewController(identifier: ViewControllerIdentifiers.signaturePadViewController) as! SignaturePadViewController
            signatureVC.titleView = titleView
            signatureVC.originView = ViewControllerIdentifiers.lotsViewController
            signatureVC.modalPresentationStyle = .overCurrentContext
            self?.present(signatureVC, animated: true, completion: nil)
        }).disposed(by: self.disposeBag)
        
        // Manda el mensaje para poder finalizar la orden
        self.lotsViewModel.askIfUserWantToFinalizeOrder.subscribe(onNext: { [weak self] message in
            let alert = UIAlertController(title: message, message: nil, preferredStyle: .alert)
            let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive, handler: nil)
            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler: { _ in self?.lotsViewModel.validIfOrderCanBeFinalized()})
            
            alert.addAction(cancelAction)
            alert.addAction(okAction)
            self?.present(alert, animated: true, completion: nil)
        }).disposed(by: self.disposeBag)
        
        // Manda el mensaje para poder cambiar la orden a pendiente
        self.lotsViewModel.askIfUserWantChageOrderToPendigStatus.subscribe(onNext: { [weak self] message in
            let alert = UIAlertController(title: message, message: nil, preferredStyle: .alert)
            let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive, handler: nil)
            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler: { [weak self] _ in self?.lotsViewModel.changeOrderToPendingStatus() })
            alert.addAction(cancelAction)
            alert.addAction(okAction)
            self?.present(alert, animated: true, completion: nil)
        }).disposed(by: self.disposeBag)
        
        // Se regrsa al inbox cuando se finaliza la orden
        self.lotsViewModel.backToInboxView.subscribe(onNext: { [weak self] _ in
            self?.navigationController?.popToRootViewController(animated: true)
        }).disposed(by: self.disposeBag)
        
        self.addLotButton.rx.tap.subscribe(onNext: { [weak self] _ in
            if let indexPath = self?.lotsAvailablesTable.indexPathForSelectedRow, let cell = self?.lotsAvailablesTable.cellForRow(at: indexPath) as? LotsAvailableTableViewCell {
                cell.quantitySelected.resignFirstResponder()
            }
        }).disposed(by: disposeBag)
        
        // Muestra los datos en la tabla Linea de documentos
        self.lotsViewModel.dataOfLots.bind(to: lineDocTable.rx.items(cellIdentifier: ViewControllerIdentifiers.lotsTableViewCell, cellType: LotsTableViewCell.self)) { [weak self] row, data, cell in
            cell.row = row
            cell.numberLabel.text = "\(row + 1)"
            cell.codeLabel.text = data.codigoProducto
            cell.descriptionLabel.text = data.descripcionProducto?.uppercased()
            cell.warehouseCodeLabel.text = data.almacen
            cell.totalNeededLabel.text =  self?.formatter.string(from: (data.totalNecesario ?? 0) as NSNumber)
            cell.totalSelectedLabel.text = self?.formatter.string(from: (data.totalSeleccionado ?? 0) as NSNumber)
            
            guard let orderDetail = self?.orderDetail.first else { return }
            if orderDetail.baseDocument == 0 {
                self?.orderNumberLabel.isHidden = true
            }
            
        }.disposed(by: self.disposeBag)
        
        // Muestra los datos en la tabla de lotes disponibles
        self.lotsViewModel.dataLotsAvailable.bind(to:  lotsAvailablesTable.rx.items(cellIdentifier: ViewControllerIdentifiers.lotsAvailableTableViewCell, cellType: LotsAvailableTableViewCell.self)) { [weak self] row, data, cell in
            cell.row = row
            cell.itemModel = data
            cell.lotsLabel.text = data.numeroLote
            cell.quantityAvailableLabel.text = self?.formatter.string(from: (data.cantidadDisponible ?? 0) as NSNumber)
            cell.quantitySelected.text = self?.formatter.string(from: (data.cantidadSeleccionada ?? 0) as NSNumber)
            cell.quantityAssignedLabel.text = self?.formatter.string(from: (data.cantidadAsignada ?? 0) as NSNumber)
            cell.setExpiredBatches(data.expiredBatch)
        }.disposed(by: self.disposeBag)
        
        //Muestra los datos en la tabla de Lotes Selecionados
        self.lotsViewModel.dataLotsSelected.bind(to: lotsSelectedTable.rx.items(cellIdentifier: ViewControllerIdentifiers.lotsSelectedTableViewCell, cellType: LotsSelectedTableViewCell.self)) { [weak self] row, data, cell in
            cell.lotsLabel.text = data.numeroLote
            cell.quantitySelectedLabel.text = self?.formatter.string(from: (data.cantidadSeleccionada ?? 0) as NSNumber)
            cell.setExpiredBatches(data.expiredBatch)
        }.disposed(by: self.disposeBag)
        
        // Detecta que item de la tabla linea de documentos fué seleccionada
        self.lineDocTable.rx.modelSelected(Lots.self).bind(to: lotsViewModel.productSelected).disposed(by: disposeBag)
        self.lineDocTable.rx.modelSelected(Lots.self).observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] item in
            self?.lotsViewModel.updateInfoSelectedBatch(lot: item)
        }).disposed(by: self.disposeBag)
        
        // Detecta que item de la tabla lotes disponibles fue selecionado
        Observable.combineLatest(self.lotsAvailablesTable.rx.itemSelected, self.lotsViewModel.lastResponder, resultSelector: { [weak self] index, responder in
            if let cell = self?.lotsAvailablesTable.cellForRow(at: index) as? LotsAvailableTableViewCell, let lastText = responder as? UITextField {
                if cell.quantitySelected != lastText && !cell.quantitySelected.isEditing {
                    self?.view.endEditing(false)
                }
            }
        }).subscribe().disposed(by: disposeBag)
        self.lotsAvailablesTable.rx.modelSelected(LotsAvailable.self).bind(to: lotsViewModel.availableSelected).disposed(by: disposeBag)
        
        // Detecta que item de la tabla lotes selecionados fué selecionado
        self.lotsSelectedTable.rx.modelSelected(LotsSelected.self).bind(to: lotsViewModel.batchSelected).disposed(by: disposeBag)
        self.lotsSelectedTable.rx.modelSelected(LotsSelected.self).observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] item in
            self?.lotsViewModel.itemLotSelected = item
        }).disposed(by: self.disposeBag)
        
        //Detecta el item de la tabla linea de documentos que fué seleccionado
        self.lineDocTable.rx.itemSelected.bind(to: lotsViewModel.indexProductSelected).disposed(by: disposeBag)
        
        // Muestra u oculta el loading
        self.lotsViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] showLoading in
            if(showLoading) {
                self?.lottieManager.showLoading()
                return
            }
            self?.lottieManager.hideLoading()
            self?.showMoreIndicators()
        }).disposed(by: self.disposeBag)
        
        // Muestra un AlertMessage
        self.lotsViewModel.showMessage.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] message in
            guard let weakSelf = self else { return }
            AlertManager.shared.showAlert(message: message, view: weakSelf)
        }).disposed(by: self.disposeBag)
    }
    
    func showMoreIndicators() {
        let count = lineDocTable.dataSource?.tableView(lineDocTable, numberOfRowsInSection: 0)
        if count ?? 0 > lineDocTable.visibleCells.count {
            lineDocTable.addMoreIndicator()
        }
    }
    
    func showIconMessage() {
        if let detail = self.orderDetail.first {
            var iconName = CommonStrings.empty
            if detail.comments != nil  {
                iconName = (detail.comments!.trimmingCharacters(in: .whitespaces).isEmpty) ? ImageButtonNames.message : ImageButtonNames.messsageFill
            } else {
                iconName = ImageButtonNames.message
            }
            
            let commentsIcons = UIBarButtonItem(image: UIImage(systemName: iconName), style: .plain, target: self, action: #selector(self.goToCommentsViewController))
            self.navigationItem.rightBarButtonItems = [getOmniconLogo(), commentsIcons]
        
        }
    }
    
    func initComponents() {
        self.showIconMessage()
        UtilsManager.shared.setStyleButtonStatus(button: self.finishOrderButton, title: StatusNameConstants.finishedStatus, color: OmicronColors.finishedStatus, titleColor: OmicronColors.finishedStatus)
        self.title = CommonStrings.batchesTitle
        UtilsManager.shared.labelsStyle(label: self.titleLabel, text: CommonStrings.documentsLines, fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.hashtagLabel, text: CommonStrings.hashtag, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.codeLabel, text: CommonStrings.code, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.descriptionLabel, text: CommonStrings.articleDescription, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.warehouseCodeLabel, text: CommonStrings.warehouseCode, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.totalNeededLabel, text: CommonStrings.totalNedded, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.totalSelectedLabel, text: CommonStrings.totalSelect, fontSize: 15)
        
        UtilsManager.shared.labelsStyle(label: self.lotsAvailableLabel, text: CommonStrings.batchAvailable, fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.laLotsLabel, text: CommonStrings.batchesTitle, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.laQuantityAvailableLabel, text: CommonStrings.quantityAvailable, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.laQuantitySelectedLabel, text: CommonStrings.quantitySelected, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.laQuantityAssignedLabel, text: CommonStrings.quantityAssigned, fontSize: 15)
        
        UtilsManager.shared.labelsStyle(label: self.lotsSelectedLabel, text: CommonStrings.batchSelected, fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.lsLotsLabel, text: CommonStrings.batchesTitle, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.lsQuantityAvailableLabel, text: CommonStrings.quantitySelected, fontSize: 15)
        
        UtilsManager.shared.setStyleButtonStatus(button: self.saveLotsButton, title: StatusNameConstants.save, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.pendingButton, title: StatusNameConstants.penddingStatus, color: OmicronColors.pendingStatus, titleColor: OmicronColors.pendingStatus)
        
        let titleFontSize = self.codeDescription.count > 170 ? CGFloat(11) : CGFloat(15)
        self.codeDescriptionLabel.text = self.codeDescription
        self.codeDescriptionLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: titleFontSize)
        
        let orderNumber = self.orderNumber == "0" ? CommonStrings.empty : self.orderNumber
        self.orderNumberLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.orderNumber) \(orderNumber)", textToBold: CommonStrings.orderNumber, fontSize: 15)
        self.manufacturingOrderLabel.attributedText = UtilsManager.shared.boldSubstring(text: "\(CommonStrings.manufacturingOrder) \(self.manufacturingOrder)", textToBold: CommonStrings.manufacturingOrder, fontSize: 15)
        self.changeTextColorOfLabels(color: .white)
        self.addLotButton.setImage(UIImage(named: ImageButtonNames.addLot), for: .normal)
        self.addLotButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 50, bottom: 15, right: 50)
        self.addLotButton.setTitle(CommonStrings.empty, for: .normal)
        
        self.removeLotButton.setImage(UIImage(named: ImageButtonNames.removeLot), for: .normal)
        self.removeLotButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 50, bottom: 15, right: 50)
        self.removeLotButton.setTitle(CommonStrings.empty, for: .normal)
        self.setStyleView(view: self.lineOfDocumentsView)
        self.setStyleView(view: self.lotsAvailable)
        self.setStyleView(view: self.lotsSelected)
        
        self.lineDocTable.delegate = self
        self.lotsAvailablesTable.delegate = self
        self.lotsSelectedTable.delegate = self
        self.lineDocTable.tableFooterView = UIView()
        self.lotsAvailablesTable.tableFooterView = UIView()
        self.lotsSelectedTable.tableFooterView = UIView()
        
        if(self.statusType == CommonStrings.finished || self.statusType == CommonStrings.pending) {
            self.addLotButton.isEnabled = false
            self.removeLotButton.isEnabled = false
            self.saveLotsButton.isEnabled = false
            self.finishOrderButton.isHidden = true
        }
        
        if(self.statusType == "En proceso") {
            self.pendingButton.isHidden = false
        } else {
            self.pendingButton.isHidden = true
        }
    }
    
    
    func changeTextColorOfLabels(color: UIColor) -> Void {
        self.titleLabel.textColor = color
        self.hashtagLabel.textColor = color
        self.codeLabel.textColor = color
        self.descriptionLabel.textColor = color
        self.warehouseCodeLabel.textColor = color
        self.totalNeededLabel.textColor = color
        self.totalSelectedLabel.textColor = color
        
        self.lotsAvailableLabel.textColor = color
        self.laLotsLabel.textColor = color
        self.laQuantityAvailableLabel.textColor = color
        self.laQuantitySelectedLabel.textColor = color
        self.laQuantityAssignedLabel.textColor = color
        self.lotsSelectedLabel.textColor = color
        self.lsLotsLabel.textColor = color
        self.lsQuantityAvailableLabel.textColor = color
        self.codeDescriptionLabel.textColor = color
        self.orderNumberLabel.textColor = color
        self.manufacturingOrderLabel.textColor = color
    }
    
    func setStyleView(view: UIView) {
        view.layer.shadowColor = UIColor.black.cgColor
        view.layer.shadowOpacity = 0.2
        view.layer.shadowOffset  = CGSize(width: 0.1, height: 0.1)
        view.layer.shadowRadius = 5
        view.layer.cornerRadius = 10
    }
    
    @objc func keyBoardActions(notification: Notification) {
        if (notification.name == UIResponder.keyboardWillShowNotification) {
            self.view.frame.origin.y = -400
        } else {
            self.view.frame.origin.y = 0
        }
    }
    
    func setupKeyboard() -> Void {
        NotificationCenter.default.addObserver(self, selector: #selector(keyBoardActions(notification:)), name: UIResponder.keyboardWillShowNotification, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(keyBoardActions(notification:)), name: UIResponder.keyboardDidHideNotification, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(keyBoardActions(notification:)), name: UIResponder.keyboardWillChangeFrameNotification, object: nil)
    }
    
    @objc func goToCommentsViewController() {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let commentsVC = storyboard.instantiateViewController(withIdentifier: ViewControllerIdentifiers.commentsViewController) as! CommentsViewController
        commentsVC.orderDetail = self.orderDetail
        commentsVC.originView = ViewControllerIdentifiers.lotsViewController
        commentsVC.modalPresentationStyle = .overCurrentContext
        self.present(commentsVC, animated: true, completion: nil)
    
    }
}

extension LotsViewController: UITableViewDelegate {
    // Pinta una fila o otra no en la tabla
    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
        let customView = UIView()
        customView.backgroundColor = OmicronColors.blue
        cell.selectedBackgroundView = customView
        if(indexPath.row%2 == 0) {
            cell.backgroundColor = OmicronColors.tableColorRow
        } else {
            cell.backgroundColor = .white
        }
    }
    
    func scrollViewDidScroll(_ scrollView: UIScrollView) {
        guard let tableView = scrollView as? UITableView else { return }
        tableView.removeMoreIndicator()
    }
}

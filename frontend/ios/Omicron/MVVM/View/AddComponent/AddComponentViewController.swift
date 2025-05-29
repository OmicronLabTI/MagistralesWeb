//
//  AddComponentController.swift
//  Omicron
//
//  Created by Daniel Vargas on 19/05/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa
import Resolver

// swiftlint:disable type_body_length
class AddComponentViewController: LotsBaseViewController, ComponentsDelegate, ChangeInputValueDelegate {
    var lastResponder = PublishSubject<Any?>()

    // MARK: - OUTLEST
    @IBOutlet weak var addComponentButton: UIButton!
    @IBOutlet weak var saveButton: UIButton!
    @Injected var addComponentViewModel: AddComponentViewModel

    let disposeBag = DisposeBag()
    var isLoading = false

    override func viewDidLoad() {
        initComponents()
        bindSubjects()
        bindTableAvailableSubjects()
        bindTableSelectedSubjects()
        bindLineDocumentsTable()
        changeTextColorOfLabels(color: .white)
        setBackButtonLabelText()
        loadInfo()
    }
    func bindLineDocumentsTable() {
        self.addComponentViewModel.renderProducts.bind(to: lineDocTable.rx.items(
            cellIdentifier: ViewControllerIdentifiers.lotsTableViewCell,
            cellType: LotsTableViewCell.self)) { [weak self] (row: Int, data: AddComponent, cell: LotsTableViewCell) in
            guard let self = self else { return }
            cell.row = row
            cell.numberLabel.text = "\(row + 1)"
            cell.codeLabel.text = data.productId
            cell.descriptionLabel.text = data.description.uppercased()
            cell.warehouseCodeLabel.text = data.warehouse
            cell.totalNeededLabel.text =  self.formatter.string(from: data.requiredQuantity as NSNumber)
            cell.totalSelectedLabel.text = self.formatter.string(from: data.selectedQuantity as NSNumber)
            if self.emptyStockProductId.contains(data.productId) {
                cell.setEmptyStock(false)
            } else {
                cell.setEmptyStock(true)
            }
            if let order = self.orderDetail.first {
                if order.baseDocument == 0 {
                    self.orderNumberLabel.isHidden = true
                }
            }
        }.disposed(by: self.disposeBag)
        
        Observable.zip(
            lineDocTable.rx.itemSelected,
            lineDocTable.rx.modelSelected(AddComponent.self)
        )
        .observe(on: MainScheduler.instance)
        .subscribe(onNext: { [weak self] indexPath, item in
            self?.addComponentViewModel.selectedLineDoc = indexPath.row
            self?.addComponentViewModel.dataLotsAvailable.onNext(item.availableLots)
        })
        .disposed(by: disposeBag)
        
        // aqui se obtiene cuando se esta editando algun elemento de la tabla
        Observable.combineLatest(self.lotsAvailablesTable.rx.itemSelected,
                                 self.lastResponder, resultSelector: { [weak self] index, responder in
            if let cell = self?.lotsAvailablesTable.cellForRow(at: index) as? LotsAvailableTableViewCell,
                let lastText = responder as? UITextField {
                if cell.quantitySelected != lastText && !cell.quantitySelected.isEditing {
                    self?.view.endEditing(false)
                }
            }
        }).subscribe().disposed(by: disposeBag)
        
        addComponentViewModel.selectLineDocIndex.subscribe(onNext: {[weak self] index in
            guard let self = self else { return }
            self.addComponentViewModel.selectedLineDoc = index
            self.selectDocumentLineIndex(index: index)
        }).disposed(by: disposeBag)

    }
    func bindSubjects() {
        addComponentViewModel.showAlert.subscribe(onNext: {[weak self] error in
            AlertManager.shared.showAlert(title: error,
                                          message: String(),
                                          actions: nil,
                                          view: self,
                                          autoDismiss: true,
                                          dismissTime: 2)
        }).disposed(by: disposeBag)
        
        addComponentViewModel.loading.subscribe(onNext: { loading in
            self.isLoading = loading
            if loading {
                LottieManager.shared.showLoading()
                return
            }
            LottieManager.shared.hideLoading()
        }).disposed(by: disposeBag)
    }
    
    
    func bindTableAvailableSubjects() {
        /*
       Observable.combineLatest(self.lotsAvailablesTable.rx.itemSelected,
                                self.addComponentViewModel.lastResponder, resultSelector: { [weak self] index, responder in
            if let cell = self?.lotsAvailablesTable.cellForRow(at: index) as? LotsAvailableTableViewCell,
                let lastText = responder as? UITextField {
                if cell.quantitySelected != lastText && !cell.quantitySelected.isEditing {
                    self?.view.endEditing(false)
                }
            }
        }).subscribe().disposed(by: disposeBag)
        self.lotsAvailablesTable.rx.modelSelected(LotsAvailable.self)
            .bind(to: addComponentViewModel.availableSelected).disposed(by: disposeBag)

        self.addLotButton.rx.tap.subscribe(onNext: { [weak self] _ in
            if let indexPath = self?.lotsAvailablesTable.indexPathForSelectedRow,
                let cell = self?.lotsAvailablesTable.cellForRow(at: indexPath) as? LotsAvailableTableViewCell {
                cell.quantitySelected.resignFirstResponder()
            }
        }).disposed(by: disposeBag)
        */

        // Muestra los datos en la tabla de lotes disponibles
        self.addComponentViewModel.dataLotsAvailable.bind(to: lotsAvailablesTable.rx.items(
            cellIdentifier: ViewControllerIdentifiers.lotsAvailableTableViewCell,
            cellType: LotsAvailableTableViewCell.self)) { [weak self] (row: Int, data: LotsAvailable, cell: LotsAvailableTableViewCell) in
            cell.itemModel = data
            cell.row = row
            cell.lotsLabel.text = data.numeroLote
            cell.quantityAvailableLabel.text = self?.formatter.string(from: (data.cantidadDisponible ?? 0) as NSNumber)
            cell.quantitySelected.text = self?.formatter.string(from: (data.cantidadSeleccionada ?? 0) as NSNumber)
            cell.quantityAssignedLabel.text = self?.formatter.string(from: (data.cantidadAsignada ?? 0) as NSNumber)
            cell.setExpiredBatches(data.expiredBatch)
            cell.delegate = self
        }.disposed(by: self.disposeBag)
        
        lotsAvailablesTable.rx.itemSelected.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            let enable = self.statusType == CommonStrings.finished || self.statusType == CommonStrings.pending
            self.addLotButton.isEnabled = !enable
        }).disposed(by: disposeBag)
    }
    func quantitySelectedChange(row: Int, selectedQuantity: Decimal) {
        addComponentViewModel.selectedQuantityChange(row: row, newQuantity: selectedQuantity)
    }
    func bindTableSelectedSubjects() {
        self.addComponentViewModel.dataLotsSelected.bind(to: lotsSelectedTable.rx.items(
            cellIdentifier: ViewControllerIdentifiers.lotsSelectedTableViewCell,
            cellType: LotsSelectedTableViewCell.self)) { [weak self] _, data, cell in
            cell.lotsLabel.text = data.numeroLote
            cell.quantitySelectedLabel.text = self?.formatter.string(from: (data.cantidadSeleccionada ?? 0) as NSNumber)
            cell.setExpiredBatches(data.expiredBatch)
        }.disposed(by: self.disposeBag)

        /*self.lotsSelectedTable.rx.modelSelected(LotsSelected.self)
            .bind(to: addComponentViewModel.batchSelected).disposed(by: disposeBag)
        self.lotsSelectedTable.rx.modelSelected(LotsSelected.self)
            .observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] item in
            self?.addComponentViewModel.itemLotSelected = item
        }).disposed(by: self.disposeBag)
        lotsSelectedTable.rx.itemSelected.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            let enable = self.statusType == CommonStrings.finished || self.statusType == CommonStrings.pending
            self.removeLotButton.isEnabled = !enable
        }).disposed(by: disposeBag)*/
    }
    
    func selectDocumentLineIndex(index: Int) {
        let section = lineDocTable.numberOfSections - 1
        guard section >= 0 else { return }

        let row = lineDocTable.numberOfRows(inSection: section) - 1
        guard row >= 0 else { return }

        let indexPath = IndexPath(row: row, section: section)
        lineDocTable.selectRow(at: indexPath, animated: true, scrollPosition: .bottom)
    }

    func initComponents() {
        self.title = CommonStrings.addComponentTitle
        UtilsManager.shared.labelsStyle(label: self.titleLabel, text: CommonStrings.documentsLines, fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.hashtagLabel, text: CommonStrings.hashtag, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.codeLabel, text: CommonStrings.code, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.descriptionLabel,
                                        text: CommonStrings.articleDescription, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.warehouseCodeLabel, text: CommonStrings.warehouseCode, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.totalNeededLabel, text: CommonStrings.totalNedded, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.totalSelectedLabel, text: CommonStrings.totalSelect, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.lotsAvailableLabel, text: CommonStrings.batchAvailable,
                                        fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.laLotsLabel, text: CommonStrings.batchesTitle, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.laQuantityAvailableLabel, text: CommonStrings.quantityAvailable,
                                        fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.laQuantitySelectedLabel, text: CommonStrings.quantitySelected,
                                        fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.laQuantityAssignedLabel, text: CommonStrings.quantityAssigned,
                                        fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.lotsSelectedLabel, text: CommonStrings.batchSelected, fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.lsLotsLabel, text: CommonStrings.batchesTitle, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.lsQuantityAvailableLabel, text: CommonStrings.quantitySelected,
                                        fontSize: 15)
        
        UtilsManager.shared.setStyleButtonStatus(button: self.addComponentButton,
                                                 title: StatusNameConstants.addComponent,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)
        
        UtilsManager.shared.setStyleButtonStatus(button: self.saveButton,
                                                 title: StatusNameConstants.save,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)
        
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
        
        addLotButton.isEnabled = false
        removeLotButton.isEnabled = false
    }
    
    @IBAction func addComponentAction(_ sender: UIButton) {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let componentsVC = storyboard.instantiateViewController(
            withIdentifier: ViewControllerIdentifiers.componentsViewController) as? ComponentsViewController
        componentsVC!.clearObservables()
        let navigationVC = UINavigationController(rootViewController: componentsVC ?? ComponentsViewController())
        navigationVC.modalPresentationStyle = .formSheet
        componentsVC?.delegate = self
        self.present(navigationVC, animated: true, completion: nil)
    }

    func okAction(component: ComponentFormValues) {
        addComponentViewModel.getLotsByProduct(component: component)        
    }
}

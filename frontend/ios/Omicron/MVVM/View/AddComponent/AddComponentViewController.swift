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
class AddComponentViewController: LotsBaseViewController, ComponentsDelegate, ChangeInputValueDelegate, SelectedDropDownOptionsDelegate {
    var lastResponder = PublishSubject<Any?>()

    // MARK: - OUTLEST
    @IBOutlet weak var addComponentButton: UIButton!
    @IBOutlet weak var saveButton: UIButton!
    @IBOutlet weak var baseQuantityLabel: UILabel!
    @IBOutlet weak var unitLabel: UILabel!
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
        setupDismissPickerOnTap()
    }

    func bindLineDocumentsTable() {
        self.addComponentViewModel.updateProduct
            .subscribe(onNext: {[weak self] (product: AddComponent, index: Int) in
                guard let self = self else { return }
                let indexPath = IndexPath(row: index, section: 0)
                lineDocTable.reloadRows(at: [indexPath], with: .none)
                lineDocTable.selectRow(at: indexPath, animated: false, scrollPosition: .none)
        }).disposed(by: disposeBag)
        
        self.addComponentViewModel.saveButtonEnabled
            .subscribe(onNext: {[weak self] enabled in
                guard let self = self else { return }
                saveButton.isEnabled = enabled
        }).disposed(by: disposeBag)
        
        addComponentViewModel.returnBackPage
            .subscribe(onNext: {[weak self] enabled in
                guard let self = self else { return }
                self.navigationController?.popViewController(animated: true)
        }).disposed(by: disposeBag)
    
        self.addComponentViewModel.renderProducts.bind(to: lineDocTable.rx.items(
            cellIdentifier: ViewControllerIdentifiers.lotsTableViewCell,
            cellType: LotsTableViewCell.self)) { [weak self] (row: Int, data: AddComponent, cell: LotsTableViewCell) in
            guard let self = self else { return }
            cell.row = row
            cell.numberLabel.text = "\(row + 1)"
            cell.codeLabel.text = data.productId
            cell.descriptionLabel.text = data.description.uppercased()
            cell.warehouseCodeLabel.text = data.warehouse
            cell.totalNeededLabel.text =  self.formatter.string(from: data.totalNecesary as NSNumber)
            cell.totalSelectedLabel.text = self.formatter.string(from: data.selectedTotal as NSNumber)
            cell.selectedOption = data.warehouse
            cell.productId = data.productId
            cell.baseQuantityLabel.text = self.formatter.string(from: data.baseQuantity as NSNumber)
            cell.unitLabel.text = data.unit
            cell.delegate = self
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
            self?.addComponentViewModel.addButtonEnabled.onNext(false)
            self?.addComponentViewModel.deleteButtonEnabled.onNext(false)
            self?.addComponentViewModel.dataLotsAvailable.onNext(item.availableLots)
            self?.addComponentViewModel.dataLotsSelected.onNext(item.selectedLots)
        })
        .disposed(by: disposeBag)
        
        Observable.zip(
            lotsSelectedTable.rx.itemSelected,
            lotsSelectedTable.rx.modelSelected(LotsSelected.self)
        )
        .observe(on: MainScheduler.instance)
        .subscribe(onNext: { [weak self] indexPath, item in
            self?.addComponentViewModel.selectedLotsSelected = indexPath.row
            self?.removeLotButton.isEnabled = true
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
        addComponentViewModel.deleteButtonEnabled.subscribe(onNext: {[weak self] enabled in
            self?.removeLotButton.isEnabled = enabled
        }).disposed(by: disposeBag)
        
        addComponentViewModel.addButtonEnabled.subscribe(onNext: {[weak self] enabled in
            self?.addLotButton.isEnabled = enabled
        }).disposed(by: disposeBag)
    
    
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
    
    override func viewWillAppear(_ animated: Bool) {
           super.viewWillAppear(animated)
           setupKeyboard()
    }
    
    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        addComponentViewModel.resetValues()
        addComponentViewModel.details = []
        NotificationCenter.default.removeObserver(self)
    }
    
    func setupKeyboard() {
        NotificationCenter.default.addObserver(self,
                                                 selector: #selector(keyboardWillShow),
                                                 name: UIResponder.keyboardWillShowNotification,
                                                 object: nil)

        NotificationCenter.default.addObserver(self,
                                                 selector: #selector(keyboardWillHide),
                                                 name: UIResponder.keyboardWillHideNotification,
                                                 object: nil)
    }
    
    // Mover la vista si el teclado tapa el campo activo
    @objc func keyboardWillShow(notification: Notification) {
        guard let userInfo = notification.userInfo,
              let keyboardFrame = userInfo[UIResponder.keyboardFrameEndUserInfoKey] as? CGRect else { return }

        let keyboardTopY = self.view.frame.height - keyboardFrame.height

        if let activeField = self.view.currentFirstResponder(),
           let fieldFrameInWindow = activeField.superview?.convert(activeField.frame, to: nil) {

            if fieldFrameInWindow.maxY > keyboardTopY {
                let overlap = fieldFrameInWindow.maxY - keyboardTopY + 40
                self.view.frame.origin.y = -overlap
            }
        }
    }

    // Restaurar la posición original
    @objc func keyboardWillHide(notification: Notification) {
        self.view.frame.origin.y = 0
    }
    
    func bindTableAvailableSubjects() {
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
        
        
        Observable.zip(
            lotsAvailablesTable.rx.itemSelected,
            lotsAvailablesTable.rx.modelSelected(LotsAvailable.self)
        )
        .observe(on: MainScheduler.instance)
        .subscribe(onNext: { [weak self] indexPath, item in
            self?.addLotButton.isEnabled = true
            self?.addComponentViewModel.selectedAvailable = indexPath.row
        })
        .disposed(by: disposeBag)
    }

    func bindTableSelectedSubjects() {
        self.addComponentViewModel.dataLotsSelected.bind(to: lotsSelectedTable.rx.items(
            cellIdentifier: ViewControllerIdentifiers.lotsSelectedTableViewCell,
            cellType: LotsSelectedTableViewCell.self)) { [weak self] _, data, cell in
            cell.lotsLabel.text = data.numeroLote
            cell.quantitySelectedLabel.text = self?.formatter.string(from: (data.cantidadSeleccionada ?? 0) as NSNumber)
            cell.setExpiredBatches(data.expiredBatch)
        }.disposed(by: self.disposeBag)
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
        
        UtilsManager.shared.labelsStyle(label: self.baseQuantityLabel, text: CommonStrings.baseQuantity, fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.unitLabel, text: CommonStrings.unit, fontSize: 15)
        
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
        
        UtilsManager.shared.setStyleButtonStatus(button: saveButton,
                                                 title: StatusNameConstants.save,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)
        
        addLotButton.setImage(UIImage(named: ImageButtonNames.addLot), for: .normal)
        addLotButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 50, bottom: 15, right: 50)
        addLotButton.setTitle(CommonStrings.empty, for: .normal)
        removeLotButton.setImage(UIImage(named: ImageButtonNames.removeLot), for: .normal)
        removeLotButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 50, bottom: 15, right: 50)
        removeLotButton.setTitle(CommonStrings.empty, for: .normal)

        
        setStyleView(view: lineOfDocumentsView)
        setStyleView(view: lotsAvailable)
        setStyleView(view: lotsSelected)
        
        lineDocTable.delegate = self
        lotsAvailablesTable.delegate = self
        lotsSelectedTable.delegate = self
        
        lineDocTable.tableFooterView = UIView()
        lotsAvailablesTable.tableFooterView = UIView()
        lotsSelectedTable.tableFooterView = UIView()
        
        addLotButton.isEnabled = false
        removeLotButton.isEnabled = false
        saveButton.isEnabled = false
        addComponentViewModel.details = orderDetail[0].details ?? []
    }
    
    func okAction(selectedOption: String, productId: String) {
        addComponentViewModel.changeWarehouseCode(productId: productId, warehouseCode: selectedOption)
    }
    
    func setupDismissPickerOnTap() {
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(dismissPicker))
        tapGesture.cancelsTouchesInView = false
        view.addGestureRecognizer(tapGesture)
    }

    @objc func dismissPicker() {
        view.endEditing(true)
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
    
    @IBAction func addButtonDidPressed(_ sender: UIButton) {
        if let cell = lotsAvailablesTable.cellForRow(at: IndexPath(row: addComponentViewModel.selectedAvailable, section: 0)) as? LotsAvailableTableViewCell {
            let quantity = Decimal(string: cell.quantitySelected.text ?? "0") ?? 0
            addComponentViewModel.selectedQuantityChange(quantity: quantity)
        }
    }
    
    @IBAction func deleteButtonDidPressed(_ sender: UIButton) {
        addComponentViewModel.deleteSelectedLot()
    }
    
    @IBAction func saveButtonButtonDidPressed(_ sender: UIButton) {
        let data = orderDetail[0]
        addComponentViewModel.saveChanges(detail: data)
    }

    func okAction(component: ComponentFormValues) {
        addComponentViewModel.getLotsByProduct(component: component)        
    }
    
    func tableView(_ tableView: UITableView,
                   trailingSwipeActionsConfigurationForRowAt indexPath: IndexPath)
    -> UISwipeActionsConfiguration? {

        guard tableView == self.lineDocTable else {
            return nil
        }

        let deleteAction = UIContextualAction(style: .destructive, title: "Eliminar") { [weak self] (_, _, completionHandler) in
            guard let self = self else { return }
            self.addComponentViewModel.deleteComponent(row: indexPath.row)
            completionHandler(true)
        }
        
        deleteAction.backgroundColor = .red
        return UISwipeActionsConfiguration(actions: [deleteAction])
    }
}

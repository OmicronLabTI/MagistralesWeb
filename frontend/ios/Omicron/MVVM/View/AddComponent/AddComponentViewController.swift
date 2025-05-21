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
class AddComponentViewController: LotsBaseViewController, ComponentsDelegate {
    
    // MARK: - OUTLEST
    @IBOutlet weak var addComponentButton: UIButton!
    @IBOutlet weak var saveButton: UIButton!
    @Injected var addComponentViewModel: AddComponentViewModel

    let disposeBag = DisposeBag()
    var isLoading = false

    override func viewDidLoad() {
        initComponents()
        bindSubjects()
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

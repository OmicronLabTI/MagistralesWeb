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

class OrderDetailViewController: UIViewController {

    // Outlets
    @IBOutlet weak var nameStatusLabel: UILabel!
    @IBOutlet weak var processButton: UIButton!
    @IBOutlet weak var finishedButton: UIButton!
    @IBOutlet weak var addComponentButton: UIButton!
    @IBOutlet weak var saveButton: UIButton!
    @IBOutlet weak var seeLotsButton: UIButton!
    @IBOutlet weak var penddingButton: UIButton!
    @IBOutlet weak var backButton: UIButton!
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
    
    // MARK: Outlets from table header
    @IBOutlet weak var htCode: UILabel!
    @IBOutlet weak var htBaseQuantity: UILabel!
    @IBOutlet weak var htrequiredQuantity: UILabel!
    @IBOutlet weak var htConsumed: UILabel!
    @IBOutlet weak var htAvailable: UILabel!
    @IBOutlet weak var htUnit: UILabel!
    @IBOutlet weak var htWerehouse: UILabel!
    @IBOutlet weak var htAmountPendingLabel: UILabel!
    @IBOutlet weak var htStockLabel: UILabel!
    @IBOutlet weak var htQuantityInStockLabel: UILabel!
    @IBOutlet weak var detailTable: UITableView!
    @IBOutlet weak var tableView: UITableView!
    
    // MARK: Variables
    var disposeBag: DisposeBag = DisposeBag()
    var orderId: Int = -1
    var orderDetailViewModel = OrderDetailViewModel()

    
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        orderDetailViewModel.getOrdenDetail(orderId: orderId)
        self.initComponents()
        self.viewModelBinding()
        splitViewController?.preferredPrimaryColumnWidthFraction = 0.08
    }
    
    //MARK: Functions
    func viewModelBinding() {
        
        self.orderDetailViewModel.orderDetailData.observeOn(MainScheduler.instance).subscribe(onNext: { res in
            
            if((res.first) != nil) {
                self.codeDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Código: \(res[0].code!)", textToBold: "Código:")
                self.containerDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Envase: \(res[0].container!)", textToBold: "Envase")
                self.tagDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Etiqueta: \(res[0].productLabel!)", textToBold: "Etiqueta:")
                self.documentBaseDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Documento base: \(res[0].baseDocument!)", textToBold: "Documento base:")
                self.sumFormulaDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Sumatoria de la fórmula: ", textToBold: "Sumatoria de la fórmula:")
                self.quantityPlannedDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Cantidad planificada: \(res[0].plannedQuantity!)", textToBold: "Cantidad planificada:")
                self.startDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Fecha orden de fabricación: \(res[0].startDate!)", textToBold: "Fecha orden de fabricación:")
                self.finishedDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Fecha de finalización: \(res[0].endDate!)", textToBold: "Fecha de finalización:")
                self.productDescritionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Descripción del producto: \(res[0].productDescription!)", textToBold: "Descripción del producto:")

            }
                }).disposed(by: self.disposeBag)
        
        self.orderDetailViewModel.tableData.bind(to: tableView.rx.items(cellIdentifier: ViewControllerIdentifiers.detailTableViewCell, cellType: DetailTableViewCell.self)){row, data, cell in
            cell.codeLabel.text = "\(data.productID!)"
            cell.baseQuantityLabel.text = "\(data.baseQuantity!)"
            cell.requiredQuantityLabel.text = "\(data.requiredQuantity!)"
            cell.consumedLabel.text = "\(data.consumed!)"
            cell.availableLabel.text = "\(data.available!)"
            cell.unitLabel.text = data.unit!
            cell.werehouseLabel.text = data.warehouse
            cell.quantityPendingLabel.text = "\(data.pendingQuantity!)"
            cell.stockLabel.text = "\(data.stock!)"
            cell.storedQuantity.text = "\(data.warehouseQuantity!)"
        }.disposed(by: disposeBag)
        
        orderDetailViewModel.error.observeOn(MainScheduler.instance).subscribe(onNext: { errorMenssage in
            AlertManager.shared.showAlert(message: "Hubo un error al cargar el detalle de la ordem de fabricación, intentar de nuevo", view: self)
        }).disposed(by: self.disposeBag)
        
        orderDetailViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { showLoading in
            if(showLoading) {
                LottieManager.shared.showLoading()
            } else {
                LottieManager.shared.hideLoading()
            }
        }).disposed(by: self.disposeBag)
    }
    
    func initComponents() -> Void {

        UtilsManager.shared.setStyleButtonStatus(button: self.finishedButton, title: StatusNameConstants.finishedStatus, color: OmicronColors.finishedStatus, titleColor: OmicronColors.finishedStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.penddingButton, title: StatusNameConstants.penddingStatus, color: OmicronColors.pendingStatus, titleColor: OmicronColors.pendingStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.processButton, title: StatusNameConstants.inProcessStatus, color: OmicronColors.processStatus, titleColor: OmicronColors.processStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.addComponentButton, title: StatusNameConstants.addComponent, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.saveButton, title: StatusNameConstants.save, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.seeLotsButton, title: StatusNameConstants.seeLots, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        self.backButton.setImage(UIImage(named: ImageButtonNames.assigned), for: .normal)
        UtilsManager.shared.labelsStyle(label: self.nameStatusLabel, text: "Asignado", fontSize: 39)
        UtilsManager.shared.labelsStyle(label: self.titleLabel, text: "Componentes", fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.htCode, text: "Código", fontSize: 15, typeFont: "bold")
        UtilsManager.shared.labelsStyle(label: self.htBaseQuantity, text: "Cant. Base", fontSize: 15, typeFont: "bold")
        UtilsManager.shared.labelsStyle(label: self.htrequiredQuantity, text: "Cant. requerida", fontSize: 15, typeFont: "bold")
        UtilsManager.shared.labelsStyle(label: self.htConsumed, text: "Consumido", fontSize: 15, typeFont: "bold")
        UtilsManager.shared.labelsStyle(label: self.htAvailable, text: "Disponible", fontSize: 15, typeFont: "bold")
        UtilsManager.shared.labelsStyle(label: self.htUnit, text: "Unidad", fontSize: 15, typeFont: "bold")
        UtilsManager.shared.labelsStyle(label: self.htWerehouse, text: "Almacén", fontSize: 15, typeFont: "bold")
        UtilsManager.shared.labelsStyle(label: self.htAmountPendingLabel, text: "Cant. Pendiente", fontSize: 15, typeFont: "bold")
        UtilsManager.shared.labelsStyle(label: self.htStockLabel, text: "En stock", fontSize: 15, typeFont: "bold")
        UtilsManager.shared.labelsStyle(label: self.htQuantityInStockLabel, text: "Cant. Almacén", fontSize: 15, typeFont: "bold")
        
        self.codeDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Código:", textToBold: "Código:")
        self.containerDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Envase:", textToBold: "Envase")
        self.tagDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Etiqueta", textToBold: "Etiqueta:")
        self.documentBaseDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Documento base:", textToBold: "Documento base:")
        self.sumFormulaDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Sumatoria de la fórmula: ", textToBold: "Sumatoria de la fórmula:")
        self.quantityPlannedDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Cantidad planificada:", textToBold: "Cantidad planificada:")
        self.startDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Fecha orden de fabricación:", textToBold: "Fecha orden de fabricación:")
        self.finishedDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Fecha de finalización:", textToBold: "Fecha de finalización:")
        self.productDescritionLabel.attributedText = UtilsManager.shared.boldSubstring(text: "Descripción del producto:", textToBold: "Descripción del producto:")
        self.detailTable.tableFooterView = UIView()
    }
    
}

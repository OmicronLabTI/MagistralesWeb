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
    
    @IBOutlet weak var lineOfDocumentsView: UIView!
    @IBOutlet weak var lotsAvailable: UIView!
    @IBOutlet weak var lotsSelected: UIView!
    
    @IBOutlet weak var saveLotsButton: UIButton!
    
    
    @IBOutlet weak var lineDocTable: UITableView!
    @IBOutlet weak var lotsAvailablesTable: UITableView!
    @IBOutlet weak var lotsSelectedTable: UITableView!
    
    // MARK: -Variables
    @Injected var lotsViewModel: LotsViewModel
    let disposeBag = DisposeBag()
    var orderId = -1
    var formatter = UtilsManager.shared.formatterDoublesTo8Decimals()
    
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
        self.lotsViewModel.resetVariables()
    }
        
    // MARK: - Functions
    func viewModelBinding() -> Void {
        
        self.addLotButton.rx.tap.bind(to: self.lotsViewModel.addLotDidTap).disposed(by: self.disposeBag)
        self.removeLotButton.rx.tap.bind(to: self.lotsViewModel.removeLotDidTap).disposed(by: self.disposeBag)
        self.saveLotsButton.rx.tap.bind(to: self.lotsViewModel.saveLotsDidTap).disposed(by: self.disposeBag)
        
        // Muestra los datos en la tabla Linea de documentos
        self.lotsViewModel.dataOfLots.subscribe(onNext: { data in
            if (self.lotsViewModel.itemSelectedLineDocuments != nil && data.count > 0) {
                DispatchQueue.main.async {
                    self.lineDocTable.selectRow(at: IndexPath(row: self.lotsViewModel.itemSelectedLineDocuments!, section: 0), animated: false, scrollPosition: .none)
                    self.lineDocTable.delegate?.tableView?(self.lineDocTable, didSelectRowAt: IndexPath(row: self.lotsViewModel.itemSelectedLineDocuments!, section: 0))
                }
            }
        }).disposed(by: disposeBag)
        self.lotsViewModel.dataOfLots.bind(to: lineDocTable.rx.items(cellIdentifier: ViewControllerIdentifiers.lotsTableViewCell, cellType: LotsTableViewCell.self)) {row, data, cell in
            cell.row = row
            cell.numberLabel.text = "\(row + 1)"
            cell.codeLabel.text = data.codigoProducto
            cell.descriptionLabel.text = data.descripcionProducto
            cell.warehouseCodeLabel.text = data.almacen
            cell.totalNeededLabel.text =  self.formatter.string(from: data.totalNecesario! as NSNumber)
            cell.totalSelectedLabel.text = self.formatter.string(from: data.totalSeleccionado! as NSNumber)
        }.disposed(by: self.disposeBag)
        
        // Muestra los datos en la tabla de lotes disponibles
        self.lotsViewModel.dataLotsAvailable.bind(to:  lotsAvailablesTable.rx.items(cellIdentifier: ViewControllerIdentifiers.lotsAvailableTableViewCell, cellType: LotsAvailableTableViewCell.self)) {row, data, cell in
            cell.row = row
            cell.lotsLabel.text = data.numeroLote
            cell.quantityAvailableLabel.text = self.formatter.string(from: data.cantidadDisponible! as NSNumber)
            cell.quantitySelected.text = self.formatter.string(from: data.cantidadSeleccionada! as NSNumber)
            cell.quantityAssignedLabel.text = self.formatter.string(from: data.cantidadAsignada! as NSNumber)
        }.disposed(by: self.disposeBag)
        
        //Muestra los datos en la tabla de Lotes Selecionados
        self.lotsViewModel.dataLotsSelected.bind(to: lotsSelectedTable.rx.items(cellIdentifier: ViewControllerIdentifiers.lotsSelectedTableViewCell, cellType: LotsSelectedTableViewCell.self)) {row, data, cell in
            cell.lotsLabel.text = data.numeroLote
            cell.quantitySelectedLabel.text = self.formatter.string(from: data.cantidadSeleccionada! as NSNumber)
        }.disposed(by: self.disposeBag)
        
        // Detecta que item de la tabla linea de documentos fué seleccionada
        self.lineDocTable.rx.modelSelected(Lots.self).bind(to: lotsViewModel.documentSelected).disposed(by: disposeBag)
        self.lineDocTable.rx.modelSelected(Lots.self).observeOn(MainScheduler.instance).subscribe(onNext: { item in
            self.lotsViewModel.itemSelectedOfLineDocTable(lot: item)
        }).disposed(by: self.disposeBag)
        
        // Detecta que item de la tabla lotes disponibles fue selecionado
        self.lotsAvailablesTable.rx.modelSelected(LotsAvailable.self).bind(to: lotsViewModel.availableSelected).disposed(by: disposeBag)
        self.lotsAvailablesTable.rx.itemSelected.subscribe(onNext: { indexPath in
            if let cell = self.lotsAvailablesTable.cellForRow(at: indexPath) as? LotsAvailableTableViewCell {
                if !cell.quantitySelected.isFirstResponder {
                    cell.quantitySelected.becomeFirstResponder()
                }
            }
        }).disposed(by: disposeBag)
        
        // Detecta que item de la tabla lotes selecionados fué selecionado
        self.lotsSelectedTable.rx.modelSelected(LotsSelected.self).bind(to: lotsViewModel.batchSelected).disposed(by: disposeBag)
        self.lotsSelectedTable.rx.modelSelected(LotsSelected.self).observeOn(MainScheduler.instance).subscribe(onNext: { item in
            self.lotsViewModel.itemLotSelected = item
        }).disposed(by: self.disposeBag)
        
        //Detecta el item de la tabla linea de documentos que fué seleccionado
        self.lineDocTable.rx.itemSelected.observeOn(MainScheduler.instance).subscribe(onNext: { index in
            self.lotsViewModel.itemSelectedLineDocuments = index.row
        }).disposed(by: self.disposeBag)
        
        // Muestra o coulta el loading
        self.lotsViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { showLoading in
            if(showLoading) {
                LottieManager.shared.showLoading()
                return
            }
            LottieManager.shared.hideLoading()
            
            if (self.lineDocTable.indexPathForSelectedRow == nil) {
                self.lineDocTable.selectRow(at: IndexPath(row: 0, section: 0), animated: false, scrollPosition: .none)
                self.lineDocTable.delegate?.tableView?(self.lineDocTable, didSelectRowAt: IndexPath(row: 0, section: 0))
            }
        }).disposed(by: self.disposeBag)
        
        // Muestra un AlertMessage
        self.lotsViewModel.showMessage.observeOn(MainScheduler.instance).subscribe(onNext: { message in
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.disposeBag)
    }
    
    func initComponents() {
        self.title = "Lotes"
        UtilsManager.shared.labelsStyle(label: self.titleLabel, text: "Líneas de documentos", fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.hashtagLabel, text: "#", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.codeLabel, text: "Código", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.descriptionLabel, text: "Descripción del artículo", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.warehouseCodeLabel, text: "Código de almacén", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.totalNeededLabel, text: "Total necesario", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.totalSelectedLabel, text: "Total Seleccionado", fontSize: 15)
        
        UtilsManager.shared.labelsStyle(label: self.lotsAvailableLabel, text: "Lotes Disponibles", fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.laLotsLabel, text: "Lotes", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.laQuantityAvailableLabel, text: "Cantidad diaponible", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.laQuantitySelectedLabel, text: "Cantidad seleccionada", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.laQuantityAssignedLabel, text: "Cantidad asignada", fontSize: 15)
        
        UtilsManager.shared.labelsStyle(label: self.lotsSelectedLabel, text: "Lotes seleccionados", fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.lsLotsLabel, text: "Lotes", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.lsQuantityAvailableLabel, text: "Cantidad selecionada", fontSize: 15)
        
        UtilsManager.shared.setStyleButtonStatus(button: self.saveLotsButton, title: StatusNameConstants.save, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        
        self.addLotButton.setImage(UIImage(named: ImageButtonNames.addLot), for: .normal)
        self.addLotButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 50, bottom: 15, right: 50)
        self.addLotButton.setTitle("", for: .normal)
        
        self.removeLotButton.setImage(UIImage(named: ImageButtonNames.removeLot), for: .normal)
        self.removeLotButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 50, bottom: 15, right: 50)
        self.removeLotButton.setTitle("", for: .normal)
        self.setStyleView(view: self.lineOfDocumentsView)
        self.setStyleView(view: self.lotsAvailable)
        self.setStyleView(view: self.lotsSelected)
        
        self.lineDocTable.delegate = self
        self.lotsAvailablesTable.delegate = self
        self.lotsSelectedTable.delegate = self
        self.lineDocTable.tableFooterView = UIView()
        self.lotsAvailablesTable.tableFooterView = UIView()
        self.lotsSelectedTable.tableFooterView = UIView()
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
}

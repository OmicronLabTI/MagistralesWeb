//
//  LotsViewController.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

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
    
    @IBOutlet weak var saveButton: UIButton!
    
    @IBOutlet weak var lineDocumentosTable: UITableView!
    @IBOutlet weak var lotsAvailablesTable: UITableView!
    @IBOutlet weak var lotsSelectedTable: UITableView!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.initComponents()
        
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
        
        UtilsManager.shared.setStyleButtonStatus(button: self.saveButton, title: StatusNameConstants.save, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        
        self.addLotButton.setImage(UIImage(named: ImageButtonNames.addLot), for: .normal)
        self.addLotButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 50, bottom: 15, right: 50)
        self.addLotButton.setTitle("", for: .normal)
        
        self.removeLotButton.setImage(UIImage(named: ImageButtonNames.removeLot), for: .normal)
        self.removeLotButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 50, bottom: 15, right: 50)
        self.removeLotButton.setTitle("", for: .normal)
        self.setStyleView(view: self.lineOfDocumentsView)
        self.setStyleView(view: self.lotsAvailable)
        self.setStyleView(view: self.lotsSelected)
        
        self.lineDocumentosTable.delegate = self
        self.lotsAvailablesTable.delegate = self
        self.lotsSelectedTable.delegate = self
    }
    
    func setStyleView(view: UIView) {
        view.layer.shadowColor = UIColor.black.cgColor
        view.layer.shadowOpacity = 0.2
        view.layer.shadowOffset  = CGSize(width: 0.1, height: 0.1)
        view.layer.shadowRadius = 5
        view.layer.cornerRadius = 10
    }
}

extension LotsViewController: UITableViewDelegate {
    
    // Pinta una fila o otra no en la tabla
    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
        cell.selectionStyle = .none
        if(indexPath.row%2 == 0) {
            cell.backgroundColor = OmicronColors.tableColorRow
        } else {
            cell.backgroundColor = .white
        }
    }
    
}

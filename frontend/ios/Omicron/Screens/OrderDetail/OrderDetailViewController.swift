//
//  OrderDetailViewController.swift
//  Omicron
//
//  Created by Axity on 07/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

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
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var codeDescriptionLabel: UILabel!
    @IBOutlet weak var documentBaseLabel: UILabel!
    @IBOutlet weak var documentBaseDescriptionLabel: UILabel!
    @IBOutlet weak var containerLabel: UILabel!
    @IBOutlet weak var containerDescriptionLabel: UILabel!
    @IBOutlet weak var tagLabel: UILabel!
    @IBOutlet weak var tagDescriptionLabel: UILabel!
    @IBOutlet weak var sumFormulaLabel: UILabel!
    @IBOutlet weak var sumFormulaDescriptionLabel: UILabel!
    @IBOutlet weak var quantityPlannedLabel: UILabel!
    @IBOutlet weak var quantityPlannedDescriptionLabel: UILabel!
    @IBOutlet weak var startDateLabel: UILabel!
    @IBOutlet weak var startDateDescriptionLabel: UILabel!
    @IBOutlet weak var finishedDateLabel: UILabel!
    @IBOutlet weak var finishedDateDescriptionLabel: UILabel!
    @IBOutlet weak var productLabel: UILabel!
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
    @IBOutlet weak var htSpaceLabel: UILabel!
    
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.initComponents()
        // Do any additional setup after loading the view.
    }
    
    func initComponents() -> Void {

        UtilsManager.shared.setStyleButtonStatus(button: self.finishedButton, title: StatusNameConstants.finishedStatus, color: OmicronColors.finishedStatus, titleColor: OmicronColors.finishedStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.penddingButton, title: StatusNameConstants.penddingStatus, color: OmicronColors.pendingStatus, titleColor: OmicronColors.pendingStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.processButton, title: StatusNameConstants.inProcessStatus, color: OmicronColors.processStatus, titleColor: OmicronColors.processStatus)
        UtilsManager.shared.setStyleButtonStatus(button: self.addComponentButton, title: StatusNameConstants.addComponent, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.saveButton, title: StatusNameConstants.save, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        UtilsManager.shared.setStyleButtonStatus(button: self.seeLotsButton, title: StatusNameConstants.seeLots, color: OmicronColors.blue, backgroudColor: OmicronColors.blue)
        self.backButton.setImage(UIImage(named: ImageButtonNames.assigned), for: .normal)
        //UtilsManager.shared.changeIconButton(button: self.backButton, iconName: ImageButtonNames.backAssigned)
        UtilsManager.shared.labelsStyle(label: self.nameStatusLabel, text: "Asignado", fontSize: 46)
        UtilsManager.shared.labelsStyle(label: self.titleLabel, text: "Componentes", fontSize: 19)
        UtilsManager.shared.labelsStyle(label: self.codeLabel, text: "Código:", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.codeDescriptionLabel, text: "", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.documentBaseLabel, text: "Documento Base", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.documentBaseDescriptionLabel, text: "", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.containerLabel, text: "Envase", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.containerDescriptionLabel, text: "", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.tagLabel, text: "Componentes", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.tagDescriptionLabel, text: "", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.sumFormulaLabel, text: "Sumatoria de la fórmula", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.sumFormulaDescriptionLabel, text: "", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.quantityPlannedLabel, text: "Cantidad planificada", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.quantityPlannedDescriptionLabel, text: "", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.startDateLabel, text: "Fecha orden de fabricación", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.startDateDescriptionLabel, text: "", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.finishedDateLabel, text: "Fecha de finalización", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.finishedDateDescriptionLabel, text: "", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.productLabel, text: "Descripción del producto", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.productDescritionLabel, text: "", fontSize: 15)
        
        UtilsManager.shared.labelsStyle(label: self.htCode, text: "Código", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htBaseQuantity, text: "Cant. Base", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htrequiredQuantity, text: "Cant. requerida", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htConsumed, text: "Consumido", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htAvailable, text: "Disponible", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htUnit, text: "Unidad", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htWerehouse, text: "Almacen", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htAmountPendingLabel, text: "Cant. Pendiente", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htStockLabel, text: "En stock", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htQuantityInStockLabel, text: "Cant. Almacen", fontSize: 12)
        UtilsManager.shared.labelsStyle(label: self.htSpaceLabel, text: "", fontSize: 12)
    }
    

    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}

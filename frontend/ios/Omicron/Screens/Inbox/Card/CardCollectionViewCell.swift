//
//  CardCollectionViewCell.swift
//  Omicron
//
//  Created by Axity on 27/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxCocoa
import RxSwift

class CardCollectionViewCell: UICollectionViewCell {

    
    @IBOutlet weak var contentCard: UIView!
    @IBOutlet weak var numberLabel: UILabel!
    @IBOutlet weak var numberDescriptionLabel: UILabel!
    @IBOutlet weak var baseDocumentLabel: UILabel!
    @IBOutlet weak var baseDocumentDescriptionLabel: UILabel!
    @IBOutlet weak var containerLabel: UILabel!
    @IBOutlet weak var containerDescriptionLabel: UILabel!
    @IBOutlet weak var tagLabel: UILabel!
    @IBOutlet weak var tagDescriptionLabel: UILabel!
    @IBOutlet weak var plannedQuantityLabel: UILabel!
    @IBOutlet weak var plannedQuantityDescriptionLabel: UILabel!
    @IBOutlet weak var startDateLabel: UILabel!
    @IBOutlet weak var startDateDescriptionLabel: UILabel!
    @IBOutlet weak var finishDateLabel: UILabel!
    @IBOutlet weak var finishDateDescriptionLabel: UILabel!
    @IBOutlet weak var productLabel: UILabel!
    @IBOutlet weak var productDescriptionLabel: UILabel!
    @IBOutlet weak var showDetail: UIButton!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        //Initialization code
        assignedStyleCard(color: OmicronColors.assignedStatus.cgColor)
        initLabels()
    }
    
    func initLabels() -> Void {
        UtilsManager.shared.labelsStyle(label: numberLabel, text: "No:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: numberDescriptionLabel, text: " ", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: baseDocumentLabel, text: "Documento Base:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: baseDocumentDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: containerLabel, text: "Envase:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: containerDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: tagLabel, text: "Etiqueta:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: tagDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: plannedQuantityLabel, text: "Cantidad planificada:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: plannedQuantityDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: startDateLabel, text: "Fecha orden de fabricación:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: startDateDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: finishDateLabel, text: "Fecha de finalización:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: finishDateDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: productLabel, text: "Descripción del producto:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: productDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.changeIconButton(button: self.showDetail, iconName: ImageButtonNames.assigned)
    }
    
    func assignedStyleCard(color: CGColor)  -> Void{
        self.contentCard.layer.cornerRadius = CGFloat(20)
        self.contentCard.layer.borderColor = color 
        self.contentCard.layer.borderWidth = CGFloat(1)
    }
}

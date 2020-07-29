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
        // Initialization code
        initLabels()
        assignedStyleCard(color: OmicronColors.assignedStatus.cgColor)
    }
    
    func initLabels() -> Void {
        self.labelsStyle(label: numberLabel, text: "No:", fontSize: 12)
        self.labelsStyle(label: numberDescriptionLabel, text: " ", fontSize: 12)
        self.labelsStyle(label: baseDocumentLabel, text: "Documento Base:", fontSize: 12)
        self.labelsStyle(label: baseDocumentDescriptionLabel, text: "", fontSize: 12)
        self.labelsStyle(label: containerLabel, text: "Envase:", fontSize: 12)
        self.labelsStyle(label: containerDescriptionLabel, text: "", fontSize: 12)
        self.labelsStyle(label: tagLabel, text: "Etiqueta:", fontSize: 12)
        self.labelsStyle(label: tagDescriptionLabel, text: "", fontSize: 12)
        self.labelsStyle(label: plannedQuantityLabel, text: "Cantidad planificada:", fontSize: 12)
        self.labelsStyle(label: plannedQuantityDescriptionLabel, text: "", fontSize: 12)
        self.labelsStyle(label: startDateLabel, text: "Fecha orden de fabricación:", fontSize: 12)
        self.labelsStyle(label: startDateDescriptionLabel, text: "", fontSize: 12)
        self.labelsStyle(label: finishDateLabel, text: "Fecha de finalización:", fontSize: 12)
        self.labelsStyle(label: finishDateDescriptionLabel, text: "", fontSize: 12)
        self.labelsStyle(label: productLabel, text: "Descripción del producto:", fontSize: 12)
        self.labelsStyle(label: productDescriptionLabel, text: "", fontSize: 12)
        changeIconButton(button: self.showDetail, iconName: "showProcessDetailButton.png")
    }
    
    func changeIconButton(button: UIButton, iconName: String) -> Void{
        button.setImage(UIImage(named: iconName), for: .normal)
    }
    
    func labelsStyle(label: UILabel, text: String, fontSize: CGFloat) -> Void {
        label.text = text
        label.font = UIFont.boldSystemFont(ofSize: fontSize)
    }
    func assignedStyleCard(color: CGColor)  -> Void{
        self.contentCard.layer.cornerRadius = 20
        self.contentCard.layer.borderColor = color
        self.contentCard.layer.borderWidth = 1
        
    }
}

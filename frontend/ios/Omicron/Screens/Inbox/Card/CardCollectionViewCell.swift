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
        self.labelsStyle(label: numberLabel, text: "No:")
        self.labelsStyle(label: numberDescriptionLabel, text: " ")
        self.labelsStyle(label: baseDocumentLabel, text: "Documento Base:")
        self.labelsStyle(label: baseDocumentDescriptionLabel, text: "")
        self.labelsStyle(label: containerLabel, text: "Envase:")
        self.labelsStyle(label: containerDescriptionLabel, text: "")
        self.labelsStyle(label: tagLabel, text: "Etiqueta:")
        self.labelsStyle(label: tagDescriptionLabel, text: "")
        self.labelsStyle(label: plannedQuantityLabel, text: "Cantidad planificada:")
        self.labelsStyle(label: plannedQuantityDescriptionLabel, text: "")
        self.labelsStyle(label: startDateLabel, text: "Fecha orden de fabricación:")
        self.labelsStyle(label: startDateDescriptionLabel, text: "")
        self.labelsStyle(label: finishDateLabel, text: "Fecha de finalización:")
        self.labelsStyle(label: finishDateDescriptionLabel, text: "")
        self.labelsStyle(label: productLabel, text: "Descripción del producto:")
        self.labelsStyle(label: productDescriptionLabel, text: "")
        changeIconButton(button: self.showDetail, iconName: "showAssignedDetailButton.png")
    }
    
    func changeIconButton(button: UIButton, iconName: String) -> Void{
        button.setImage(UIImage(named: iconName), for: .normal)
    }
    
    func labelsStyle(label: UILabel, text: String) -> Void {
        label.text = text
        label.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 13)
    }
    
    func assignedStyleCard(color: CGColor)  -> Void{
        self.contentCard.layer.cornerRadius = CGFloat(20)
        self.contentCard.layer.borderColor = color 
        self.contentCard.layer.borderWidth = CGFloat(1)
    }
}

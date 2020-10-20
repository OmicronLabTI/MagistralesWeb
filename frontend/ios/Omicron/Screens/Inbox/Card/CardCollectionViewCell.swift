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

protocol CardCellDelegate: NSObjectProtocol {
    func detailTapped(order: Order)
}

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
    @IBOutlet weak var missingStockImage: UIImageView!
    weak var delegate: CardCellDelegate?
    var row: Int = -1
    weak var order: Order? {
        didSet {
            self.setColor()
        }
    }
    override func awakeFromNib() {
        super.awakeFromNib()
        //Initialization code
        assignedStyleCard(color: OmicronColors.assignedStatus.cgColor)
        initLabels()
        makeRoundedMissingStockImage()
    }
    func initLabels() {
        UtilsManager.shared.labelsStyle(label: numberLabel, text: "Orden de fabricación:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: numberDescriptionLabel, text: " ", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: baseDocumentLabel, text: "Número de pedido:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: baseDocumentDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: containerLabel, text: "Envase:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: containerDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: tagLabel, text: "Etiqueta:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: tagDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: plannedQuantityLabel, text: "Cantidad planificada:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: plannedQuantityDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: startDateLabel, text: "Fecha orden fabricación:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: startDateDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: finishDateLabel, text: "Fecha de finalización:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: finishDateDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: productLabel, text: "Descripción producto:", fontSize: 13)
        UtilsManager.shared.labelsStyle(label: productDescriptionLabel, text: "", fontSize: 13)
        UtilsManager.shared.changeIconButton(button: self.showDetail, iconName: ImageButtonNames.assigned)
        missingStockImage.isHidden = true
    }
    func setColor() {
        switch self.order?.statusId ?? 0 {
        case 1:
            self.propertyCard(
                cell: self,
                borderColor: OmicronColors.assignedStatus,
                iconName: ImageButtonNames.assigned)
        case 2:
            self.propertyCard(
                cell: self,
                borderColor: OmicronColors.processStatus,
                iconName: ImageButtonNames.inProcess)
        case 3:
            self.propertyCard(
                cell: self,
                borderColor: OmicronColors.pendingStatus,
                iconName: ImageButtonNames.pendding)
        case 4:
            self.propertyCard(
                cell: self,
                borderColor: OmicronColors.finishedStatus,
                iconName: ImageButtonNames.finished)
        case 5:
            self.propertyCard(
                cell: self,
                borderColor: OmicronColors.reassignedStatus,
                iconName: ImageButtonNames.reasigned)
        default: break
        }
    }
    func propertyCard(cell: CardCollectionViewCell, borderColor: UIColor, iconName: String) {
        cell.assignedStyleCard(color: borderColor.cgColor)
        UtilsManager.shared.changeIconButton(button: cell.showDetail, iconName: iconName)
        missingStockImage.layer.borderColor = borderColor.cgColor
        missingStockImage.tintColor = borderColor
    }
    func assignedStyleCard(color: CGColor) {
        self.contentCard.layer.cornerRadius = CGFloat(20)
        self.contentCard.layer.borderColor = color
        self.contentCard.layer.borderWidth = CGFloat(1)
    }
    override var isSelected: Bool {
        didSet {
            if isSelected {
                contentCard.layer.borderWidth = CGFloat(5)
                missingStockImage.layer.borderWidth = 3
            } else {
                contentCard.layer.borderWidth = CGFloat(1)
                missingStockImage.layer.borderWidth = 1
            }
        }
    }
    @IBAction func detail() {
        guard let order = self.order else { return }
        self.delegate?.detailTapped(order: order)
    }
    private func makeRoundedMissingStockImage() {
        missingStockImage.layer.borderWidth = 1
        missingStockImage.layer.masksToBounds = false
        missingStockImage.layer.cornerRadius = missingStockImage.frame.height/2
        missingStockImage.clipsToBounds = true
    }
}

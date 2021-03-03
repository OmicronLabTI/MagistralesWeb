//
//  CardIsolatedOrderCollectionViewCell.swift
//  Omicron
//
//  Created by Vicente Cantú on 28/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class CardIsolatedOrderCollectionViewCell: UICollectionViewCell {

    @IBOutlet weak var numberDescriptionLabel: UILabel!
    @IBOutlet weak var plannedQuantityDescriptionLabel: UILabel!
    @IBOutlet weak var startDateDescriptionLabel: UILabel!
    @IBOutlet weak var finishDateDescriptionLabel: UILabel!
    @IBOutlet weak var productDescriptionLabel: UILabel!
    @IBOutlet weak var missingStockImage: UIImageView!
    @IBOutlet weak var itemCode: UILabel!

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
        makeRoundedMissingStockImage()
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

    func propertyCard(cell: CardIsolatedOrderCollectionViewCell, borderColor: UIColor, iconName: String) {
        cell.assignedStyleCard(color: borderColor.cgColor)
        missingStockImage.layer.borderColor = borderColor.cgColor
        missingStockImage.tintColor = borderColor
    }

    func assignedStyleCard(color: CGColor) {
        layer.cornerRadius = CGFloat(20)
        layer.borderColor = color
        layer.borderWidth = CGFloat(1)
    }

    override var isSelected: Bool {
        didSet {
            if isSelected {
                layer.borderWidth = CGFloat(5)
                missingStockImage.layer.borderWidth = 3
            } else {
                layer.borderWidth = CGFloat(1)
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

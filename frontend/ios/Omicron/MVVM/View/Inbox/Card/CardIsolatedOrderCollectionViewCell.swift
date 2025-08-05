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
    @IBOutlet weak var qfbName: UILabel!
    @IBOutlet weak var qfbNameContainer: UIView!
    @IBOutlet weak var itemCodeConstrains: NSLayoutConstraint!
    @IBOutlet weak var descriptionConstraint: NSLayoutConstraint!
    @IBOutlet weak var splitIsolateOrderIcon: UIImageView!
    weak var delegate: CardCellDelegate?
    var row: Int = -1

    weak var order: Order? {
        didSet {
            self.setColor()
        }
    }

    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        assignedStyleCard(color: OmicronColors.assignedStatus.cgColor)
        makeRoundedMissingStockImage()
    }

    func setColor() {
        switch self.order?.statusId ?? 0 {
        case 1:
            self.propertyCard(
                cell: self,
                borderColor: self.colorBatchesSign(OmicronColors.assignedStatus),
                iconName: ImageButtonNames.assigned,
                backgroundColor: self.backgroundBatchesSign(OmicronColors.ligthGray))
        case 2:
            self.propertyCard(
                cell: self,
                borderColor: self.colorBatchesSign(OmicronColors.processStatus),
                iconName: ImageButtonNames.inProcess,
                backgroundColor: self.backgroundBatchesSign(OmicronColors.ligthGray))
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
                borderColor: self.colorBatchesSign(OmicronColors.reassignedStatus),
                iconName: ImageButtonNames.reasigned,
                backgroundColor: self.backgroundBatchesSign(OmicronColors.ligthGray))
        default: break
        }
    }

    func colorBatchesSign(_ colorDefault: UIColor) -> UIColor {
        let signOk = self.order?.technicalSign ?? false
        let batchesComplete = self.order?.areBatchesComplete ?? false
        if signOk || batchesComplete {
            return signOk ? OmicronColors.signColor : OmicronColors.batchesColor
        }
        return colorDefault
    }

    func backgroundBatchesSign(_ colorDefault: UIColor) -> UIColor {
        let signOk = self.order?.technicalSign ?? false
        let batchesComplete = self.order?.areBatchesComplete ?? false
        if signOk || batchesComplete {
            return signOk ? OmicronColors.signColorBackground : OmicronColors.batchesColorBackground
        }
        return colorDefault
    }

    func propertyCard(
        cell: CardIsolatedOrderCollectionViewCell,
        borderColor: UIColor,
        iconName: String,
        orderTextColor: UIColor = .black,
        backgroundColor: UIColor = OmicronColors.ligthGray) {
        cell.assignedStyleCard(color: borderColor.cgColor)
        missingStockImage.layer.borderColor = borderColor.cgColor
        missingStockImage.tintColor = borderColor
        numberDescriptionLabel.textColor = orderTextColor
        cell.backgroundColor = backgroundColor
    }

    func assignedStyleCard(color: CGColor) {
        layer.cornerRadius = CGFloat(20)
        layer.borderColor = color
        layer.borderWidth = CGFloat(4)
    }

    override var isSelected: Bool {
        didSet {
            if isSelected {
                layer.borderWidth = CGFloat(8)
                missingStockImage.layer.borderWidth = 2
            } else {
                layer.borderWidth = CGFloat(2)
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

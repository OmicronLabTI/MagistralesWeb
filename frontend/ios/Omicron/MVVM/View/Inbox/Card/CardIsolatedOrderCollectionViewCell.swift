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
                orderTextColor: colorBatchesSign(.black))
        case 2:
            self.propertyCard(
                cell: self,
                borderColor: self.colorBatchesSign(OmicronColors.processStatus),
                iconName: ImageButtonNames.inProcess,
                orderTextColor: colorBatchesSign(.black))
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
                orderTextColor: colorBatchesSign(.black))
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

    func propertyCard(cell: CardIsolatedOrderCollectionViewCell, borderColor: UIColor, iconName: String,
                      orderTextColor: UIColor = .black) {
        cell.assignedStyleCard(color: borderColor.cgColor)
        missingStockImage.layer.borderColor = borderColor.cgColor
        missingStockImage.tintColor = borderColor
        numberDescriptionLabel.textColor = orderTextColor
    }

    func assignedStyleCard(color: CGColor) {
        layer.cornerRadius = CGFloat(20)
        layer.borderColor = color
        layer.borderWidth = CGFloat(4)
    }

    override var isSelected: Bool {
        didSet {
            if isSelected {
                layer.borderWidth = CGFloat(10)
                missingStockImage.layer.borderWidth = 3
            } else {
                layer.borderWidth = CGFloat(4)
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

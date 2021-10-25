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

    @IBOutlet weak var numberDescriptionLabel: UILabel!
    @IBOutlet weak var baseDocumentDescriptionLabel: UILabel!
    @IBOutlet weak var containerLabel: UILabel!
    @IBOutlet weak var tagDescriptionLabel: UILabel!
    @IBOutlet weak var plannedQuantityDescriptionLabel: UILabel!
    @IBOutlet weak var startDateDescriptionLabel: UILabel!
    @IBOutlet weak var finishDateDescriptionLabel: UILabel!
    @IBOutlet weak var productDescriptionLabel: UILabel!
    @IBOutlet weak var missingStockImage: UIImageView!
    @IBOutlet weak var itemCode: UILabel!
    @IBOutlet weak var destiny: UILabel!
    @IBOutlet weak var orderText: UILabel!
    @IBOutlet weak var manufacturingOrder: UILabel!
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
        let textColor =  self.order?.areBatchesComplete ?? false ?
            UIColor(red: 0.33, green: 0.84, blue: 0.96, alpha: 1.00) : .black
        switch self.order?.statusId {
        case 1:
            self.propertyCard(
                cell: self,
                borderColor: OmicronColors.assignedStatus,
                iconName: ImageButtonNames.assigned)
        case 2:
            let borderColor: UIColor = self.order?.areBatchesComplete ?? false ?
                UIColor(red: 0.33, green: 0.84, blue: 0.96, alpha: 1.00) : OmicronColors.processStatus
            self.propertyCard(
                cell: self,
                borderColor: borderColor,
                iconName: ImageButtonNames.inProcess, orderTextColor: textColor)
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
            let borderColor: UIColor = self.order?.areBatchesComplete ?? false ?
                UIColor(red: 0.33, green: 0.84, blue: 0.96, alpha: 1.00) : OmicronColors.reassignedStatus
            self.propertyCard(
                cell: self,
                borderColor: borderColor,
                iconName: ImageButtonNames.reasigned,
                orderTextColor: textColor)
        default: break
        }
    }

    func propertyCard(cell: CardCollectionViewCell, borderColor: UIColor, iconName: String,
                      orderTextColor: UIColor = .black) {
        cell.assignedStyleCard(color: borderColor.cgColor)
        missingStockImage.layer.borderColor = borderColor.cgColor
        missingStockImage.tintColor = borderColor
        cell.numberDescriptionLabel.textColor = orderTextColor
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

    func detail() {
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

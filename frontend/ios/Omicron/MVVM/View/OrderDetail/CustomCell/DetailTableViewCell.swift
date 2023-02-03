//
//  DetailTableViewCell.swift
//  Omicron
//
//  Created by Axity on 10/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class DetailTableViewCell: UITableViewCell {
    // MARK: Outlets
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    @IBOutlet weak var baseQuantityLabel: UILabel!
    @IBOutlet weak var requiredQuantityLabel: UILabel!
    @IBOutlet weak var unitLabel: UILabel!
    @IBOutlet weak var werehouseLabel: UILabel!
    @IBOutlet weak var hashTagLabel: UILabel!
    var textColor: UIColor = .black
    override func awakeFromNib() {
        super.awakeFromNib()
        let fontSize = CGFloat(17)
        UtilsManager.shared.labelsStyle(label: self.codeLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.baseQuantityLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.requiredQuantityLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.unitLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.werehouseLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.descriptionLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.hashTagLabel, text: "", fontSize: fontSize)
    }
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)
        let textColor = selected ? .white : self.textColor
        self.updateTextColor(color: textColor)
        // Configure the view for the selected state
    }
    func setEmptyStock(_ hasStock: Bool) {
        self.textColor = hasStock ? .black : .systemOrange
        self.updateTextColor(color: self.textColor)
    }

    func updateTextColor(color:UIColor){
        codeLabel.textColor = color
        descriptionLabel.textColor = color
        baseQuantityLabel.textColor = color
        requiredQuantityLabel.textColor = color
        unitLabel.textColor = color
        werehouseLabel.textColor = color
        hashTagLabel.textColor = color
    }
}

//
//  LotsTableViewCell.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class LotsTableViewCell: UITableViewCell {
    
    // MARK: -OUTLETS
    @IBOutlet weak var numberLabel: UILabel!
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    @IBOutlet weak var warehouseCodeLabel: UILabel!
    @IBOutlet weak var totalNeededLabel: UILabel!
    @IBOutlet weak var totalSelectedLabel: UILabel!

    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        UtilsManager.shared.labelsStyle(label: self.numberLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.codeLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.descriptionLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.warehouseCodeLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.totalNeededLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.totalSelectedLabel, text: "", fontSize: 14)
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}

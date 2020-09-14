//
//  LotsSelectedTableViewCell.swift
//  Omicron
//
//  Created by Axity on 21/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class LotsSelectedTableViewCell: UITableViewCell {
    
    // MARK: -OUTLETS
    
    @IBOutlet weak var lotsLabel: UILabel!
    @IBOutlet weak var quantitySelectedLabel: UILabel!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code

    lotsLabel.textColor = UIColor.red
    quantitySelectedLabel.textColor = UIColor.red
        UtilsManager.shared.labelsStyle(label: self.lotsLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.quantitySelectedLabel, text: "", fontSize: 14)
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}

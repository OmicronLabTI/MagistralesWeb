//
//  LotsAvailableTableViewCell.swift
//  Omicron
//
//  Created by Axity on 21/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class LotsAvailableTableViewCell: UITableViewCell {
    
    // MARK: -OUTLETS
    
    @IBOutlet weak var lotsLabel: UILabel!
    @IBOutlet weak var quantityAvailableLabel: UILabel!
    @IBOutlet weak var quantitySelected: UITextField!
    @IBOutlet weak var quantityAssignedLabel: UILabel!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        UtilsManager.shared.labelsStyle(label: self.lotsLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.quantityAvailableLabel, text: "", fontSize: 14)
        self.quantitySelected.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 14)
        UtilsManager.shared.labelsStyle(label: self.quantityAssignedLabel, text: "", fontSize: 14)
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}

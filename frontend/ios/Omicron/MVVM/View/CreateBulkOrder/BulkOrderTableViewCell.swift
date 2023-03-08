//
//  BulkOrderTableViewCell.swift
//  Omicron
//
//  Created by Daniel Velez on 01/03/23.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import UIKit

class BulkOrderTableViewCell: UITableViewCell {
    @IBOutlet weak var productCodeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        let fontSize = CGFloat(12)
        productCodeLabel.font = UIFont(name: FontsNames.SFProDisplayRegular, size: fontSize)
        descriptionLabel.font = UIFont(name: FontsNames.SFProDisplayRegular, size: fontSize)
    }
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)
        // Configure the view for the selected state
    }
}

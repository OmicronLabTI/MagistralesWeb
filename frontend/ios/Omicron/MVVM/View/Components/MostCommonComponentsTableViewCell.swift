//
//  MostCommonComponentsTableViewCell.swift
//  Omicron
//
//  Created by Sergio Flores Ramirez on 19/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import UIKit

class MostCommonComponentsTableViewCell: UITableViewCell {
    @IBOutlet weak var productCodeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        let fontSize = CGFloat(17)
        productCodeLabel.font = UIFont(name: FontsNames.SFProDisplayMedium, size: fontSize)
        descriptionLabel.font = UIFont(name: FontsNames.SFProDisplayMedium, size: fontSize)
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}

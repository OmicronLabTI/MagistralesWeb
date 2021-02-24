//
//  ContainerTableViewCell.swift
//  Omicron
//
//  Created by Vicente Cantu Garcia on 19/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import UIKit

class ContainerTableViewCell: UITableViewCell {

    @IBOutlet weak var containerLabel: UILabel!
    @IBOutlet weak var quantityLabel: UILabel!
    @IBOutlet weak var unitLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

}

//
//  HistoryTableViewCell.swift
//  Omicron
//
//  Created by Daniel Vargas on 14/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import UIKit

class HistoryTableViewCell: UITableViewCell {
    @IBOutlet var codeLabel: UILabel!
    @IBOutlet var descriptionLabel: UILabel!
    @IBOutlet var quantityLabel: UILabel!
    @IBOutlet var unitLabel: UILabel!
    @IBOutlet var destinationStoreLabel: UILabel!
    @IBOutlet var dateOrderLabel: UILabel!
    @IBOutlet var statusOrderLabel: UILabel!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}

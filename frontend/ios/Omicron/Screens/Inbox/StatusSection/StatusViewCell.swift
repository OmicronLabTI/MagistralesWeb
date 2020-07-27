//
//  StatusViewCell.swift
//  Omicron
//
//  Created by Axity on 25/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class StatusViewCell: UITableViewCell {

    @IBOutlet weak var statusNameLabel: UILabel!
    @IBOutlet weak var numberTaskLabel: UILabel!
    @IBOutlet weak var indicatorImageView: UIImageView!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)
        // Configure the view for the selected state
    }
    

}

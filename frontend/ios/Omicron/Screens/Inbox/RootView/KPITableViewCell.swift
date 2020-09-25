//
//  KPITableViewCell.swift
//  Omicron
//
//  Created by Vicente Cantú on 22/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class KPITableViewCell: UITableViewCell {

    @IBOutlet weak var kpiImage: UIImageView!
    @IBOutlet weak var kpiLabel: UILabel!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }

}

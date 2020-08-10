//
//  DetailTableViewCell.swift
//  Omicron
//
//  Created by Axity on 10/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class DetailTableViewCell: UITableViewCell {

    //MARK: Outlets
    
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var baseQuantityLabel: UILabel!
    @IBOutlet weak var requiredQuantityLabel: UILabel!
    @IBOutlet weak var consumedLabel: UILabel!
    @IBOutlet weak var availableLabel: UILabel!
    @IBOutlet weak var unitLabel: UILabel!
    @IBOutlet weak var werehouseLabel: UILabel!
    @IBOutlet weak var quantityPendingLabel: UILabel!
    @IBOutlet weak var stockLabel: UILabel!
    @IBOutlet weak var storedQuantity: UILabel!
    @IBOutlet weak var editButton: UIButton!
    @IBOutlet weak var deleteButton: UIButton!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        //editButton.setImage(UIImage(systemName: ), for: .normal)
        editButton.setTitle("", for: .normal)
        editButton.setImage(UIImage(systemName: "circle.fill"), for: .normal)
        deleteButton.setTitle("", for: .normal)
        deleteButton.setImage(UIImage(systemName: "circle.fill"), for: .normal)
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
    
}

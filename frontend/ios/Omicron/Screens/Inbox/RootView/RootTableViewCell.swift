//
//  RootTableViewCell.swift
//  Omicron
//
//  Created by Axity on 29/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class RootTableViewCell: UITableViewCell {

    // MARK: Outlets
    @IBOutlet weak var indicatorStatusImageView: UIImageView!
    @IBOutlet weak var indicatorStatusNameLabel: UILabel!
    @IBOutlet weak var indicatorStatusNumber: UILabel!
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        initComponents()
    }
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)

        // Configure the view for the selected state
    }
    func initComponents() {
        self.backgroundColor = OmicronColors.tableStatus
        self.indicatorStatusNameLabel.font = UIFont(name: FontsNames.SFProDisplayRegular, size: 22)
        self.indicatorStatusNumber.font = UIFont(name: FontsNames.SFProDisplayRegular, size: 22)
    }
}

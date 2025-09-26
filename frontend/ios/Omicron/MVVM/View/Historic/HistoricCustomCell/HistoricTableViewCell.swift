//
//  HistoricTableViewCell.swift
//  Omicron
//
//  Created by Josue Castillo on 16/09/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import UIKit

class HistoricTableViewCell: UITableViewCell {
    
    @IBOutlet weak var parentOrderIdLabel: UILabel!
    @IBOutlet weak var totalPiecesLabel: UILabel!
    @IBOutlet weak var availablePiecesLabel: UILabel!
    @IBOutlet weak var qfbLabel: UILabel!
    @IBOutlet weak var childrenOrdersLabel: UILabel!
}

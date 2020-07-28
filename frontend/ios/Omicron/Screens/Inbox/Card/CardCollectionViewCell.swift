//
//  CardCollectionViewCell.swift
//  Omicron
//
//  Created by Axity on 27/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class CardCollectionViewCell: UICollectionViewCell {

    
    @IBOutlet weak var contentCard: UIView!
    @IBOutlet weak var numberLabel: UILabel!
    @IBOutlet weak var numberDescriptionLabel: UILabel!
    @IBOutlet weak var baseDocumentLabel: UILabel!
    @IBOutlet weak var baseDocumentDescriptionLabel: UILabel!
    @IBOutlet weak var containerLabel: UILabel!
    @IBOutlet weak var containerDescriptionLabel: UILabel!
    @IBOutlet weak var tagLabel: UILabel!
    @IBOutlet weak var tagDescriptionLabel: UILabel!
    @IBOutlet weak var plannedQuantityLabel: UILabel!
    @IBOutlet weak var plannedQuantityDescriptionLabel: UILabel!
    @IBOutlet weak var startDateLabel: UILabel!
    @IBOutlet weak var startDateDescriptionLabel: UILabel!
    @IBOutlet weak var finishDateLabel: UILabel!
    @IBOutlet weak var finishDateDescriptionLabel: UILabel!
    @IBOutlet weak var productLabel: UILabel!
    @IBOutlet weak var productDescriptionLabel: UILabel!
    @IBOutlet weak var showDetail: UIButton!
    
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        
        self.contentView.backgroundColor = .white
        self.contentView.layer.borderColor = UIColor.black.cgColor
        self.contentView.layer.borderWidth = 2
        self.contentView.layer.cornerRadius = 10
    }

}

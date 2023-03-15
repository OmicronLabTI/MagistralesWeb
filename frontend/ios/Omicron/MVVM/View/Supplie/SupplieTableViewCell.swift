//
//  SupplieTableViewCell.swift
//  Omicron
//
//  Created by Daniel Vargas on 28/02/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import UIKit
import Resolver

class SupplieTableViewCell: UITableViewCell, UITextFieldDelegate {
    @IBOutlet weak var idLabel: UILabel!
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    @IBOutlet weak var quantityTextField: UITextField!
    @IBOutlet weak var storeDestinationLabel: UILabel!
    @IBOutlet weak var unityLabel: UILabel!
    @Injected var supplieViewModel: SupplieViewModel
    var index = 0
    var supplie: Supplie = Supplie()
    var textColor: UIColor = .black

    override func awakeFromNib() {
        super.awakeFromNib()
        quantityTextField.delegate = self
        quantityTextField.keyboardType = .decimalPad
        // Initialization code
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)
        let textColor = selected ? .white : self.textColor
        self.updateTextColor(color: textColor)
    }

    func updateTextColor(color: UIColor) {
        idLabel.textColor = color
        codeLabel.textColor = color
        descriptionLabel.textColor = color
        storeDestinationLabel.textColor = color
        unityLabel.textColor = color
    }

}

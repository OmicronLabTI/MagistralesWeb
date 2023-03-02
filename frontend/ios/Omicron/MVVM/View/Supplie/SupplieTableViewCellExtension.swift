//
//  SupplieTableViewCellExtension.swift
//  Omicron
//
//  Created by Daniel Vargas on 01/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit
extension SupplieTableViewCell {

    func textField(_ textField: UITextField,
                   shouldChangeCharactersIn range: NSRange,
                   replacementString string: String) -> Bool {
        let pattern = "^(?![0.]+$)[0-9]{1,9}(?:\\.[0-9]{0,9})?$"
        return !validateRegex(pattern, "\(range.description)\(string)")
    }
    func textFieldDidEndEditing(_ textField: UITextField, reason: UITextField.DidEndEditingReason) {
        print("Hola")
    }

    func validateRegex(_ pattern: String, _ valueToEvaluate: String) -> Bool {
        return NSPredicate(format: "SELF MATCHES %@", pattern).evaluate(with: valueToEvaluate)
    }

    func textFieldDidEndEditing(_ textField: UITextField) {
        print("Hola \(textField.text)")
    }
}

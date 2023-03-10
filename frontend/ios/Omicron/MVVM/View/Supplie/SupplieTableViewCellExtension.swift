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
        let pattern = "^(?![.]+$)[0-9]{1,9}(?:\\.[0-9]{0,6})?$"
        let text = textField.text ?? ""
        let textRange = Range(range, in: text)!
        let currentText = text.replacingCharacters(in: textRange, with: string)
        let patternResult = validateRegex(pattern, currentText)
        let isCorrect = patternResult || currentText.isEmpty
        if currentText.isEmpty || Double(currentText) ?? 1 <= 0 {
            textField.layer.borderWidth = 1
            textField.layer.borderColor = OmicronColors.processStatus.cgColor
        } else {
            textField.layer.borderWidth = 1
            textField.layer.borderColor = OmicronColors.ligthGray.cgColor
        }
        return isCorrect
    }
    func textFieldDidEndEditing(_ textField: UITextField, reason: UITextField.DidEndEditingReason) {
        self.supplieViewModel.changeQuantityPieces(itemCode: self.supplie.productId ?? "",
                                                   quantity: Double(textField.text ?? "0") ?? 0)
    }
    func textFieldDidBeginEditing(_ textField: UITextField) {
        if textField.text == "0.0" {
            textField.text = String()
        }
    }
    func validateRegex(_ pattern: String, _ valueToEvaluate: String) -> Bool {
        return NSPredicate(format: "SELF MATCHES %@", pattern).evaluate(with: valueToEvaluate)
    }
}

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
        let patternResult = validateRegex(pattern, currentText.replacingOccurrences(of: ",",
                                                                                    with: String()))
        let isCorrect = patternResult || currentText.isEmpty
        if currentText.isEmpty || Double(currentText) ?? 1 <= 0 {
            textField.layer.borderWidth = 1
            textField.layer.borderColor = OmicronColors.processStatus.cgColor
            return isCorrect
        } else {
            textField.layer.borderWidth = 1
            textField.layer.borderColor = OmicronColors.ligthGray.cgColor
            let number = currentText.replacingOccurrences(of: ",",
                                                          with: String())
            if currentText.contains(".") || !isCorrect {
                return isCorrect
            }
            textField.text = getFormattedText(number: Double(number) ?? 0)
            return false
        }
    }
    func getFormattedText(number: Double) -> String {
        let formatter = NumberFormatter()
        formatter.numberStyle = .none
        formatter.usesGroupingSeparator = true
        formatter.groupingSeparator = ","
        formatter.groupingSize = 3
        formatter.maximumIntegerDigits = 9
        formatter.maximumFractionDigits = 6
        return formatter.string(from: number as NSNumber) ?? String()
    }
    func textFieldDidEndEditing(_ textField: UITextField, reason: UITextField.DidEndEditingReason) {
        let textQuantity = textField.text ?? "0"
        let quantity = Decimal(string: textQuantity.replacingOccurrences(of: ",", with: ""))
        self.supplieViewModel.changeQuantityPieces(itemCode: self.supplie.productId ?? "",
                                                   quantity: quantity ?? 0)
    }
    func textFieldDidBeginEditing(_ textField: UITextField) {
        if textField.text == "0.000000" {
            textField.text = String()
        }
    }
    func validateRegex(_ pattern: String, _ valueToEvaluate: String) -> Bool {
        return NSPredicate(format: "SELF MATCHES %@", pattern).evaluate(with: valueToEvaluate)
    }
}

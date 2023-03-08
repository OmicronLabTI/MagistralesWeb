//
//  OrderDetailTextFieldController.swift
//  Omicron
//
//  Created by Daniel Velez
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit

extension OrderDetailViewController:UITextFieldDelegate{
    func textField(_ textField: UITextField,shouldChangeCharactersIn range: NSRange,replacementString string: String) -> Bool
    {
        guard let oldText = textField.text, let r = Range(range, in: oldText) else {
                return true
            }
            let newText = oldText.replacingCharacters(in: r, with: string)
            let isNumeric = newText.isEmpty || (Double(newText) != nil)
            let numberOfDots = newText.components(separatedBy: ".").count - 1

            let numberOfDecimalDigits: Int
            let numberOfEntiresDigits: Int
            if let dotIndex = newText.firstIndex(of: ".") {
                numberOfEntiresDigits = newText.distance(from: newText.startIndex, to: dotIndex) - 1
                numberOfDecimalDigits = newText.distance(from: dotIndex, to: newText.endIndex) - 1
            } else {
                numberOfEntiresDigits = newText.count - 1
                numberOfDecimalDigits = 0
            }
            return isNumeric && numberOfDots <= 1 && numberOfDecimalDigits <= 6 && numberOfEntiresDigits < 9
    }
}

//
//  Utils.swift
//  Omicron
//
//  Created by Axity on 07/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit
import ObjectMapper

class UtilsManager {
    // MARK: Variables
    static let shared = UtilsManager()
    // MARK: Functions
    func setStyleButtonStatus(
        button: UIButton, title: String, color: UIColor = OmicronColors.blue,
        backgroudColor: UIColor = UIColor.white, titleColor: UIColor = .white) {
        button.setTitle(title, for: .normal)
        button.setTitleColor(titleColor, for: .normal)
        button.setTitleColor(titleColor.withAlphaComponent(0.35), for: .disabled)
        button.layer.borderWidth = 1
        button.layer.cornerRadius = 10
        button.layer.borderColor = color.cgColor
        button.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayBold, size: 16)
        button.backgroundColor = backgroudColor
    }
    func labelsStyle(label: UILabel, text: String, fontSize: CGFloat, typeFont: String = "medium") {
        label.text = text
        switch typeFont {
        case "bold":
            label.font = UIFont(name: FontsNames.SFProDisplayBold, size: fontSize)
        default:
            label.font = UIFont(name: FontsNames.SFProDisplayMedium, size: fontSize)
        }
    }
    func changeIconButton(button: UIButton, iconName: String) {
        button.setImage(UIImage(named: iconName), for: .normal)
    }
    func boldSubstring(text: String, textToBold: String?, fontSize: CGFloat = 19,
                       textColor: UIColor = .black) -> NSMutableAttributedString {
        let str = text as NSString
        let att = NSMutableAttributedString(string: str as String)
        // swiftlint:disable legacy_constructor
        let range = str.range(of: textToBold!,
                              options: .regularExpression, range: NSMakeRange(0, str.length))
        if range.length > 0 { att.addAttribute(NSAttributedString.Key.foregroundColor, value: textColor, range: range)
            att.addAttribute(NSAttributedString.Key.font,
                             value: UIFont(name: FontsNames.SFProDisplayBold, size: fontSize) as Any, range: range)
        }
        return att
    }
    func formattedDateFromString(dateString: String, withFormat format: String) -> String? {
           let inputFormatter = DateFormatter()
           inputFormatter.dateFormat = "dd/MM/yyyy"
           if let date = inputFormatter.date(from: dateString) {
               let outputFormatter = DateFormatter()
             outputFormatter.dateFormat = format
               return outputFormatter.string(from: date)
           }
           return nil
    }
    func formattedDateFromString(dateString: String, inputFormat: String, outputFormat: String) -> String? {
           let inputFormatter = DateFormatter()
           inputFormatter.dateFormat = inputFormat
           if let date = inputFormatter.date(from: dateString) {
               let outputFormatter = DateFormatter()
             outputFormatter.dateFormat = outputFormat
               return outputFormatter.string(from: date)
           }
           return nil
    }
    func formattedDateToString(date: Date, withFormat format: String = "dd/MM/yyyy") -> String {
        let inputFormatter = DateFormatter()
        inputFormatter.dateFormat = format
        return inputFormatter.string(from: date)
    }
    func formatterDoublesTo6Decimals() -> NumberFormatter {
        let formatter = NumberFormatter()
        formatter.minimumFractionDigits = 6
        formatter.numberStyle = .decimal
        return formatter
    }
    func messageErrorWhenNoBatches(error: ValidateOrder) -> String {
        var messageConcat = String()
        messageConcat += "No es posible Terminar, faltan lotes para: "
        messageConcat += "\n"
        messageConcat += error.listItems?.joined(separator: "\n") ?? ""
        messageConcat += "\n\n"
        return messageConcat
    }
    func messageErrorWhenOutOfStock(error: ValidateOrder) -> String {
        var messageConcat = String()
        messageConcat += "No es posible Terminar, falta existencia para: "
        messageConcat += "\n"
        messageConcat += error.listItems?.joined(separator: "\n") ?? CommonStrings.empty
        return messageConcat
    }
}
open class DecimalTransform: TransformType {
    public typealias Object = Decimal
    public typealias JSON = String
    public init() { }
    public func transformFromJSON(_ value: Any?) -> Decimal? {
        if let string = value as? String {
            return Decimal(string: string)
        } else if let number = value as? NSNumber {
            return number.decimalValue
        } else if let double = value as? Double {
            // swiftlint:disable compiler_protocol_init
            return Decimal(floatLiteral: double)
        }
        return nil
    }
    public func transformToJSON(_ value: Decimal?) -> String? {
        guard let value = value else { return nil }
        return value.description
    }
}

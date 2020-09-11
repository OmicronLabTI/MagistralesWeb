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
    //MARK: Variables
    static let shared = UtilsManager()
    
    //MARK: Functions
    func setStyleButtonStatus( button: UIButton ,title: String, color: UIColor = OmicronColors.blue, backgroudColor: UIColor = UIColor.white, titleColor: UIColor = .white) -> Void {
        button.setTitle(title, for: .normal)
        button.setTitleColor(titleColor, for: .normal)
        button.setTitleColor(titleColor.withAlphaComponent(0.35), for: .disabled)
        button.layer.borderWidth = 1
        button.layer.cornerRadius = 10
        button.layer.borderColor = color.cgColor
        button.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayBold, size: 16)
        button.backgroundColor = backgroudColor
    }
    
    func labelsStyle(label: UILabel, text: String, fontSize: CGFloat, typeFont: String = "medium") -> Void {
        label.text = text
        switch typeFont {
        case "bold":
            label.font = UIFont(name: FontsNames.SFProDisplayBold, size: fontSize)
        default:
            label.font = UIFont(name: FontsNames.SFProDisplayMedium, size: fontSize)
        }
    }
    
    func changeIconButton(button: UIButton, iconName: String) -> Void{
        button.setImage(UIImage(named: iconName), for: .normal)
    }
    
    func boldSubstring( text: String, textToBold: String?, fontSize: CGFloat = 19) -> NSMutableAttributedString {
        
        let s = text as NSString
        let att = NSMutableAttributedString(string: s as String)
        let r = s.range(of: textToBold!, options: .regularExpression, range: NSMakeRange(0,s.length))
        if r.length > 0 { att.addAttribute(NSAttributedString.Key.foregroundColor, value: UIColor.black, range: r)
            att.addAttribute(NSAttributedString.Key.font, value: UIFont(name: FontsNames.SFProDisplayBold, size: fontSize) as Any, range: r)
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
    
    func formatterDoublesTo6Decimals() ->  NumberFormatter {
        let formatter = NumberFormatter()
        formatter.minimumFractionDigits = 6
        formatter.numberStyle = .decimal
        return formatter
    }
}

open class DecimalTransform: TransformType {
    public typealias Object = Decimal
    public typealias JSON = String
    
    public init() {}
    
    public func transformFromJSON(_ value: Any?) -> Decimal? {
        if let string = value as? String {
            return Decimal(string: string)
        } else if let number = value as? NSNumber {
            return number.decimalValue
        } else if let double = value as? Double {
            return Decimal(floatLiteral: double)
        }
        return nil
    }
    
    public func transformToJSON(_ value: Decimal?) -> String? {
        guard let value = value else { return nil }
        return value.description
    }
}

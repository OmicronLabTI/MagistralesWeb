//
//  Utils.swift
//  Omicron
//
//  Created by Axity on 07/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit

class UtilsManager {
    static let shared = UtilsManager()
    
    func setStyleButtonStatus( button: UIButton ,title: String, color: UIColor = OmicronColors.blue, backgroudColor: UIColor = UIColor.white, titleColor: UIColor = .white) -> Void {
        button.setTitle(title, for: .normal)
        button.setTitleColor(titleColor, for: .normal)
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
    
    func boldSubstring( text: String, textToBold: String?) -> NSMutableAttributedString {
        
        let s = text as NSString
        let att = NSMutableAttributedString(string: s as String)
        let r = s.range(of: textToBold!, options: .regularExpression, range: NSMakeRange(0,s.length))
        if r.length > 0 { att.addAttribute(NSAttributedString.Key.foregroundColor, value: UIColor.black, range: r)
            att.addAttribute(NSAttributedString.Key.font, value: UIFont(name: FontsNames.SFProDisplayBold, size: 15) as Any, range: r)
        }
        return att
    }
}

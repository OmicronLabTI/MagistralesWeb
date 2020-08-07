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
    
    func labelsStyle(label: UILabel, text: String, fontSize: CGFloat) -> Void {
        label.text = text
        label.font = UIFont(name: FontsNames.SFProDisplayMedium, size: fontSize)
    }
    
    func changeIconButton(button: UIButton, iconName: String) -> Void{
        button.setImage(UIImage(named: iconName), for: .normal)
    }
}

//
//  ExtensionsUI.swift
//  Omicron
//
//  Created by Diego Cárcamo on 08/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit

extension UIView {
    func addMoreIndicator(size: CGFloat = 25) {
        let img = UIImageView(image: UIImage(named: "arrowDown"))
        let size = CGFloat(size)
        img.tag = Constants.Tags.moreIndicator.rawValue
        img.frame = CGRect(x: (frame.size.width / 2) - (size / 2), y: frame.size.height - size, width: size, height: size)
        self.addSubview(img)
    }
    
    func removeMoreIndicator() {
        subviews.forEach({ $0.viewWithTag(Constants.Tags.moreIndicator.rawValue)?.removeFromSuperview() })
    }
}

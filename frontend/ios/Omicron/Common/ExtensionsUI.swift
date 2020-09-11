//
//  ExtensionsUI.swift
//  Omicron
//
//  Created by Diego Cárcamo on 08/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit

extension UITableView {
    func addMoreIndicator() {
        let img = UIImageView(image: UIImage(named: "arrowDown"))
        let size = CGFloat(25.0)
        img.tag = Constants.Tags.moreIndicator.rawValue
        img.frame = CGRect(x: (self.frame.size.width / 2) - (size / 2), y: self.frame.size.height - size, width: size, height: size)
        self.addSubview(img)
    }
    
    func removeMoreIndicator() {
        guard let img = self.viewWithTag(Constants.Tags.moreIndicator.rawValue) else { return }
        img.removeFromSuperview()
    }
}

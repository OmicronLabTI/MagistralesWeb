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
        img.frame = CGRect(
            x: (frame.size.width / 2) - (size / 2), y: frame.size.height - size,
            width: size, height: size)
        self.addSubview(img)
    }
    func removeMoreIndicator() {
        subviews.forEach({ $0.viewWithTag(Constants.Tags.moreIndicator.rawValue)?.removeFromSuperview() })
    }
}
extension UIViewController {
    func getOmniconLogo() -> UIBarButtonItem {
        let logo = UIImage(named: "AppIcon")
        let imageView = UIImageView(frame: CGRect(x: 0, y: 0, width: 40, height: 40))
        imageView.contentMode = .scaleAspectFit
        imageView.image = logo
        imageView.layer.cornerRadius = imageView.frame.size.height / 2
        imageView.clipsToBounds = true
        return UIBarButtonItem(customView: imageView)
    }
}

extension UITextView {

    func centerText() {
        self.textAlignment = .center
        let fittingSize = CGSize(width: bounds.width, height: CGFloat.greatestFiniteMagnitude)
        let size = sizeThatFits(fittingSize)
        let topOffset = (bounds.size.height - size.height * zoomScale) / 2
        let positiveTopOffset = max(1, topOffset)
        contentOffset.y = -positiveTopOffset
    }

}

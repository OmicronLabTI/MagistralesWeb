//
//  UIViewExtensions.swift
//  Omicron
//
//  Created by Daniel Vargas on 29/05/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import UIKit
extension UIView {
    func currentFirstResponder() -> UIView? {
        if self.isFirstResponder { return self }
        for subview in self.subviews {
            if let responder = subview.currentFirstResponder() {
                return responder
            }
        }
        return nil
    }
}

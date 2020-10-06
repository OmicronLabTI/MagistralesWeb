//
//  LottieManager.swift
//  Omicron
//
//  Created by Diego Cárcamo on 06/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import Lottie

class LottieManager {
    // MARK: Variables
    static let shared = LottieManager()
    // MARK: Functions
    func showLoading() {
        if let window = UIApplication.shared.windows.filter({$0.isKeyWindow}).first {
            let loadingView = AnimationView(name: "loading")
            loadingView.loopMode = .loop
            loadingView.play()
            let backView = UIView(frame: window.bounds)
            backView.tag = Constants.Tags.loading.rawValue
            backView.backgroundColor = UIColor(white: 0.0, alpha: 0.25)
            backView.addSubview(loadingView)
            loadingView.center = backView.center
            window.addSubview(backView)
        }
    }
    func hideLoading() {
        if let window = UIApplication.shared.windows.filter({$0.isKeyWindow}).first {
            if let loadingView = window.viewWithTag(Constants.Tags.loading.rawValue) {
                loadingView.removeFromSuperview()
            }
        }
    }
}

//
//  AlertManager.swift
//  Omicron
//
//  Created by Axity on 06/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit

class AlertManager {
    // MARK: - Variables
    static let shared = AlertManager()
    // MARK: - Functions
    func showAlert(
        title: String? = nil, message: String? = nil, actions: [UIAlertAction]? = nil, view: UIViewController? = nil,
        autoDismiss: Bool? = false, dismissTime: Int? = 0) {
        let alert = UIAlertController(title: title, message: message, preferredStyle: .alert)
        for action in actions ?? [] {
            alert.addAction(action)
        }
        if actions?.count ?? 0 == 0 {
            let okAction = UIAlertAction(title: "Aceptar", style: .default, handler: nil)
            alert.addAction(okAction)
        }
        if  autoDismiss == true && (dismissTime ?? 0) > 0 {
            view?.present(alert,
                          animated: true,
                          completion: {Timer.scheduledTimer(withTimeInterval: TimeInterval(dismissTime ?? 0),
                                                            repeats: false,
                                                            block: {_ in
                              alert.dismiss(animated: true, completion: nil)
                          })})
        } else {
            view?.present(alert, animated: true, completion: nil)
        }
    }
}

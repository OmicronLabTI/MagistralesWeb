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
    
    //MARK: VAriables
    static let shared = AlertManager()
    
    // MARK: Functions
    func showAlert(title: String? = nil, message: String? = nil, actions: [UIAlertAction]? = nil, view: UIViewController? = nil) {
        let alert = UIAlertController(title: title, message: message, preferredStyle: .alert)
        for action in actions ?? [] {
            alert.addAction(action)
        }
        if actions?.count ?? 0 == 0 {
            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler: nil)
            alert.addAction(okAction)
        }
        view?.present(alert, animated: true, completion: nil)
    }
}

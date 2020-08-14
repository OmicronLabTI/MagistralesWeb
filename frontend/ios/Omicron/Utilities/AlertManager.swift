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
    static let shared = AlertManager()
    
    func showAlert(message: String, view: UIViewController) {
        let alert = UIAlertController(title: CommonStrings.Emty, message: message, preferredStyle: .alert)
        let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler: nil)
        alert.addAction(okAction)
        view.present(alert, animated: true, completion: nil)
    }
    
//    func showAlertConfirmation(message: String, view: UIViewController) -> Int {
//        var response = -1
//        let alert = UIAlertController(title: CommonStrings.Emty, message: message, preferredStyle: .alert)
//        let cancelAction = UIAlertAction(title: "Cancelar", style: .cancel, handler: {action in response = 0})
//        let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler: {action in response = 1})
//        alert.addAction(cancelAction)
//        alert.addAction(okAction)
//        view.present(alert, animated: true, completion: nil)
//        return response
//    }
}

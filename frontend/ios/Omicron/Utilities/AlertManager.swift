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
    func showAlert(message: String, view: UIViewController) {
        let alert = UIAlertController(title: CommonStrings.Emty, message: message, preferredStyle: .alert)
        let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler: nil)
        alert.addAction(okAction)
        view.present(alert, animated: true, completion: nil)
    }
    
//    func showAlertConfirmation(message: String, view: UIViewController) -> Void {
//
//        let alert = UIAlertController(title: CommonStrings.Emty, message: message, preferredStyle: .alert)
//        let cancelAction = UIAlertAction(title: "Cancelar", style: .cancel, handler: responseFromAlertConfirmation)
//        let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler: responseFromAlertConfirmation)
//        alert.addAction(cancelAction)
//        alert.addAction(okAction)
//        view.present(alert, animated: true, completion: nil)
//    }
//
//    func responseFromAlertConfirmation(action: UIAlertAction) {
//
//    }
}

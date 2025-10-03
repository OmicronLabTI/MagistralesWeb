//
//  RootViewControllerExtension.swift
//  Omicron
//
//  Created by Daniel Vargas on 03/04/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit

extension RootViewController {
    func initComponents() {
        self.viewTable.tableFooterView = UIView()
        self.viewTable.backgroundColor = OmicronColors.tableStatus
        self.myOrdesLabel.backgroundColor = OmicronColors.tableStatus
        self.myOrdesLabel.font = .fontDefaultBold(25)
        self.searchOrdesSearchBar.placeholder = CommonStrings.searchOrden
        self.searchOrdesSearchBar.backgroundColor = OmicronColors.tableStatus
        self.searchOrdesSearchBar.barTintColor = OmicronColors.tableStatus
        self.view.backgroundColor = OmicronColors.tableStatus
        self.refreshControl.tintColor = OmicronColors.blue
        self.refreshControl.attributedTitle = NSAttributedString(string: "Actualizando datos")
        self.logoutButton.setTitle("Cerrar sesión", for: .normal)
        self.logoutButton.tintColor = .darkGray
        self.logoutButton.setImage(UIImage(named: ImageButtonNames.logout), for: .normal)
        self.logoutButton.imageView?.contentMode = .scaleAspectFit
        self.logoutButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 10, bottom: 15, right: 260)
        self.logoutButton.titleEdgeInsets.left = 20
        self.logoutButton.titleLabel?.font = .fontDefaultMedium(17)
        self.versionLabel.attributedText = UtilsManager.shared
            .boldSubstring(
                text: "Versión: \(CommonStrings.version) (\(CommonStrings.build))",
                textToBold: "Versión: ")
        self.versionLabel.textColor = OmicronColors.blue
        self.versionLabel.font = .fontDefaultBold(12)
        UtilsManager.shared.setStyleButtonStatus(button: self.createSupplies,
                                                 title: StatusNameConstants.getSupplies,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)
        UtilsManager.shared.setStyleButtonStatus(button: self.createBulk,
                                                 title: StatusNameConstants.createBulk,
                                                 color: OmicronColors.primaryBlue,
                                                 backgroudColor: OmicronColors.primaryBlue,
                                                 titleColor: .white)
        self.createBulk.isHidden = rootViewModel.userType != .qfb
        self.createSupplies.isHidden = rootViewModel.userType != .qfb
        self.createBulk.rx.tap.bind { _ in
            self.openBulkProducts()
        }
        setLayers()
    }
    
    func setLayers() {
        let topBorder = CALayer()
        topBorder.frame = CGRect(x: 0, y: 0, width: historicView.frame.width, height: 1)
        topBorder.backgroundColor = UIColor(red: 220/255, green: 220/255, blue: 205/255, alpha: 1).cgColor
        
        let bottomBorder = CALayer()
        bottomBorder.frame = CGRect(x: 0, y: historicView.frame.height - 1, width: historicView.frame.width, height: 1)
        bottomBorder.backgroundColor = UIColor(red: 220/255, green: 220/255, blue: 205/255, alpha: 1).cgColor
        
        historicView.layer.addSublayer(topBorder)
        historicView.layer.addSublayer(bottomBorder)
        
        let indicatorViewbottomBorder = CALayer()
        indicatorViewbottomBorder.frame = CGRect(x: 0, y: indicatorsView.frame.height - 1, width: indicatorsView.frame.width, height: 1)
        indicatorViewbottomBorder.backgroundColor = UIColor(red: 220/255, green: 220/255, blue: 205/255, alpha: 1).cgColor
        
        indicatorsView.layer.addSublayer(indicatorViewbottomBorder)
        
        
    }

    func openBulkProducts() {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let componentsVC = storyboard.instantiateViewController(
            withIdentifier: ViewControllerIdentifiers.componentsViewController) as? ComponentsViewController
        componentsVC?.typeOpen = .bulkOrder
        componentsVC!.clearObservables()
        let navigationVC = UINavigationController(rootViewController: componentsVC ?? ComponentsViewController())
        navigationVC.modalPresentationStyle = .formSheet
        self.present(navigationVC, animated: true, completion: nil)
    }
}

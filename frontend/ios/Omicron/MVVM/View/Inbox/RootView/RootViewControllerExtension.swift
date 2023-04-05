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
        self.myOrdesLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 25)
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
        self.logoutButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 17)
        self.versionLabel.attributedText = UtilsManager.shared
            .boldSubstring(
                text: "Versión: \(CommonStrings.version) (\(CommonStrings.build))",
                textToBold: "Versión: ")
        self.versionLabel.textColor = OmicronColors.blue
        self.versionLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 12)
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

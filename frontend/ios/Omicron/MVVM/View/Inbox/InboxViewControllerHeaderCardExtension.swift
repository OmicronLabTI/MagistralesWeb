//
//  InboxViewControllerHeaderCardExtension.swift
//  Omicron
//
//  Created by Daniel Velez on 25/01/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa
import Resolver
import RxDataSources
import Charts
import PDFKit


extension InboxViewController: HeaderSelectedDelegate {

    func headerSelected(productID: Int) {
        inboxViewModel.getConnection()
        self.productID = productID
    }

    func tapPatientList(productID: Int) {
        if let order = self.getNamesByOrder(productID: productID) {
            if order.patientName != "" {
                self.inboxViewModel.selectedOrder = order
                self.performSegue(withIdentifier: ViewControllerIdentifiers.patientListViewController, sender: nil)
            }
        }
    }

}

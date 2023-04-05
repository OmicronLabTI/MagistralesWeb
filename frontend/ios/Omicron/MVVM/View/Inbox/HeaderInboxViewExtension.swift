//
//  HeaderInboxDelegateExtension.swift
//  Omicron
//
//  Created by Daniel Velez on 25/01/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//
import Foundation
extension InboxViewController: HeaderSelectedDelegate {
    func downloadPDF(_ ordersId: [Int]) {
        inboxViewModel.downloadPDF(ordersId)
    }

    func showPatientList(_ title: String, _ patientNames: [String]) {
        let list = self.getArrayNames(patientNames)
        if  list.count > 0 {
            let dataSend = PatientListData(
                title: title,
                list: list
            )
            self.performSegue(withIdentifier: ViewControllerIdentifiers.patientListViewController, sender: dataSend)
        }
    }

}

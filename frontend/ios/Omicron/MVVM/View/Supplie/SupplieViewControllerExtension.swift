//
//  SupplieViewControllerExtension.swift
//  Omicron
//
//  Created by Daniel Vargas on 28/02/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit
extension SupplieViewController: UITableViewDelegate, UITableViewDataSource {

    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return supplieList.count
    }

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        return loadTableRow(tableView, cellForRowAt: indexPath)
    }

    func loadTableRow(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> SupplieTableViewCell {
        guard let cell = tableView.dequeueReusableCell(withIdentifier: ViewControllerIdentifiers.supplieTableViewCell,
            for: indexPath) as? SupplieTableViewCell
        else { return SupplieTableViewCell() }
        let supplie: ComponentO = supplieList[indexPath.row]
        cell.idLabel?.text = String(indexPath.row)
        cell.codeLabel?.text = supplie.productId
        cell.descriptionLabel?.text = supplie.description
        cell.quantityTextField?.text = String(0)
        cell.storeDestinationLabel?.text = supplie.warehouse
        cell.unityLabel?.text = supplie.unit
        cell.index = indexPath.row
        cell.supplie = supplie
        return cell
    }

}

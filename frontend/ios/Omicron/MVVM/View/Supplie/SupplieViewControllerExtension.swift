//
//  SupplieViewControllerExtension.swift
//  Omicron
//
//  Created by Daniel Vargas on 28/02/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit
extension SupplieViewController: UITableViewDelegate, UITableViewDataSource, UITextViewDelegate {

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
        supplieList = []
        cell.idLabel?.text = String(indexPath.row)
        cell.codeLabel?.text = supplie.productId
        cell.descriptionLabel?.text = supplie.description
        cell.quantityTextField?.text = String(0)
        cell.storeDestinationLabel?.text = supplie.warehouse
        cell.unityLabel?.text = supplie.unit
        cell.index = indexPath.row
        cell.supplie = supplie
        supplieList.append(supplie)
        return cell
    }
    // Selected Cell
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        tableView.deselectRow(at: indexPath, animated: false)
        guard let cell = tableView.cellForRow(at: indexPath) else { return }
        let itemCode = self.supplieList[indexPath.row].productId ?? ""
        self.supplieViewModel.validateItemsToDelete(itemCode: itemCode)
        let colorDefault = indexPath.row % 2 == 0 ? OmicronColors.tableColorRow : .white
        let selected = self.supplieViewModel.validateExistsInList(itemCode: itemCode)
        let color = selected ? OmicronColors.processStatus : colorDefault
        cell.isSelected = selected
        cell.backgroundColor = color
    }
    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
        let customView = UIView()
        customView.backgroundColor = OmicronColors.processStatus
        cell.selectedBackgroundView = customView
        if indexPath.row%2 == 0 {
            cell.backgroundColor = OmicronColors.tableColorRow
        } else {
            cell.backgroundColor = .white
        }
    }
    func textView(_ textView: UITextView, shouldChangeTextIn range: NSRange, replacementText text: String) -> Bool {
        return textView.text.count + text.count - range.length <= 500
    }

}

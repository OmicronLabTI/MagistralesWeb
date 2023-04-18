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
        if tableView == self.tableComponents {
            return supplieList.count
        } else {
            return historyViewModel.historyList.count
        }

    }
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        return loadTableRow(tableView, cellForRowAt: indexPath)
    }

    func loadTableRow(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> SupplieTableViewCell {
        guard let cell = tableView.dequeueReusableCell(withIdentifier: ViewControllerIdentifiers.supplieTableViewCell,
            for: indexPath) as? SupplieTableViewCell
        else { return SupplieTableViewCell() }
        let supplie: Supplie = supplieList[indexPath.row]
        cell.idLabel?.text = String(self.supplieList.count - indexPath.row)
        cell.codeLabel?.text = supplie.productId
        cell.descriptionLabel?.text = supplie.description
        cell.quantityTextField?.text = self.formatter.string(from: (supplie.requestQuantity ?? 0) as NSNumber)
        cell.storeDestinationLabel?.text = supplie.warehouse
        cell.unityLabel?.text = supplie.unit
        cell.supplie = supplie
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
        let lastSectionIndex = tableView.numberOfSections - 1
        let lastRowIndex = tableView.numberOfRows(inSection: lastSectionIndex) - 1
        if indexPath.section == lastSectionIndex &&
            indexPath.row == lastRowIndex - 3 &&
            !self.isLoading &&
            lastRowIndex > 10 {
            tableView.scrollToRow(at: [0, lastRowIndex - 4],
                                  at: .middle,
                                  animated: false)
            self.historyViewModel.onScroll.onNext(())
        }
        if indexPath.row%2 == 0 {
            cell.backgroundColor = OmicronColors.tableColorRow
        } else {
            cell.backgroundColor = .white
        }
    }
    func textView(_ textView: UITextView, shouldChangeTextIn range: NSRange, replacementText text: String) -> Bool {
        if text == "\n" {
            textView.resignFirstResponder()
            return false
        }
        return textView.text.count + text.count - range.length <= 500
    }

    func textViewDidEndEditing(_ textView: UITextView) {
        if textView.text.isEmpty {
            textView.text = CommonStrings.placeholderObservations
            textView.textColor = UIColor.lightGray
        }
    }
    func textViewDidBeginEditing(_ textView: UITextView) {
        if textView.textColor == UIColor.lightGray {
            textView.text = String()
            textView.textColor = UIColor.black
        }
    }
    func setupUI() {
        tableComponents.delegate = self
        tableComponents.dataSource = self
        tableComponents.rowHeight = UITableView.automaticDimension
        tableHistory.delegate = self
        tableHistory.dataSource = self
        tableHistory.rowHeight = UITableView.automaticDimension
        sendToStore.isEnabled = false
        deleteComponents.isEnabled = false
        observationsField.delegate = self
        observationsField.text = CommonStrings.placeholderObservations
        observationsField.textColor = UIColor.lightGray
        UtilsManager.shared.setStyleButtonStatus(button: self.deleteComponents,
                                                 title: StatusNameConstants.deleteMultiComponents,
                                                 color: OmicronColors.processStatus,
                                                 titleColor: OmicronColors.processStatus)

        UtilsManager.shared.setStyleButtonStatus(button: self.addComponent,
                                                 title: StatusNameConstants.addComponent,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)
        setBlueButtonStyle()
        changeView(false)
        observationsField.layer.borderWidth = 1
        observationsField.layer.cornerRadius = 10
        observationsField.layer.borderColor = OmicronColors.disabledButton.cgColor
    }

    func setBlueButtonStyle() {
        sendToStore.setTitle(StatusNameConstants.sendToStore, for: .normal)
        sendToStore.setTitleColor(UIColor.white, for: .normal)
        sendToStore.setTitleColor(UIColor.black, for: .disabled)
        sendToStore.layer.borderWidth = 1
        sendToStore.layer.cornerRadius = 10
        sendToStore.titleLabel?.font = UIFont(name: FontsNames.FrutigerNextLTBold, size: 16)
        changeBGButton(button: sendToStore,
                       backgroundColor: OmicronColors.disabledButton)
    }
    func changeBGButton(button: UIButton, backgroundColor: UIColor) {
        button.layer.borderColor = backgroundColor.cgColor
        button.backgroundColor = backgroundColor
    }
}

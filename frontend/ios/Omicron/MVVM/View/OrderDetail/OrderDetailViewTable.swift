//
//  OrderDetailViewTable.swift
//  Omicron
//
//  Created by Daniel Velez on 13/01/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//
import UIKit
import RxSwift
import RxCocoa
import Resolver

extension OrderDetailViewController: UITableViewDelegate {
    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
        cell.selectionStyle = .none
        let colorDefault = indexPath.row % 2 == 0 ? OmicronColors.tableColorRow : .white
        let selected = self.orderDetailViewModel.indexDeleteExist(indexPath.row)
        let color = selected ? OmicronColors.processStatus : colorDefault
        cell.isSelected = selected
        cell.backgroundColor = color
    }
    // Selected Cell
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        tableView.deselectRow(at: indexPath, animated: false)
        guard let cell = tableView.cellForRow(at: indexPath) else { return }
        if self.actionsIsEnabled(indexPath.row) {
            self.orderDetailViewModel.addIndexDeleteTable(index: indexPath.row)
        }
        let colorDefault = indexPath.row % 2 == 0 ? OmicronColors.tableColorRow : .white
        let selected = self.orderDetailViewModel.indexDeleteExist(indexPath.row)
        let color = selected ? OmicronColors.processStatus : colorDefault
        cell.isSelected = selected
        cell.backgroundColor = color
    }

    func actionsIsEnabled(_ index: Int) -> Bool {
        (self.statusType == StatusNameConstants.inProcessStatus
         || self.statusType == StatusNameConstants.reassignedStatus) &&
        (orderDetail.count > 0 && !(orderDetail[0].details?[index].hasBatches ?? true))
        && (orderDetail[0].details?.count ?? 0)>1
    }

    func tableView(_ tableView: UITableView, trailingSwipeActionsConfigurationForRowAt indexPath: IndexPath)
        -> UISwipeActionsConfiguration? {
            if self.actionsIsEnabled(indexPath.row) {
                // Lógica para editar un item de la tabla
                let editItem = UIContextualAction(
                style: .normal, title: CommonStrings.edit) { [weak self] ( _, _, _) in
                    self?.indexOfTableToEditItem = indexPath.row
                    self?.performSegue(
                        withIdentifier: ViewControllerIdentifiers.orderDetailFormViewController, sender: nil)
                }
                // Logica para borrar un elemento de la tabla
                let deleteItem = UIContextualAction(
                style: .destructive, title: CommonStrings.delete) { [weak self] (_, _, _) in
                    let alert = UIAlertController(title: CommonStrings.deleteComponentMessage,
                                                  message: nil, preferredStyle: .alert)
                    let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive,
                                                     handler: { [weak self] _ in self?.dismiss(animated: true)})
                    let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default,
                                                 handler: { [weak self] _ in
                                                    self?.sendIndexToDelete(index: indexPath.row)})
                    alert.addAction(cancelAction)
                    alert.addAction(okAction)
                    self?.present(alert, animated: true)
                }
                let swipeActions = UISwipeActionsConfiguration(actions: [editItem, deleteItem])
                return swipeActions
            }
            return nil
    }
}

//
//  ComponentsBulkOrdersViewController.swift
//  Omicron
//
//  Created by Daniel Velez on 08/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit

extension ComponentsViewController {

    func viewModelBindingBulkOrders() {
        self.searchBar.rx.text.orEmpty.bind(to: self.bulkOrderViewModel.searchBulk).disposed(by: disposeBag)
        self.searchBar.rx.searchButtonClicked.bind(to: bulkOrderViewModel.searchDidEnter).disposed(by: disposeBag)
        self.bulkOrderViewModel.dataChips.map({ data -> Bool in
            return data.count == 0
        }).asDriver(onErrorJustReturn: false).drive(self.tagsView.rx.isHidden).disposed(by: disposeBag)
        self.bulkOrderViewModel.dataChips.subscribe(onNext: { [weak self] data in
            self?.tagsView.removeAllTags()
            if data.count == 0 {
                return
            }
            self?.tagsView.addTags(data)
            self?.searchBar.text = ""
        }).disposed(by: disposeBag)
        self.bulkOrderViewModel.dataResults.bind(to: tableView.rx.items(
            cellIdentifier: ViewControllerIdentifiers.componentsTableViewCell,
            cellType: ComponentsTableViewCell.self)) { _, data, cell in
                cell.productCodeLabel.text = data.productoId
                cell.descriptionLabel.text = data.largeDescription?.uppercased()
        }.disposed(by: disposeBag)
        bulkOrderViewModel.dataResults.map({ data -> Bool in
            return data.count > 0
        }).asDriver(onErrorJustReturn: true).drive(labelNoResults.rx.isHidden).disposed(by: disposeBag)
        selectBulkElementBind()
        okCreateBulkOrderBind()
        itemSelectedOfMostCommonComponentsBulkOrder()
    }

    func selectBulkElementBind() {
        self.tableView.rx.modelSelected(BulkProduct.self).subscribe(onNext: { [weak self] data in
            guard let self = self else { return }
            self.createBulkOrderQuestion(data)
        }).disposed(by: disposeBag)
    }

    func createBulkOrderQuestion(_ data: BulkProduct) {
        let alert = UIAlertController(
            title: "¿Desea crear la orden de fabricación de granel \(data.productoId ?? "")?",
            message: nil,
            preferredStyle: .alert)
        let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive, handler: nil)
        let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default, handler: { [weak self] _ in
            guard let self = self else { return }
            self.bulkOrderViewModel.createBulkOrder(data)
        })
        alert.addAction(cancelAction)
        alert.addAction(okAction)
        self.present(alert, animated: true, completion: nil)
    }

    func okCreateBulkOrderBind() {
        self.bulkOrderViewModel.okCreateOrder.subscribe(onNext: { [weak self] message in
            guard let self = self else { return }
            let alert = UIAlertController(
                title: message,
                message: nil,
                preferredStyle: .alert)
            let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default, handler: nil)
            alert.addAction(okAction)
            self.present(alert, animated: true, completion: nil)
            if message == CommonStrings.okCreateBulkOrder {
                DispatchQueue.main.asyncAfter(deadline: .now() + .milliseconds(3000)) {
                    self.view.window?.rootViewController?.dismiss(animated: true)
                }
            }
        }).disposed(by: disposeBag)
    }
    func itemSelectedOfMostCommonComponentsBulkOrder() {
        self.mostCommontTableView.rx.modelSelected(ComponentO.self).subscribe(onNext: { [weak self] data in
            let bulk = BulkProduct()
            bulk.productoId = data.productId
            bulk.largeDescription = data.description
            self!.createBulkOrderQuestion(bulk)
        }).disposed(by: disposeBag)
    }
}

//
//  HistoryViewController.swift
//  Omicron
//
//  Created by Daniel Vargas on 14/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import UIKit
extension SupplieViewController {
    func loadStyles() {
        dateOrderView?.layer.cornerRadius = 10
        estatusView?.layer.cornerRadius = 10
    }
    func bindSegmentedControl() {
        self.segmentedControl?.addTarget(self, action: #selector(onChanged(_:)), for: .valueChanged)
    }
    @IBAction func changeSelectedDateDidPressed(_ sender: Any) {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let selectorVC = storyboard.instantiateViewController(
            withIdentifier: ViewControllerIdentifiers.dateRangeSelectorViewController)
            as? DateRangeSelectorViewController
        let navigationVC = UINavigationController(rootViewController: selectorVC ?? DateRangeSelectorViewController())
        selectorVC?.startDate = self.historyViewModel.startDate
        selectorVC?.endDate = self.historyViewModel.endDate
        navigationVC.modalPresentationStyle = .formSheet
        self.present(navigationVC, animated: true, completion: nil)
    }
    @IBAction func changeStatusOptionsDidPressed(_ sender: Any) {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let dropdownVC = storyboard.instantiateViewController(
            withIdentifier: ViewControllerIdentifiers.dropdownViewController)
            as? DropdownViewController
        dropdownVC?.modalPresentationStyle = .popover
        dropdownVC?.selectedOptions = historyViewModel.selectedStatus
        dropdownVC?.preferredContentSize = CGSize(width: 200, height: 230)
        let navigationVC = UINavigationController(rootViewController: dropdownVC ?? DropdownViewController())
        self.present(navigationVC, animated: true, completion: nil)
    }

    @objc func onChanged(_ sender: UISegmentedControl) {
        switch segmentedControl?.selectedSegmentIndex {
        case 0: changeSegmentedView(isSupplie: true)
        case 1: changeSegmentedView(isSupplie: false)
        default: break
        }
    }
    func changeSegmentedView(isSupplie: Bool) {
        self.historySupplie.isHidden = isSupplie
        self.newSupplie.isHidden = !isSupplie
    }
    func bindHistoryTable() {
        self.historyViewModel.selectedHistoryList.bind(to: self.tableHistory.rx.items(
                cellIdentifier: ViewControllerIdentifiers.historyTableViewCell,
                cellType: HistoryTableViewCell.self)) { index, item, cell  in
        }.disposed(by: disposeBag)
    }
}

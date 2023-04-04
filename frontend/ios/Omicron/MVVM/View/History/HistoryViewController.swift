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
    func bindRecognizers() {
        let tap = UITapGestureRecognizer(target: self, action: #selector(handleTapStatus))
        self.estatusView.addGestureRecognizer(tap)
        let tap2 = UITapGestureRecognizer(target: self, action: #selector(handleTapDateSelector))
        self.dateOrderView.addGestureRecognizer(tap2)
    }
    @objc func handleTapDateSelector() {
        openDateRangeSelector()
    }
    @objc func handleTapStatus() {
        guard let popoverVC = storyboard?.instantiateViewController(identifier: "DropdownViewController") as? DropdownViewController
        else { return }
        popoverVC.modalPresentationStyle = .popover
        popoverVC.popoverPresentationController?.sourceView = estatusView
        present(popoverVC, animated: true, completion: nil)
    }
    @IBAction func changeSelectedDateDidPressed(_ sender: Any) {
        openDateRangeSelector()
    }
    func openDateRangeSelector() {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let selectorVC = storyboard.instantiateViewController(
            withIdentifier: ViewControllerIdentifiers.dateRangeSelectorViewController)
            as? DateRangeSelectorViewController
        let navigationVC = UINavigationController(rootViewController: selectorVC ?? DateRangeSelectorViewController())
        selectorVC?.startDate = self.historyViewModel.startDate
        selectorVC?.endDate = self.historyViewModel.endDate
        selectorVC?.delegate = self
        navigationVC.modalPresentationStyle = .formSheet
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
                cellType: HistoryTableViewCell.self)) { _, item, cell  in
                    let dateFormatter = DateFormatter()
                    dateFormatter.dateFormat = "dd/MM/yyyy"
                    let docEntry = item.docEntry ?? 0
                    cell.sapId.text = String(docEntry)
                    cell.descriptionLabel.text = item.description
                    cell.codeLabel.text = item.itemCode
                    let quantity = UtilsManager.shared
                        .formatterDoublesToString().string(from: (item.quantity ?? 0) as NSNumber)
                    cell.quantityLabel.text = quantity
                    cell.unitLabel.text = item.unit
                    cell.destinationStoreLabel.text = item.targetWarehosue
                    let selectedDate = item.docDate ?? String()
                    cell.dateOrderLabel.text = selectedDate
                    cell.statusOrderLabel.text = item.status
                    cell.circleStatus.layer.cornerRadius = 5
                    cell.circleStatus.backgroundColor = self.getBackGround(status: item.status ?? String())
        }.disposed(by: disposeBag!)
    }
    func getBackGround(status: String) -> UIColor {
        let options = ["Abierto": OmicronColors.historyStatusOpen,
                       "Cerrado": OmicronColors.historyStatusClosed,
                       "Cancelado": OmicronColors.historyStatusCancel]
        return options[status] ?? OmicronColors.historyStatusOpen
    }
    func bindIsLoading() {
        self.historyViewModel.loading.subscribe(onNext: {[weak self] loading in
            guard let self = self else { return }
            if loading {
                self.lottieManager.showLoading()
                return
            }
            self.lottieManager.hideLoading()
        }).disposed(by: disposeBag!)
    }
    func validateHistoryResults(totalInfo: Int) {
        tableHistory.isHidden = totalInfo == 0
        noHistoryResults.isHidden = totalInfo > 0
    }
    func validateHasInfo() {
        self.historyViewModel.selectedHistoryList.subscribe(onNext: {[weak self] data in
            guard let self = self else { return }
            self.validateHistoryResults(totalInfo: data.count)
        }).disposed(by: disposeBag!)
    }
    func bindShowAlert() {
        self.historyViewModel.showAlert.subscribe(onNext: {[weak self] error in
            guard let self = self else { return }
            self.showAlert(alert: (title: error, msg: String(), autoDismiss: true))
        }).disposed(by: disposeBag!)
    }
    func bindinChangeFilters() {
        self.historyViewModel.changeFilters.subscribe(onNext: {[weak self] _ in
            guard let self = self else { return }
            self.repaintFilters()
        }).disposed(by: disposeBag!)
    }
    func repaintFilters() {
        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "dd/MM/yyyy"
        statusSelectedsLabel.text = historyViewModel.selectedStatus.sorted().joined(separator: ", ")
        let startDateString = dateFormatter.string(from: historyViewModel.startDate)
        let endDateString = dateFormatter.string(from: historyViewModel.endDate)
        dateRangeSelectedLabel.text = "\(startDateString)-\(endDateString)"
    }
}

extension SupplieViewController: DateRangeSelectorViewDelegate {
    func acceptRange(startDate: Date, endDate: Date) {
        historyViewModel.selectedRangeDateObs.onNext((startDate: startDate, endDate: endDate))
    }
}

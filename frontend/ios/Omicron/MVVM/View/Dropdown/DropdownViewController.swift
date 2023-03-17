//
//  DropdownViewController.swift
//  Omicron
//
//  Created by Daniel Vargas on 15/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import UIKit
import Resolver
class DropdownViewController: UIViewController {
    @IBOutlet weak var optionsTable: UITableView!
    var options = ["ABIERTO", "CERRADO", "CANCELADO"]
    @Injected var historyViewModel: HistoryViewModel
    var selectedOptions: [String] = []
    override func viewDidLoad() {
        super.viewDidLoad()
        optionsTable.delegate = self
        optionsTable.dataSource = self
        selectedOptions = historyViewModel.selectedStatus
        setSelectedOptions()
        self.preferredContentSize = CGSize(width: 210, height: 250)
    }
    func setSelectedOptions() {
        selectedOptions.forEach ({
            if let selectedIndex = self.options.firstIndex(of: $0) {
                self.optionsTable.selectRow(at: IndexPath(row: selectedIndex, section: 0),
                                            animated: false,
                                            scrollPosition: .none)
                self.optionsTable.cellForRow(at: IndexPath(row: selectedIndex,
                                                           section: 0))?.accessoryType = .checkmark
            }
        })
    }

    @IBAction func acceptDidPressed(_ sender: Any) {
        var selectedItems: [String] = []
        if let selectedRows = optionsTable.indexPathsForSelectedRows {
            for ipath in selectedRows {
                selectedItems.append(options[ipath.row])
            }
            historyViewModel.selectedStatusObs.onNext(selectedItems)
            self.dismiss(animated: true)
        }
    }
}

extension DropdownViewController: UITableViewDelegate, UITableViewDataSource {
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return self.options.count
    }
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = optionsTable.dequeueReusableCell(withIdentifier: "dropdownCell", for: indexPath)
        cell.textLabel?.text = options[indexPath.row]
        cell.selectionStyle = .none
        return cell
    }
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        optionsTable.cellForRow(at: indexPath)?.accessoryType = .checkmark
    }
    func tableView(_ tableView: UITableView, didDeselectRowAt indexPath: IndexPath) {
        optionsTable.cellForRow(at: indexPath)?.accessoryType = .none
    }
}

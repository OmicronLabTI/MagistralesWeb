//
//  StatusSectionTableViewController.swift
//  Omicron
//
//  Created by Axity on 25/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class StatusSectionTableViewController: UITableViewController {
    
    var arrayData: [Int : Section] = [1: Section(statusName: "Asignadas", numberTask: 5, imageIndicatorStatus: "assignedStatus"), 2: Section(statusName: "En Proceso", numberTask: 5, imageIndicatorStatus: "processStatus"), 3: Section(statusName: "Pendientes", numberTask: 1, imageIndicatorStatus: "pendingStatus"), 4: Section(statusName: "Terminado", numberTask: 1, imageIndicatorStatus: "finishedStatus"), 5: Section(statusName: "Reasignado", numberTask: 1, imageIndicatorStatus: "reassignedStatus")]

    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.view.backgroundColor = OmicronColors.tableStatus
        let index = NSIndexPath(row: 0, section: 0)
        self.tableView.selectRow(at: index as IndexPath, animated: true, scrollPosition: .middle)
        self.title = "Mis órdenes"
        tableView.tableFooterView = UIView()
    
    }
    
    override func viewDidAppear(_ animated: Bool) {
    
    }
    
    override func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return arrayData.count
    }
    
    override func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "StatusViewCell", for: indexPath) as! StatusViewCell
        cell.statusNameLabel.text = "\(arrayData[indexPath.row + 1]!.statusName)"
        cell.numberTaskLabel.text = "\(arrayData[indexPath.row + 1]!.numberTask)"
        cell.indicatorImageView.image = UIImage(named: arrayData[indexPath.row + 1]!.imageIndicatorStatus)
        return cell
    }
    
    override func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        let vc = storyboard?.instantiateViewController(withIdentifier: "InboxViewController") as! InboxViewController
        splitViewController?.showDetailViewController(vc, sender: nil)
        let statusName = "\(arrayData[indexPath.row + 1]!.statusName)"
        vc.statusNameLabel.text = statusName
        changeButtons(statusName: statusName, vc: vc)
    }
    
    func changeButtons(statusName: String, vc: InboxViewController) {
        switch statusName {
        case "Asignadas":
            vc.processButton.isHidden = false
            vc.finishedButton.isHidden = true
            vc.pendingButton.isHidden = false
        case "En Proceso":
            vc.processButton.isHidden = true
            vc.finishedButton.isHidden = false
            vc.pendingButton.isHidden = false
        case "Pendientes":
            vc.processButton.isHidden = false
            vc.finishedButton.isHidden = true
            vc.pendingButton.isHidden = true
        case "Terminado":
            vc.processButton.isHidden = true
            vc.finishedButton.isHidden = true
            vc.pendingButton.isHidden = true
        case "Reasignado":
            vc.processButton.isHidden = true
            vc.finishedButton.isHidden = false
            vc.pendingButton.isHidden = true
        default:
            print("")
        }
    }
}

//
//  PatientListViewController.swift
//  Omicron
//
//  Created by Daniel Velez on 17/11/22.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa
import Resolver

class PatientListViewController: UIViewController, UITableViewDataSource, UITableViewDelegate {

    @IBOutlet weak var tableView: UITableView!
    @IBOutlet weak var orderId: UILabel!
    var name: String?
    var order: Int = 0
    var patientList: String = ""
    var list: [String] = []
    let disposeBag = DisposeBag()
    override func viewDidLoad() {
        super.viewDidLoad()
        self.list = self.patientList.components(separatedBy: ",")
        tableView.register(PatientListTableViewCell.self, forCellReuseIdentifier: "PatientListTableViewCell")
        tableView.delegate = self
        tableView.dataSource = self
        self.orderId.text = "Pedido: \(order)"
    }

    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        list.count
    }

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        // swiftlint:disable force_cast
        let cell = tableView.dequeueReusableCell(withIdentifier: "PatientListTableViewCell", for: indexPath) as! PatientListTableViewCell
        let name = list[indexPath.row]
        cell.nameLabel?.text = name.trimmingCharacters(in: .whitespacesAndNewlines)
        cell.nameLabel?.textColor = .black
        cell.nameLabel?.backgroundColor = .black
        print(name.trimmingCharacters(in: .whitespacesAndNewlines))
        return cell
    }

    @IBAction func acceptDidPressed(_ sender: Any) {
        dismiss(animated: true, completion: nil)
    }
}



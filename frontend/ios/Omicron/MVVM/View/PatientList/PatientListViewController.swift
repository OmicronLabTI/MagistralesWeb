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
    @IBOutlet weak var contentView: UIView!
    @IBOutlet weak var buttonAcept: UIButton!
    @IBOutlet weak var tableView: UITableView!
    @IBOutlet weak var orderId: UILabel!
    var name: String = ""
    var list: [String] = []
    let disposeBag = DisposeBag()
    override func viewDidLoad() {
        super.viewDidLoad()
        self.setUI()
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        tableView.reloadData()
    }

    func setUI() {
        buttonAcept.layer.cornerRadius = 15
        contentView.layer.cornerRadius = 15
        tableView.delegate = self
        tableView.dataSource = self
        orderId.text = self.name
        tableView.rowHeight = UITableView.automaticDimension
    }

    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        list.count
    }

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "PatientListTableViewCell", for: indexPath)
        let name = list[indexPath.row]
        cell.textLabel?.text = name.trimmingCharacters(in: .whitespacesAndNewlines)
        return cell
    }

    @IBAction func acceptDidPressed(_ sender: Any) {
        dismiss(animated: true, completion: nil)
    }
}

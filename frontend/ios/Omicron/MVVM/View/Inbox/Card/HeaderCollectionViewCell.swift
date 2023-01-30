//
//  HeaderCollectionViewCell.swift
//  Omicron
//
//  Created by Vicente Cantú on 13/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

protocol HeaderSelectedDelegate: AnyObject {
    func downloadPDF(_ ordersID: [Int])
    func showPatientList(_ title: String, _ patientList: [String])
}

class HeaderCollectionViewCell: UICollectionViewCell {

    @IBOutlet weak var productID: UILabel!
    @IBOutlet weak var pdfImageView: UIImageView!
    @IBOutlet weak var patientListButton: UIButton!
    @IBOutlet weak var doctorName: UILabel!
    var orders: Set<Int> = []
    var titlePatients: String = ""
    var patientNames: Set<String> = []

    @IBAction func patientListAction(_ sender: Any) {
        delegate?.showPatientList(titlePatients, Array(patientNames))
    }
    weak var delegate: HeaderSelectedDelegate?

    override func awakeFromNib() {
        super.awakeFromNib()

        let tap = UITapGestureRecognizer(target: self, action: #selector(handleTap))
        contentView.addGestureRecognizer(tap)
    }

    @objc func handleTap() {
        delegate?.downloadPDF(Array(orders))
    }

}

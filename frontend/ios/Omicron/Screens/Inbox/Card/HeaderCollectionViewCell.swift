//
//  HeaderCollectionViewCell.swift
//  Omicron
//
//  Created by Vicente Cantú on 13/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

protocol HeaderSelectedDelegate: class {
    func headerSelected(productID: Int)
}

class HeaderCollectionViewCell: UICollectionViewCell {

    @IBOutlet weak var productID: UILabel!
    @IBOutlet weak var pdfImageView: UIImageView!

    var productId = 0

    weak var delegate: HeaderSelectedDelegate?

    override func awakeFromNib() {
        super.awakeFromNib()

        let tap = UITapGestureRecognizer(target: self, action: #selector(handleTap))
        contentView.addGestureRecognizer(tap)

    }

    @objc func handleTap() {
        delegate?.headerSelected(productID: productId)
    }

}

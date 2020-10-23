//
//  ErrorViewController.swift
//  Omicron
//
//  Created by Vicente Cantú on 22/10/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class ErrorViewController: UIViewController {

    @IBOutlet weak var errorsTextView: UITextView!
    var errorDescription = ""

    override func viewDidLoad() {
        super.viewDidLoad()
        errorsTextView.text = errorDescription
        errorsTextView.centerText()
        errorsTextView.textAlignment = .left
    }

}

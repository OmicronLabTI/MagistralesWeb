//
//  PDFViewController.swift
//  Omicron
//
//  Created by Vicente Cantú on 12/10/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import PDFKit

class PDFViewController: UIViewController {

    var pdfView = PDFView()
    var pdfURL: URL!

    override func viewDidLoad() {
        super.viewDidLoad()

        if let document = PDFDocument(url: pdfURL) {

            DispatchQueue.main.async {
                self.pdfView.document = document
                self.view.addSubview(self.pdfView)
            }

        }

    }

    override func viewDidLayoutSubviews() {
        pdfView.frame = view.frame
    }

}

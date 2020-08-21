//
//  LotsViewController.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class LotsViewController: UIViewController {
    
    // MARK: -OUTLEST
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var hashtagLabel: UILabel!
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    @IBOutlet weak var warehouseCodeLabel: UILabel!
    @IBOutlet weak var totalNeededLabel: UILabel!
    @IBOutlet weak var totalSelectedLabel: UILabel!
    @IBOutlet weak var lineOfDocumentsView: UIView!
    
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.initComponents()
        
    }
    
    func initComponents() {
        self.title = "Lotes"
        UtilsManager.shared.labelsStyle(label: self.titleLabel, text: "Líneas de documentos", fontSize: 20)
        UtilsManager.shared.labelsStyle(label: self.hashtagLabel, text: "#", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.codeLabel, text: "Código", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.descriptionLabel, text: "Descripción del artículo", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.warehouseCodeLabel, text: "Código de almacén", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.totalNeededLabel, text: "Total necesario", fontSize: 15)
        UtilsManager.shared.labelsStyle(label: self.totalSelectedLabel, text: "Total Seleccionado", fontSize: 15)
        
        self.lineOfDocumentsView.layer.shadowColor = UIColor.black.cgColor
        self.lineOfDocumentsView.layer.shadowOpacity = 0.2
        self.lineOfDocumentsView.layer.shadowOffset  = CGSize(width: 0.1, height: 0.1)
        self.lineOfDocumentsView.layer.shadowRadius = 5
        self.lineOfDocumentsView.layer.cornerRadius = 10
    }

    /*
    // MARK: - Navigation

    // In a storyboard-based application, you will often want to do a little preparation before navigation
    override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
        // Get the new view controller using segue.destination.
        // Pass the selected object to the new view controller.
    }
    */

}

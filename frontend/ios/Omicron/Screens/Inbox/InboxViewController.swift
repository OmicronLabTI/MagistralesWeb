//
//  InboxViewController.swift
//  Omicron
//
//  Created by Axity on 24/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

class InboxViewController: UIViewController {
    
    @IBOutlet weak var numberlLabel: UILabel!
    @IBOutlet weak var statusNameLabel: UILabel!
    @IBOutlet weak var finishedButton: UIButton!
    @IBOutlet weak var pendingButton: UIButton!
    @IBOutlet weak var processButton: UIButton!
    
    
    
    var text = ""
    override func viewDidLoad() {
        super.viewDidLoad()
        self.initComponents()
        numberlLabel.text = self.text
        // Do any additional setup after loading the view.
    }
    
    // MARK: Functions
    func initComponents() -> Void {
        self.statusNameLabel.text = "Asignadas"
        self.statusNameLabel.font = UIFont.systemFont(ofSize: 22, weight: .bold)
        
        self.setStyleButton(button: self.finishedButton, title: "Terminado", color: OmicronColors.finishedStatus)
        self.setStyleButton(button: self.pendingButton, title: "Pendiente", color: OmicronColors.pendingStatus)
        self.setStyleButton(button: self.processButton, title: "En proceso", color: OmicronColors.processStatus)
    }
    
    func setStyleButton( button: UIButton ,title: String, color: UIColor) {
        button.setTitle(title, for: .normal)
        button.setTitleColor(color, for: .normal)
        button.layer.borderWidth = 1
        button.layer.cornerRadius = 10
        button.layer.borderColor = color.cgColor
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

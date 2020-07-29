//
//  InboxViewController.swift
//  Omicron
//
//  Created by Axity on 24/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa

class InboxViewController: UIViewController {
    
    @IBOutlet weak var statusNameLabel: UILabel!
    @IBOutlet weak var finishedButton: UIButton!
    @IBOutlet weak var pendingButton: UIButton!
    @IBOutlet weak var processButton: UIButton!
    @IBOutlet weak var collectionView: UICollectionView!
    
    lazy var inboxModel: InboxViewModel = InboxViewModel()
    let disposeBag = DisposeBag()
    
    let listImages = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10",]
    private let cardWidth = UIScreen.main.bounds.width / 2.5
    override func viewDidLoad() {
        super.viewDidLoad()
        viewModelBinding()
        self.initComponents()
        collectionView.dataSource = self
        collectionView.delegate = self
        collectionView.register(UINib(nibName:
            "CardCollectionViewCell", bundle: nil), forCellWithReuseIdentifier: "card")
        finishedButton.isHidden = true
        // Do any additional setup after loading the view.
    }
    
    // MARK: Functions
    
    func viewModelBinding() -> Void {
        [
            finishedButton.rx.tap.bind(to: inboxModel.finishedDidTab),
            pendingButton.rx.tap.bind(to: inboxModel.pendingDidTab),
            processButton.rx.tap.bind(to: inboxModel.processDidTab),
        ].forEach({ $0.disposed(by: disposeBag) })
        
    }
    
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
extension InboxViewController: UICollectionViewDataSource {
    func collectionView(_ collectionView: UICollectionView, numberOfItemsInSection section: Int) -> Int {
        return 9
    }
    
    func collectionView(_ collectionView: UICollectionView, cellForItemAt indexPath: IndexPath) -> UICollectionViewCell {
        let cell = collectionView.dequeueReusableCell(withReuseIdentifier: "card", for: indexPath) as? CardCollectionViewCell
        return cell!
    }
    
    
}

extension InboxViewController: UICollectionViewDelegate {
    func collectionView(_ collectionView: UICollectionView, didSelectItemAt indexPath: IndexPath) {
        
    }
}

extension InboxViewController: UICollectionViewDelegateFlowLayout {
//    func collectionView(_ collectionView: UICollectionView, layout collectionViewLayout: UICollectionViewLayout, sizeForItemAt indexPath: IndexPath) -> CGSize {
//        return CGSize(width: 300, height: cardWidth - 200)
//    }
}

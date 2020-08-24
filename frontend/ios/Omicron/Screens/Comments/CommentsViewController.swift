//
//  CommentsViewController.swift
//  Omicron
//
//  Created by Axity on 14/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxCocoa
import RxSwift
import Resolver

class CommentsViewController: UIViewController {
    
    // MARK: -Outlets
    @IBOutlet weak var mainView: UIView!
    @IBOutlet weak var labelView: UIView!
    @IBOutlet weak var buttonsView: UIView!
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var textView: UITextView!
    @IBOutlet weak var cancelButton: UIButton!
    @IBOutlet weak var aceptButton: UIButton!
    
    // MARK: -Variables
    @Injected var commentsViewModel: CommentsViewModel
    @Injected var orderDetailVC: OrderDetailViewModel
    var orderDetail: [OrderDetail] = []
    var disposeBag = DisposeBag()
    
    // MARK: - LifeCycles
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        
        self.initComponents()
        self.viewModelBinding()
        self.commentsViewModel.orderDetail = self.orderDetail
        self.textView.text = self.orderDetail[0].comments != nil ? self.orderDetail[0].comments: ""
    }
    
    //MARK: - Functions
    
    @IBAction func cancelButtonAction(_ sender: Any) {
        self.backToOrderDetail()
    }
    
    func backToOrderDetail() -> Void {
        self.dismiss(animated: true)
    }
    
    
    func viewModelBinding() -> Void {
        self.commentsViewModel.backToOrderDetail.observeOn(MainScheduler.instance).subscribe(onNext: { _ in
            // TODO: chechar para actualizar
            // self.orderDetailVC.getOrdenDetail()
            self.backToOrderDetail()
        }).disposed(by: self.disposeBag)
        
        self.aceptButton.rx.tap.bind(to: commentsViewModel.aceptDidTap).disposed(by: self.disposeBag)
        self.textView.rx.text.orEmpty.bind(to: commentsViewModel.textView).disposed(by: self.disposeBag)
        
        self.commentsViewModel.showAlert.observeOn(MainScheduler.instance).subscribe(onNext: { message in
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.disposeBag)
        
        self.commentsViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { showLoading in
            if(showLoading) {
                LottieManager.shared.showLoading()
                return
            }
            LottieManager.shared.hideLoading()
        }).disposed(by: self.disposeBag)
    }
    
    func initComponents() -> Void {
        self.mainView.layer.cornerRadius = 50
        self.labelView.backgroundColor = OmicronColors.comments
        self.buttonsView.backgroundColor = OmicronColors.comments
        
        self.titleLabel.text = "Comentarios"
        self.titleLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 20)
        
        self.cancelButton.setTitle("Cancelar", for: .normal)
        self.cancelButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 15)
        self.cancelButton.setTitleColor(.red, for: .normal)
        
        self.aceptButton.setTitle("Aceptar", for: .normal)
        self.aceptButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 15)
        
        self.textView.text = ""
        self.textView.font = UIFont(name: FontsNames.SFProDisplayRegular, size: 18)
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



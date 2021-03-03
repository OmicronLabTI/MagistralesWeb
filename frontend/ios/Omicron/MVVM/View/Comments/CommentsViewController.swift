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
    // MARK: - Outlets
    @IBOutlet weak var mainView: UIView!
    @IBOutlet weak var labelView: UIView!
    @IBOutlet weak var buttonsView: UIView!
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var textView: UITextView!
    @IBOutlet weak var cancelButton: UIButton!
    @IBOutlet weak var aceptButton: UIButton!

    // MARK: - Variables
    @Injected var commentsViewModel: CommentsViewModel
    @Injected var orderDetailVC: OrderDetailViewModel
    @Injected var lottieManager: LottieManager
    @Injected var lotsViewModel: LotsViewModel

    var orderDetail: [OrderDetail] = []
    var disposeBag = DisposeBag()
    var originView = String()

    // MARK: - LifeCycles
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        self.initComponents()
        self.viewModelBinding()
        self.commentsViewModel.orderDetail = self.orderDetail
        self.textView.text = self.orderDetail.first?.comments != nil ? self.orderDetail.first?.comments : String()
    }
    // MARK: - Functions
    @IBAction func cancelButtonAction(_ sender: Any) {
        self.dismissCommentsView()
    }
    func dismissCommentsView() {
        self.dismiss(animated: true)
    }
    func viewModelBinding() {
        self.commentsViewModel.originView = self.originView
        self.commentsViewModel.backToOrderDetail.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.dismissCommentsView()
            self?.orderDetailVC.getOrdenDetail()
        }).disposed(by: self.disposeBag)
        self.commentsViewModel.backToLots.subscribe(onNext: { [weak self] _ in
            self?.dismissCommentsView()
            self?.lotsViewModel.updateOrderDetail()
        }).disposed(by: self.disposeBag)
        self.aceptButton.rx.tap.bind(to: commentsViewModel.aceptDidTap).disposed(by: self.disposeBag)
        self.textView.rx.text.orEmpty.bind(to: commentsViewModel.textView).disposed(by: self.disposeBag)
        self.commentsViewModel.showAlert.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] message in
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.disposeBag)
        self.commentsViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] showLoading in
            if showLoading {
                self?.lottieManager.showLoading()
                return
            }
            self?.lottieManager.hideLoading()
        }).disposed(by: self.disposeBag)
    }
    func initComponents() {
        mainView.layer.cornerRadius = 10
        titleLabel.text = "Comentarios"
        titleLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 22)
        cancelButton.setTitle("Cancelar", for: .normal)
        cancelButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayRegular, size: 17)
        cancelButton.setTitleColor(.systemRed, for: .normal)
        aceptButton.setTitle("Aceptar", for: .normal)
        aceptButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayRegular, size: 17)
//        aceptButton.setTitleColor(.white, for: .normal)
//        aceptButton.backgroundColor = UIColor.systemGreen
        textView.text = String()
        textView.font = UIFont(name: FontsNames.SFProDisplayRegular, size: 20)
    }
}

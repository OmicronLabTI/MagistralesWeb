//
//  LoginViewController.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxCocoa
import RxSwift

class LoginViewController: UIViewController {
    @IBOutlet weak var usernameTextField : UITextField!
    @IBOutlet weak var passwordTextField : UITextField!
    @IBOutlet weak var loginButton : UIButton!
    
    lazy var viewModel: LoginViewModel = LoginViewModel()
    
    let disposeBag = DisposeBag()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        viewModelBinding()
    }
    
    func viewModelBinding() {
        [
            usernameTextField.rx.text.orEmpty.bind(to: viewModel.username),
            passwordTextField.rx.text.orEmpty.bind(to: viewModel.password),
            loginButton.rx.tap.bind(to: viewModel.loginDidTap),
            viewModel.canLogin.drive(loginButton.rx.isEnabled),
            ].forEach({ $0.disposed(by: disposeBag)})
        
        viewModel.loading
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] loading in
                self?.loginButton.isEnabled = !loading
                if loading {
                    LottieManager.shared.showLoading()
                } else {
                    LottieManager.shared.hideLoading()
                }
            }).disposed(by: disposeBag)
        
        viewModel.error
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] (error) in
                self?.showError(error: error)
            })
            .disposed(by: disposeBag)
        
        viewModel.loginResponse
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { res in
                self.showSuccess(message: res.token ?? "")
            })
            .disposed(by: disposeBag)
    }
    
    func showSuccess(message: String) {
        showAlert(message: message)
    }
    
    func showError(error: String) {
        showAlert(message: error)
    }
    
    private func showAlert(message: String) {
        let alert = UIAlertController(title: "", message: message, preferredStyle: .alert)
        let okAction = UIAlertAction(title: "OK", style: .default, handler: nil)
        alert.addAction(okAction)
        self.present(alert, animated: true, completion: nil)
    }
}

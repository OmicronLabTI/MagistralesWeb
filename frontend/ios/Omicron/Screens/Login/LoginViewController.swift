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
    @IBOutlet weak var logoView: UIView!
    @IBOutlet weak var loginView: UIView!
    @IBOutlet weak var loginLabel: UILabel!
    @IBOutlet weak var loginDescriptionLabel: UILabel!
    @IBOutlet weak var loginButtonDescriptionLabel: UILabel!
    
    lazy var viewModel: LoginViewModel = LoginViewModel()
    
    let disposeBag = DisposeBag()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        viewModelBinding()
        initComponents()
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
    
    func initComponents() {

        self.view.backgroundColor = UIColor.init(red: 46/255, green: 134/255, blue: 193/255, alpha: 1.0)

        self.logoView.backgroundColor = UIColor.init(red: 46/255, green: 134/255, blue: 193/255, alpha: 1.0)

        self.loginView.backgroundColor = UIColor.white
        self.loginView.layer.cornerRadius = 30
        
        self.loginLabel.text = "Login"
        self.loginLabel.font = UIFont.boldSystemFont(ofSize: 12)
        self.loginLabel.textColor = UIColor.black
        
        self.loginDescriptionLabel.text = "Ingresa a tu cuenta"
        self.loginDescriptionLabel.font = UIFont.boldSystemFont(ofSize: 22)
        self.loginDescriptionLabel.textColor = UIColor.black
        
        self.usernameTextField.backgroundColor = UIColor.init(red: 248/255, green: 249/255, blue: 249/255, alpha: 1)
        self.usernameTextField.textColor = UIColor.black
        self.usernameTextField.attributedPlaceholder = NSAttributedString(string: "Usuario", attributes: [NSAttributedString.Key.foregroundColor : UIColor.lightGray])
        
        self.passwordTextField.backgroundColor = UIColor.init(red: 248/255, green: 249/255, blue: 249/255, alpha: 1)
        self.passwordTextField.textColor = UIColor.black
        self.passwordTextField.attributedPlaceholder = NSAttributedString(string: "Contraseña", attributes: [NSAttributedString.Key.foregroundColor: UIColor.lightGray])
        
        self.loginButtonDescriptionLabel.text = "Ingresar"
        self.loginButtonDescriptionLabel.font = UIFont.boldSystemFont(ofSize: 20)
        self.loginButtonDescriptionLabel.textColor = UIColor.black
    }
    
    private func showAlert(message: String) {
        let alert = UIAlertController(title: "", message: message, preferredStyle: .alert)
        let okAction = UIAlertAction(title: "OK", style: .default, handler: nil)
        alert.addAction(okAction)
        self.present(alert, animated: true, completion: nil)
    }
}

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
    
    // MARK: - OUTLETS
    @IBOutlet weak var usernameTextField : UITextField!
    @IBOutlet weak var passwordTextField : UITextField!
    @IBOutlet weak var loginButton : UIButton!
    @IBOutlet weak var logoView: UIView!
    @IBOutlet weak var loginView: UIView!
    @IBOutlet weak var loginLabel: UILabel!
    @IBOutlet weak var loginDescriptionLabel: UILabel!
    @IBOutlet weak var loginButtonDescriptionLabel: UILabel!
    @IBOutlet weak var userLabel: UILabel!
    @IBOutlet weak var passwordLabel: UILabel!
    
    // MARK: - VARIABLES
    lazy var viewModel: LoginViewModel = LoginViewModel()
    let disposeBag = DisposeBag()
    
    // MARK: - LIFE CYCLES
    override func viewDidLoad() {
        super.viewDidLoad()
        viewModelBinding()
        initComponents()
        
        NotificationCenter.default.addObserver(self, selector: #selector(keyBoardActions(notification:)), name: UIResponder.keyboardWillShowNotification, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(keyBoardActions(notification:)), name: UIResponder.keyboardDidHideNotification, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(keyBoardActions(notification:)), name: UIResponder.keyboardWillChangeFrameNotification, object: nil)
    }
    
    // MARK: - FUNCTIONS
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

        self.view.backgroundColor = UIColor.init(red: 84/255, green: 128/255, blue: 166/255, alpha: 1)

        self.logoView.backgroundColor = UIColor.init(red: 84/255, green: 128/255, blue: 166/255, alpha: 1)

        self.loginView.backgroundColor = UIColor.white
        self.loginView.layer.cornerRadius = 30
        
        self.loginLabel.text = "Login"
        self.loginLabel.font = UIFont.boldSystemFont(ofSize: 12)
        self.loginLabel.textColor = UIColor.black
        
        self.loginDescriptionLabel.text = "Ingresa a tu cuenta"
        self.loginDescriptionLabel.font = UIFont.boldSystemFont(ofSize: 24)
        self.loginDescriptionLabel.textColor = UIColor.black
        
        self.usernameTextField.backgroundColor = UIColor.init(red: 246/255, green: 246/255, blue: 246/255, alpha: 1)
        self.usernameTextField.textColor = UIColor.black
        
        self.passwordTextField.backgroundColor = UIColor.init(red: 246/255, green: 246/255, blue: 246/255, alpha: 1)
        self.passwordTextField.textColor = UIColor.black
        
        self.loginButtonDescriptionLabel.text = "Entrar"
        self.loginButtonDescriptionLabel.font = UIFont.boldSystemFont(ofSize: 18)
        self.loginButtonDescriptionLabel.textColor = UIColor.init(red: 84/255, green: 128/255, blue: 136/255, alpha: 1)
        self.loginButtonDescriptionLabel.textColor = UIColor.black
        
        self.userLabel.text = "Usuario"
        self.userLabel.textColor = UIColor.black
        self.userLabel.font = UIFont.boldSystemFont(ofSize: 12)
        
        self.passwordLabel.text = "Password"
        self.passwordLabel.textColor = UIColor.black
        self.passwordLabel.font = UIFont.boldSystemFont(ofSize: 12)
    }
    
    private func showAlert(message: String) {
        let alert = UIAlertController(title: "", message: message, preferredStyle: .alert)
        let okAction = UIAlertAction(title: "OK", style: .default, handler: nil)
        alert.addAction(okAction)
        self.present(alert, animated: true, completion: nil)
    }
    
    
    @objc func keyBoardActions(notification: Notification) {
        if (notification.name == UIResponder.keyboardWillShowNotification) {
            self.view.frame.origin.y = -100
        } else {
            self.view.frame.origin.y = 0
        }
    }
    
    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        self.view.endEditing(true)
    }
    
}

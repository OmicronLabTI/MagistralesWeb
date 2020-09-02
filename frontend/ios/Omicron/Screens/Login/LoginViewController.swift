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
import Resolver

class LoginViewController: UIViewController {
    
    // MARK: - OUTLETS
    @IBOutlet weak var usernameTextField : UITextField!
    @IBOutlet weak var passwordTextField : UITextField!
    @IBOutlet weak var loginButton : UIButton!
    @IBOutlet weak var logoView: UIView!
    @IBOutlet weak var loginView: UIView!
    @IBOutlet weak var loginDescriptionLabel: UILabel!
    @IBOutlet weak var loginButtonDescriptionLabel: UILabel!
    @IBOutlet weak var userLabel: UILabel!
    @IBOutlet weak var passwordLabel: UILabel!
    let button = UIButton(type: .custom)
    
    // MARK: - VARIABLES
    @Injected var viewModel: LoginViewModel
    @Injected var lottieManager: LottieManager
    let disposeBag = DisposeBag()
    
    // MARK: - LIFE CYCLES
    override func viewDidLoad() {
        super.viewDidLoad()
        viewModelBinding()
        initComponents()
        setupKeyboard()
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
                    self?.lottieManager.showLoading()
                } else {
                    self?.lottieManager.hideLoading()
                }
            }).disposed(by: disposeBag)
        
        viewModel.error
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] (error) in
                self?.showError(error: error)
            })
            .disposed(by: disposeBag)
        
        viewModel.finishedLogin
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: {
                let splitVC = self.getViewController(storyBoardName: ViewControllerIdentifiers.storieboardName, viewControllerName: ViewControllerIdentifiers.splitViewController) as! UISplitViewController
                self.view.addSubview(splitVC.view)
                self.view.bounds = splitVC.view.bounds
                self.addChild(splitVC)
            })
            .disposed(by: disposeBag)
    }
    
    func getViewController(storyBoardName: String, viewControllerName: String) -> UIViewController{
        let storyBoard = UIStoryboard(name: storyBoardName, bundle: nil)
        return storyBoard.instantiateViewController(identifier: viewControllerName)
    }
    
    func showSuccess(message: String) {
        AlertManager.shared.showAlert(message: message, view: self)
    }
    
    func showError(error: String) {
        AlertManager.shared.showAlert(message: error, view: self)
    }
    
    func initComponents() {
        
        self.view.backgroundColor =  OmicronColors.blue

        self.logoView.backgroundColor = OmicronColors.blue

        self.loginView.backgroundColor = UIColor.white
        self.loginView.layer.cornerRadius = 30
                
        self.loginDescriptionLabel.text = CommonStrings.logIntoYourAccount
        self.loginDescriptionLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 24)
        self.loginDescriptionLabel.textColor = UIColor.black
        
        self.usernameTextField.backgroundColor = OmicronColors.ligthGray
        self.usernameTextField.textColor = UIColor.black
        
        self.passwordTextField.backgroundColor = OmicronColors.ligthGray
        self.passwordTextField.textColor = UIColor.black
        
        self.loginButtonDescriptionLabel.text = CommonStrings.enter
        self.loginButtonDescriptionLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 18)
        self.loginButtonDescriptionLabel.textColor = OmicronColors.blue
        self.loginButtonDescriptionLabel.textColor = UIColor.black
        
        self.userLabel.text = CommonStrings.user
        self.userLabel.textColor = UIColor.black
        self.userLabel.font = UIFont(name: FontsNames.SFProDisplayRegular, size: 12)
        
        self.passwordLabel.text = CommonStrings.password
        self.passwordLabel.textColor = UIColor.black
        self.passwordLabel.font = UIFont(name: FontsNames.SFProDisplayRegular, size: 12)
        
        self.passwordTextField.rightViewMode = .unlessEditing
        self.button.setImage(UIImage(named: ImagesNames.openEye), for: .normal)
        self.button.imageEdgeInsets = UIEdgeInsets(top: 0, left: -24, bottom: 0, right: 10)
        self.button.frame = CGRect(x: CGFloat(passwordTextField.frame.size.width - 25), y: 5, width: 15, height: 25)
        self.button.addTarget(self, action: #selector(self.btnPasswordVisibilityClicked), for: .touchUpInside)
        self.passwordTextField.rightView = button
        self.passwordTextField.rightViewMode = .always
        
         button.setImage(UIImage(named: ImagesNames.closeEye), for: .normal)
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
    
    func setupKeyboard() -> Void {
        self.navigationController?.isNavigationBarHidden = true
        NotificationCenter.default.addObserver(self, selector: #selector(keyBoardActions(notification:)), name: UIResponder.keyboardWillShowNotification, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(keyBoardActions(notification:)), name: UIResponder.keyboardDidHideNotification, object: nil)
        NotificationCenter.default.addObserver(self, selector: #selector(keyBoardActions(notification:)), name: UIResponder.keyboardWillChangeFrameNotification, object: nil)
    }
    
    @IBAction func btnPasswordVisibilityClicked( sender: Any) {
        (sender as! UIButton).isSelected = !(sender as! UIButton).isSelected
        if (sender as! UIButton).isSelected {
            passwordTextField.isSecureTextEntry = false
            button.setImage(UIImage(named: ImagesNames.openEye), for: .normal)
        } else {
            passwordTextField.isSecureTextEntry = true
            button.setImage(UIImage(named: ImagesNames.closeEye), for: .normal)
        }
    }
}

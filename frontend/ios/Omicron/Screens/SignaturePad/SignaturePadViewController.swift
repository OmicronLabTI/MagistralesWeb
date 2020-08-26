//
//  SignaturePadViewController.swift
//  Omicron
//
//  Created by Axity on 26/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import SignaturePad
import Resolver
import RxCocoa
import RxSwift

class SignaturePadViewController: UIViewController {

    // MARK: -Outlets
    @IBOutlet weak var signatureTitleLabel: UILabel!
    @IBOutlet weak var clearButton: UIButton!
    @IBOutlet weak var signaturePadView: SignaturePad!
    @IBOutlet weak var cancelButton: UIButton!
    @IBOutlet weak var acceptButton: UIButton!
    
    @IBOutlet weak var signatureTitleView: UIView!
    @IBOutlet weak var buttonsView: UIView!
    
    
    // MARK: -Variables
    @Injected var signaturePadViewModel: SignaturePadViewModel
    let diposeBag = DisposeBag()
    var orderId: Int = -1
    
    // MARK: -Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view.
        self.initComponents()
        self.viewModelBinding()
    }
    
    
    // MARK: Functions
    
    @IBAction func cancelActionButton(_ sender: Any) {
        self.dismiss(animated: true)
    }
    
    @IBAction func clearActionButton(_ sender: Any) {
        self.signaturePadView.clear()
         self.signaturePadViewModel.signatureIsDone.onNext(false)
    }
    
    func viewModelBinding() {
        self.acceptButton.rx.tap.bind(to: signaturePadViewModel.acceptDidTap).disposed(by: self.diposeBag)
        self.signaturePadViewModel.canGetSignature.drive(self.acceptButton.rx.isEnabled).disposed(by: self.diposeBag)
        
        // Muestra u oculta el loading
        self.signaturePadViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { showLoading in
            if(showLoading) {
                LottieManager.shared.showLoading()
                return
            }
            LottieManager.shared.hideLoading()
        }).disposed(by: self.diposeBag)
        
        // Muestra un Alert
        self.signaturePadViewModel.showAlertMessage.observeOn(MainScheduler.instance).subscribe(onNext: { message in
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.diposeBag)
        
        // Si el cambio del status a finalización fue éxitosa se regresa a Inbox
        self.signaturePadViewModel.backToInboxVC.observeOn(MainScheduler.instance).subscribe(onNext: { _ in
            self.navigationController?.popToRootViewController(animated: true)
        }).disposed(by: self.diposeBag)
    }
    
    func initComponents() -> Void {
        self.signaturePadView.delegate = self
        
        self.signatureTitleView.backgroundColor = OmicronColors.comments
        self.buttonsView.backgroundColor = OmicronColors.comments
        
        self.signatureTitleLabel.text = "Firma del QFB"
        self.signatureTitleLabel.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 15)
        
        self.clearButton.setTitle("Limpiar", for: .normal)
        self.clearButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 15)
        
        self.cancelButton.setTitle("Cancelar", for: .normal)
        self.cancelButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 15)
        self.cancelButton.setTitleColor(.systemRed, for: .normal)
        
        self.acceptButton.setTitle("Aceptar", for: .normal)
        self.acceptButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 15)
    }
}

extension SignaturePadViewController: SignaturePadDelegate {
    func didStart() {
            self.signaturePadViewModel.signatureIsDone.onNext(true)
    }
    
    func didFinish() {
        if let signature = self.signaturePadView.getSignature() {
            if let signatureBase64 = signature.toBase64() {
                self.signaturePadViewModel.getOrder.onNext(self.orderId)
                self.signaturePadViewModel.getSignature.onNext(signatureBase64)
            }
        }
    }
}

extension UIImage {
    func toBase64() -> String? {
        guard let imageData = self.pngData() else { return nil }
        return imageData.base64EncodedString(options: Data.Base64EncodingOptions.lineLength64Characters)
    }
}

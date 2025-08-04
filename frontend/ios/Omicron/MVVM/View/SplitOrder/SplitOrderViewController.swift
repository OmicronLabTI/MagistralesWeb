//
//  SplitOrderViewController.swift
//  Omicron
//
//  Created by Josue Castillo on 28/07/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import UIKit
import Resolver
import RxSwift

protocol AcceptButtonPressed: AnyObject {
    func splitOrderAcceptButton(request: SplitOrderRequest)
}

class SplitOrderViewController: UIViewController, UITextFieldDelegate {
    @IBOutlet weak var piecesQuantityTextField: UITextField!
    @IBOutlet weak var messageLabel: UILabel!
    @IBOutlet weak var quantityLabel: UILabel!
    @IBOutlet weak var acceptButton: UIButton!
    @IBOutlet weak var cancelButton: UIButton!
    @IBOutlet weak var errorLabel: UILabel!
    
    @Injected var splitOrderViewModel: SplitOrderViewModel
    
    var availableQuantity: Int = 0
    var fabOrderId: Int = 0
    var isReadytoSplit: Bool = false
    var dxpTransactionId: String?
    var orderId: Int?
    var delegate: AcceptButtonPressed?
    var disposeBag: DisposeBag = DisposeBag()
    var totalPieces: Int = 0
    var showOnGoingProcessMessage: ((Bool) -> Void)?
    var orderType: String = String()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        initComponents()
        
        
    }
    
    func initComponents(){
        splitOrderViewModel.closeModal.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] mssg in
            let alert = UIAlertController(title: mssg, message: nil, preferredStyle: .alert)
            let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default, handler: { [weak self] _ in
                guard let self = self else { return }
                closeModal(true)
            })
            alert.addAction(okAction)
            self?.present(alert, animated: true, completion: nil)
        }).disposed(by: disposeBag)
        
        configButtons()
        configTextField()
        quantityLabel.text = "\(availableQuantity)"
    }
    
    func configButtons(){
        acceptButton.isEnabled = isReadytoSplit
        acceptButton.isOpaque = !isReadytoSplit
        acceptButton.layer.cornerRadius = 8
        
        cancelButton.layer.borderWidth = 1
        cancelButton.layer.cornerRadius = 8
        cancelButton.layer.borderColor = OmicronColors.blue.cgColor
    }
    
    func configTextField(){
        piecesQuantityTextField.delegate = self
        piecesQuantityTextField.keyboardType = .numberPad
        piecesQuantityTextField.addTarget(self, action: #selector(textFieldDidChange), for: .editingChanged)
        
        let stepperView = UIStackView()
        stepperView.axis = .vertical
        stepperView.distribution = .fillEqually
        stepperView.spacing = 0
        stepperView.frame = CGRect(x: 0, y: 0, width: 0, height: 0) // ancho visible al lado derecho

        let incrementButton = UIButton(type: .system)
        incrementButton.setTitle("▲", for: .normal)
        incrementButton.addTarget(self, action: #selector(incrementValue), for: .touchUpInside)

        let decrementButton = UIButton(type: .system)
        decrementButton.setTitle("▼", for: .normal)
        decrementButton.addTarget(self, action: #selector(decrementValue), for: .touchUpInside)

        stepperView.addArrangedSubview(incrementButton)
        stepperView.addArrangedSubview(decrementButton)
        
        piecesQuantityTextField.rightView = stepperView
        piecesQuantityTextField.rightViewMode = .always
    }

    func validateInput(_ text: String?) {
        var errors: [String] = []

        guard let text = text, !text.isEmpty else {
            errors.append("El campo es obligatorio.")
            showErrors(errors)
            return
        }

        guard let number = Int(text) else {
            errors.append("Debe ingresar un número válido.")
            showErrors(errors)
            return
        }

        if number < 1 {
            errors.append("El número debe ser mayor o igual a 1.")
        }

        if number > availableQuantity {
            errors.append("Las piezas asignadas superan las disponibles.")
        }
        
        if number == availableQuantity && orderType == OrderRelationTypes.completa {
            errors.append("El número de piezas seleccionadas es igual al total de la orden. No es posible dividir la orden.")
        }
        validateIsReadytoAplit(errors)
    }
    
    func validateIsReadytoAplit(_ errors: [String]) {
        isReadytoSplit = errors.isEmpty
        acceptButton.isEnabled = isReadytoSplit
        if isReadytoSplit {
            hideError()
        } else {
            showErrors(errors)
        }
    }

    func showErrors(_ messages: [String]) {
        errorLabel.text = messages.joined(separator: "\n")
        errorLabel.isHidden = false
    }

    func hideError() {
        errorLabel.text = ""
        errorLabel.isHidden = true
    }

    
    func textField(_ textField: UITextField, shouldChangeCharactersIn range: NSRange, replacementString string: String) -> Bool {
        let allowedCharacters = CharacterSet.decimalDigits
        let characterSet = CharacterSet(charactersIn: string)
        return allowedCharacters.isSuperset(of: characterSet)
    }
    
    @objc func incrementValue() {
        let current = Int(piecesQuantityTextField.text ?? "") ?? 0
        piecesQuantityTextField.text = "\(current + 1)"
        validateInput(piecesQuantityTextField.text)
    }

    @objc func decrementValue() {
        let current = Int(piecesQuantityTextField.text ?? "") ?? 0
        if current > 0 {
            piecesQuantityTextField.text = "\(current - 1)"
            validateInput(piecesQuantityTextField.text)
        }
    }
    
    func closeModal(_ disable: Bool) {
        showOnGoingProcessMessage?(disable)
        self.dismiss(animated: true)
    }
    
    @IBAction func acceptAction(_ sender: Any) {
        let piecesToSend = Int(piecesQuantityTextField.text ?? "") ?? 0
        let dataToSend = SplitOrderRequest(productionOrderId: fabOrderId, pieces: piecesToSend,
                                           userId: Persistence.shared.getUserData()?.id ?? String(),
                                           dxpOrder: dxpTransactionId, sapOrder: orderId, totalPieces: availableQuantity)
        delegate?.splitOrderAcceptButton(request: dataToSend)
    }
    
    @IBAction func cancelAction(_ sender: Any) {
        closeModal(false)
    }
    
    @objc func textFieldDidChange(_ textField: UITextField) {
        validateInput(piecesQuantityTextField.text)
    }
    
}

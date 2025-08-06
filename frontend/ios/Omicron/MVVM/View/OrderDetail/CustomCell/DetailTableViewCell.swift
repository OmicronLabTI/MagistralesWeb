//
//  DetailTableViewCell.swift
//  Omicron
//
//  Created by Axity on 10/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

protocol SelectedPickerInput: AnyObject {
    func okAction(selectedOption: String, productId: String)
    func emptyOptions()
    func hassBatches()
}


class DetailTableViewCell: UITableViewCell, UIPickerViewDelegate, UIPickerViewDataSource, UITextFieldDelegate {
    // MARK: Outlets
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    @IBOutlet weak var baseQuantityLabel: UILabel!
    @IBOutlet weak var requiredQuantityLabel: UILabel!
    @IBOutlet weak var unitLabel: UILabel!
    @IBOutlet weak var hashTagLabel: UILabel!
    @IBOutlet weak var pickerContainerView: UIView!
    var textColor: UIColor = .black
    
    private var textField = UITextField()
    private var pickerView = UIPickerView()
    var options: [String] = []
    var productId = String()
    var delegate: SelectedPickerInput?
    var enableAction = true
    var hasBatches = false
    var selectedOption: String = "" {
            didSet {
                validateShowDropDown()
            }
    }

    override func awakeFromNib() {
        super.awakeFromNib()
        let fontSize = CGFloat(17)
        UtilsManager.shared.labelsStyle(label: self.codeLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.baseQuantityLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.requiredQuantityLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.unitLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.descriptionLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.hashTagLabel, text: "", fontSize: fontSize)
        configuraTextField()
        validateShowDropDown()
    }
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)
        let textColor = selected ? .white : self.textColor
        self.updateTextColor(color: textColor)
        // Configure the view for the selected state
    }
    func setEmptyStock(_ hasStock: Bool) {
        self.textColor = hasStock ? .black : .systemOrange
        self.updateTextColor(color: self.textColor)
    }

    func updateTextColor(color: UIColor) {
        codeLabel.textColor = color
        descriptionLabel.textColor = color
        baseQuantityLabel.textColor = color
        requiredQuantityLabel.textColor = color
        unitLabel.textColor = color
        hashTagLabel.textColor = color
    }
    
    func validateShowDropDown() {
        pickerContainerView.isHidden = false
        setupPicker()
        setupTextField()
    }
    
    private func setupTextField() {
        if (options.count != 0) {
            textField.inputView = pickerView
            
            addDoneButtonToPicker()
            if let index = options.firstIndex(of: selectedOption) {
                 textField.text = selectedOption
                 pickerView.selectRow(index, inComponent: 0, animated: false)
             } else {
                 // Por si el valor no está en las opciones (fallback opcional)
                 textField.text = selectedOption
                 pickerView.selectRow(0, inComponent: 0, animated: false)
             }
        } else {
            textField.text = selectedOption
        }
    }
    
    private func configuraTextField()
    {
        textField.translatesAutoresizingMaskIntoConstraints = false
        textField.borderStyle = .roundedRect
        textField.placeholder = "Selecciona un lote"

        textField.textAlignment = .center
        textField.delegate = self
        textField.font = .fontDefaultMedium(14)
        pickerContainerView.addSubview(textField)
        // Ajustar textField al tamaño de dropdownView
        NSLayoutConstraint.activate([
            textField.topAnchor.constraint(equalTo: pickerContainerView.topAnchor),
            textField.bottomAnchor.constraint(equalTo: pickerContainerView.bottomAnchor),
            textField.leadingAnchor.constraint(equalTo: pickerContainerView.leadingAnchor),
            textField.trailingAnchor.constraint(equalTo: pickerContainerView.trailingAnchor),
        ])
    }
    
    private func addDoneButtonToPicker() {
        let toolbar = UIToolbar()
        toolbar.sizeToFit()
        let flexibleSpace = UIBarButtonItem(barButtonSystemItem: .flexibleSpace, target: nil, action: nil)
        let doneButton = UIBarButtonItem(title: "Done", style: .done, target: self, action: #selector(donePicker))
        toolbar.setItems([flexibleSpace, doneButton], animated: false)
        textField.inputAccessoryView = toolbar
    }

    @objc private func donePicker() {
        if (options.count > 0) {
            let selectedRow = pickerView.selectedRow(inComponent: 0)
            let selectedValue = options[selectedRow]
            textField.text = selectedValue
            delegate?.okAction(selectedOption: selectedValue, productId: self.productId)
            textField.resignFirstResponder()
        }
    }
    
    private func setupPicker() {
        pickerView.delegate = self
        pickerView.dataSource = self
    }
    
    func numberOfComponents(in pickerView: UIPickerView) -> Int {
         return 1
     }

     func pickerView(_ pickerView: UIPickerView, numberOfRowsInComponent component: Int) -> Int {
         return options.count
     }

     func pickerView(_ pickerView: UIPickerView, titleForRow row: Int, forComponent component: Int) -> String? {
         return options[row]
     }

     func pickerView(_ pickerView: UIPickerView, didSelectRow row: Int, inComponent component: Int) {
     }
    
    func textFieldDidBeginEditing(_ textField: UITextField) {
    }

    func textFieldDidEndEditing(_ textField: UITextField) {
    }
    
    func textFieldShouldBeginEditing(_ textField: UITextField) -> Bool {
        if (!enableAction) {
            return false
        }
        
        if (hasBatches) {
            delegate?.hassBatches()
            return false
        }

        if (options.isEmpty) {
            delegate?.emptyOptions()
            return false
        }

        return true
    }
}

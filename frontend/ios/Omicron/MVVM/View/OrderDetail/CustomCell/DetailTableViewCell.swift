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
}

class DetailTableViewCell: UITableViewCell, UIPickerViewDelegate, UIPickerViewDataSource, UITextFieldDelegate {
    // MARK: Outlets
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    @IBOutlet weak var baseQuantityLabel: UILabel!
    @IBOutlet weak var requiredQuantityLabel: UILabel!
    @IBOutlet weak var unitLabel: UILabel!
    @IBOutlet weak var werehouseLabel: UILabel!
    @IBOutlet weak var hashTagLabel: UILabel!
    @IBOutlet weak var pickerContainerView: UIView!
    var textColor: UIColor = .black
    
    private var textField = UITextField()
    private var pickerView = UIPickerView()
    var options: [String] = []
    var productId = String()
    var delegate: SelectedPickerInput?
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
        UtilsManager.shared.labelsStyle(label: self.werehouseLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.descriptionLabel, text: "", fontSize: fontSize)
        UtilsManager.shared.labelsStyle(label: self.hashTagLabel, text: "", fontSize: fontSize)
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
        werehouseLabel.textColor = color
        hashTagLabel.textColor = color
    }
    
    func validateShowDropDown() {
        if (selectedOption.isEmpty) {
            werehouseLabel.isHidden = false
        } else {
            UtilsManager.shared.labelsStyle(label: self.baseQuantityLabel, text: "", fontSize: 14)
            UtilsManager.shared.labelsStyle(label: self.unitLabel, text: "", fontSize: 14)
            pickerContainerView.isHidden = false
            werehouseLabel.isHidden = true
            setupPicker()
            setupTextField()
        }
    }
    
    private func setupTextField() {
        textField.translatesAutoresizingMaskIntoConstraints = false
        textField.borderStyle = .roundedRect
        textField.placeholder = "Selecciona un lote"
        textField.inputView = pickerView
        textField.textAlignment = .center
        textField.delegate = self
        
        pickerContainerView.addSubview(textField)
        
        // Ajustar textField al tamaño de dropdownView
        NSLayoutConstraint.activate([
            textField.topAnchor.constraint(equalTo: pickerContainerView.topAnchor),
            textField.bottomAnchor.constraint(equalTo: pickerContainerView.bottomAnchor),
            textField.leadingAnchor.constraint(equalTo: pickerContainerView.leadingAnchor),
            textField.trailingAnchor.constraint(equalTo: pickerContainerView.trailingAnchor),
        ])
        
        textField.font = .fontDefaultMedium(14)
        addDoneButtonToPicker()
        
        if let index = options.firstIndex(of: selectedOption) {
             textField.text = selectedOption
             pickerView.selectRow(index, inComponent: 0, animated: false)
         } else {
             // Por si el valor no está en las opciones (fallback opcional)
             textField.text = options.first
             pickerView.selectRow(0, inComponent: 0, animated: false)
         }
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
        let selectedRow = pickerView.selectedRow(inComponent: 0)
        let selectedValue = options[selectedRow]
        textField.text = selectedValue
        delegate?.okAction(selectedOption: selectedValue, productId: self.productId)
        textField.resignFirstResponder()
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
        print("El textField tiene el foco")
    }

    func textFieldDidEndEditing(_ textField: UITextField) {
    }
}

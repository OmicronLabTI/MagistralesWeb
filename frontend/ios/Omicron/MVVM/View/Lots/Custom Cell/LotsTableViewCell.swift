//
//  LotsTableViewCell.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit

protocol SelectedDropDownOptionsDelegate: AnyObject {
    func okAction(selectedOption: String, productId: String)
}

class LotsTableViewCell: UITableViewCell, UIPickerViewDelegate, UIPickerViewDataSource {
    // MARK: - OUTLETS
    @IBOutlet weak var numberLabel: UILabel!
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    @IBOutlet weak var warehouseCodeLabel: UILabel!
    @IBOutlet weak var totalNeededLabel: UILabel!
    @IBOutlet weak var totalSelectedLabel: UILabel!
    
    @IBOutlet weak var baseQuantityLabel: UILabel!
    @IBOutlet weak var unitLabel: UILabel!
    @IBOutlet weak var dropdownView: UIView!

    private let textField = UITextField()
    private let pickerView = UIPickerView()
    var options: [String] = []
    var productId = String()
    var delegate: SelectedDropDownOptionsDelegate?
    var selectedOption: String = "" {
            didSet {
                validateShowDropDown()
            }
    }

    var row: Int?
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        UtilsManager.shared.labelsStyle(label: self.numberLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.codeLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.descriptionLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.warehouseCodeLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.totalNeededLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.totalSelectedLabel, text: "", fontSize: 14)
        validateShowDropDown()
    }
    
    func validateShowDropDown() {
        if (selectedOption.isEmpty) {
            warehouseCodeLabel.isHidden = false
        } else {
            UtilsManager.shared.labelsStyle(label: self.baseQuantityLabel, text: "", fontSize: 14)
            UtilsManager.shared.labelsStyle(label: self.unitLabel, text: "", fontSize: 14)
            dropdownView.isHidden = false
            warehouseCodeLabel.isHidden = true
            setupPicker()
            setupTextField()
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


    // MARK: - Setup Methods
       private func setupTextField() {
           textField.translatesAutoresizingMaskIntoConstraints = false
           textField.borderStyle = .roundedRect
           textField.placeholder = "Selecciona un lote"
           textField.inputView = pickerView
           textField.textAlignment = .center
           
           dropdownView.addSubview(textField)
           
           // Ajustar textField al tamaño de dropdownView
           NSLayoutConstraint.activate([
               textField.topAnchor.constraint(equalTo: dropdownView.topAnchor),
               textField.bottomAnchor.constraint(equalTo: dropdownView.bottomAnchor),
               textField.leadingAnchor.constraint(equalTo: dropdownView.leadingAnchor),
               textField.trailingAnchor.constraint(equalTo: dropdownView.trailingAnchor),
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

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)
        // Configure the view for the selected state
    }
    func setEmptyStock(_ hasStock: Bool) {
        numberLabel.textColor = hasStock ? .black : .systemOrange
        codeLabel.textColor = hasStock ? .black : .systemOrange
        descriptionLabel.textColor = hasStock ? .black : .systemOrange
        warehouseCodeLabel.textColor = hasStock ? .black : .systemOrange
        totalNeededLabel.textColor = hasStock ? .black : .systemOrange
        totalSelectedLabel.textColor = hasStock ? .black : .systemOrange
    }
}

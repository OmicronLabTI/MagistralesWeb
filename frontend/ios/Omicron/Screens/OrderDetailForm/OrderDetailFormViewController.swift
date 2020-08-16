//
//  OrderDetailFormViewController.swift
//  Omicron
//
//  Created by Axity on 15/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import Eureka

class OrderDetailFormViewController:  FormViewController {
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        
        LabelRow.defaultCellUpdate = { cell, row in
            cell.contentView.backgroundColor = .red
            cell.textLabel?.textColor = .white
            cell.textLabel?.font = UIFont.boldSystemFont(ofSize: 13)
            cell.textLabel?.textAlignment = .right
            
        }
        
        TextRow.defaultCellUpdate = { cell, row in
            if !row.isValid {
                cell.titleLabel?.textColor = .red
            }
        }
        
        form
            
            +++ Section(header: "More sophisticated validations UX using callbacks", footer: "")
            
            <<< IntRow() {
                $0.title = "Cantidad base: "
                // Validaciones
                let fieldNoEmpty = RuleClosure<Int> { rowValue in
                    return rowValue == nil ? ValidationError(msg: "El campo no puede ir vacio") : nil
                }
                
                let fieldShouldNotNegativeNumbers = RuleClosure<Int> { rowValue in
                    return rowValue?.signum() == -1 ? ValidationError(msg: "No debe contener números negativos") : nil
                }
                
                $0.add(rule: fieldNoEmpty)
                $0.add(rule: fieldShouldNotNegativeNumbers)
                $0.add(rule: RuleGreaterThan(min: 1, msg: "El campo debe tener un número mayor que 0", id: ""))
            }
                
            .cellUpdate { cell, row in
                if !row.isValid {
                    cell.titleLabel?.textColor = .red
                }
            }
            .onRowValidationChanged { cell, row in
                let rowIndex = row.indexPath!.row
                while row.section!.count > rowIndex + 1 && row.section?[rowIndex  + 1] is LabelRow {
                    row.section?.remove(at: rowIndex + 1)
                }
                if !row.isValid {
                    for (index, validationMsg) in row.validationErrors.map({ $0.msg }).enumerated() {
                        let labelRow = LabelRow() {
                            $0.title = validationMsg
                            $0.cell.height = { 30 }
                        }
                        let indexPath = row.indexPath!.row + index + 1
                        row.section?.insert(labelRow, at: indexPath)
                    }
                }
            }
            
            <<< IntRow() {
                $0.title = "Cantidad requerida: "
                
                // Validaciones
                let fieldNoEmpty = RuleClosure<Int> { rowValue in
                    return rowValue == nil ? ValidationError(msg: "El campo no puede ir vacio") : nil
                }
                
                let fieldShouldNotNegativeNumbers = RuleClosure<Int> { rowValue in
                    return rowValue?.signum() == -1 ? ValidationError(msg: "No debe contener números negativos") : nil
                }
                
                $0.add(rule: fieldNoEmpty)
                $0.add(rule: fieldShouldNotNegativeNumbers)
                $0.add(rule: RuleGreaterThan(min: 1, msg: "El campo debe tener un número mayor que 0", id: ""))
            }
            .cellUpdate { cell, row in
                if !row.isValid {
                    cell.titleLabel?.textColor = .red
                }
            }
            .onRowValidationChanged { cell, row in
                let rowIndex = row.indexPath!.row
                while row.section!.count > rowIndex + 1 && row.section?[rowIndex  + 1] is LabelRow {
                    row.section?.remove(at: rowIndex + 1)
                }
                if !row.isValid {
                    for (index,  message) in row.validationErrors.map({ $0.msg }).enumerated() {
                        let labelRow = LabelRow() {
                            $0.title = message
                            $0.cell.height = { 30 }
                        }
                        let indexPath = row.indexPath!.row + index + 1
                        row.section?.insert(labelRow, at: indexPath)
                    }
                }
            }
            
            <<< PickerInlineRow<String>() {
                $0.title = "Almacen"
                $0.options = ["One","Two","Three"]
                $0.value = "One"    // initially selected
            }

            +++ Section()
            <<< ButtonRow() {
                $0.title = "Aceptar"
            }
            .onCellSelection { cell, row in
                row.section?.form?.validate()
                if (row.isValid) {
                    print("Holi")
                }
            }
            <<< ButtonRow() {
                $0.title = "Cancelar"
            }
            .onCellSelection { cell, row in
                row.section?.form?.validate()
  
        }
    }
}

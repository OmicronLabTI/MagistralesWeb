//
//  OrderDetailFormViewController.swift
//  Omicron
//
//  Created by Axity on 15/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import Eureka
import RxSwift
import  RxCocoa

class OrderDetailFormViewController:  FormViewController {
    
    // MARK: Variables
    var dataOfTable: OrderDetail? = nil
    var indexOfItemSelected: Int = -1
    let orderDetailFormViewModel = OrderDetailFormViewModel()
    var disposeBag = DisposeBag()
    
    // MARK: Life cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.buildForm()
        self.viewModelBinding()
    }
    
    // MARK: Functions
    func buildForm() -> Void {
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
            +++ Section(header: "Editar elemento", footer: "")
            
            <<< IntRow() {
                $0.title = "Cantidad base: "
                $0.tag = "baseQuantity"
                $0.value = self.dataOfTable?.details![self.indexOfItemSelected].baseQuantity!
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
                $0.value = self.dataOfTable?.details![self.indexOfItemSelected].requiredQuantity!
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
                $0.title = "Almacen: "
                $0.options = ["PZ", "MG"]
                $0.value = "PZ"
            }
            
            +++ Section()
            <<< ButtonRow() {
                $0.title = "Aceptar"
            }
            .onCellSelection { cell, row in
                row.section?.form?.validate()
                if (row.isValid) {
                    let baseQuantity: IntRow? = self.form.rowBy(tag: "baseQuantity")
                    let requiredQuantity: IntRow? = self.form.rowBy(tag: "requiredQuantity")
                    let werehouse: PickerInlineRow<String>? = self.form.rowBy(tag: "werehouse")
                    self.orderDetailFormViewModel.editItemTable(index: self.indexOfItemSelected, data: self.dataOfTable!, baseQuantity: baseQuantity!.value!, requiredQuantity: requiredQuantity!.value!, werehouse: (werehouse?.value)!)
                }
            }
            <<< ButtonRow() {
                $0.title = "Cancelar"
            }
            .onCellSelection { cell, row in
                self.navigationController?.popViewController(animated: true)
        }
    }
    
    func viewModelBinding () -> Void {
        // Muestra o oculta el loading
        orderDetailFormViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { showLoading in
            if(showLoading) {
                LottieManager.shared.showLoading()
                return
            }
            LottieManager.shared.hideLoading()
        }).disposed(by: self.disposeBag)
        
        orderDetailFormViewModel.showAlert.observeOn(MainScheduler.instance).subscribe(onNext: { message in
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.disposeBag)
    }
}

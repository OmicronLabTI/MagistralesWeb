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
import RxCocoa
import Resolver

class OrderDetailFormViewController:  FormViewController {
    
    // MARK: Variables
    @Injected var orderDetailViewModel: OrderDetailViewModel
    @Injected var orderDetailFormViewModel: OrderDetailFormViewModel
    @Injected var lottieManager: LottieManager
    weak var dataOfTable: OrderDetail? = nil
    var indexOfItemSelected: Int = -1
    var disposeBag = DisposeBag()
    weak var baseQuantity: TextRow? = nil
    weak var requiredQuantity: TextRow? = nil
    
    
//    var baseQuantityToSave: Decimal? = nil
//    var requiredQuantity:Decimal? = nil
    weak var werehouse: PickerInlineRow<String>? = nil
    let formatter = UtilsManager.shared.formatterDoublesTo6Decimals()
   
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
            cell.textLabel?.font = UIFont.boldSystemFont(ofSize: 9)
            cell.textLabel?.textAlignment = .left
            cell.textLabel?.numberOfLines = 2
        }

        TextRow.defaultCellUpdate = { cell, row in
            if !row.isValid {
                cell.titleLabel?.textColor = .red
            }
        }
        
        let fieldShouldNotNegativeNumbers = RuleClosure<String> { rowValue in
            let range = NSRange(location: 0, length: rowValue?.description.utf16.count ?? 0)
            let regex = try! NSRegularExpression(pattern: "^([0-9]+)?(\\.([0-9]{1,6})?)?$")
            return !(regex.firstMatch(in: rowValue?.description ?? "", options: [], range: range) != nil) ? ValidationError(msg: "No se permite números negativos, caracteres o más de 6 decimas") : nil
        }
        
        let fieldNoEmpty = RuleClosure<String> { rowValue in
        return (rowValue == nil || rowValue!.isEmpty) ? ValidationError(msg: "El campo no puede ir vacio") : nil
        }
        
        form
              
            +++ Section(header: self.dataOfTable!.details![self.indexOfItemSelected].detailDescription!, footer: "")
            
            <<< TextRow() {
                $0.title = "Cantidad base: "
                $0.tag = "baseQuantity"
                $0.value = self.dataOfTable!.details![self.indexOfItemSelected].unit == "Pieza" ? String(format: "%.0f", self.dataOfTable!.details![self.indexOfItemSelected].baseQuantity!) : self.formatter.string(from: NSNumber(value: self.dataOfTable!.details![self.indexOfItemSelected].baseQuantity!))
                $0.cellSetup{cell, row in
                    cell.textField.keyboardType = .numberPad
                }
                $0.onCellHighlightChanged{ cell, row in
                
                    if (row.value != nil && self.canOperation(rowValue: row.value ?? "f")) {
                        let requireQuantityField = self.form.rowBy(tag: "requiredQuantity") as? TextRow
                        let baseQuantity = Decimal(string: row.value ?? "0")
                        let requiredQuantity = Decimal(self.dataOfTable?.plannedQuantity ?? 0)
                        let result = baseQuantity! * requiredQuantity
                        requireQuantityField?.value = self.dataOfTable!.details![self.indexOfItemSelected].unit == "Pieza" ? String(format: "%.0f", NSDecimalNumber(decimal: result).doubleValue) : String(format: "%.6f", NSDecimalNumber(decimal: result).doubleValue)
                        requireQuantityField?.reload()
                    }
                }
                
                // Validaciones
                var rules = RuleSet<String>()
                rules.add(rule: fieldNoEmpty)
                rules.add(rule: fieldShouldNotNegativeNumbers)
                $0.add(ruleSet: rules)
                $0.validationOptions = .validatesOnChangeAfterBlurred
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
                            $0.cell.height = { 28 }
                        }
                        let indexPath = row.indexPath!.row + index + 1
                        row.section?.insert(labelRow, at: indexPath)
                    }
                } else {
                    row.cleanValidationErrors()
                }
            }
            
            <<< TextRow() {
                $0.title = "Cantidad requerida: "
                $0.value =  self.dataOfTable!.details![self.indexOfItemSelected].unit == "Pieza" ? String(format: "%.0f", self.dataOfTable!.details![self.indexOfItemSelected].requiredQuantity!) : self.formatter.string(from: NSNumber(value: self.dataOfTable!.details![self.indexOfItemSelected].requiredQuantity!))
                $0.tag = "requiredQuantity"
                $0.cellSetup{cell, row in
                    cell.textField.keyboardType = .numberPad
                }
                $0.onCellHighlightChanged{ cell, row in
                    if(!(row.value?.isEmpty ?? true) && !(row.value == "0") && self.canOperation(rowValue: row.value ?? "d")) {
                        let requiredQuantity = Decimal(string: row.value ?? "0")
                        let baseQuantity = Decimal((self.dataOfTable?.plannedQuantity)!)
                        let result = requiredQuantity!  / baseQuantity
                        let baseQuantityField = self.form.rowBy(tag: "baseQuantity") as? TextRow
                        baseQuantityField?.value = self.dataOfTable!.details![self.indexOfItemSelected].unit == "Pieza" ? String(format: "%.0f", NSDecimalNumber(decimal: result).doubleValue) : String(format: "%.6f", NSDecimalNumber(decimal: result).doubleValue)
                        baseQuantityField?.reload()
                    }
                }
                
                
                // Validaciones
                var rules = RuleSet<String>()
                rules.add(rule: fieldNoEmpty)
                rules.add(rule: fieldShouldNotNegativeNumbers)
                $0.add(ruleSet: rules)
                $0.validationOptions = .validatesOnChangeAfterBlurred
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
                            $0.cell.height = { 28 }
                        }
                        let indexPath = row.indexPath!.row + index + 1
                        row.section?.insert(labelRow, at: indexPath)
                    }
                } else {
                    row.cleanValidationErrors()
                }
            }
            
            <<< PickerInlineRow<String>() {
                $0.title = "Almacén: "
                $0.tag = "werehouse"
                $0.options = ["AMP", "BE", "GENERAL", "INCI", "MER", "MG", "MN", "MP", "PROD", "PRONATUR", "PT", "TALLERES", "WEB"]
                $0.value = self.dataOfTable?.details![self.indexOfItemSelected].warehouse!
            }
            
            +++ Section()
            <<< ButtonRow() {
                $0.title = "Aceptar"
                $0.disabled = Condition.function(
                    form.allRows.compactMap { $0.tag },
                { !$0.validate().isEmpty })
            }
            .onCellSelection { cell, row in
                row.section?.form?.validate()
                if (row.isValid && !row.isDisabled) {
                    
                    self.baseQuantity = self.form.rowBy(tag: "baseQuantity")
                    self.requiredQuantity = self.form.rowBy(tag: "requiredQuantity")
                    self.werehouse = self.form.rowBy(tag: "werehouse")
                    
                    let alert = UIAlertController(title: CommonStrings.Emty, message: "¿Deseas guardar los cambios ingresados?", preferredStyle: .alert)
                    let cancelAction = UIAlertAction(title: "Cancelar", style: .cancel, handler: {[weak self] _ in self?.dismiss(animated: true)})
                    let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler:  {[weak self] res in self?.saveChanges()})
                    alert.addAction(cancelAction)
                    alert.addAction(okAction)
                    self.present(alert, animated: true, completion: nil)
                }
            }
            <<< ButtonRow() {
                $0.title = "Cancelar"
            }
            .cellSetup{cell, row in
                cell.tintColor = .red
            }
            .onCellSelection { cell, row in
                self.dismiss(animated: true)
        }
    }
    
    func saveChanges () {
//        self.baseQuantity.
        
        if(self.canOperation(rowValue: self.baseQuantity?.value ?? "f") && self.canOperation(rowValue: (self.requiredQuantity?.value) ?? "r")) {
            let requiredQuantityValue = Double( self.requiredQuantity?.value ?? "0.0")
            let baseQuantityValue = Double(self.baseQuantity?.value ?? "0.0")
            self.orderDetailFormViewModel.editItemTable(index: self.indexOfItemSelected, data: self.dataOfTable!, baseQuantity: baseQuantityValue ?? 0.0, requiredQuantity: requiredQuantityValue ?? 0.0, werehouse: (werehouse?.value)!)
        }
    }
    
    func viewModelBinding () -> Void {
        
        // Muestra o oculta el loading
        orderDetailFormViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { showLoading in
            if(showLoading) {
                self.lottieManager.showLoading()
                return
            }
            self.lottieManager.hideLoading()
        }).disposed(by: self.disposeBag)
        
        orderDetailFormViewModel.showAlert.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] message in
            
            let alert = UIAlertController(title: CommonStrings.Emty, message: message, preferredStyle: .alert)
            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler: {_ in self?.refreshOrderDetail()})
            alert.addAction(okAction)
            self?.present(alert, animated: true, completion: nil)
            
        }).disposed(by: self.disposeBag)
        
        // Aqui es en donde se hace el manda a llamar el servicio para volver a traer los datos de detalle de la fórmnula
        orderDetailFormViewModel.success.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] orderId in
            self?.orderDetailViewModel.getOrdenDetail()
        }).disposed(by: self.disposeBag)
    }
    
    func refreshOrderDetail() {
        self.dismiss(animated: true)
    }
    
    func canOperation(rowValue: String) -> Bool {
        let range = NSRange(location: 0, length: rowValue.description.utf16.count)
        let regex = try! NSRegularExpression(pattern: "^([0-9]+)?(\\.([0-9]{1,6})?)?$")
        return regex.firstMatch(in: rowValue.description , options: [], range: range) != nil
    }
}

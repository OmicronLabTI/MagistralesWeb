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

    var dataOfTable: OrderDetail? = nil
    var indexOfItemSelected: Int = -1
    var disposeBag = DisposeBag()
    var baseQuantity: DecimalRow? = nil
    var requiredQuantity: DecimalRow? = nil
    var werehouse: PickerInlineRow<String>? = nil
    let formatter = UtilsManager.shared.formatterDoublesTo8Decimals()
   
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
        
        let fieldShouldNotNegativeNumbers = RuleClosure<Double> { rowValue in
            let range = NSRange(location: 0, length: rowValue?.description.utf16.count ?? 0)
            let regex = try! NSRegularExpression(pattern: "[-][0-9]{1,9}(?:.[0-9]{1,9})?$")
            return (regex.firstMatch(in: rowValue?.description ?? "", options: [], range: range) != nil) ? ValidationError(msg: "No debe contener números negativos") : nil
        }
        
        let fieldNoEmpty = RuleClosure<Double> { rowValue in
            return rowValue == nil ? ValidationError(msg: "El campo no puede ir vacio") : nil
        }
        
        form
              
            +++ Section(header: self.dataOfTable!.details![self.indexOfItemSelected].detailDescription!, footer: "")
            
            <<< DecimalRow() {
                $0.title = "Cantidad base: "
                $0.tag = "baseQuantity"
                $0.value = Double(self.dataOfTable!.details![self.indexOfItemSelected].baseQuantity!)
                $0.formatter = self.formatter
                $0.cellSetup{cell, row in
                    row.formatter = self.formatter
                }
                $0.onCellSelection{ cell, row in
                    row.formatter = self.formatter
                }
                $0.onCellHighlightChanged{ cell, row in
                    if (row.value != nil) {
                        let ss = self.form.rowBy(tag: "requiredQuantity") as? DecimalRow
                        let requiredQuantity  = Double((self.dataOfTable?.plannedQuantity)!) * row.value!
                        ss?.value = requiredQuantity
                        ss?.reload()
                    }
                }
                
                // Validaciones
                var rules = RuleSet<Double>()
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
                            $0.cell.height = { 30 }
                        }
                        let indexPath = row.indexPath!.row + index + 1
                        row.section?.insert(labelRow, at: indexPath)
                    }
                } else {
                    row.cleanValidationErrors()
                }
            }
            
            <<< DecimalRow() {
                $0.title = "Cantidad requerida: "
                $0.value = Double( self.dataOfTable!.details![self.indexOfItemSelected].requiredQuantity! )
                $0.tag = "requiredQuantity"
                $0.formatter = self.formatter
                $0.cellSetup{cell, row in
                    row.formatter = self.formatter
                }
                $0.onCellSelection{ cell, row in
                    row.formatter = self.formatter
                }
                $0.onCellHighlightChanged{ cell, row in
                                   if (row.value ??  1 > 0 &&  row.value != nil ) {
                                       let baseQuantity  =  row.value! / Double((self.dataOfTable?.plannedQuantity)!)
                                       let ss = self.form.rowBy(tag: "baseQuantity") as? DecimalRow
                                       ss?.value = baseQuantity
                                       ss?.reload()
                                   }
                               }
                // Validaciones
                var rules = RuleSet<Double>()
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
                if row.isValid {
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
                if (row.isValid) {
                    
                    self.baseQuantity = self.form.rowBy(tag: "baseQuantity")
                    self.requiredQuantity = self.form.rowBy(tag: "requiredQuantity")
                    self.werehouse = self.form.rowBy(tag: "werehouse")
                    
                    let alert = UIAlertController(title: CommonStrings.Emty, message: "Deseas guardar los cambios ingresados?", preferredStyle: .alert)
                    let cancelAction = UIAlertAction(title: "Cancelar", style: .cancel, handler: {_ in self.dismiss(animated: true)})
                    let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler:  {res in self.saveChanges()})
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
        self.orderDetailFormViewModel.editItemTable(index: self.indexOfItemSelected, data: self.dataOfTable!, baseQuantity: baseQuantity!.value!, requiredQuantity: requiredQuantity!.value!, werehouse: (werehouse?.value)!)
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
            
            let alert = UIAlertController(title: CommonStrings.Emty, message: message, preferredStyle: .alert)
            let okAction = UIAlertAction(title: CommonStrings.OK, style: .default, handler: {_ in self.refreshOrderDetail()})
            alert.addAction(okAction)
            self.present(alert, animated: true, completion: nil)
            
        }).disposed(by: self.disposeBag)
        
        // Aqui es en donde se hace el manda a llamar el servicio para volver a traer los datos de detalle de la fórmnula
        orderDetailFormViewModel.success.observeOn(MainScheduler.instance).subscribe(onNext: { orderId in
            self.orderDetailViewModel.getOrdenDetail()
        }).disposed(by: self.disposeBag)
    }
    
    func refreshOrderDetail() {
        self.dismiss(animated: true)
    }
}

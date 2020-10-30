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

class OrderDetailFormViewController: FormViewController {
    // MARK: Variables
    @Injected var orderDetailViewModel: OrderDetailViewModel
    @Injected var orderDetailFormViewModel: OrderDetailFormViewModel
    @Injected var lottieManager: LottieManager
    weak var dataOfTable: OrderDetail?
    var indexOfItemSelected: Int = -1
    var disposeBag = DisposeBag()
    weak var baseQuantity: TextRow?
    weak var requiredQuantity: TextRow?
    weak var werehouse: PickerInlineRow<String>?
    let formatter = UtilsManager.shared.formatterDoublesTo6Decimals()
    // MARK: Life cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.buildForm()
        self.viewModelBinding()
        self.isModalInPresentation = true
    }
    deinit {
        print("Se muere el ipad")
    }
    // MARK: Functions
    // swiftlint:disable function_body_length
    func buildForm() {
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
            let regex = try? NSRegularExpression(pattern: "^([0-9]+)?(\\.([0-9]{1,6})?)?$")
            return !(regex?.firstMatch(in: rowValue?.description ?? "", options: [], range: range) != nil) ?
                ValidationError(msg: "No se permite números negativos, caracteres o más de 6 decimas") : nil
        }
        let fieldNoEmpty = RuleClosure<String> { rowValue in
        return (rowValue == nil || rowValue!.isEmpty) ? ValidationError(msg: "El campo no puede ir vacio") : nil
        }
        form
            +++ Section(header: self.dataOfTable!.details![self.indexOfItemSelected].detailDescription!, footer: "")
            <<< TextRow { [weak self] in
                $0.title = "Cantidad base: "
                $0.tag = "baseQuantity"
                $0.value = self?.dataOfTable!.details![self?.indexOfItemSelected ?? 0].unit == "Pieza" ?
                    String(
                        format: "%.0f", self?.dataOfTable!.details![self?.indexOfItemSelected ?? 0].baseQuantity ?? 0)
                    : self?.formatter.string(
                        from: NSNumber(
                            value: self?.dataOfTable!.details![self?.indexOfItemSelected ?? 0].baseQuantity! ?? 0))
                $0.cellSetup { cell, _ in
                    cell.textField.keyboardType = .decimalPad
                }
                $0.onCellHighlightChanged { [weak self] _, row in
                    if row.value != nil && ((self?.canOperation(rowValue: row.value ?? "f")) != nil) {
                        let requireQuantityField = self?.form.rowBy(tag: "requiredQuantity") as? TextRow
                        let baseQuantity = Decimal(string: row.value ?? "0")
                        let requiredQuantity = self?.dataOfTable?.plannedQuantity ?? 0.0
                        let result = baseQuantity! * requiredQuantity
                        requireQuantityField?.value =
                            self?.dataOfTable!.details![self?.indexOfItemSelected ?? 0].unit == "Pieza" ?
                            String(format: "%.0f", NSDecimalNumber(decimal: result).doubleValue) :
                            String(format: "%.6f", NSDecimalNumber(decimal: result).doubleValue)
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
                        let labelRow = LabelRow {
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
            <<< TextRow { [weak self] in
                $0.title = "Cantidad requerida: "
                $0.value =  self?.dataOfTable!.details![self?.indexOfItemSelected ?? 0].unit == "Pieza" ?
                    String(format: "%.0f",
                           self?.dataOfTable!.details![self?.indexOfItemSelected ?? 0].requiredQuantity ?? 0) :
                    self?.formatter.string(from: NSNumber(
                        value: self?.dataOfTable!.details![self?.indexOfItemSelected ?? 0].requiredQuantity! ?? 0))
                $0.tag = "requiredQuantity"
                $0.cellSetup { cell, _ in
                    cell.textField.keyboardType = .decimalPad
                }
                $0.onCellHighlightChanged { [weak self] _, row in
                    if !(row.value?.isEmpty ?? true) && !(row.value == "0")
                        && ((self?.canOperation(rowValue: row.value ?? "d")) != nil) {
                        let requiredQuantity = Decimal(string: row.value ?? "0")
                        let baseQuantity = self?.dataOfTable?.plannedQuantity!
                        let result = requiredQuantity!  / baseQuantity!
                        let baseQuantityField = self?.form.rowBy(tag: "baseQuantity") as? TextRow
                        baseQuantityField?.value =
                            self?.dataOfTable!.details![self?.indexOfItemSelected ?? 0].unit == "Pieza" ?
                                String(format: "%.0f", NSDecimalNumber(decimal: result).doubleValue) :
                            String(format: "%.6f", NSDecimalNumber(decimal: result).doubleValue)
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
                        let labelRow = LabelRow {
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
            <<< TextRow { [weak self] in
                $0.title = "Unidad:"
                $0.value = self?.dataOfTable!.details![self?.indexOfItemSelected ?? 0].unit
                $0.disabled = true
            }.cellUpdate { cell, _ in
                cell.titleLabel?.textColor = .black
                cell.textField.textColor = .black
            }
            <<< PickerInlineRow<String> { [weak self] in
                $0.title = "Almacén: "
                $0.tag = "werehouse"
                $0.options = CommonStrings.options
                $0.value = self?.dataOfTable?.details![self?.indexOfItemSelected ?? 0].warehouse ?? ""
            }
            +++ Section()
            <<< ButtonRow { [weak self] in
                $0.title = "Aceptar"
                $0.disabled = Condition.function(
                    self?.form.allRows.compactMap { $0.tag } ?? [""], { !$0.validate().isEmpty })
            }
            .onCellSelection { [weak self] _, row in
                row.section?.form?.validate()
                if row.isValid && !row.isDisabled {
                    self?.baseQuantity = self?.form.rowBy(tag: "baseQuantity")
                    self?.requiredQuantity = self?.form.rowBy(tag: "requiredQuantity")
                    self?.werehouse = self?.form.rowBy(tag: "werehouse")
                    let alert = UIAlertController(title: "¿Deseas guardar los cambios ingresados?",
                                                  message: nil, preferredStyle: .alert)
                    let cancelAction = UIAlertAction(title: "Cancelar", style: .destructive,
                                                     handler: {[weak self] _ in
                                                        self?.navigationController?.popViewController(animated: true)})
                    let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default,
                                                 handler: {[weak self] _ in self?.saveChanges()})
                    alert.addAction(cancelAction)
                    alert.addAction(okAction)
                    self?.present(alert, animated: true, completion: nil)
                }
            }
            <<< ButtonRow {
                $0.title = "Cancelar"
            }
            .cellSetup { cell, _ in
                cell.tintColor = .red
            }
            .onCellSelection { [weak self] _, _ in
                self?.dismiss(animated: true)
        }
    }
    func saveChanges () {
        if self.canOperation(rowValue: self.baseQuantity?.value ?? "f")
            && self.canOperation(rowValue: (self.requiredQuantity?.value) ?? "r") {
            let requiredQuantityValue = Double( self.requiredQuantity?.value ?? "0.0")
            let baseQuantityValue = Double(self.baseQuantity?.value ?? "0.0")
            self.orderDetailFormViewModel.editItemTable(index: self.indexOfItemSelected,
                                                        data: self.dataOfTable!, baseQuantity: baseQuantityValue ?? 0.0,
                                                        requiredQuantity: requiredQuantityValue ?? 0.0,
                                                        werehouse: (werehouse?.value)!)
        }
    }
    func viewModelBinding () {
        // Muestra o oculta el loading
        orderDetailFormViewModel.loading.observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] showLoading in
            if showLoading {
                self?.lottieManager.showLoading()
                return
            }
            self?.lottieManager.hideLoading()
        }).disposed(by: self.disposeBag)
        orderDetailFormViewModel.showAlert.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] message in
            let alert = UIAlertController(title: message, message: nil, preferredStyle: .alert)
            let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default,
                                         handler: {[weak self] _ in self?.refreshOrderDetail()})
            alert.addAction(okAction)
            self?.present(alert, animated: true, completion: nil)
        }).disposed(by: self.disposeBag)
        // Aqui es en donde se hace el manda a llamar el servicio para volver a traer
        // los datos de detalle de la fórmnula
        orderDetailFormViewModel.success.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.orderDetailViewModel.getOrdenDetail()
        }).disposed(by: self.disposeBag)
    }
    func refreshOrderDetail() {
        self.dismiss(animated: true)
    }
    func canOperation(rowValue: String) -> Bool {
        let range = NSRange(location: 0, length: rowValue.description.utf16.count)
        let regex = try? NSRegularExpression(pattern: "^([0-9]+)?(\\.([0-9]{1,6})?)?$")
        return regex?.firstMatch(in: rowValue.description, options: [], range: range) != nil
    }
}

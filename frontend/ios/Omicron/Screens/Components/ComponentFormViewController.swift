//
//  ComponentFormViewController.swift
//  Omicron
//
//  Created by Diego Cárcamo on 10/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import Eureka
import RxSwift
import RxCocoa
import Resolver

class ComponentFormViewController: FormViewController {
    // MARK: Variables
    @Injected var componentsViewModel: ComponentsViewModel
    @Injected var lottieManager: LottieManager
    @Injected var inboxViewModel: InboxViewModel
    var disposeBag = DisposeBag()
    weak var baseQuantity: TextRow?
    weak var requiredQuantity: TextRow?
    weak var warehouse: PickerInlineRow<String>?
    let formatter = UtilsManager.shared.formatterDoublesTo6Decimals()
    deinit {
        print("deinit form")
    }
    // MARK: Life cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.viewModelBinding()
    }
    // MARK: Functions
    // swiftlint:disable function_body_length
    func buildForm(component: ComponentO, order: Order) {
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
        let shouldNotBeZero = RuleClosure<String> { rowValue in
            if let doubleNumber = Double(rowValue ?? "0"), rowValue != nil {
                return doubleNumber > 0 ? nil : ValidationError(msg: "Debe contener números mayores a 0")
            }
            return ValidationError(msg: "Debe contener números mayores a 0")
        }
        form
            +++ Section(header: component.description, footer: "")
            <<< TextRow { [weak self] in
                $0.title = "Cantidad base: "
                $0.tag = "baseQuantity"
                $0.value = ""
                $0.cellSetup { cell, _ in
                    cell.textField.keyboardType = .decimalPad
                }
                $0.onCellHighlightChanged { [weak self] _, row in
                    if row.value != nil && self?.canOperation(rowValue: row.value ?? "f") ?? false
                        && !row.value!.isEmpty {
                        let requireQuantityField = self?.form.rowBy(tag: "requiredQuantity") as? TextRow
                        let baseQuantity = Decimal(string: row.value ?? "0")
                        let requiredQuantity = order.plannedQuantity ?? 0
                        let result = baseQuantity! * requiredQuantity
                        requireQuantityField?.value = component.unit == "Pieza" ?
                            String(format: "%.0f", NSDecimalNumber(decimal: result).doubleValue) :
                            String(format: "%.6f", NSDecimalNumber(decimal: result).doubleValue)
                        requireQuantityField?.reload()
                    }
                }
                // Validaciones
                var rules = RuleSet<String>()
                rules.add(rule: fieldNoEmpty)
                rules.add(rule: fieldShouldNotNegativeNumbers)
                rules.add(rule: shouldNotBeZero)
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
                $0.value = ""
                $0.tag = "requiredQuantity"
                $0.cellSetup { cell, _ in
                    cell.textField.keyboardType = .decimalPad
                }
                $0.onCellHighlightChanged { _, row in
                    if !(row.value?.isEmpty ?? true) && !(row.value == "0") &&
                        (self?.canOperation(rowValue: row.value ?? "d") ?? false && !row.value!.isEmpty) {
                        let requiredQuantity = Decimal(string: row.value ?? "0")
                        let baseQuantity = order.plannedQuantity ?? 0
                        let result = requiredQuantity!  / baseQuantity
                        let baseQuantityField = self?.form.rowBy(tag: "baseQuantity") as? TextRow
                        baseQuantityField?.value = component.unit == "Pieza" ?
                            String(format: "%.0f", NSDecimalNumber(decimal: result).doubleValue) :
                            String(format: "%.6f", NSDecimalNumber(decimal: result).doubleValue)
                        baseQuantityField?.reload()
                    }
                }
                // Validaciones
                var rules = RuleSet<String>()
                rules.add(rule: fieldNoEmpty)
                rules.add(rule: fieldShouldNotNegativeNumbers)
                rules.add(rule: shouldNotBeZero)
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
            <<< TextRow {
                $0.title = "Unidad:"
                $0.value = component.unit
                $0.disabled = true
            }
            .cellUpdate { cell, _ in
                cell.titleLabel?.textColor = .black
                cell.textField.textColor = .black
            }
            <<< PickerInlineRow<String> {
                $0.title = "Almacén: "
                $0.tag = "warehouse"
                $0.options = CommonStrings.options
                guard let warehouse = component.warehouse else { return }
                $0.value = warehouse
            }
            .cellUpdate { cell, _ in
                cell.detailTextLabel?.textColor = .black
            }
            +++ Section()
            <<< ButtonRow {
                $0.title = "Aceptar"
                $0.disabled = Condition.function(
                    form.allRows.compactMap { $0.tag }, { !$0.validate().isEmpty })
            }
            .onCellSelection { [weak self] _, row in
                row.section?.form?.validate()
                if row.isValid && !row.isDisabled {
                    self?.baseQuantity = self?.form.rowBy(tag: "baseQuantity")
                    self?.requiredQuantity = self?.form.rowBy(tag: "requiredQuantity")
                    self?.warehouse = self?.form.rowBy(tag: "warehouse")
                    let alert = UIAlertController(title: "¿Deseas guardar los cambios ingresados?",
                                                  message: nil, preferredStyle: .alert)
                    let cancelAction = UIAlertAction(title: "Cancelar", style: .destructive, handler: nil)
                    let okAction = UIAlertAction(title: CommonStrings.OKConst,
                                                 style: .default, handler: { _ in self?.saveChanges()})
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
                self?.navigationController?.popViewController(animated: true)
        }
    }
    func saveChanges () {
        if self.canOperation(rowValue: self.baseQuantity?.value ?? "f") &&
            self.canOperation(rowValue: (self.requiredQuantity?.value) ?? "r") {
            let requiredQuantityValue = Double( self.requiredQuantity?.value ?? "0.0") ?? 0
            let baseQuantityValue = Double(self.baseQuantity?.value ?? "0.0") ?? 0
            let warehouseValue = self.warehouse?.value ?? ""
            let values = ComponentFormValues(baseQuantity: baseQuantityValue,
                                             requiredQuantity: requiredQuantityValue,
                                             warehouse: warehouseValue)
            self.componentsViewModel.saveDidTap.onNext(values)
        }
    }
    func viewModelBinding() {
        self.componentsViewModel.selectedComponent.subscribe(onNext: { [weak self] comp in
            guard let component = comp else { return }
            guard let order = self?.inboxViewModel.selectedOrder else { return }
            self?.buildForm(component: component, order: order)
        }).disposed(by: disposeBag)
        self.componentsViewModel.saveSuccess.subscribe(onNext: { [weak self] _ in
            self?.dismiss(animated: true, completion: nil)
        }).disposed(by: disposeBag)
    }
    func canOperation(rowValue: String) -> Bool {
        let range = NSRange(location: 0, length: rowValue.description.utf16.count)
        let regex = try? NSRegularExpression(pattern: "^([0-9]+)?(\\.([0-9]{1,6})?)?$")
        return regex?.firstMatch(in: rowValue.description, options: [], range: range) != nil
    }
}

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
    // MARK: Functions
    // swiftlint:disable function_body_length
    // swiftlint:disable:next cyclomatic_complexity
    func buildForm() {
        LabelRow.defaultCellUpdate = { cell, _ in
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
            let regex = try? NSRegularExpression(pattern: RegularExpresions.onlyNumbers.rawValue)
            return !(regex?.firstMatch(
                        in: rowValue?.description ?? CommonStrings.empty, options: [], range: range) != nil) ?
                ValidationError(msg: Constants.Errors.validatingNumbers.rawValue) : nil
        }
        let fieldNoEmpty = RuleClosure<String> { rowValue in
            return (rowValue == nil || rowValue!.isEmpty) ? ValidationError(
                msg: Constants.Errors.emptyField.rawValue) : nil
        }
        form
            +++ Section(header: self.dataOfTable!.details![self.indexOfItemSelected].detailDescription!,
                        footer: CommonStrings.empty)
            <<< TextRow { [weak self] in
                $0.title = CommonStrings.baseQtyTitle
                $0.tag = CommonStrings.baseQtyField
                $0.value = self?.dataOfTable?.details?[self?.indexOfItemSelected ?? 0].unit == CommonStrings.piece ?
                    String(
                        format: DecimalFormat.zero.rawValue,
                        self?.dataOfTable?.details?[self?.indexOfItemSelected ?? 0].baseQuantity ?? 0)
                    : self?.formatter.string(
                        from: NSNumber(
                            value: self?.dataOfTable?.details?[self?.indexOfItemSelected ?? 0].baseQuantity ?? 0))
                $0.cellSetup { cell, _ in
                    cell.textField.keyboardType = .decimalPad
                }
                $0.onCellHighlightChanged { [weak self] _, row in
                    if row.value != nil && ((self?.canOperation(rowValue: row.value ?? "f")) ?? false) {
                        guard let baseQuantity = Decimal(string: row.value ?? CommonStrings.zero),
                              let requiredQuantity = self?.dataOfTable?.plannedQuantity else {
                            self?.assigResultToRequireQty(result: Decimal(0))
                            return
                        }
                        let result = baseQuantity * requiredQuantity
                        self?.assigResultToRequireQty(result: result)
                    }
                }
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
                let rowIndex = row.indexPath?.row ?? 0
                while row.section?.count ?? 0 > rowIndex + 1 && row.section?[rowIndex  + 1] is LabelRow {
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
                $0.title = CommonStrings.reqQtyTitle
                $0.value =  self?.dataOfTable?.details?[self?.indexOfItemSelected ?? 0].unit == CommonStrings.piece ?
                    String(format: DecimalFormat.zero.rawValue,
                           self?.dataOfTable?.details?[self?.indexOfItemSelected ?? 0].requiredQuantity ?? 0) :
                    self?.formatter.string(from: NSNumber(
                        value: self?.dataOfTable?.details?[self?.indexOfItemSelected ?? 0].requiredQuantity ?? 0))
                $0.tag = CommonStrings.reqQtyField
                $0.cellSetup { cell, _ in
                    cell.textField.keyboardType = .decimalPad
                }
                $0.onCellHighlightChanged { [weak self] _, row in
                    if !(row.value?.isEmpty ?? true) && !(row.value == CommonStrings.zero)
                        && ((self?.canOperation(rowValue: row.value ?? "d")) ?? false) {
                        guard let requiredQuantity = Decimal(string: row.value ?? CommonStrings.zero),
                              let baseQuantity = self?.dataOfTable?.plannedQuantity else {
                            self?.assingResultToBaseQtyField(result: Decimal(0))
                            return
                        }
                        let result = requiredQuantity / baseQuantity
                        self?.assingResultToBaseQtyField(result: result)
                    }
                }
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
                let rowIndex = row.indexPath?.row ?? 0
                while row.section?.count ?? 0 > rowIndex + 1 && row.section?[rowIndex  + 1] is LabelRow {
                    row.section?.remove(at: rowIndex + 1)
                }
                if !row.isValid {
                    for (index, validationMsg) in row.validationErrors.map({ $0.msg }).enumerated() {
                        let labelRow = LabelRow {
                            $0.title = validationMsg
                            $0.cell.height = { 28 }
                        }
                        let indexPath = row.indexPath?.row ?? 0 + index + 1
                        row.section?.insert(labelRow, at: indexPath)
                    }
                } else {
                    row.cleanValidationErrors()
                }
            }
            <<< TextRow { [weak self] in
                $0.title = CommonStrings.unit
                $0.value = self?.dataOfTable?.details?[self?.indexOfItemSelected ?? 0].unit
                $0.disabled = true
            }.cellUpdate { cell, _ in
                cell.titleLabel?.textColor = .black
                cell.textField.textColor = .black
            }
            <<< PickerInlineRow<String> { [weak self] in
                $0.title = CommonStrings.werehouseTitle
                $0.tag = CommonStrings.werehouseProperty
                $0.options = CommonStrings.options
                $0.value = self?.dataOfTable?.details?[self?.indexOfItemSelected ?? 0].warehouse ?? CommonStrings.empty
            }
            .cellUpdate { cell, _ in
                cell.detailTextLabel?.textColor = .black
            }
            +++ Section()
            <<< ButtonRow { [weak self] in
                $0.title = CommonStrings.OKConst
                $0.disabled = Condition.function(
                    self?.form.allRows.compactMap { $0.tag } ?? [CommonStrings.empty], { !$0.validate().isEmpty })
            }
            .onCellSelection { [weak self] _, row in
                row.section?.form?.validate()
                if row.isValid && !row.isDisabled {
                    self?.baseQuantity = self?.form.rowBy(tag: CommonStrings.baseQtyField)
                    self?.requiredQuantity = self?.form.rowBy(tag: CommonStrings.reqQtyField)
                    self?.werehouse = self?.form.rowBy(tag: CommonStrings.werehouseProperty)
                    let alert = UIAlertController(title: CommonStrings.saveChanges,
                                                  message: nil, preferredStyle: .alert)
                    let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive,
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
                $0.title = CommonStrings.cancel
            }
            .cellSetup { cell, _ in
                cell.tintColor = .red
            }
            .onCellSelection { [weak self] _, _ in
                self?.dismiss(animated: true)
        }
    }
}

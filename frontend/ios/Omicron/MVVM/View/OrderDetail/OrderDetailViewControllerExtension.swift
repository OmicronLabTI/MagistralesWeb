//
//  OrderDetailViewControllerExtension.swift
//  Omicron
//
//  Created by Daniel Velez on 27/01/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift

extension OrderDetailViewController {
    func viewModelBinding2() {
        self.orderDetailViewModel.showAlertConfirmation.observe(on: MainScheduler.instance)
            .subscribe(onNext: { [weak self] data in
                let alert = UIAlertController(title: data.message, message: nil, preferredStyle: .alert)
                let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive,
                                                 handler: {[weak self] _ in
                                                    self?.dismiss(animated: true, completion: nil)})
                let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default,
                                             handler: { [weak self] _ in
                                                self?.orderDetailViewModel.terminateOrChangeStatusOfAnOrder(
                                                    actionType: data.typeOfStatus)})
                alert.addAction(cancelAction)
                alert.addAction(okAction)
                self?.present(alert, animated: true, completion: nil)
            }).disposed(by: self.disposeBag)
        self.orderDetailViewModel.sumFormula.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] sum in
                   guard let self = self else { return }
                   if sum != -1 {
                       if !self.isolatedOrder {
                           self.sumFormulaDescriptionLabel.attributedText = UtilsManager
                               .shared
                               .boldSubstring(
                                   text: "\(CommonStrings.sumOfFormula)" +
                                   "\(self.formatter.string(from: NSNumber(value: sum)) ?? CommonStrings.empty)",
                                   textToBold: "\(CommonStrings.sumOfFormula)")
                       } else {
                           self.tagDescriptionLabel.attributedText = UtilsManager
                               .shared
                               .boldSubstring(
                                   text: "\(CommonStrings.sumOfFormula)" +
                                   "\(self.formatter.string(from: NSNumber(value: sum)) ?? CommonStrings.empty)",
                                   textToBold: "\(CommonStrings.sumOfFormula)")
                       }
                   }
               }).disposed(by: self.disposeBag)
    }
    func viewModelBinding3() {
        self.orderDetailViewModel.orderDetailData.observe(on: MainScheduler.instance)
        .subscribe(onNext: { [weak self] res in
            guard let self = self else { return }
            self.quantityTextField.isHidden = true
            self.labelSpaceQuantity.isHidden = true
            self.quantityButtonChange.isHidden = true
            self.quantityPlannedDescriptionLabel.isHidden = false
            self.destinyLabel.isHidden = false
            self.isolatedOrder = false
            if res.first != nil {
                self.initLabelsWithContent(detail: res.first!)
                self.changeTextColorLabel(color: .black)
                self.orderDetail = res
                self.onGoingSplitProcess = res[0].onSplitProcess ?? false
                self.disableButtonsForFatherOrder(res[0].orderRelationType ?? OrderRelationTypes.completa)
                let detail = res.first!
                buildObjectToUpdateData(detail)
                componentsToUpdate = []
                saveWarehousesChangesButton.isEnabled = false
                self.productDescritionLabel.textColor = .black
                self.productDescritionLabel.attributedText = self.getRichText(detail: detail)
                self.destinyLabel.attributedText = UtilsManager.shared.boldSubstring(
                    text: "\(CommonStrings.destiny) \(self.destiny)", textToBold: CommonStrings.destiny)
                if detail.baseDocument == 0 {
                    self.isolatedOrder = true
                    self.quantityTextField.text = self.getFormattedText(
                        number: Double(truncating:
                                        NSDecimalNumber(decimal: detail.plannedQuantity ?? 0)))
                    self.destinyLabel.text = ""
                    self.codeDescriptionLabel.isHidden = true
                    self.containerDescriptionLabel.isHidden = true
                    self.labelSpaceQuantity.isHidden = false
                    self.validateStatusIsolated()
                    let plannedQ = self.quantityTextField.isHidden ?
                        String(describing: detail.plannedQuantity ?? 0) :
                        ""
                    self.sumFormulaDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(
                        text: "\(CommonStrings.plannedQuantity) \(plannedQ)",
                        textToBold: CommonStrings.plannedQuantity)
                    self.quantityPlannedDescriptionLabel.text = ""
                    self.quantityPlannedDescriptionLabel.isHidden = true
                    self.destinyLabel.isHidden = true
                }
            }
        }).disposed(by: self.disposeBag)
    }
    
    func buildObjectToUpdateData(_ data: OrderDetail) {
        orderDetailViewModel.updateObjectToSend.fabOrderID = data.productionOrderID ?? 0
        orderDetailViewModel.updateObjectToSend.plannedQuantity = data.plannedQuantity ?? 0
        orderDetailViewModel.updateObjectToSend.fechaFin = UtilsManager.shared.formattedDateFromString(
            dateString: data.dueDate ?? String(), withFormat: DateFormat.yyyymmdd) ?? ""
        orderDetailViewModel.updateObjectToSend.comments = data.comments ?? ""
        orderDetailViewModel.updateObjectToSend.warehouse = data.warehouse ?? ""
    }
    
    func getRichText(detail: OrderDetail) -> NSMutableAttributedString {
        let titleFontSize = CGFloat(22.0)
        let code = UtilsManager.shared.boldSubstring(text: "\(detail.code ?? CommonStrings.empty)",
            textToBold: detail.code, fontSize: titleFontSize, textColor: OmicronColors.blue)
        let description = UtilsManager.shared.boldSubstring(
            text: "\(detail.productDescription ?? CommonStrings.empty)",
            textToBold: detail.productDescription, fontSize: titleFontSize, textColor: .gray)
        let pipe = UtilsManager.shared.boldSubstring(text: " | ", textToBold: " | ",
                                                     fontSize: titleFontSize, textColor: .black)
        let richText = NSMutableAttributedString()
        richText.append(code)
        richText.append(pipe)
        richText.append(description)
        return richText
    }
    func validateStatusIsolated() {
        let statusValid = [
            StatusNameConstants.assignedStatus,
            StatusNameConstants.inProcessStatus,
            StatusNameConstants.reassignedStatus
        ]
        if statusValid.contains(self.statusType) && self.rootViewModel.userType == .qfb {
            self.quantityTextField.isHidden = false
            self.quantityButtonChange.isHidden = false
        }
    }

    func initLabelsWithContent(detail: OrderDetail) {
        let partDecimal = self.getDecimalPartOfDouble(
            number: NSDecimalNumber(decimal: detail.plannedQuantity ?? 0.0).doubleValue)
        let number = detail.baseDocument == 0 ? CommonStrings.empty : "\(detail.baseDocument ?? 0)"
        self.codeDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(
            text: "\(CommonStrings.orderNumber) \(number)",
            textToBold: CommonStrings.orderNumber)
        self.containerDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(
            text: "\(CommonStrings.container) \(detail.container ?? CommonStrings.empty)",
            textToBold: CommonStrings.container)
        self.tagDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(
            text: "\(CommonStrings.tag) \(detail.productLabel ?? CommonStrings.empty)",
            textToBold: CommonStrings.tag)
        self.documentBaseDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(
            text: "\(CommonStrings.manufacturingOrder) \(detail.productionOrderID ?? 0)",
            textToBold: CommonStrings.manufacturingOrder)
        let plannedQuantityText = partDecimal > 0.0 ?
            String(format: "%6f", NSDecimalNumber(decimal: detail.plannedQuantity ?? 0.0).doubleValue) :
        "\(detail.plannedQuantity ?? 0.0)"
        self.quantityPlannedDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(
            text: "\(CommonStrings.plannedQuantity) \(plannedQuantityText)",
            textToBold: CommonStrings.plannedQuantity)
        self.startDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(
            text: "\(CommonStrings.manufacturingDate) \(detail.orderCreateDate ?? CommonStrings.empty)",
            textToBold: CommonStrings.manufacturingDate)
        self.finishedDateDescriptionLabel.attributedText = UtilsManager.shared.boldSubstring(
            text: "\(CommonStrings.finishdate) \(detail.dueDate ?? CommonStrings.empty)",
            textToBold: CommonStrings.finishdate)
    }
    func viewModelBinding4() {
        // Cambia de color los labels encabezado de tabla cuando termina de cargar las ordene
        self.orderDetailViewModel.changeColorLabelsHt.subscribe(onNext: { [weak self] _ in
            self?.changeTextColorHtLabels(color: .black)
            self?.splitButton.isEnabled = true
        }).disposed(by: self.disposeBag)
        self.orderDetailViewModel.tableData.bind(to: tableView.rx.items(
            cellIdentifier: ViewControllerIdentifiers.detailTableViewCell,
            cellType: DetailTableViewCell.self)) { [weak self] row, data, cell in
                cell.hashTagLabel.text = "\(row + 1)"
                cell.enableAction = self?.enablePicketContainer() ?? true
                cell.hasBatches = data.hasBatches ?? false
                cell.codeLabel.text = "\(data.productID ?? String())"
                cell.descriptionLabel.text = data.detailDescription?.uppercased()
                cell.baseQuantityLabel.text =  data.unit == CommonStrings.piece ?
                    String(format: "%.0f", data.baseQuantity ?? 0.0) :
                    self?.formatter.string(from: NSNumber(value: data.baseQuantity ?? 0.0))
        
                cell.requiredQuantityLabel.text = data.unit == CommonStrings.piece ?
                    String(format: "%.0f", data.requiredQuantity ?? 0.0) :
                    self?.formatter.string(from: NSNumber(value: data.requiredQuantity ?? 0.0))
    
                cell.unitLabel.text = data.unit ?? String()
                let hasStock = data.stock ?? 0.0 > 0.0
                cell.setEmptyStock(hasStock)
                cell.options = self?.orderDetailViewModel.warehousesOptions ?? []
                cell.selectedOption = data.warehouse ?? String()
                cell.delegate = self
                cell.productId = data.productID ?? String()
                cell.pickerContainerView.isUserInteractionEnabled = self?.enablePickerBySplitProcess(
                    typeOrder: self?.orderDetail[0].orderRelationType ?? OrderRelationTypes.completa,
                    onSpliProcess: self?.orderDetail[0].onSplitProcess ?? false) ?? true
        }.disposed(by: disposeBag)
        orderDetailViewModel.tableData.subscribe(onNext: { [weak self] details in
            guard let self = self else { return }
            self.emptyStockProductId = details.map { detail -> String in
                if !(detail.stock ?? 0.0 > 0.0) { return detail.productID ?? "" }
                return ""
            }
        }).disposed(by: disposeBag)
        orderDetailViewModel.splitButtonEnableFlag.subscribe(onNext: { [weak self] typeOrder in
            guard let self = self else { return }
            splitButton.isEnabled = typeOrder != OrderRelationTypes.hija
        }).disposed(by: disposeBag)
    }
    
    func enablePickerBySplitProcess(typeOrder: String, onSpliProcess: Bool) -> Bool {
        let enablePicker = typeOrder != OrderRelationTypes.padre || !onSpliProcess
        return enablePicker
    }
    
    func enablePicketContainer() -> Bool{
        return statusType == StatusNameConstants.inProcessStatus || statusType == StatusNameConstants.reassignedStatus
    }

    func quantityButtonBindind() {
        self.quantityButtonChange.rx.tap.bind {
            self.quantityButtonChange.isEnabled = false
            let quantity = Decimal(string: self.quantityTextField.text?.replacingOccurrences(of: ",", with: "") ?? "0")
            self.orderDetailViewModel.updateQuantity(quantity ?? 0)
        }
        .disposed(by: self.disposeBag)
    }
    
    func disableButtonsForFatherOrder(_ typeOrder: String) {
        if typeOrder == OrderRelationTypes.padre {
            self.processButton.isEnabled = false
            self.finishedButton.isEnabled = false
            self.penddingButton.isEnabled = false
            self.addComponentButton.isEnabled = false
            self.saveButton.isEnabled = false
            self.seeLotsButton.isEnabled = false
            self.saveWarehousesChangesButton.isEnabled = false
        }
    }
}

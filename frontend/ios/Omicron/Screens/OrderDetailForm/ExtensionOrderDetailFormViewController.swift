//
//  ExtensionOrderDetailFormViewController.swift
//  Omicron
//
//  Created by Sergio Flores Ramirez on 22/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import Eureka

extension OrderDetailFormViewController {

    func saveChanges () {
        if self.canOperation(rowValue: self.baseQuantity?.value ?? "f")
            && self.canOperation(rowValue: (self.requiredQuantity?.value) ?? "r") {
            let requiredQuantityValue = Double( self.requiredQuantity?.value ?? "0.0")
            let baseQuantityValue = Double(self.baseQuantity?.value ?? "0.0")
            guard let dataOfTable = self.dataOfTable else { return }
            self.orderDetailFormViewModel.editItemTable(
                index: self.indexOfItemSelected, data: dataOfTable,
                baseQuantity: baseQuantityValue ?? 0.0, requiredQuantity: requiredQuantityValue ?? 0.0,
                werehouse: werehouse?.value ?? CommonStrings.empty)
        }
    }

    func viewModelBinding () {
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
            self?.orderDetailViewModel.getOrdenDetail(isRefresh: true)
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

    func assingResultToBaseQtyField(result: Decimal) {
        let baseQuantityField = self.form.rowBy(tag: "baseQuantity") as? TextRow
        baseQuantityField?.value =
            self.dataOfTable?.details?[self.indexOfItemSelected].unit == "Pieza" ?
                String(format: "%.0f", NSDecimalNumber(decimal: result).doubleValue) :
            String(format: "%.6f", NSDecimalNumber(decimal: result).doubleValue)
        baseQuantityField?.reload()
    }

    func assigResultToRequireQty(result: Decimal) {
        let requireQuantityField = self.form.rowBy(tag: "requiredQuantity") as? TextRow
        requireQuantityField?.value =
            self.dataOfTable?.details?[self.indexOfItemSelected].unit == "Pieza" ?
            String(format: "%.0f", NSDecimalNumber(decimal: result).doubleValue) :
            String(format: "%.6f", NSDecimalNumber(decimal: result).doubleValue)
        requireQuantityField?.reload()
    }
}

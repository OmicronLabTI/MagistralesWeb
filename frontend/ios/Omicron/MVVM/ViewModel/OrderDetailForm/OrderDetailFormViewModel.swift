//
//  OrderDetailFormViewModel.swift
//  Omicron
//
//  Created by Axity on 15/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import Resolver

class  OrderDetailFormViewModel {
    // MARK: Variables
    var loading = PublishSubject<Bool>()
    var showAlert = PublishSubject<String>()
    var disposeBag = DisposeBag()
    var success = PublishSubject<Int>()
    var response = PublishSubject<String>()
    @Injected var networkManager: NetworkManager
    // MARK: - Init
    init() { }

    // MARK: - Functions
    func editItemTable(index: Int, data: OrderDetail, baseQuantity: Double,
                       requiredQuantity: Double, werehouse: String) {
        self.loading.onNext(true)
        let componets = [Component(orderFabID: data.details?[index].orderFabID ?? 0,
                                   productId: data.details?[index].productID ?? CommonStrings.empty,
                                   componentDescription: data.details?[index].detailDescription ?? CommonStrings.empty,
                                   baseQuantity: baseQuantity, requiredQuantity: requiredQuantity,
                                   consumed: data.details?[index].consumed ?? 0.0,
                                   available: data.details?[index].available ?? 0,
                                   unit: data.details?[index].unit ?? CommonStrings.empty,
                                   warehouse: werehouse, pendingQuantity: data.details?[index].pendingQuantity ?? 0,
                                   stock: data.details?[index].stock ?? 0,
                                   warehouseQuantity: data.details?[index].warehouseQuantity ?? 0,
                                   action: Actions.update.rawValue)]
        let fechaFinFormated = UtilsManager.shared.formattedDateFromString(
            dateString: data.dueDate ?? CommonStrings.empty, withFormat: DateFormat.yyyymmdd
        )
        let order = OrderDetailRequest(
            fabOrderID: data.productionOrderID ?? 0,
            plannedQuantity: data.plannedQuantity ?? 0.0,
            fechaFin: fechaFinFormated ?? CommonStrings.empty, comments: CommonStrings.empty,
            warehouse: data.warehouse ?? CommonStrings.empty, components: componets)
        self.networkManager.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order)
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] res in
                self?.loading.onNext(false)
                self?.showAlert.onNext(CommonStrings.changesSuccess)
                self?.response.onNext(res.response ?? CommonStrings.empty)
                self?.success.onNext(data.details?[index].orderFabID ?? 0)
                }, onError: {  [weak self] _ in
                    self?.loading.onNext(false)
                    self?.showAlert.onNext(Constants.Errors.editItemTable.rawValue)
            }).disposed(by: self.disposeBag)
    }
}

//
//  OrderDetailFormViewModel.swift
//  Omicron
//
//  Created by Axity on 15/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift

class  OrderDetailFormViewModel {
    
    // MARK: Variables
    var loading = PublishSubject<Bool>()
    var showAlert = PublishSubject<String>()
    var disposeBag = DisposeBag()
    var success = PublishSubject<Int>()
    
    // MARK: Init
    init() {
        
    }
    
    deinit {
        print("Se muere el viewModel")
    }
    
    //MARK: Functions
    func editItemTable(index: Int, data: OrderDetail, baseQuantity: Double, requiredQuantity: Double, werehouse: String) -> Void {
        self.loading.onNext(true)
        let componets = [Component(orderFabID: data.details![index].orderFabID!, productId: data.details![index].productID!, componentDescription: data.details![index].detailDescription!, baseQuantity: baseQuantity, requiredQuantity: requiredQuantity, consumed: data.details![index].consumed!, available: data.details![index].available!, unit: data.details![index].unit!, warehouse: werehouse, pendingQuantity: data.details![index].pendingQuantity!, stock: data.details![index].stock!, warehouseQuantity: data.details![index].warehouseQuantity!, action: "update")]
        
        let fechaFinFormated = UtilsManager.shared.formattedDateFromString(dateString: (data.dueDate)!, withFormat: "yyyy-MM-dd")
        
        let order = OrderDetailRequest(fabOrderID: (data.productionOrderID)!, plannedQuantity: (data.plannedQuantity)!, fechaFin: fechaFinFormated!, comments: "", components: componets)
        
        NetworkManager.shared.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order).observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] res in
            self?.loading.onNext(false)
             self?.showAlert.onNext("Se registraron los cambios correctamente")
           self?.success.onNext(data.details![index].orderFabID!)
        }, onError: {  [weak self] error in
            self?.loading.onNext(false)
            self?.showAlert.onNext("Hubo un error al editar el elemento,  intente de nuevo")
        }).disposed(by: self.disposeBag)
    }
}

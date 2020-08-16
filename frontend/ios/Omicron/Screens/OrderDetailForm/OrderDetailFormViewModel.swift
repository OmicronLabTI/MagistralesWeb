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
    
    // MARK: Init
    init() {
        
    }
    
    //MARK: Functions
    func editItemTable(index: Int, data: OrderDetail, baseQuantity: Int, requiredQuantity: Int, werehouse: String) -> Void {
        self.loading.onNext(true)
        
        let componets = [Component(orderFabID: data.details![index].orderFabID!, productId: data.details![index].productID!, componentDescription: data.details![index].detailDescription!, baseQuantity: baseQuantity, requiredQuantity: requiredQuantity, consumed: data.details![index].consumed!, available: data.details![index].available!, unit: data.details![index].unit!, warehouse: data.details![index].warehouse!, pendingQuantity: data.details![index].pendingQuantity!, stock: data.details![index].stock!, warehouseQuantity: data.details![index].warehouseQuantity!, action: "update")]
        
        let fechaFinFormated = UtilsManager.shared.formattedDateFromString(dateString: (data.dueDate)!, withFormat: "yyyy-MM-dd")
        
        let order = OrderDetailRequest(fabOrderID: (data.productionOrderID)!, plannedQuantity: (data.plannedQuantity)!, fechaFin: fechaFinFormated!, comments: "", components: componets)
        
        NetworkManager.shared.deleteItemOfOrdenDetail(orderDetailRequest: order).observeOn(MainScheduler.instance).subscribe(onNext: { res in
            self.loading.onNext(false)
        }, onError: {  error in
            self.loading.onNext(false)
            self.showAlert.onNext("Hubo un error al eliminar el elemento,  intente de nuevo")
        }).disposed(by: self.disposeBag)
    }
}

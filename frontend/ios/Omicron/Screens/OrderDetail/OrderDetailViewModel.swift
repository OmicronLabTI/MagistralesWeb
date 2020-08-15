//
//  OrderDetailViewModel.swift
//  Omicron
//
//  Created by Axity on 07/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa

class OrderDetailViewModel {
    
    // MARK: Variables
    var disposeBag: DisposeBag = DisposeBag()
    var orderDetailData: BehaviorRelay<[OrderDetail]> = BehaviorRelay<[OrderDetail]>(value: [])
    var tempOrderDetailData: OrderDetail? = nil
    var tableData: BehaviorRelay<[Detail]> = BehaviorRelay<[Detail]>(value: [])
    var showAlert: PublishSubject<String> = PublishSubject()
    var showAlertConfirmation: PublishSubject<String> = PublishSubject()
    var loading: BehaviorSubject<Bool> = BehaviorSubject<Bool>(value: false)
    var sumFormula: BehaviorRelay<Int> = BehaviorRelay<Int>(value: 0)
    var auxTabledata:[Detail] = []
    var processButtonDidTap: PublishSubject<Void> = PublishSubject<Void>()
    let backToInboxView: PublishSubject<Void> = PublishSubject<Void>()
    // MARK: Init
    init() {
        
        self.processButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: {
            self.showAlertConfirmation.onNext("La orden cambiará a estatus En proceso ¿quieres continuar?")
        }).disposed(by: self.disposeBag)

    }
    
    // MARK: Functions
    func getOrdenDetail(orderId: Int) -> Void {
        loading.onNext(true)
        NetworkManager.shared.getOrdenDetail(orderId: orderId).observeOn(MainScheduler.instance).subscribe(onNext: {res in
            self.orderDetailData.accept([res.response!])
            self.tableData.accept(res.response!.details!)
            self.auxTabledata = res.response!.details!
            self.tempOrderDetailData = res.response!
            self.loading.onNext(false)
            self.sumFormula.accept(self.sum(tableDetails: res.response!.details!))
        }, onError: { error in
            self.loading.onNext(false)
            self.showAlert.onNext("Hubo un error al cargar el detalle de la orden de fabricación, intentar de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func sum(tableDetails: [Detail]) -> Int {
        var sum = 0
        if(tableDetails.count > 0) {
            for detail in tableDetails {
                sum = sum + detail.requiredQuantity!
            }
            return sum
        }
        return sum
    }
    
    func changeStatus() {
        self.loading.onNext(true)
        let changeStatus = ChangeStatusRequest(userId: (Persistence.shared.getUserData()?.id)!, orderId: (self.tempOrderDetailData?.productionOrderID)!, status: "Proceso")
        NetworkManager.shared.changeStatusOrder(changeStatusRequest: [changeStatus]).observeOn(MainScheduler.instance).subscribe(onNext: { res in
            self.loading.onNext(false)
            self.backToInboxView.onNext(())
        }, onError: { error in
            self.loading.onNext(false)
            self.showAlert.onNext("Ocurrió un error al cambiar de estatus la orden, por favor intente de nuevo")
            }).disposed(by: self.disposeBag)
    }

    func deleteItemFromTable(index: Int) {
        self.loading.onNext(true)
        let itemToDelete = auxTabledata[index]
        let componets = [Component(orderFabID: itemToDelete.orderFabID!, productId: itemToDelete.productID!, componentDescription: itemToDelete.detailDescription!, baseQuantity: itemToDelete.baseQuantity!, requiredQuantity: itemToDelete.requiredQuantity!, consumed: itemToDelete.consumed!, available: itemToDelete.available!, unit: itemToDelete.unit!, warehouse: itemToDelete.warehouse!, pendingQuantity: itemToDelete.pendingQuantity!, stock: itemToDelete.stock!, warehouseQuantity: itemToDelete.warehouseQuantity!, action: "delete")]

        let fechaFinFormated = self.formattedDateFromString(dateString: (tempOrderDetailData?.dueDate)!, withFormat: "yyyy-MM-dd")
        let order = OrderDetailRequest(fabOrderID: (tempOrderDetailData?.productionOrderID)!, plannedQuantity: (tempOrderDetailData?.plannedQuantity)!, fechaFin: fechaFinFormated!, comments: "", components: componets)

        NetworkManager.shared.deleteItemOfOrdenDetail(orderDetailRequest: order).observeOn(MainScheduler.instance).subscribe(onNext: { res in
            self.loading.onNext(false)
                self.tempOrderDetailData?.details?.remove(at: index)
            self.tableData.accept((self.tempOrderDetailData?.details)!)
            self.sumFormula.accept(self.sum(tableDetails: (self.tempOrderDetailData?.details)!))
        }, onError: {  error in
            self.loading.onNext(false)
            self.showAlert.onNext("Hubo un error al eliminar el elemento,  intente de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    
    func formattedDateFromString(dateString: String, withFormat format: String) -> String? {

        let inputFormatter = DateFormatter()
        inputFormatter.dateFormat = "dd/MM/yyyy"
        if let date = inputFormatter.date(from: dateString) {
            let outputFormatter = DateFormatter()
          outputFormatter.dateFormat = format
            return outputFormatter.string(from: date)
        }
        return nil
    }
}

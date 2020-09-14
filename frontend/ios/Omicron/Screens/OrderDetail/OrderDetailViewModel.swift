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
    var tableData: BehaviorSubject<[Detail]> = BehaviorSubject<[Detail]>(value: [])
    var showAlert: PublishSubject<String> = PublishSubject()
    var showAlertConfirmationProcess = PublishSubject<String>()
    var showAlertConfirmationFinished = PublishSubject<String>()
    var loading: BehaviorSubject<Bool> = BehaviorSubject<Bool>(value: false)
    var sumFormula: BehaviorRelay<Double> = BehaviorRelay<Double>(value: 0)
    var auxTabledata:[Detail] = []
    var processButtonDidTap = PublishSubject<Void>()
    var finishedButtonDidTap = PublishSubject<Void>()
    var seeLotsButtonDidTap = PublishSubject<Void>()
    var goToSeeLotsViewController = PublishSubject<Void>()
    let backToInboxView: PublishSubject<Void> = PublishSubject<Void>()
    var showIconComments = PublishSubject<String>()
    var orderId: Int = -1
    var showSignatureView = PublishSubject<String>()
    var  qfbSignatureIsGet = false
    var technicalSignatureIsGet = false
    var sqfbSignature = ""
    var technicalSignature = ""
    var endRefreshing = PublishSubject<Void>()
    
    // MARK: Init
    init() {
        
        self.finishedButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.showAlertConfirmationFinished.onNext("¿Deseas terminar la orden?")
        }).disposed(by: self.disposeBag)
        
        self.processButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.showAlertConfirmationProcess.onNext("La orden cambiará a estatus En proceso ¿quieres continuar?")
        }).disposed(by: disposeBag)
        
        self.seeLotsButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self]  _ in
            self?.goToSeeLotsViewController.onNext(())
        }).disposed(by: self.disposeBag)
    }
    
    deinit {
        print("Se muere OrderDetailViewModel")
    }
    
    // MARK: Functions
    func getOrdenDetail(isRefresh: Bool = false) -> Void {
        loading.onNext(true)
        NetworkManager.shared.getOrdenDetail(orderId: self.orderId).observeOn(MainScheduler.instance).subscribe(onNext: {[weak self] res in
            self?.orderDetailData.accept([res.response!])
            self?.tableData.onNext(res.response!.details!)
            self?.auxTabledata = res.response!.details!
            self?.tempOrderDetailData = res.response!
            self?.loading.onNext(false)
            self?.sumFormula.accept(self!.sum(tableDetails: res.response!.details!))
            let iconName = res.response?.comments != nil ? "message.fill": "message"
            self?.showIconComments.onNext(iconName)
            if(isRefresh) {
                self?.endRefreshing.onNext(())
            }
        }, onError: { [weak self] error in
            self?.loading.onNext(false)
            if(isRefresh) {
                self?.endRefreshing.onNext(())
            }
            self?.showAlert.onNext("Hubo un error al cargar el detalle de la orden de fabricación, intentar de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func sum(tableDetails: [Detail]) -> Double {
        var sum = 0.0
        if(tableDetails.count > 0) {
            for detail in tableDetails {
                if(detail.unit  != "Pieza") {
                    sum = sum + detail.requiredQuantity!
                }
            }
            return sum
        }
        return sum
    }
    
    func changeStatus() {
        self.loading.onNext(true)
        let changeStatus = ChangeStatusRequest(userId: (Persistence.shared.getUserData()?.id)!, orderId: (self.tempOrderDetailData?.productionOrderID)!, status: "Proceso")
        NetworkManager.shared.changeStatusOrder(changeStatusRequest: [changeStatus]).observeOn(MainScheduler.instance).subscribe(onNext: {[weak self] res in
            self?.loading.onNext(false)
            self?.backToInboxView.onNext(())
        }, onError: { [weak self] error in
            self?.loading.onNext(false)
            self?.showAlert.onNext("Ocurrió un error al cambiar de estatus la orden, por favor intente de nuevo")
            }).disposed(by: self.disposeBag)
    }

    func deleteItemFromTable(index: Int) {
        self.loading.onNext(true)
        let itemToDelete = auxTabledata[index]
        let componets = [Component(orderFabID: itemToDelete.orderFabID!, productId: itemToDelete.productID!, componentDescription: itemToDelete.detailDescription!, baseQuantity: itemToDelete.baseQuantity!, requiredQuantity: itemToDelete.requiredQuantity!, consumed: itemToDelete.consumed!, available: itemToDelete.available!, unit: itemToDelete.unit!, warehouse: itemToDelete.warehouse!, pendingQuantity: itemToDelete.pendingQuantity!, stock: itemToDelete.stock!, warehouseQuantity: itemToDelete.warehouseQuantity!, action: "delete")]

        let fechaFinFormated = UtilsManager.shared.formattedDateFromString(dateString: (tempOrderDetailData?.dueDate)!, withFormat: "yyyy-MM-dd")
        let order = OrderDetailRequest(fabOrderID: (tempOrderDetailData?.productionOrderID)!, plannedQuantity: (tempOrderDetailData?.plannedQuantity)!, fechaFin: fechaFinFormated!, comments: "", components: componets)

        NetworkManager.shared.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order).observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            if(self?.tempOrderDetailData != nil ) {
                self?.loading.onNext(false)
                self?.tempOrderDetailData?.details?.remove(at: index)
                self?.auxTabledata = self!.tempOrderDetailData!.details!
                self?.tableData.onNext((self?.tempOrderDetailData?.details)!)
                self?.sumFormula.accept((self?.sum(tableDetails: (self?.tempOrderDetailData?.details)!))!)
            }
            }, onError: {  [weak self] error in
                self?.loading.onNext(false)
                self?.showAlert.onNext("Hubo un error al eliminar el elemento,  intente de nuevo")
        }).disposed(by: self.disposeBag)
    }
    
    func getDataTableToEdit() -> OrderDetail  {
        return self.tempOrderDetailData!
    }
    
    // Valida si el usuario obtuvo las firmas y finaliza la orden
    func validSignatures() -> Void {
        
        if(self.technicalSignatureIsGet && self.qfbSignatureIsGet) {
            self.loading.onNext(true)
            let finishOrder = FinishOrder(userId: Persistence.shared.getUserData()!.id!, fabricationOrderId: self.orderId, qfbSignature: self.sqfbSignature, technicalSignature: self.technicalSignature)

            NetworkManager.shared.finishOrder(order: finishOrder).observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
                self?.loading.onNext(false)
                self?.backToInboxView.onNext(())
            }, onError: { [weak self] error in
                self?.loading.onNext(false)
            }).disposed(by: self.disposeBag)
        }
    }
    
    func validIfOrderCanBeFinalized() -> Void {
        self.loading.onNext(true)
        NetworkManager.shared.askIfOrderCanBeFinalized(orderId: self.orderId).subscribe(onNext: { [weak self] _ in
            self?.loading.onNext(false)
            self?.showSignatureView.onNext("Firma del  QFB")
            }, onError: { [weak self] error in
                self?.loading.onNext(false)
                self?.showAlert.onNext("La orden no puede ser Terminada, revisa que todos los artículos tengan un lote asignado")
        }).disposed(by: self.disposeBag)
    }
}

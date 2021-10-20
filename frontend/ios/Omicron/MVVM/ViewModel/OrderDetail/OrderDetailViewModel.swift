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
import Resolver

class OrderDetailViewModel {
    // MARK: - Variables
    var disposeBag: DisposeBag = DisposeBag()
    var orderDetailData: BehaviorRelay<[OrderDetail]> = BehaviorRelay<[OrderDetail]>(value: [])
    weak var tempOrderDetailData: OrderDetail?
    var tableData: BehaviorSubject<[Detail]> = BehaviorSubject<[Detail]>(value: [])
    var showAlert: PublishSubject<String> = PublishSubject()
    var showAlertConfirmation = PublishSubject<MessageToChangeStatus>()
    var loading: BehaviorSubject<Bool> = BehaviorSubject<Bool>(value: false)
    var sumFormula: BehaviorRelay<Double> = BehaviorRelay<Double>(value: -1)
    var auxTabledata: [Detail] = []
    var processButtonDidTap = PublishSubject<Void>()
    var finishedButtonDidTap = PublishSubject<Void>()
    var pendingButtonDidTap = PublishSubject<Void>()
    var seeLotsButtonDidTap = PublishSubject<Void>()
    var goToSeeLotsViewController = PublishSubject<Void>()
    let backToInboxView: PublishSubject<Void> = PublishSubject<Void>()
    var showIconComments = PublishSubject<String>()
    var orderId: Int = -1
    var showSignatureView = PublishSubject<String>()
    var qfbSignatureIsGet = false
    var technicalSignatureIsGet = false
    var sqfbSignature = ""
    var technicalSignature = ""
    var endRefreshing = PublishSubject<Void>()
    var needsRefresh = true
    var changeColorLabelsHt = PublishSubject<Void>()
    var catalogGroup = String()
    @Injected var rootViewModel: RootViewModel
    @Injected var networkManager: NetworkManager
    // MARK: - Init
    init() {
        self.finishedButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            let message = MessageToChangeStatus(message: CommonStrings.doYouWantToFinishTheOrder,
                                                typeOfStatus: StatusNameConstants.finishedStatus)
            self?.showAlertConfirmation.onNext(message)
        }).disposed(by: self.disposeBag)
        self.processButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            let message = MessageToChangeStatus(message: CommonStrings.confirmationMessageProcessStatus,
                                                typeOfStatus: StatusNameConstants.inProcessStatus)
            self?.showAlertConfirmation.onNext(message)
        }).disposed(by: disposeBag)
        self.pendingButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            let message = MessageToChangeStatus(message: CommonStrings.confirmationMessagePendingStatus,
                                                typeOfStatus: StatusNameConstants.penddingStatus)
            self?.showAlertConfirmation.onNext(message)
        }).disposed(by: self.disposeBag)
        self.seeLotsButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self]  _ in
            self?.goToSeeLotsViewController.onNext(())
        }).disposed(by: self.disposeBag)
    }

    // MARK: - Functions
    func getOrdenDetail(
        isRefresh: Bool = false, needsError: Bool = false, statusCode: Int = 500, testData: Data = Data()) {
        if needsRefresh { loading.onNext(true) }
        self.networkManager.getOrdenDetail(
            orderId: self.orderId, needsError: needsError, statusCode: statusCode, testData: testData)
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: {[weak self] res in
            guard let self = self else { return }
            if res.response != nil {
                self.orderDetailData.accept([res.response!])
                self.tableData.onNext(res.response!.details!)
                self.auxTabledata = res.response!.details ?? []
                self.tempOrderDetailData = res.response!
                if self.needsRefresh {
                    self.loading.onNext(false)
                    self.needsRefresh.toggle()
                }
                self.sumFormula.accept(self.sum(tableDetails: res.response!.details ?? []))
                var iconName = CommonStrings.empty
                if res.response?.comments != nil {
                    let comments = res.response!.comments ?? String()
                    iconName = (comments.trimmingCharacters(in: .whitespaces).isEmpty ?
                        ImageButtonNames.message : ImageButtonNames.messsageFill)
                } else {
                    iconName = ImageButtonNames.message
                }
                self.showIconComments.onNext(iconName)
                if isRefresh {
                    self.endRefreshing.onNext(())
                }
                self.changeColorLabelsHt.onNext(())
                self.catalogGroup = res.response?.catalogGroupName ?? String()
            }
        }, onError: { [weak self] error in
            guard let self = self else { return }
            if self.needsRefresh {
                self.loading.onNext(false)
                self.needsRefresh.toggle()
                print(error.localizedDescription)
            }
            if isRefresh {
                self.endRefreshing.onNext(())
            }
            self.showAlert.onNext(CommonStrings.formulaDetailCouldNotBeLoaded)
        }).disposed(by: self.disposeBag)
    }
    func terminateOrChangeStatusOfAnOrder(actionType: String) {
        switch actionType {
        case StatusNameConstants.finishedStatus:        // Realiza el proceso para terminar la orden
            self.validIfOrderCanBeFinalized(orderId: orderId)
        case StatusNameConstants.inProcessStatus:       // Realiza el proceso para cambiar el estatus a proceso
            self.changeStatus(actionType: actionType)
        case StatusNameConstants.penddingStatus:        // Realiza el proceso para cambiar el status a pendiente
            self.changeStatus(actionType: actionType)
        default:
            break
        }
    }
    func sum(tableDetails: [Detail]) -> Double {
        var sum = 0.0
        if tableDetails.count > 0 {
            for detail in tableDetails {
                // swiftlint:disable for_where
                if detail.unit != CommonStrings.piece {
                    sum += detail.requiredQuantity ?? 0.0
                }
            }
            return sum
        }
        return sum
    }
    func changeStatus(actionType: String, needsError: Bool = false, statusCode: Int = 500, testData: Data = Data()) {
        self.loading.onNext(true)
        let status = actionType == StatusNameConstants.inProcessStatus ? CommonStrings.process : CommonStrings.pending
        let changeStatus = ChangeStatusRequest(userId: (Persistence.shared.getUserData()?.id) ?? String(),
                                               orderId: (self.tempOrderDetailData?.productionOrderID) ?? 0,
                                               status: status)
        self.networkManager.changeStatusOrder(
            changeStatusRequest: [changeStatus], needsError: needsError,
            statusCode: statusCode, testData: testData)
            .observeOn(MainScheduler.instance).subscribe(onNext: {[weak self] _ in
            self?.rootViewModel.needsRefresh = true
            self?.loading.onNext(false)
            self?.backToInboxView.onNext(())
        }, onError: { [weak self] error in
            self?.loading.onNext(false)
            self?.showAlert.onNext(CommonStrings.errorToChangeStatus)
            print(error.localizedDescription)
            }).disposed(by: self.disposeBag)
    }

    func deleteItemFromTable(index: Int, needsError: Bool = false, statusCode: Int = 500, testData: Data = Data()) {
        self.loading.onNext(true)
        let itemToDelete = auxTabledata[index]
        let componets = [Component(
                            orderFabID: itemToDelete.orderFabID ?? 0, productId: itemToDelete.productID ?? String(),
                            componentDescription: itemToDelete.detailDescription ?? String(),
                            baseQuantity: itemToDelete.baseQuantity ?? 0.0,
                            requiredQuantity: itemToDelete.requiredQuantity ?? 0.0,
                            consumed: itemToDelete.consumed ?? 0.0, available: itemToDelete.available ?? 0.0,
                            unit: itemToDelete.unit ?? String(), warehouse: itemToDelete.warehouse ?? String(),
                            pendingQuantity: itemToDelete.pendingQuantity ?? 0.0,
                            stock: itemToDelete.stock ?? 0.0,
                            warehouseQuantity: itemToDelete.warehouseQuantity ?? 0.0, action: Actions.delete.rawValue)]

        let fechaFinFormated = UtilsManager.shared.formattedDateFromString(
            dateString: tempOrderDetailData?.dueDate ?? String(), withFormat: DateFormat.yyyymmdd)
        let order = OrderDetailRequest(
            fabOrderID: tempOrderDetailData?.productionOrderID ?? 0,
            plannedQuantity: tempOrderDetailData?.plannedQuantity ?? Decimal(0),
            fechaFin: fechaFinFormated ?? String(), comments: String(),
            warehouse: tempOrderDetailData?.warehouse ?? String(), components: componets)
        self.networkManager.updateDeleteItemOfTableInOrderDetail(
            orderDetailRequest: order, needsError: needsError,
            statusCode: statusCode, testData: testData)
            .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            if self?.tempOrderDetailData != nil {
                self?.loading.onNext(false)
                self?.tempOrderDetailData?.details?.remove(at: index)
                self?.auxTabledata = self?.tempOrderDetailData?.details ?? []
                self?.tableData.onNext(self?.tempOrderDetailData?.details ?? [])
                self?.sumFormula.accept(self?.sum(tableDetails: (self?.tempOrderDetailData?.details ?? [])) ?? 0.0)
            }
            }, onError: {  [weak self] error in
                self?.loading.onNext(false)
                self?.showAlert.onNext(CommonStrings.couldNotDeleteItem)
                print(error.localizedDescription)
        }).disposed(by: self.disposeBag)
    }
    func getDataTableToEdit() -> OrderDetail {
        return self.tempOrderDetailData!
    }
    // Valida si el usuario obtuvo las firmas y finaliza la orden
    func validSignatures(needsError: Bool = false, statusCode: Int = 500, testData: Data = Data()) {
        if self.technicalSignatureIsGet && self.qfbSignatureIsGet {
            self.loading.onNext(true)
            let finishOrder = FinishOrder(userId: Persistence.shared.getUserData()?.id ?? String(),
                                          fabricationOrderId: [self.orderId],
                                          qfbSignature: self.sqfbSignature,
                                          technicalSignature: self.technicalSignature)
            self.networkManager.finishOrder(
                order: finishOrder, needsError: needsError,
                statusCode: statusCode, testData: testData)
                .observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
                    guard let self = self else { return }
                    self.loading.onNext(false)
                    self.backToInboxView.onNext(())
                    self.rootViewModel.needsRefresh = true
                }, onError: { [weak self] error in
                    self?.loading.onNext(false)
                    print(error.localizedDescription)
                }).disposed(by: self.disposeBag)
        }
    }
    func validIfOrderCanBeFinalized(
        orderId: Int, needsError: Bool = false, statusCode: Int = 500, testData: Data = Data()) {
        self.loading.onNext(true)
        networkManager.validateOrders(
            orderIDs: [orderId], needsError: needsError, statusCode: statusCode, testData: testData)
            .subscribe(onNext: { [weak self] response in
                guard let self = self else { return }
                self.loading.onNext(false)
                guard response.code == 400, !(response.success ?? false) else {
                    self.showSignatureView.onNext(CommonStrings.signatureViewTitleQFB)
                    return
                }
                guard let errors = response.response, errors.count > 0 else { return }
                var messageConcat = String()
                for error in errors {
                    if error.type == .some(.batches) && error.listItems?.count ?? 0 > 0 {
                        messageConcat = UtilsManager.shared.messageErrorWhenNoBatches(error: error)
                    } else if error.type == .some(.stock) && error.listItems?.count ?? 0 > 0 {
                        let messageError = UtilsManager.shared.messageErrorWhenOutOfStock(error: error)
                        messageConcat = "\(messageConcat) \(messageError)"
                    }
                }
                self.showAlert.onNext(messageConcat)
            }, onError: { [weak self] _ in

                guard let self = self else { return }
                self.loading.onNext(false)
                self.showAlert.onNext(Constants.Errors.errorData.rawValue)

            }).disposed(by: disposeBag)
    }
    
    func getCatalogGroup() -> String {
        return catalogGroup
    }
}

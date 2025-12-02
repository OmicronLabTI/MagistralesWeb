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
    var orderDetailData = PublishSubject<[OrderDetail]>()
    weak var tempOrderDetailData: OrderDetail?
    var tableData = PublishSubject<[Detail]>()
    var showAlert: PublishSubject<String> = PublishSubject()
    var showAlertConfirmation = PublishSubject<MessageToChangeStatus>()
    var loading: PublishSubject<Bool> = PublishSubject()
    var sumFormula = PublishSubject<Double>()
    var auxTabledata: [Detail] = []
    var processButtonDidTap = PublishSubject<Void>()
    var finishedButtonDidTap = PublishSubject<Void>()
    var pendingButtonDidTap = PublishSubject<Void>()
    var seeLotsButtonDidTap = PublishSubject<Void>()
    var deleteManyButtonDidTap = PublishSubject<Void>()
    var deleteManyButtonIsEnable = PublishSubject<Bool>()
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
    var itemSelectedDetail: [Int] = []
    var showTwoModals = false
    var dataError = PublishSubject<String>()
    var disableSaveButton = PublishSubject<Void>()
    var clearComponentsToUpdate = PublishSubject<Void>()
    var updateObjectToSend: OrderDetailRequest = OrderDetailRequest(
        fabOrderID: 0, plannedQuantity: 0, fechaFin: "", comments: "", warehouse: "", components: []
    )
    var warehousesOptions: [String] = []
    var itemCode = String()
    var splitButtonEnableFlag = PublishSubject<String>()
    @Injected var rootViewModel: RootViewModel
    @Injected var inboxViewModel: InboxViewModel
    @Injected var networkManager: NetworkManager
    // MARK: - Init
    init() {
        finishBtnActionBinding()
        processBtnBinding()
        pendingBtnbinding()
        seeLotsBtnBinding()
    }
    
    // MARK: - Functions Binding
    // MARK: - FINISH BINDINGACTION
    func finishBtnActionBinding() {
        self.finishedButtonDidTap.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.validIfOrderCanBeFinalized(orderId: self.orderId)
        }).disposed(by: self.disposeBag)
    }
    // MARK: - DELETE MANY BUTTON BINDIND ACTION
    func deleteItemsFromTableDidTap() {
        self.deleteItemFromTable(indexs: self.itemSelectedDetail)
    }
    
    func processBtnBinding() {
        self.processButtonDidTap.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            let message = MessageToChangeStatus(
                message: CommonStrings.confirmationMessageProcessStatus,
                typeOfStatus: StatusNameConstants.inProcessStatus)
            self.showAlertConfirmation.onNext(message)
        }).disposed(by: disposeBag)
    }
    
    func pendingBtnbinding() {
        self.pendingButtonDidTap.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            let message = MessageToChangeStatus(message: CommonStrings.confirmationMessagePendingStatus,
                                                typeOfStatus: StatusNameConstants.penddingStatus)
            self.showAlertConfirmation.onNext(message)
        }).disposed(by: self.disposeBag)
    }
    
    func seeLotsBtnBinding() {
        self.seeLotsButtonDidTap.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self]  _ in
            guard let self = self else { return }
            self.goToSeeLotsViewController.onNext(())
        }).disposed(by: self.disposeBag)
    }
    
    func getOrdenDetail(isRefresh: Bool = false) {
        itemSelectedDetail = []
        deleteManyButtonIsEnable.onNext(false)
        loading.onNext(true)
        networkManager.getOrdenDetail(self.orderId)
            .observe(on: MainScheduler.instance)
            .subscribe(onNext: {[weak self] res in
                guard let self = self else { return }
                self.getComponentWarehouses(response: res, isRefresh: isRefresh)
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.onFaliedOrderDetail(isRefresh)
            }).disposed(by: self.disposeBag)
    }
    
    func onSuccessOrderDetail(response: OrderDetailResponse, _ isRefresh: Bool) {
        if let order = response.response, let details = order.details {
            orderDetailData.onNext([order])
            tableData.onNext(details)
            auxTabledata = details
            tempOrderDetailData = order
            needsRefresh(needsRefresh)
            sumFormula.onNext(self.sum(tableDetails: details))
            setComments(order: order)
            endRefreshingAction(isRefresh)
            changeColorLabelsHt.onNext(())
            catalogGroup = order.catalogGroupName ?? String()
            splitButtonEnableFlag.onNext(order.orderRelationType ?? String())
        }
    }
    
    func onFaliedOrderDetail(_ isRefresh: Bool) {
        self.needsRefresh(self.needsRefresh)
        self.endRefreshingAction(isRefresh)
        self.showAlert.onNext(CommonStrings.formulaDetailCouldNotBeLoaded)
    }
    
    func needsRefresh(_ needsRefresh: Bool) {
        if needsRefresh {
            self.loading.onNext(false)
            self.needsRefresh.toggle()
        }
    }
    
    func endRefreshingAction(_ isRefresh: Bool) {
        if isRefresh {
            self.endRefreshing.onNext(())
        }
    }
    
    func getCatalogGroup() -> String {
        return warehousesOptions.first ?? catalogGroup
    }
    
    func updateComponents() {
        let request = updateObjectToSend
        self.loading.onNext(true)
        networkManager.updateDeleteItemOfTableInOrderDetail(request).subscribe(onNext: {[weak self] res in
            guard let self = self else { return }
            guard let response = res.response else { return }
            self.loading.onNext(false)
            if (res.code == 200) {
                self.showAlert.onNext(CommonStrings.processSuccess)
                clearComponentsToUpdate.onNext(())
                disableSaveButton.onNext(())
                updateObjectToSend.components = []
                return
            }
            let errorMessage = UtilsManager.shared.getResponseErrors(jsonString: response)
            self.showAlert.onNext(errorMessage)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorSaveLots)
        }).disposed(by: disposeBag)
    }
    
    func getComponentWarehouses(response: OrderDetailResponse, isRefresh: Bool = false) {
        let itemcode = response.response?.code ?? String()
        self.itemCode = itemcode
        networkManager.getWarehouses(itemcode).subscribe(onNext: {
            [weak self] res in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.warehousesOptions = res.response
            self.clearComponentsToUpdate.onNext(())
            self.onSuccessOrderDetail(response: response, isRefresh)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.onSuccessOrderDetail(response: response, isRefresh)
            self.loading.onNext(false)
            self.dataError.onNext(Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag)
    }
    
    func cancelChildOrderRequest(request: CancelChildOrderRequest) {
        let req: [CancelChildOrderRequest] = [request]
        self.loading.onNext(true)
        networkManager.cancelChildOrder(req).subscribe(onNext: {
            [weak self] res in
            guard let self = self else { return }
            self.loading.onNext(false)
            if res.code == 200 {
                self.showAlert.onNext(CommonStrings.processSuccess)
                return
            }
        }, onError: {
            [weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.dataError.onNext(Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag)
    }
}

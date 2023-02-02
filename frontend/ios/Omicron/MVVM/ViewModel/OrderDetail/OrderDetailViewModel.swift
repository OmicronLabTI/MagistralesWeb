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
    var deleteManyButtonDidTap = PublishSubject<Void>()
    var deleteManyButtonIsEnable = BehaviorSubject<Bool>(value: false)
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
    @Injected var rootViewModel: RootViewModel
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
        self.finishedButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            let message = MessageToChangeStatus(
                message: CommonStrings.doYouWantToFinishTheOrder, typeOfStatus: StatusNameConstants.finishedStatus)
            self.showAlertConfirmation.onNext(message)
        }).disposed(by: self.disposeBag)
    }
    // MARK: - DELETE MANY BUTTON BINDIND ACTION
    func deleteItemsFromTableDidTap() {
        self.deleteItemFromTable(indexs: self.itemSelectedDetail)
    }

    func processBtnBinding() {
        self.processButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            let message = MessageToChangeStatus(
                message: CommonStrings.confirmationMessageProcessStatus,
                typeOfStatus: StatusNameConstants.inProcessStatus)
            self.showAlertConfirmation.onNext(message)
        }).disposed(by: disposeBag)
    }

    func pendingBtnbinding() {
        self.pendingButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            let message = MessageToChangeStatus(message: CommonStrings.confirmationMessagePendingStatus,
                                                typeOfStatus: StatusNameConstants.penddingStatus)
            self.showAlertConfirmation.onNext(message)
        }).disposed(by: self.disposeBag)
    }

    func seeLotsBtnBinding() {
        self.seeLotsButtonDidTap.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self]  _ in
            guard let self = self else { return }
            self.goToSeeLotsViewController.onNext(())
        }).disposed(by: self.disposeBag)
    }

    func getOrdenDetail(isRefresh: Bool = false) {
        itemSelectedDetail = []
        deleteManyButtonIsEnable.onNext(false)
        if needsRefresh { loading.onNext(true) }
        networkManager.getOrdenDetail(self.orderId)
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: {[weak self] res in
                guard let self = self else { return }
                self.rootViewModel.requireTechnical = res.response?.requireTechnical ?? false
                self.onSuccessOrderDetail(response: res, isRefresh)
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.onFaliedOrderDetail(isRefresh)
            }).disposed(by: self.disposeBag)
    }

    func onSuccessOrderDetail(response: OrderDetailResponse, _ isRefresh: Bool) {
        if let order = response.response, let details = order.details {
            orderDetailData.accept([order])
            tableData.onNext(details)
            auxTabledata = details
            tempOrderDetailData = order
            needsRefresh(needsRefresh)
            sumFormula.accept(self.sum(tableDetails: details))
            setComments(order: order)
            endRefreshingAction(isRefresh)
            changeColorLabelsHt.onNext(())
            catalogGroup = order.catalogGroupName ?? String()
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
        return catalogGroup
    }
}

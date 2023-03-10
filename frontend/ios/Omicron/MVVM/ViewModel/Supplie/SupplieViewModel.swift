//
//  SupplieViewModel.swift
//  Omicron
//
//  Created by Daniel Vargas on 28/02/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//
import Foundation
import RxSwift
import RxCocoa
import CryptoKit
import Resolver

class SupplieViewModel {
    var disposeBag: DisposeBag = DisposeBag()
    var supplieList: [Supplie] = []
    @Injected var networkManager: NetworkManager
    var componentsList = PublishSubject<[Supplie]>()
    var componentsListToDelete = PublishSubject<[String]>()
    let addComponent = PublishSubject<ComponentO>()
    var selectedComponentsToDelete: [String] = []
    let selectedButtonIsEnable = PublishSubject<Bool>()
    let deleteComponents = PublishSubject<Void>()
    var showSuccessAlert: PublishSubject<(title: String, msg: String, autoDismiss: Bool)> =
    PublishSubject<(title: String, msg: String, autoDismiss: Bool)>()
    let isSendToStoreEnabled = PublishSubject<Bool>()
    let sendToStore = PublishSubject<String>()
    let loading = PublishSubject<Bool>()
    let returnBack = PublishSubject<Void>()
    init() {
        bindProperties()
        bindSendToStore()
    }
    func bindProperties() {
        self.addComponent.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] supplie in
            guard let self = self else { return }
            let exists = self.supplieList.firstIndex(where: { $0.productId == supplie.productId })
            if exists != nil {
                let productId = supplie.productId ?? ""
                self.showSuccessAlert.onNext((
                    title: "El componente \(productId) ya existe para esta solicitud",
                    msg: String(),
                    autoDismiss: true))
                return
            }
            let newSupplie = self.convertSupplie(supplie)
            self.supplieList.insert(newSupplie, at: 0)
            self.componentsList.onNext(self.supplieList)
            self.selectedComponentsToDelete = []
            self.selectedButtonIsEnable.onNext(self.selectedComponentsToDelete.count > 0)
            self.validateSendToStoreIsEnabled()
        }).disposed(by: disposeBag)

        self.deleteComponents.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.deleteSelectedComponents()
        }).disposed(by: disposeBag)
    }
    func bindSendToStore() {
        self.sendToStore.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] observations in
            guard let self = self else { return }
            self.sentToStoreService(observations)
        }).disposed(by: disposeBag)
    }

    func sentToStoreService(_ observations: String) {
        let dataReq = DataStore(productionOrderIds: [],
                           signature: "",
                           observations: observations,
                           orderedProducts: self.supplieList)
        let userId = Persistence.shared.getUserData()?.id ?? ""
        let req = SendToStoreRequest(data: dataReq, userId: userId)
        self.loading.onNext(true)
        networkManager.createComponentsOrder(req)
            .subscribe(onNext: {[weak self] res in
                guard let self = self else { return }
                self.loading.onNext(false)
                if res.response != nil {
                    self.onSuccessResponse(res: res)
                    return
                }
                let error = res.userError ?? CommonStrings.errorComponents
                self.showSuccessAlert.onNext((
                    title: "Error \n\(error)",
                    msg: String(),
                    autoDismiss: false))
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.showSuccessAlert.onNext((
                    title: "Error \n\(CommonStrings.errorComponents)",
                    msg: String(),
                    autoDismiss: false))
            }).disposed(by: self.disposeBag)
    }

    func onSuccessResponse(res: CreateComponentsOrderResponse) {
        guard let response = res.response else { return }
        let faileds = response.failed
        if res.success == true && response.failed.count > 0 {
            let error = generateErrorMessage(errors: faileds)
            self.showSuccessAlert.onNext((
                title: "Error \n\(error)",
                msg: String(),
                autoDismiss: false
            ))
        } else {
            self.supplieList = []
            self.selectedComponentsToDelete = []
            self.componentsList.onNext(self.supplieList)
            self.returnBack.onNext(())
        }
    }
    func generateErrorMessage(errors: [ProductionOrder?]) -> String {
        return errors.reduce(String(), { _, error in
            let id = error?.productionOrderId ?? 0
            return "Ya se ha generado una solicitud para la orden \(id) \n"
        })
    }
    func convertSupplie(_ supplie: ComponentO) -> Supplie {
        let newSupplie = Supplie()
        newSupplie.orderFabId = supplie.orderFabId
        newSupplie.productId = supplie.productId
        newSupplie.description = supplie.description
        newSupplie.baseQuantity = supplie.baseQuantity
        newSupplie.requiredQuantity = supplie.requiredQuantity
        newSupplie.consumed = supplie.consumed
        newSupplie.available = supplie.available
        newSupplie.unit = supplie.unit
        newSupplie.warehouse = supplie.warehouse
        newSupplie.pendingQuantity = supplie.pendingQuantity
        newSupplie.stock = supplie.stock
        newSupplie.warehouseQuantity = supplie.warehouseQuantity
        newSupplie.requestQuantity = 0
        newSupplie.isLabel = supplie.isLabel
        return newSupplie
    }
    func deleteSelectedComponents() {
        let isSingular = selectedComponentsToDelete.count == 1
        selectedComponentsToDelete.forEach { itemCode in
            let indexToDelete = supplieList.firstIndex(where: { $0.productId == itemCode })
            if indexToDelete != nil {
                supplieList.remove(at: indexToDelete ?? 0)
            }
        }
        componentsList.onNext(self.supplieList)
        selectedComponentsToDelete = []
        self.selectedButtonIsEnable.onNext(selectedComponentsToDelete.count > 0)
        self.validateSendToStoreIsEnabled()
        let message = isSingular ? CommonStrings.successDeleteSingular :
            CommonStrings.successDeletePlural
            self.showSuccessAlert.onNext((title: "\(message)",
                                      msg: String(),
                                      autoDismiss: true))
    }
    func validateItemsToDelete(itemCode: String) {
        let existIndex = selectedComponentsToDelete.firstIndex(where: { $0 == itemCode })
        if existIndex == nil {
            selectedComponentsToDelete.append(itemCode)
        } else {
            selectedComponentsToDelete.remove(at: existIndex ?? 0)
        }
        self.selectedButtonIsEnable.onNext(selectedComponentsToDelete.count > 0)
    }
    func validateExistsInList(itemCode: String) -> Bool {
        selectedComponentsToDelete.firstIndex(where: { $0 == itemCode }) != nil
    }
    func changeQuantityPieces(itemCode: String, quantity: Double) {
        let selectedIndex = supplieList.firstIndex(where: { $0.productId == itemCode })
        guard let index = selectedIndex else { return }
        supplieList[index].requestQuantity = quantity
        validateSendToStoreIsEnabled()
    }
    func validateSendToStoreIsEnabled() {
        let allHasQuantity = supplieList.allSatisfy { ($0.requestQuantity ?? 0) > 0 }
        isSendToStoreEnabled.onNext(allHasQuantity && supplieList.count > 0)
    }
    func getDeleteMessageBody() -> String {
        return self.selectedComponentsToDelete.count == 1 ?
            CommonStrings.confirmDeleteSingular :
            CommonStrings.confirmDeletePlural
    }

}

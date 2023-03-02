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
    var supplieList: [ComponentO] = []
    @Injected var networkManager: NetworkManager
    let componentsList = BehaviorSubject<[ComponentO]>(value: [])
    let componentsListToDelete = BehaviorSubject<[String]>(value: [])
    let addComponent = PublishSubject<ComponentO>()
    var selectedComponentsToDelete: [String] = []
    let selectedButtonIsEnable = PublishSubject<Bool>()
    let deleteComponents = PublishSubject<Void>()
    var showSuccessAlert: PublishSubject<(title: String, msg: String, autoDismiss: Bool)> =
    PublishSubject<(title: String, msg: String, autoDismiss: Bool)>()
    init() {
        bindProperties()
    }
    func bindProperties() {
        self.addComponent.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] supplie in
            guard let self = self else { return }
            let exists = self.supplieList.firstIndex(where: { $0.productId == supplie.productId })
            if exists != nil {
                self.showSuccessAlert.onNext((
                    title: "Error",
                    msg: "El componente \(supplie.productId) ya existe para esta solicitud",
                    autoDismiss: true))
                return
            }
            self.supplieList.insert(supplie, at: 0)
            self.componentsList.onNext(self.supplieList)
            self.selectedComponentsToDelete = []
            self.selectedButtonIsEnable.onNext(self.selectedComponentsToDelete.count > 0)
        }).disposed(by: disposeBag)
        self.deleteComponents.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.deleteSelectedComponents()
        }).disposed(by: disposeBag)
    }
    func deleteSelectedComponents() {
        selectedComponentsToDelete.forEach { itemCode in
            let indexToDelete = supplieList.firstIndex(where: { $0.productId == itemCode })
            if indexToDelete != nil {
                supplieList.remove(at: indexToDelete ?? 0)
            }
        }
        componentsList.onNext(self.supplieList)
        selectedComponentsToDelete = []
        self.selectedButtonIsEnable.onNext(selectedComponentsToDelete.count > 0)
        self.showSuccessAlert.onNext((title: "Error",
                                      msg: "Los componentes se han eliminado correctamente",
                                      autoDismiss: false))
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
}

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
    let showSuccessAlert = PublishSubject<String>()

    init() {
        bindProperties()
    }
    func bindProperties() {
        self.addComponent.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] supplie in
            guard let self = self else { return }
            self.supplieList.insert(supplie, at: 0)
            self.componentsList.onNext(self.supplieList)
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
        self.showSuccessAlert.onNext("Los componentes se han eliminado correctamente")
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

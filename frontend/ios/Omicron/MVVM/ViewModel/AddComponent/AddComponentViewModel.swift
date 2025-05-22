//
//  AddComponentViewModel.swift
//  Omicron
//
//  Created by Daniel Vargas on 20/05/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa
import CryptoKit
import Resolver

class AddComponentViewModel {
    var disposeBag: DisposeBag = DisposeBag()
    @Injected var networkManager: NetworkManager
    var loading = PublishSubject<Bool>()
    var showAlert = PublishSubject<String>()
    var renderProducts = PublishSubject<[AddComponent]>()
    var products: [AddComponent] = []
    var dataLotsAvailable = BehaviorSubject<[LotsAvailable]>(value: [])

    
    func getLotsByProduct(component: ComponentFormValues) {
        let productId = component.selectedComponent.productId ?? String()
        if (products.contains { $0.productId == productId }) {
            self.showAlert.onNext("El componente \(productId) ya existe para esta solicitud")
            return
        }
        // self.loading.onNext(true)
        networkManager.getLotsByProductAndWarehouse(warehouseCode: component.warehouse, product: productId).subscribe(onNext: {[weak self] res in
            let product = res.response
            guard let self = self else { return }
            let newProduct = AddComponent(productId: productId,
                                          description: component.selectedComponent.description ?? String(),
                                          warehouse: component.warehouse,
                                          availableLots: product?.lotes ?? [],
                                          selectedLots: [],
                                          requiredQuantity: component.requiredQuantity,
                                          selectedQuantity: 0,
                                          baseQuantity: component.baseQuantity,
                                          totalNecesary: Decimal(component.requiredQuantity),
                                          selectedTotal: 0)
            self.products.append(newProduct)
            self.renderProducts.onNext(self.products)
            self.dataLotsAvailable.onNext(newProduct.availableLots)
            // self.dataOfLots.onNext(self.products)
            // self.loading.onNext(false)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorGetLotsByProduct)
        }).disposed(by: disposeBag)
    }
}

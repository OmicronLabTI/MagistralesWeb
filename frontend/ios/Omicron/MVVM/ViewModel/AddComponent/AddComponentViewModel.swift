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
    var dataLotsSelected = BehaviorSubject<[LotsSelected]>(value: [])
    var selectLineDocIndex = PublishSubject<Int>()

    var selectedLineDoc = 0
    
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
            self.selectedLineDoc = products.count - 1
            self.selectLineDocIndex.onNext(self.selectedLineDoc)
            self.dataLotsAvailable.onNext(newProduct.availableLots)
            self.dataLotsSelected.onNext(newProduct.selectedLots)
            // self.loading.onNext(false)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorGetLotsByProduct)
        }).disposed(by: disposeBag)
    }
    
    func selectedQuantityChange(row: Int) {
        let product = self.products[selectedLineDoc]
        let available = product.availableLots[row]
    
        // valida si la cantidad es cero o si es mayor al necesario, en este caso no hacer nada
        let quantity = available.cantidadSeleccionada ?? 0
        if quantity == 0 || quantity > product.totalNecesary || available.cantidadDisponible
            == 0 || quantity > (available.cantidadDisponible ?? 0) {
            return
        }

        // en caso de que si restarle el total necesario esta cantidad
        product.totalNecesary = (product.totalNecesary) - quantity
        product.selectedTotal = product.selectedLots.compactMap({ $0.cantidadSeleccionada }).reduce(0, +)
        product.availableLots.forEach({ lot in
            if lot.numeroLote == available.numeroLote {
                lot.cantidadDisponible = (lot.cantidadDisponible ?? 0) - quantity
            }
            lot.cantidadSeleccionada = min(product.totalNecesary, lot.cantidadDisponible ?? 0)
        })
        /*if let availableBatches = product.lotesDisponibles {
            self?.dataLotsAvailable.onNext(availableBatches)
        }
        self?.dataOfLots.onNext(self?.documentLines ?? [])*/
    }
}

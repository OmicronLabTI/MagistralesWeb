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
    var updateProduct = PublishSubject<(AddComponent, Int)>()
    var deleteButtonEnabled = PublishSubject<Bool>()
    var addButtonEnabled = PublishSubject<Bool>()
    var saveButtonEnabled = PublishSubject<Bool>()
    var details: [Detail] = []
    var returnBackPage = PublishSubject<Void>()

    var selectedLineDoc = -1
    var selectedAvailable = -1
    var selectedLotsSelected = -1
    
    func getLotsByProduct(component: ComponentFormValues) {
        let productId = component.selectedComponent.productId ?? String()
        if (products.contains { $0.productId == productId } || details.contains { $0.productID == productId }) {
            self.showAlert.onNext("El componente \(productId) ya existe para esta solicitud")
            return
        }
        let newProduct = AddComponent(productId: productId,
                                      description: component.selectedComponent.description ?? String(),
                                      warehouse: component.warehouse,
                                      availableLots: [],
                                      selectedLots: [],
                                      requiredQuantity: component.requiredQuantity,
                                      selectedQuantity: 0,
                                      baseQuantity: component.baseQuantity,
                                      totalNecesary: UtilsManager.shared.doubleToDecimal(value: component.requiredQuantity),
                                      selectedTotal: 0,
                                      componentInfo: component.selectedComponent,
                                      unit: component.selectedComponent.unit ?? String(),
                                      managedByBatches: component.selectedComponent.managedByBatches ?? false)
        
        loadLotsByProductAndWarehouse(productId: productId, warehouseCode: component.warehouse, product: newProduct, callback: addNewComponent)
    }

    func addNewLot(quantity: Decimal) {
        let row = self.selectedAvailable
        let product = self.products[selectedLineDoc]
        let available = product.availableLots[row]
    

        let quantityDecimal = quantity as NSDecimalNumber
        let totalNecesaryDecimal = product.totalNecesary as NSDecimalNumber
        
        let first = quantityDecimal.compare(totalNecesaryDecimal) == .orderedDescending
        let second = available.cantidadDisponible == 0
        let third = quantity > (available.cantidadDisponible ?? 0)

        if quantity == 0 || first || second || third {
            return
        }
        
        // primero agregar este nuevo lote a selectedLots si no existe, en caso contrario solo sumar
        let selectedLotIndex = product.selectedLots.firstIndex(where: ({ $0.numeroLote == available.numeroLote }))
        if (selectedLotIndex != nil) {
            product.selectedLots[selectedLotIndex ?? 0].cantidadSeleccionada! += quantity
        } else {
            product.selectedLots.append(LotsSelected(numeroLote: available.numeroLote ?? String(),
                                                     cantidadSeleccionada: quantity,
                                                     sysNumber: available.sysNumber ?? 0,
                                                     expiredBatch: available.expiredBatch))
        }
        // restarle a cantidad disponible la cantidad usada
        product.availableLots[row].cantidadDisponible! -= quantity
        self.calcValues(product: product)
        self.addButtonEnabled.onNext(false)
        self.validateSaveButton()
    }
    
    func deleteSelectedLot() {
        let product = products[self.selectedLineDoc]
        let selectedLot = product.selectedLots[selectedLotsSelected]
        
        let availableIndex = product.availableLots.firstIndex(where: { $0.numeroLote == selectedLot.numeroLote })
        if (availableIndex != nil) {
            product.availableLots[availableIndex ?? 0].cantidadDisponible! += selectedLot.cantidadSeleccionada ?? 0
            product.selectedLots.remove(at: selectedLotsSelected)
        } else {
            return
        }

        calcValues(product: product)
        self.selectedLotsSelected = -1
        self.deleteButtonEnabled.onNext(false)
        validateSaveButton()
    }
    
    func calcValues(product: AddComponent)  {
        calculateSelectedQuanties(product: product)
        self.updateProduct.onNext((product, self.selectedLineDoc))
        self.dataLotsAvailable.onNext(product.availableLots)
        self.dataLotsSelected.onNext(product.selectedLots)
    }
    
    func calculateSelectedQuanties(product: AddComponent) {
        // despues hacer los calculos del total necesario, total seleccionado
        let selectedsQuantityTotal = product.selectedLots.compactMap({ $0.cantidadSeleccionada }).reduce(0, +)
        product.totalNecesary = UtilsManager.shared.doubleToDecimal(value: product.requiredQuantity) - selectedsQuantityTotal
        product.selectedTotal = selectedsQuantityTotal

        product.availableLots.forEach({ lot in
            lot.cantidadSeleccionada = min(product.totalNecesary, lot.cantidadDisponible ?? 0)
        })
    }
    
    func deleteComponent(row: Int) {
        products.remove(at: row)
        renderProducts.onNext(products)
        selectedLineDoc = -1
        selectedAvailable = -1
        selectedLotsSelected = -1
        dataLotsAvailable.onNext([])
        dataLotsSelected.onNext([])
        addButtonEnabled.onNext(false)
        deleteButtonEnabled.onNext(false)
        validateSaveButton()
    }
    
    func resetValues() {
        products = []
        selectedLineDoc = -1
        selectedAvailable = -1
        selectedLotsSelected = -1
        renderProducts.onNext([])
        dataLotsAvailable.onNext([])
        dataLotsSelected.onNext([])
        saveButtonEnabled.onNext(false)
    }
    
    func validateSaveButton() {
        if products.isEmpty {
            saveButtonEnabled.onNext(false)
            return
        }
        
        let allBatchesAssigned = products.allSatisfy(({ ($0.totalNecesary == 0 && $0.managedByBatches) || !$0.managedByBatches}))
        saveButtonEnabled.onNext(allBatchesAssigned)
    }

    func saveChanges(detail: OrderDetail) {
        var components: [Component] = []
        products.forEach(({
            let product = $0
            let assignedBatches: [AssignedBatch] = product.selectedLots
                .map(({ return AssignedBatch(assignedQty: NSDecimalNumber(decimal:  $0.cantidadSeleccionada ?? 0.0).doubleValue,
                                             batchNumber: $0.numeroLote ?? String(),
                                             areBatchesComplete: 1,
                                             sysNumber: $0.sysNumber ?? 0)
            }))
            
            let newComponent = Component(orderFabID: 0,
                                         productId: product.productId,
                                         componentDescription: product.description,
                                         baseQuantity: product.baseQuantity,
                                         requiredQuantity: product.requiredQuantity,
                                         consumed: NSDecimalNumber(decimal: product.componentInfo.consumed ?? 0.0).doubleValue,
                                         available: NSDecimalNumber(decimal: product.componentInfo.available ?? 0.0).doubleValue,
                                         unit: product.componentInfo.unit ?? String(),
                                         warehouse: product.warehouse,
                                         pendingQuantity: 0,
                                         stock: NSDecimalNumber(decimal: product.componentInfo.stock ?? 0.0).doubleValue,
                                         warehouseQuantity: NSDecimalNumber(decimal: product.componentInfo.warehouseQuantity ?? 0.0).doubleValue,
                                         action: "insert",
                                         assignedBatches: assignedBatches)
            
            components.append(newComponent)
        }))
        
        let fechaFinFormated = UtilsManager.shared.formattedDateFromString(
            dateString: detail.dueDate ?? String(), withFormat: DateFormat.yyyymmdd)
        let request = OrderDetailRequest(fabOrderID: detail.productionOrderID ?? 0,
                                         plannedQuantity: detail.plannedQuantity ?? 0,
                                         fechaFin: fechaFinFormated ?? String(),
                                         comments: detail.comments ?? String(),
                                         warehouse: detail.warehouse ?? String(),
                                         components: components)

        self.loading.onNext(true)
        networkManager.updateDeleteItemOfTableInOrderDetail(request).subscribe(onNext: {[weak self] res in
            guard let self = self else { return }
            guard let response = res.response else { return }
            self.loading.onNext(false)
            if (res.code == 200) {
                self.returnBackPage.onNext(())
                return
            }

            let errorMessage = getResponseErrors(jsonString: response)
            self.showAlert.onNext(errorMessage)
        }, onError: { [weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorSaveLots)
        }).disposed(by: disposeBag)
    }
    
    func getResponseErrors(jsonString: String) -> String {
        guard let data = jsonString.data(using: .utf8) else {
                return ""
        }
        
        do {
            if let dictionary = try JSONSerialization.jsonObject(with: data, options: []) as? [String: String] {
                // Ordena las keys alfabéticamente y extrae los valores
                let valores = dictionary.keys.sorted().compactMap { dictionary[$0] }
                return valores.joined(separator: ", ")
            } else {
                return ""
            }
        } catch {
            return ""
        }
    }

    func changeWarehouseCode(productId: String, warehouseCode: String) {
        let productIndex = products.firstIndex(where: ({ $0.productId == productId }))
        if (productIndex != nil) {
            products[productIndex ?? 0].warehouse = warehouseCode
            loadLotsByProductAndWarehouse(productId: productId, warehouseCode: warehouseCode, product: products[productIndex ?? 0], callback: editComponent)
        }
    }
    
    
    func editComponent(availableLots: [LotsAvailable], newProduct: AddComponent) -> Void {
            newProduct.availableLots = availableLots
            newProduct.selectedLots = []
            let productIndex = products.firstIndex(where: ({ $0.productId == newProduct.productId })) ?? 0
            self.calculateSelectedQuanties(product: newProduct)
            self.updateProduct.onNext((newProduct, productIndex))
            self.dataLotsAvailable.onNext(newProduct.availableLots)
            self.dataLotsSelected.onNext(newProduct.selectedLots)
            self.selectedLineDoc = productIndex
            self.deleteButtonEnabled.onNext(false)
            self.addButtonEnabled.onNext(false)
            self.validateSaveButton()
    }
    
    func addNewComponent(availableLots: [LotsAvailable], newProduct: AddComponent) -> Void {
            newProduct.availableLots = availableLots
            calculateSelectedQuanties(product: newProduct)
            products.append(newProduct)
            renderProducts.onNext(self.products)
            selectedLineDoc = products.count - 1
            selectLineDocIndex.onNext(self.selectedLineDoc)
            dataLotsAvailable.onNext(newProduct.availableLots)
            dataLotsSelected.onNext(newProduct.selectedLots)
            validateSaveButton()
    }
    
    func loadLotsByProductAndWarehouse(productId: String,
                                       warehouseCode: String,
                                       product: AddComponent,
                                       callback: @escaping ([LotsAvailable], AddComponent) -> Void) {
        self.loading.onNext(true)
        networkManager.getLotsByProductAndWarehouse(warehouseCode: warehouseCode, product: productId)
            .subscribe(onNext: { [weak self] res in
                guard self != nil else { return }
                guard let self = self else { return }
                guard let productResponse = res.response else { return }
                callback(productResponse.lotes, product)
                self.loading.onNext(false)
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.showAlert.onNext(CommonStrings.errorGetLotsByProduct)
            }).disposed(by: disposeBag)
    }

}

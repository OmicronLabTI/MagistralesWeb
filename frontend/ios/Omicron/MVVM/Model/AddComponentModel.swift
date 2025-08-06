//
//  AddComponentModel.swift
//  Omicron
//
//  Created by Daniel Vargas on 20/05/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//
import ObjectMapper

class AddComponent {
    var productId: String
    var description: String
    var warehouse: String
    var availableLots: [LotsAvailable]
    var selectedLots: [LotsSelected]
    var requiredQuantity, selectedQuantity, baseQuantity: Double
    var totalNecesary, selectedTotal: Decimal
    var componentInfo: ComponentO
    var unit: String
    var managedByBatches: Bool
    var warehouses: [String]
    init(productId: String, description: String, warehouse: String, availableLots: [LotsAvailable], selectedLots: [LotsSelected], requiredQuantity: Double, selectedQuantity: Double, baseQuantity: Double, totalNecesary: Decimal, selectedTotal: Decimal, componentInfo: ComponentO, unit: String, managedByBatches: Bool, warehouses: [String]) {
        self.productId = productId
        self.description = description
        self.warehouse = warehouse
        self.availableLots = availableLots
        self.selectedLots = selectedLots
        self.requiredQuantity = requiredQuantity
        self.selectedQuantity = selectedQuantity
        self.baseQuantity = baseQuantity
        self.totalNecesary = totalNecesary
        self.selectedTotal = selectedTotal
        self.componentInfo = componentInfo
        self.unit = unit
        self.managedByBatches = managedByBatches
        self.warehouses = warehouses
    }
}

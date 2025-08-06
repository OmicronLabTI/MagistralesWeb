//
//  ComponentsModel.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

class ComponentRequest: Codable {
    var offset: Int?
    var limit: Int?
    var chips: [String]?
    var catalogGroup: String
    var userId: String
    init(offset: Int?, limit: Int?, chips: [String]?, catalogGroup: String, userId: String) {
        self.offset = offset
        self.limit = limit
        self.chips = chips
        self.catalogGroup = catalogGroup
        self.userId = userId
    }
    func toDictionary() -> [String: Any] {
        var dict = [String: Any]()
        let otherSelf = Mirror(reflecting: self)
        for child in otherSelf.children {
            if let key = child.label {
                if let arr = child.value as? [String] {
                    dict[key] = arr.joined(separator: ",")
                } else if (child.value is String) && (child.value as? String != "") || !(child.value is String) {
                    dict[key] = child.value
                }
            }
        }
        return dict
    }
}
class ComponentResponse: HttpResponse {
    var response: [ComponentO]?
    var comments: Int?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
        comments <- map["comments"]
    }
}
class ComponentO: Mappable, Codable {
    var orderFabId: Int?
    var productId: String?
    var description: String?
    var baseQuantity: Decimal?
    var requiredQuantity: Decimal?
    var consumed: Decimal?
    var available: Decimal?
    var unit: String?
    var warehouse: String?
    var pendingQuantity: Decimal?
    var stock: Decimal?
    var warehouseQuantity: Decimal?
    var isLabel, managedByBatches: Bool?
    required init?(map: Map) {}
    init() { }
    func mapping(map: Map) {
        self.orderFabId <- map["orderFabId"]
        self.productId <- map["productId"]
        self.description <- map["description"]
        self.baseQuantity <- (map["baseQuantity"], DecimalTransform())
        self.requiredQuantity <- (map["requiredQuantity"], DecimalTransform())
        self.consumed <- (map["consumed"], DecimalTransform())
        self.available <- (map["available"], DecimalTransform())
        self.unit <- map["unit"]
        self.warehouse <- map["warehouse"]
        self.pendingQuantity <- (map["pendingQuantity"], DecimalTransform())
        self.stock <- (map["stock"], DecimalTransform())
        self.warehouseQuantity <- (map["warehouseQuantity"], DecimalTransform())
        self.isLabel <- map["isLabel"]
        self.managedByBatches <- map["managedByBatches"]
    }
}
class Supplie: Codable {
    var orderFabId: Int?
    var productId: String?
    var description: String?
    var baseQuantity: Decimal?
    var requiredQuantity: Decimal?
    var consumed: Decimal?
    var available: Decimal?
    var unit: String?
    var warehouse: String?
    var pendingQuantity: Decimal?
    var stock: Decimal?
    var warehouseQuantity: Decimal?
    var isLabel: Bool?
    var requestQuantity: Decimal?
    init() { }
    required init?(map: Map) {
        self.requestQuantity <- map["requestQuantity"]
        self.orderFabId <- map["orderFabId"]
        self.productId <- map["productId"]
        self.description <- map["description"]
        self.baseQuantity <- (map["baseQuantity"], DecimalTransform())
        self.requiredQuantity <- (map["requiredQuantity"], DecimalTransform())
        self.consumed <- (map["consumed"], DecimalTransform())
        self.available <- (map["available"], DecimalTransform())
        self.unit <- map["unit"]
        self.warehouse <- map["warehouse"]
        self.pendingQuantity <- (map["pendingQuantity"], DecimalTransform())
        self.stock <- (map["stock"], DecimalTransform())
        self.warehouseQuantity <- (map["warehouseQuantity"], DecimalTransform())
        self.isLabel <- map["isLabel"]
    }
}
class ComponentFormValues {
    var baseQuantity: Double
    var requiredQuantity: Double
    var warehouse: String
    var selectedComponent: ComponentO
    var warehouses: [String]
    init(baseQuantity: Double, requiredQuantity: Double,
         warehouse: String,
         selectedComponent: ComponentO,
         warehouses: [String]) {
        self.baseQuantity = baseQuantity
        self.requiredQuantity = requiredQuantity
        self.warehouse = warehouse
        self.selectedComponent = selectedComponent
        self.warehouses = warehouses
    }
}

class CommonComponentRequest {
    var catalogGroup: String
    var userId: String
    var type: String
    init(catalogGroup: String, userId: String, type: String) {
        self.catalogGroup = catalogGroup
        self.userId = userId
        self.type = type
    }
    func toDictionary() -> [String: Any] {
        var dict = [String: Any]()
        let otherSelf = Mirror(reflecting: self)
        for child in otherSelf.children {
            if let key = child.label {
                if let arr = child.value as? [String] {
                    dict[key] = arr.joined(separator: ",")
                } else {
                    dict[key] = child.value
                }
            }
        }
        return dict
    }
}

enum TypeComponentsOpenDialog: String {
    case supplies = "Supplies"
    case detailOrder = "DetailOrder"
    case bulkOrder = "BulkOrder"
}

enum TypeMostCommonRequest: String {
    case detailOrder = "detailOrder"
    case inputRequest = "inputRequest"
    case bulkOrder = "bulkOrder"
}

class SendToStoreRequest: Codable {
    var data: DataStore?
    var userId: String?
    init (data: DataStore, userId: String) {
        self.data = data
        self.userId = userId
    }
}

class DataStore: Codable {
    var productionOrderIds: [String]
    var signature: String
    var observations: String
    var orderedProducts: [Supplie]
    init (productionOrderIds: [String],
          signature: String,
          observations: String,
          orderedProducts: [Supplie]) {
        self.productionOrderIds = productionOrderIds
        self.signature = signature
        self.observations = observations
        self.orderedProducts = orderedProducts
    }
}

class CreateComponentsOrderResponse: HttpResponse {
    var response: CreateComponentsOrders?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

class CreateComponentsOrders: Mappable {
    var success: [ProductionOrder?]
    var failed: [ProductionOrder?]
    required init?(map: Map) {
        self.success = []
        self.failed = []
    }
    func mapping(map: Map) {
        self.success <- map["success"]
        self.failed <- map["failed"]
    }
}
class ProductionOrder {
    var productionOrderId: Int?
    var reason: String?
}


class WarehouseResponse: HttpResponse {
    var response: [String]
    required init?(map: Map) {
        self.response = []
        super.init(map: map)
    }

    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

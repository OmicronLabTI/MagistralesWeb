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
    
    init(offset: Int?, limit: Int?, chips: [String]?) {
        self.offset = offset
        self.limit = limit
        self.chips = chips
    }
    
    func toDictionary() -> [String: Any] {
        var dict = [String:Any]()
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

class ComponentResponse: HttpResponse {
    var response: [ComponentO]?
    
    required init?(map: Map) {
        super.init(map: map)
    }
    
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

class ComponentO: Mappable {
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
    
    required init?(map: Map) {}
    
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
    }
}

class ComponentFormValues {
    var baseQuantity: Double
    var requiredQuantity: Double
    var warehouse: String
    
    init(baseQuantity: Double, requiredQuantity: Double, warehouse: String) {
        self.baseQuantity = baseQuantity
        self.requiredQuantity = requiredQuantity
        self.warehouse = warehouse
    }
}

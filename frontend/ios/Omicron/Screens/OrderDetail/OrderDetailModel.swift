//
//  OrderDetailModel.swift
//  Omicron
//
//  Created by Axity on 07/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

// MARK: OrderDetailModel
class OrderDetailResponse: HttpResponse {
    var response: OrderDetail?
    required init?(map: Map) {
        super.init(map: map)
    }
    
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

class OrderDetail {
    var productionOrderID: Int?
    var code, productDescription, type, status: String?
    var plannedQuantity: Int?
    var unit, warehouse: String?
    var number: Int?
    var fabDate, dueDate, startDate, endDate: String?
    var user, origin: String?
    var baseDocument: Int?
    var client: String?
    var completeQuantity: Int?
    var realEndDate, productLabel, container: String?
    var isChecked: Bool?
    var details: [Detail]?
    required init?(map: Map) {}
}

extension OrderDetail: Mappable {
    func mapping(map: Map) {
        self.productionOrderID <- map["productionOrderId"]
        self.code <- map["code"]
        self.productDescription <- map["productDescription"]
        self.type <- map["type"]
        self.status <- map["status"]
        self.plannedQuantity <- map["plannedQuantity"]
        self.unit <- map["unit"]
        self.warehouse <- map["warehouse"]
        self.number <- map["number"]
        self.fabDate <- map["fabDate"]
        self.dueDate <- map["dueDate"]
        self.startDate <- map["startDate"]
        self.endDate <- map["endDate"]
        self.user <- map["user"]
        self.origin <- map["origin"]
        self.baseDocument <- map["baseDocument"]
        self.client <- map["client"]
        self.completeQuantity <- map["completeQuantity"]
        self.realEndDate <- map["realEndDate"]
        self.productLabel <- map["productLabel"]
        self.container <- map["container"]
        self.isChecked <- map["isChecked"]
        self.details <- map["details"]
    }
}

class Detail {
    var orderFabID: Int?
    var productID, detailDescription: String?
    var baseQuantity, requiredQuantity, consumed, available: Int?
    var unit, warehouse: String?
    var pendingQuantity, stock, warehouseQuantity: Int?
    required init?(map: Map) { }
}

extension Detail: Mappable {
    func mapping(map: Map) {
        self.orderFabID <- map["orderFabId"]
        self.productID <- map["productId"]
        self.detailDescription <- map["description"]
        self.baseQuantity <- map["baseQuantity"]
        self.requiredQuantity <- map["requiredQuantity"]
        self.consumed <- map["consumed"]
        self.available <- map["available"]
        self.unit <- map["unit"]
        self.warehouse <- map["warehouse"]
        self.pendingQuantity <- map["pendingQuantity"]
        self.stock <- map["stock"]
        self.warehouseQuantity <- map["warehouseQuantity"]
    }
}

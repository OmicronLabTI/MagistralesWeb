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
    var realEndDate, productLabel, container, comments: String?
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
        self.comments <- map["comments"]
    }
}

class Detail {
    var orderFabID: Int?
    var productID, detailDescription: String?
    var baseQuantity, requiredQuantity, pendingQuantity, stock, warehouseQuantity, consumed, available: Double?
    var unit, warehouse: String?
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

class OrderDetailRequest: Codable {
    let fabOrderID, plannedQuantity: Int
    let fechaFin, comments: String
    let components: [Component]
    
    init(fabOrderID: Int, plannedQuantity: Int, fechaFin: String, comments: String, components:[Component]) {
        self.fabOrderID = fabOrderID
        self.plannedQuantity = plannedQuantity
        self.fechaFin = fechaFin
        self.comments = comments
        self.components = components
    }
}

class Component: Codable {
    let orderFabId: Int
    let productId, componentDescription: String
    let baseQuantity, requiredQuantity, pendingQuantity, stock, warehouseQuantity,  consumed, available: Double
    let unit, warehouse: String
    let action: String
    
    init(orderFabID: Int, productId: String, componentDescription: String,  baseQuantity: Double, requiredQuantity: Double, consumed: Double, available: Double, unit: String, warehouse: String, pendingQuantity: Double, stock: Double, warehouseQuantity: Double, action: String) {
        self.orderFabId = orderFabID
        self.productId = productId
        self.componentDescription = componentDescription
        self.baseQuantity = baseQuantity
        self.requiredQuantity = requiredQuantity
        self.consumed = consumed
        self.available = available
        self.unit = unit
        self.warehouse = warehouse
        self.pendingQuantity = pendingQuantity
        self.stock = stock
        self.warehouseQuantity = warehouseQuantity
        self.action = action
    }
}

class ChangeStatusRequest: Codable {
    var userId: String
    var orderId: Int
    var status: String
    
    init(userId: String, orderId: Int, status: String) {
        self.userId = userId
        self.orderId = orderId
        self.status = status
    }
}

class ChangeStatusRespose: HttpResponse {
    var response: String?
    
    required init?(map: Map)  {
        super.init(map: map)
    }
    
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

//class ChangeStatus {
//    var id: Int?
//    var userId: String?
//    var salesOrderId: String?
//    var productionOrderId: String?
//    var status: String?
//    required init?(map: Map)  {    }
//}
//
//extension ChangeStatus: Mappable {
//     func mapping(map: Map) {
//        id <- map["Id"]
//        userId <- map["Userid"]
//        salesOrderId <- map["Salesorderid"]
//        productionOrderId <- map["Productionorderid"]
//        status <- map["Status"]
//    }
//}

class FinishOrder: Codable {
    var userId: String?
    var fabricationOrderId: Int?
    var qfbSignature: String?
    var technicalSignature: String?
    
    init(userId: String, fabricationOrderId: Int, qfbSignature: String, technicalSignature: String) {
        self.userId = userId
        self.fabricationOrderId = fabricationOrderId
        self.qfbSignature = qfbSignature
        self.technicalSignature = technicalSignature
    }
}

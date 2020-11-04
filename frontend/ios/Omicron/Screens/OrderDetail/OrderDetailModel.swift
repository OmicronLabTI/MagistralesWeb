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
    var plannedQuantity: Decimal?
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
    init(productionOrderID: Int, code: String, productDescription: String, type: String,
         status: String, plannedQuantity: Decimal, unit: String, warehouse: String,
         number: Int, fabDate: String, dueDate: String, startDate: String, endDate: String,
         user: String, origin: String, baseDocument: Int, client: String, completeQuantity: Int,
         realEndDate: String, productLabel: String, container: String, comments: String,
         isChecked: Bool, details: [Detail]) {
        self.productionOrderID = productionOrderID
        self.code = code
        self.productDescription = productDescription
        self.type = type
        self.status = status
        self.plannedQuantity = plannedQuantity
        self.unit = unit
        self.warehouse = warehouse
        self.number = number
        self.fabDate = fabDate
        self.dueDate = dueDate
        self.startDate = startDate
        self.endDate = endDate
        self.user = user
        self.origin = origin
        self.baseDocument = baseDocument
        self.client = client
        self.completeQuantity = completeQuantity
        self.realEndDate = realEndDate
        self.productLabel = productLabel
        self.container = container
        self.comments = comments
        self.isChecked = isChecked
        self.details = details
    }
    required init?(map: Map) {}
}
extension OrderDetail: Mappable {
    func mapping(map: Map) {
        self.productionOrderID <- map["productionOrderId"]
        self.code <- map["code"]
        self.productDescription <- map["productDescription"]
        self.type <- map["type"]
        self.status <- map["status"]
        self.plannedQuantity <- (map["plannedQuantity"], DecimalTransform())
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
    init(orderFabID: Int, productID: String, detailDescription: String, baseQuantity: Double,
         requiredQuantity: Double, pendingQuantity: Double, stock: Double,
         warehouseQuantity: Double,
         consumed: Double, available: Double, unit: String, warehouse: String) {
        self.orderFabID = orderFabID
        self.productID = productID
        self.detailDescription = detailDescription
        self.baseQuantity = baseQuantity
        self.requiredQuantity = requiredQuantity
        self.pendingQuantity = pendingQuantity
        self.stock = stock
        self.warehouseQuantity = warehouseQuantity
        self.consumed = consumed
        self.available = available
        self.unit = unit
        self.warehouse = warehouse
    }
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
    let fabOrderID: Int
    let plannedQuantity: Decimal
    let fechaFin, comments, warehouse: String
    let components: [Component]
    init(fabOrderID: Int, plannedQuantity: Decimal, fechaFin: String,
         comments: String, warehouse: String ,components: [Component]) {
        self.fabOrderID = fabOrderID
        self.plannedQuantity = plannedQuantity
        self.fechaFin = fechaFin
        self.comments = comments
        self.components = components
        self.warehouse = warehouse
    }
}
class Component: Codable {
    let orderFabId: Int
    let productId, componentDescription: String
    let baseQuantity, requiredQuantity, pendingQuantity, stock,
    warehouseQuantity, consumed, available: Double
    let unit, warehouse: String
    let action: String
    init(orderFabID: Int, productId: String, componentDescription: String,
         baseQuantity: Double, requiredQuantity: Double, consumed: Double, available: Double,
         unit: String, warehouse: String, pendingQuantity: Double, stock: Double,
         warehouseQuantity: Double, action: String) {
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
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}
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
class FinishOrderResponse: HttpResponse {
    var response: OrderFinished?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        response <- map["response"]
    }
}
class OrderFinished {
    var userId: String?
    var fabricationOrderId: String?
    var qfbSignature: String?
    var technicalSignature: String?
    required init?(map: Map) { }
}
extension OrderFinished: Mappable {
    func mapping(map: Map) {
        self.userId <- map["userId"]
        self.fabricationOrderId <- map["fabricationOrderId"]
        self.qfbSignature <- map["qfbSignature"]
        self.technicalSignature <- map["technicalSignature"]
    }
}
class DeleteOrUpdateItemOfTableResponse: HttpResponse {
    var response: String?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        response <- map["response"]
    }
}

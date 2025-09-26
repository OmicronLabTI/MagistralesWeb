//
//  HistoricModel.swift
//  Omicron
//
//  Created by Josue Castillo on 10/09/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import ObjectMapper

class HistoricRequestModel: Codable {
    var qfb: String
    var orders: String
    var offset: Int?
    var limit: Int?
    
    init(qfb: String, orders: String, offset: Int? = nil, limit: Int? = nil) {
        self.qfb = qfb
        self.orders = orders
        self.offset = offset
        self.limit = limit
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

class HistoricResponseModel: HttpResponse {
    var response: [ParentOrders]?
    
    required init?(map: Map) {
        super.init(map: map)
    }
    
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

class ParentOrders: Codable {
    var orderProductionId: Int = 0
    var totalPieces: Int = 0
    var availablePieces: Int = 0
    var qfbWhoSplit: String = String()
    var detailOrdersCount: Int = 0
    var orderProductionDetail: [ChildrenOrders] = []
    var autoExpandOrderDetail: Bool = false
    
    init(orderProductionId: Int, totalPieces: Int, availablePieces: Int,
         qfbWhoSplit: String, detailOrdersCount: Int, orderProductionDetail: [ChildrenOrders],
         autoExpandOrderDetail: Bool) {
        self.orderProductionId = orderProductionId
        self.totalPieces = totalPieces
        self.availablePieces = availablePieces
        self.qfbWhoSplit = qfbWhoSplit
        self.detailOrdersCount = detailOrdersCount
        self.orderProductionDetail = orderProductionDetail
        self.autoExpandOrderDetail = autoExpandOrderDetail
    }
    
    required init?(map: Map) {}
}

extension ParentOrders: Mappable {
    func mapping(map: Map) {
        self.orderProductionId <- map["orderProductionId"]
        self.totalPieces <- map["totalPieces"]
        self.availablePieces <- map["availablePieces"]
        self.qfbWhoSplit <- map["qfbWhoSplit"]
        self.detailOrdersCount <- map["detailOrdersCount"]
        self.orderProductionDetail <- map["orderProductionDetail"]
        self.autoExpandOrderDetail <- map["autoExpandOrderDetail"]
    }
}

class ChildrenOrders: Codable {
    var orderProductionDetailId: Int = 0
    var assignedPieces: Int = 0
    var assignedQfb: String = String()
    var dateCreated: String = String()
    
    init(orderProductionDetailId: Int, assignedPieces: Int, assignedQfb: String, dateCreated: String) {
        self.orderProductionDetailId = orderProductionDetailId
        self.assignedPieces = assignedPieces
        self.assignedQfb = assignedQfb
        self.dateCreated = dateCreated
    }
    
    required init?(map: Map) {}
}

extension ChildrenOrders: Mappable {
    func mapping(map: Map) {
        self.orderProductionDetailId <- map["orderProductionDetailId"]
        self.assignedPieces <- map["assignedPieces"]
        self.assignedQfb <- map["assignedQfb"]
        self.assignedPieces <- map["dateCreated"]
    }
}

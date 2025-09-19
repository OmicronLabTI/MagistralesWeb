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
    var orderProductionId: Int
    var totalPieces: Int
    var availablePieces: Int
    var qfbWhoSplit: String
    var detailOrdersCount: Int
    var orderProductionDetail: [ChildrenOrders]
    var autoExpandOrderDetail: Bool
    
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
}

class ChildrenOrders: Codable {
    var orderProductionDetailId: Int
    var assignedPieces: Int
    var assignedQfb: String
    var dateCreated: String
    
    init(OrderProductionDetailId: Int, AssignedPieces: Int, AssignedQfb: String, DateCreated: String) {
        self.orderProductionDetailId = OrderProductionDetailId
        self.assignedPieces = AssignedPieces
        self.assignedQfb = AssignedQfb
        self.dateCreated = DateCreated
    }
}

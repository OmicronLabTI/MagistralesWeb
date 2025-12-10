//
//  SplitOrderModel.swift
//  Omicron
//
//  Created by Josue Castillo on 29/07/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import ObjectMapper

class SplitOrderRequest: Codable {
    var productionOrderId: Int
    var pieces: Int
    var userId: String
    var dxpOrder: String?
    var sapOrder: Int?
    var totalPieces: Int
    
    init(productionOrderId: Int, pieces: Int, userId: String, dxpOrder: String?, sapOrder: Int?, totalPieces: Int) {
        self.productionOrderId = productionOrderId
        self.pieces = pieces
        self.userId = userId
        self.dxpOrder = dxpOrder
        self.sapOrder = sapOrder
        self.totalPieces = totalPieces
    }
}

class SplitOrderResponse: HttpResponse {
    var response: String?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        response <- map["response"]
        code <- map["code"]
    }
}


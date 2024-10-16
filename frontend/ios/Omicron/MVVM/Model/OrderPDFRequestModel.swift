//
//  OrderPDFRequestModel.swift
//  Omicron
//
//  Created by Daniel Vargas on 10/10/24.
//  Copyright © 2024 Diego Cárcamo. All rights reserved.
//
import Foundation
import ObjectMapper

class OrderPDFRequestModel: Codable {
    var orderId: Int
    var clientType: String
    init (orderId: Int, clientType: String) {
        self.orderId = orderId
        self.clientType = clientType
    }
}

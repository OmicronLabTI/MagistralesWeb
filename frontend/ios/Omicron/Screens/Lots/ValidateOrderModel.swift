//
//  ValidateOrder.swift
//  Omicron
//
//  Created by Vicente Cantú on 04/11/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

class ValidateOrderModel: HttpResponse {
    var response: [ValidateOrder]?

    required init?(map: Map) {
        super.init(map: map)
    }

    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }

}

class ValidateOrder {

    var type: TypeError?
    var listItems: [String]?

    required init?(map: Map) {}

}

extension ValidateOrder: Mappable {
    func mapping(map: Map) {
        self.type <- map["type"]
        self.listItems <- map["listItems"]
    }
}

enum TypeError: String {
    case batches = "Batches"
    case stock = "Stock"
}

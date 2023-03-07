//
//  BulkOrderModel.swift
//  Omicron
//
//  Created by Daniel Velez on 01/03/23.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

class BulkOrderCreate: Codable {
    var productCode: String
    var userId: String
    var isFromQfbProfile: Bool
    init(productCode: String, userId: String, isFromQfbProfile: Bool) {
        self.productCode = productCode
        self.userId = userId
        self.isFromQfbProfile = isFromQfbProfile
    }
}

class BulkListRequest: Codable {
    var offset: Int?
    var limit: Int?
    var chips: [String]?
    var catalogGroup: String
    init(offset: Int?, limit: Int?, chips: [String]?, catalogGroup: String) {
        self.offset = offset
        self.limit = limit
        self.chips = chips
        self.catalogGroup = catalogGroup
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


class BulkListResponse: HttpResponse {
    var response: [BulkProduct]?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}
class BulkProduct: Mappable {
    var productoId: String?
    var largeDescription: String?
    required init?(map: Map) {}
    init() { }
    func mapping(map: Map) {
        self.productoId <- map["productoId"]
        self.largeDescription <- map["largeDescription"]
    }
}

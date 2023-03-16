//
//  History.swift
//  Omicron
//
//  Created by Daniel Vargas on 15/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

class RawMaterialHistoryReq {
    var offset: Int
    var limit: Int
    var fini: String
    var ffin: String
    var status: [String]
    init(offset: Int,
         limit: Int,
         fini: String,
         ffin: String,
         status: [String]) {
        self.offset = offset
        self.limit = limit
        self.ffin = ffin
        self.fini = fini
        self.status = status
    }
    func toDictionary() -> [String: Any] {
        var dict = [String: Any]()
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
class RawMaterialHistory: HttpResponse {
    var response: [RawMaterialItem]?
    var comments: Int?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        super.mapping(map: map)
        comments <- map["comments"]
        response <- map["response"]
    }
}
class RawMaterialItem: Mappable {
    var docEntry: Int?
    var itemCode: String?
    var description: String?
    var applicationName: String?
    var quantity: Decimal?
    var unit: String?
    var targetWarehosue: String?
    var docDate: Date?
    var status: String?
    
    required init?(map: Map) {}
    init() { }
    func mapping(map: Map) {
        self.docEntry <- map["docEntry"]
        self.itemCode <- map["itemCode"]
        self.description <- map["description"]
        self.applicationName <- map["applicationName"]
        self.quantity <- map["quantity"]
        self.unit <- map["unit"]
        self.targetWarehosue <- map["targetWarehosue"]
        self.docDate <- map["docDate"]
        self.status <- map["status"]
    }
}

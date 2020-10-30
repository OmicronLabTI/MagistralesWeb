//
//  ConnectModel.swift
//  Omicron
//
//  Created by Vicente Cantú on 29/10/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import ObjectMapper

class ConnectModel {
    var code: Int?
    var success: String?
    var response: [String]?
    required init?(map: Map) {}
}

extension ConnectModel: Mappable {
    func mapping(map: Map) {
        code <- map["Code"]
        success <- map["Success"]
        response <- map["Response"]
    }
}

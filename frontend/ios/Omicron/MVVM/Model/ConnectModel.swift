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

    init(code: Int, success: String, response: [String]) {
        self.code = code
        self.success = success
        self.response = response
    }
}

extension ConnectModel: Mappable {
    func mapping(map: Map) {
        code <- map["Code"]
        success <- map["Success"]
        response <- map["Response"]
    }
}

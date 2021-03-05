//
//  OrderPDF.swift
//  Omicron
//
//  Created by Vicente Cantú on 23/10/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import ObjectMapper

class OrderPDF: HttpResponse {

    var response: [String]?
    var comments: String?

    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
        comments <- map["comments"]
    }

}

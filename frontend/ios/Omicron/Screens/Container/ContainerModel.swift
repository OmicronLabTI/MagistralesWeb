//
//  ContainerModel.swift
//  Omicron
//
//  Created by Vicente Cantu Garcia on 19/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

class ContainerResponse: HttpResponse {

    var response: [Container]?

    required init?(map: Map) {
        super.init(map: map)
    }

    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }

}

class Container: Mappable {

    var container: String?
    var description: String?
    var quantity: Float?
    var unit: String?

    required init?(map: Map) {}

    func mapping(map: Map) {
        self.container <- map["container"]
        self.description <- map["description"]
        self.quantity <- map["quantity"]
        self.unit <- map["unit"]
    }

}

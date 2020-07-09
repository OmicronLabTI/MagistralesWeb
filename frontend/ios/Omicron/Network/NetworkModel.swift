//
//  NetworkModel.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

class HttpError {
    var error: String?
    
    required init?(map: Map) {}
}

extension HttpError: Mappable {
    func mapping(map: Map) {
        error <- map["error"]
    }
}

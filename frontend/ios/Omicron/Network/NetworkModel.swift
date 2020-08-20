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
    var errorCode: Int?
    var error: String?
    var userError: String?
    var info: String?
    
    required init?(map: Map) {}
}

extension HttpError: Mappable {
    func mapping(map: Map) {
        errorCode <- map["errorCode"]
        error <- map["error"]
        userError <- map["userError"]
        info <- map["info"]
    }
}

class HttpResponse: Mappable {
    var code: Int?
    var userError: String?
    var exceptionMessage: String?
    var success: Bool?
    
    required init?(map: Map) {}
    
    func mapping(map: Map) {
        code <- map["code"]
        userError <- map["userError"]
        exceptionMessage <- map["exceptionMessage"]
        success <- map["success"]
    }
}

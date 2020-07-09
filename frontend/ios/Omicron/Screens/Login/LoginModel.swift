//
//  LoginModel.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

struct Login: Codable {
    let username: String
    let password: String
    
    init(username: String, password: String) {
        self.username = username
        self.password = password
    }
}

class LoginResponse {
    var token: String?
    
    required init?(map: Map) {}
}

extension LoginResponse: Mappable {
    func mapping(map: Map) {
        token <- map["token"]
    }
}

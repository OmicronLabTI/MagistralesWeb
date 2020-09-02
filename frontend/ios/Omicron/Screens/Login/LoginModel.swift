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
    var user: String
    var password: String
    var redirectUri: String?
    var clientId2: String?
    var origin: String?
    
    init(username: String, password: String, redirectUri: String , clientId2: String, origin: String) {
        self.user = username
        self.password = password
        self.redirectUri = redirectUri
        self.clientId2 = clientId2
        self.origin = origin
    }
}

struct Renew: Codable {
    var refresh_token: String
    
    init(refreshToken: String) {
        self.refresh_token = refreshToken
    }
}

class LoginResponse: Codable {
    var access_token: String?
    var refresh_token: String?
    var token_type: String?
    var expires_in: Int?
    var scope: String?
    
    required init?(map: Map) {}
}

extension LoginResponse: Mappable {
    func mapping(map: Map) {
        access_token <- map["access_token"]
        refresh_token <- map["refresh_token"]
        token_type <- map["token_type"]
        expires_in <- map["expires_in"]
        scope <- map["scope"]
    }
}

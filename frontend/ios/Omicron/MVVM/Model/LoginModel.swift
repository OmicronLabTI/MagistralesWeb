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
    init(username: String, password: String, redirectUri: String, clientId2: String, origin: String) {
        self.user = username
        self.password = password
        self.redirectUri = redirectUri
        self.clientId2 = clientId2
        self.origin = origin
    }
}
struct Renew: Codable {
    // swiftlint:disable identifier_name
    var refresh_token: String
    init(refresh_token: String) {
        self.refresh_token = refresh_token
    }
}
class LoginResponse: Codable {
    var accessToken: String?
    var refreshToken: String?
    var tokenType: String?
    var expiresIn: Int?
    var scope: String?
    required init?(map: Map) {}
}
extension LoginResponse: Mappable {
    func mapping(map: Map) {
        accessToken <- map["access_token"]
        refreshToken <- map["refresh_token"]
        tokenType <- map["token_type"]
        expiresIn <- map["expires_in"]
        scope <- map["scope"]
    }
}

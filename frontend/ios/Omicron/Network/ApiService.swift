//
//  ApiService.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import Moya

enum ApiService {
    case login(data: Login)
    //case getInfoUser)
}

extension ApiService: TargetType {
    
    var baseURL: URL { return URL(string: Config.baseUrl)! }
    var path: String {
        switch self {
        case .login(_):
            return "/oauth/oauthrs/authorize"
//        case.getInfoUser(_):
//            return "/usuarios/user/sergio"
        }
    }
    
    var method: Moya.Method {
        switch self {
        case .login:
            return .post
//        case .getInfoUser:
//            return .get
        }
    }
    
    var task: Task {
        switch self {
        case .login(let data):
            return .requestJSONEncodable(data)
//        case .getInfoUser(_):
//            return .requestJSONEncodable("")
        }
    }
        
    var sampleData: Data {
        switch self {
        case .login:
            let loginResponse = "{\"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY0NzM4ODAsInVzZXIiOiJzZXJjaCJ9.v3RAx7cmoBUXq8WexeGTux-1-qy_wYM-JCLmVzpsCRY\",\"token_type\": \"Bearer\",\"expires_in\": 3600,\"scope\": \"\"}"
            return loginResponse.utf8Encoded
//        case .getInfoUser:
//            return "".utf8Encoded
        }
    }
    var headers: [String: String]? {
        return ["Content-type": "application/json"]
    }
}

// MARK: - Helpers
private extension String {
    var urlEscaped: String {
        return addingPercentEncoding(withAllowedCharacters: .urlHostAllowed)!
    }
    
    var utf8Encoded: Data {
        return data(using: .utf8)!
    }
}

extension ApiService: AccessTokenAuthorizable {
    var authorizationType: AuthorizationType? {
        switch self {
        case .login:
            return .none
        default:
            return .bearer
        }
    }
}

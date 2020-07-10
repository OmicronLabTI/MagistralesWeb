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
}

extension ApiService: TargetType {
    var baseURL: URL { return URL(string: Config.baseUrl)! }
    
    var path: String {
        switch self {
        case .login(_):
            return "/login"
        }
    }
    
    var method: Moya.Method {
        switch self {
        case .login:
            return .post
        }
    }
    var task: Task {
        switch self {
        case .login(let data):
            return .requestJSONEncodable(data)
        }
    }
    var sampleData: Data {
        switch self {
        case .login:
            return "{\"token\": \"12345\"}".utf8Encoded
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

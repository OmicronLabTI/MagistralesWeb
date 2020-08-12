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
    case getInfoUser(userId: String)
    case getStatusList(qfbId: String)
    case renew(data: Renew)
    case getOrdenDetail(orderId: Int)
}

extension ApiService: AuthorizedTargetType {
    var needsAuth: Bool {
        switch self {
        case .login:
            return false
        default:
            return true
        }
    }
    
    var baseURL: URL { return URL(string: Config.baseUrl)! }
    var path: String {
        switch self {
        case .login(_):
            return "/oauth/oauthrs/authorize"
        case .getInfoUser(let userId):
            return "/usuarios/user/\(userId)"
        case .getStatusList(let userId):
            return "/statusList\(userId)"
        case .renew:
            return "/oauth/oauthrs/renew"
        case .getOrdenDetail(let ordenId):
            return "/pedidos/formula/\(ordenId)"
        }
    }
    
    var method: Moya.Method {
        switch self {
        case .login, .renew:
            return .post
        case .getInfoUser, .getStatusList, .getOrdenDetail:
            return .get
        }
    }
    
    var task: Task {
        switch self {
        case .login(let data):
            return .requestJSONEncodable(data)
        case .getInfoUser:
            return .requestPlain
        case .getStatusList(let qfbId):
            return .requestJSONEncodable(qfbId)
        case .renew(let data):
            return .requestJSONEncodable(data)
        case .getOrdenDetail(let data):
            return .requestJSONEncodable(data)
        }
    }
        
    var sampleData: Data {
        switch self {
        case .login:
            guard let url = Bundle.main.url(forResource: "login", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
        case .getStatusList:
            guard let url = Bundle.main.url(forResource: "Status", withExtension: "json"),
                let data = try? Data(contentsOf: url)  else { return Data() }
            return data
        case .getInfoUser:
            guard let url = Bundle.main.url(forResource: "UserInfo", withExtension: "json"),
                let data = try? Data(contentsOf: url)  else { return Data() }
            return data
        case .renew:
            guard let url = Bundle.main.url(forResource: "refresh_token", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
        case .getOrdenDetail:
            guard let url = Bundle.main.url(forResource: "orderDetail", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
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

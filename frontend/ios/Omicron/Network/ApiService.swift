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
    case getInfoUser(username: String)
    case getStatusList(userId: String)
    case renew(data: Renew)
    case getOrdenDetail(orderId: Int)
    case deleteItemOfOrdenDetail(orderDetailRequest: OrderDetailRequest)
    case changeStatusOrder(changeStatusRequest: [ChangeStatusRequest])
    case getLots(orderId: Int)
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
        case .getInfoUser(let username):
            return "/usuarios/user/\(username)"
        case .getStatusList(let userId):
            return "/pedidos/qfbOrders/\(userId)"
        case .renew:
            return "/oauth/oauthrs/renew"
        case .getOrdenDetail(let orderId):
            return "/sapadapter/formula/\(orderId)"
        case .deleteItemOfOrdenDetail:
            return "/pedidos/formula"
        case .changeStatusOrder:
            return "/pedidos/status/fabOrder"
        case .getLots(let orderId):
            return "sapadapter/componentes/lotes/\(orderId)"
        }
    }
    
    var method: Moya.Method {
        switch self {
        case .login, .renew:
            return .post
        case .getInfoUser,
             .getStatusList,
             .getLots,
             .getOrdenDetail:
            return .get
        case .deleteItemOfOrdenDetail,
             .changeStatusOrder:
            return .put
        }
    }
    
    var task: Task {
        switch self {
        case .login(let data):
            return .requestJSONEncodable(data)
        case .getInfoUser,
             .getStatusList,
             .getLots,
             .getOrdenDetail:
            return .requestPlain
        case .renew(let data):
            return .requestJSONEncodable(data)
        case .deleteItemOfOrdenDetail(let data):
            return .requestJSONEncodable(data)
        case .changeStatusOrder(let data):
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
            
        case .deleteItemOfOrdenDetail:
            guard let url = Bundle.main.url(forResource: "orderDetail", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
        case .changeStatusOrder:
            guard let url = Bundle.main.url(forResource: "requestChangeStatusProcess", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
            
        case .getLots:
            guard let url = Bundle.main.url(forResource: "getLots", withExtension: "json"),
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

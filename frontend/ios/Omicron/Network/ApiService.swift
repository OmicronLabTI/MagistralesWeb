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
    case finishOrder(finishOrder: FinishOrder)
    case assingLots(lotsRequest: [BatchSelected])
    case askIfOrderCanBeFinalized(orderId: Int)
    case getComponents(data: ComponentRequest)
    case getWorkload(data: WorkloadRequest)
    case validateOrders(orderId: [Int])
    case postOrdersPDF(orders: [Int])
    case getConnect
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
    var baseURL: URL {
        switch self {
        case .getConnect: return URL(string: Config.serverOmicron)!
        default: return URL(string: Config.baseUrl)!
        }
    }
    var path: String {
        switch self {
        case .login:
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
        case .finishOrder:
            return "pedidos/finishOrder"
        case .assingLots:
            return "/pedidos/assignBatches"
        case .askIfOrderCanBeFinalized(let orderId):
            return "pedidos/completedBatches/\(orderId)"
        case .getComponents:
            return "sapadapter/componentes"
        case .getWorkload:
            return "/pedidos/qfb/workload"
        case .validateOrders(let orderId):
            return "/sapadapter/validate/order/\(orderId)"
        case .postOrdersPDF:
            return "/pedidos/saleorder/pdf"
        case .getConnect:
            return "SapDiApi/connect"
        }
    }
    var method: Moya.Method {
        switch self {
        case .login,
             .renew,
             .finishOrder,
             .postOrdersPDF,
             .validateOrders:
            return .post
        case .getInfoUser,
             .getStatusList,
             .getLots,
             .getOrdenDetail,
             .askIfOrderCanBeFinalized,
             .getComponents,
             .getWorkload,
             .getConnect:
            return .get
        case .deleteItemOfOrdenDetail,
             .changeStatusOrder,
             .assingLots:
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
             .getOrdenDetail,
             .askIfOrderCanBeFinalized,
             .validateOrders,
             .getConnect:
            return .requestPlain
        case .renew(let data):
            return .requestJSONEncodable(data)
        case .deleteItemOfOrdenDetail(let data):
            return .requestJSONEncodable(data)
        case .changeStatusOrder(let data):
            return .requestJSONEncodable(data)
        case .finishOrder(let data):
            return .requestJSONEncodable(data)
        case .assingLots(let data):
            return .requestJSONEncodable(data)
        case .getComponents(let data):
            return .requestParameters(parameters: data.toDictionary(), encoding: URLEncoding.queryString)
        case .getWorkload(let data):
            return .requestParameters(parameters: data.dictionary ?? [:], encoding: URLEncoding.queryString)
        case .postOrdersPDF(let data):
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
            guard let url = Bundle.main.url(forResource: "updateOrDeleteItemOfTable", withExtension: "json"),
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
        case .finishOrder:
            guard let url = Bundle.main.url(forResource: "finishedOrderResponse", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
        case .assingLots:
            guard let url = Bundle.main.url(forResource: "assingBatchesResponse", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
        case .askIfOrderCanBeFinalized:
            guard let url = Bundle.main.url(forResource: "getLots", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
        case .getComponents:
            guard let url = Bundle.main.url(forResource: "getComponents", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
        case .getWorkload:
            guard let url = Bundle.main.url(forResource: "workload", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data

        case .validateOrders:
            guard let url = Bundle.main.url(forResource: "getValidateOrder", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
        case .postOrdersPDF:
            guard let url = Bundle.main.url(forResource: "order_pdf", withExtension: "json"),
                let data = try? Data(contentsOf: url) else {
                    return Data()
            }
            return data
        case .getConnect:
            guard let url = Bundle.main.url(forResource: "connect", withExtension: "json"),
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

//
//  NetworkManager.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import Moya
import RxSwift
import ObjectMapper

enum RequestError: Error {
    case unknownError
    case invalidRequest(error: HttpError?)
    case unauthorized(error: HttpError?)
    case notFound
    case invalidResponse
    case serverError(error: HttpError?)
    case serverUnavailable
}

protocol AuthorizedTargetType: TargetType {
    var needsAuth: Bool { get }
}

class NetworkManager {
    
    static let shared: NetworkManager = NetworkManager()
    
    class TokenSource {
        var token: String?
        init() { }
    }
    
 struct AuthPlugin: PluginType {
   let tokenClosure: () -> String?

   func prepare(_ request: URLRequest, target: TargetType) -> URLRequest {
     guard
       let token = tokenClosure(),
       let target = target as? AuthorizedTargetType,
       target.needsAuth
     else {
       return request
     }

     var request = request
     request.addValue("Bearer " + token, forHTTPHeaderField: "Authorization")
     return request
   }
 }

    private var provider: MoyaProvider<ApiService> = MoyaProvider<ApiService>()
    
    init(provider: MoyaProvider<ApiService> = MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub,plugins: [
        AuthPlugin(tokenClosure: { return Persistence.shared.getLoginData()?.access_token })
    ])) {
        self.provider = provider
    }
    
    func login(data: Login) -> Observable<LoginResponse> {
        let req: ApiService = ApiService.login(data: data)
        let res: Observable<LoginResponse> = makeRequest(request: req)
        return res
    }
    
    func getInfoUser(userId: String) -> Observable<UserInfoResponse> {
        let req: ApiService = ApiService.getInfoUser(userId: userId)
        let res: Observable<UserInfoResponse> = makeRequest(request: req)
        return res
    }
    
    func getStatusList(qfbId: StatusRequest) -> Observable<StatusResponse> {
        let req: ApiService = ApiService.getStatusList(qfbId: qfbId)
            let res: Observable<StatusResponse> = makeRequest(request: req)
            return res
        }

    func renew(data: Renew) -> Observable<LoginResponse> {
        let req: ApiService = ApiService.renew(data: data)
        let res: Observable<LoginResponse> = makeRequest(request: req)
        return res
    }
    
    func getOrdenDetail(orderId: Int) -> Observable<OrderDetailResponse> {
        let req: ApiService = ApiService.getOrdenDetail(orderId: orderId)
        let res: Observable<OrderDetailResponse> = makeRequest(request: req)
        return res
    }

    
    private func makeRequest<T: BaseMappable>(request: ApiService) -> Observable<T> {
        return Observable<T>.create({ [weak self] observer in
            self?.provider.request(request) { result in
                switch result {
                case let .success(response):
                    let statusCode = response.statusCode
                    let json = try? response.mapJSON()
                    
                    switch statusCode {
                    case 200...299:
                        let res = Mapper<T>().map(JSONObject: json)
                        if (res != nil){
                            observer.onNext(res!)
                        } else {
                            observer.onError(RequestError.invalidResponse)
                        }
                        break
                    case 400:
                        let err = Mapper<HttpError>().map(JSONObject: json)
                        observer.onError(RequestError.invalidRequest(error: err))
                        break
                    case 401:
                        if request.needsAuth {
                            // TODO Renew Token
                        } else {
                            let err = Mapper<HttpError>().map(JSONObject: json)
                            observer.onError(RequestError.unauthorized(error: err))
                        }
                        break
                    case 404:
                        observer.onError(RequestError.notFound)
                        break
                    case 500...:
                        let err = Mapper<HttpError>().map(JSONObject: json)
                        observer.onError(RequestError.serverError(error: err))
                        break
                    default:
                        observer.onError(RequestError.unknownError)
                        break
                    }
                case .failure(_):
                    observer.onError(RequestError.serverUnavailable)
                    break
                }
                
                observer.onCompleted()
            }
            return Disposables.create()
        })
    }
    
//    private func handleTokenRefresh() -> Observable<Bool> {
//        if let userData = Persistence.shared.getUserData() {
//            guard let refreshToken = userData.refresh_token else {
//                return Observable.just(false)
//            }
//            let data = Renew(refreshToken: refreshToken)
//        }
//
//        return Disposables.create()
//    }
}
